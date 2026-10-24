using Cihan.Core.Ast;
using Cihan.Core.Lexing;

namespace Cihan.Core.Parsing;

public sealed class Parser(List<Token> tokens)
{
    private int _index = 0;
    private IReadOnlyList<Token> Tokens { get; } = tokens;

    private Token Peek()
    {
        return Tokens[_index];
    }

    private Token Previous()
    {
        return Tokens[_index - 1];
    }

    private Token Advance()
    {
        Token token = Tokens[_index];
        _index++;
        return token;
    }

    private bool IsAtEnd()
    {
        return Tokens[_index].Kind is TokenKind.Eof;
    }

    private bool Match(TokenKind kind)
    {
        if (kind == Peek().Kind)
        {
            Advance();
            return true;
        }
        return false;
    }

    public Program ParseProgram()
    {
        List<Stmt> stmts = [];
        while (!IsAtEnd())
        {
            stmts.Add(ParseStatement());
        }
        return new Program(stmts);
    }

    private Expr ParsePrimary()
    {
        switch (Peek().Kind)
        {
            case TokenKind.StringLiteral:
                return new StringLiteralExpr(Advance().Text);
            case TokenKind.NumberLiteral:
                return new NumberLiteralExpr(double.Parse(Advance().Text));
            case TokenKind.True:
                Advance();
                return new BoolLiteralExpr(true);
            case TokenKind.False:
                Advance();
                return new BoolLiteralExpr(false);
            case TokenKind.LeftParen:
                Advance(); // 吃掉 (
                Expr inner = ParseOr(); // 递归解析括号里的完整表达式
                if (!Match(TokenKind.RightParen))
                {
                    throw new Exception("缺少右括号");
                }
                return inner;
            case TokenKind.Identifier:
                Expr expr = new IdentifierExpr(Advance().Text);

                while (Peek().Kind is TokenKind.LeftParen or TokenKind.Dot)
                {
                    if (Match(TokenKind.Dot))
                    {
                        string name = Advance().Text;
                        expr = new MemberAccessExpr(expr, name);
                    }
                    else if (Match(TokenKind.LeftParen))
                    {
                        IReadOnlyList<Expr> args = ParseArgs();
                        expr = new MethodCallExpr(expr, args);
                    }
                    else
                    {
                        break;
                    }
                }
                return expr;
            default:
                throw new Exception("没有匹配的类型");
        }
    }

    private Expr ParseExpression()
    {
        // 1. 先让下一层去解析左边（可能是数字、字符串、或者更复杂的表达式）
        Expr left = ParseTerm();

        // 2. 循环检查：后面还有没有 + 或 - ？
        while (Peek().Kind is TokenKind.Plus or TokenKind.Minus)
        {
            // 3. 吃掉这个运算符
            Token opToken = Advance();
            BinaryOp op = opToken.Kind == TokenKind.Plus ? BinaryOp.Add : BinaryOp.Subtract;

            // 4. 让下一层去解析右边
            Expr right = ParseTerm();

            // 5. 把左边和右边包装成一个新的 BinaryExpr，并更新 left
            left = new BinaryExpr(left, op, right);
        }

        // 6. 没有更多 + 或 - 了，返回
        return left;
    }

    private Expr ParseTerm()
    {
        // 1. 先让下一层去解析左边
        Expr left = ParseUnary();

        // 2. 循环检查：后面还有没有 * 或 / ？
        while (Peek().Kind is TokenKind.Star or TokenKind.Slash)
        {
            Token opToken = Advance();
            BinaryOp op = opToken.Kind == TokenKind.Star ? BinaryOp.Multiply : BinaryOp.Divide;

            Expr right = ParseUnary();

            left = new BinaryExpr(left, op, right);
        }

        return left;
    }

    private Expr ParseComparison()
    {
        Expr left = ParseExpression();   // 下层先解析左右两边

        while (Peek().Kind is TokenKind.Less or TokenKind.Greater
                                  or TokenKind.LessOrEqual or TokenKind.GreaterOrEqual)
        {
            Token opToken = Advance();
            BinaryOp op = opToken.Kind switch
            {
                TokenKind.Less => BinaryOp.Less,
                TokenKind.Greater => BinaryOp.Greater,
                TokenKind.LessOrEqual => BinaryOp.LessOrEqual,
                TokenKind.GreaterOrEqual => BinaryOp.GreaterOrEqual,
                _ => throw new Exception()
            };


            Expr right = ParseExpression();
            left = new BinaryExpr(left, op, right);
        }

        return left;
    }

    private Expr ParseEquality()
    {
        Expr left = ParseComparison();

        while (Peek().Kind is TokenKind.EqualEqual or TokenKind.BangEquals)
        {
            Token opToken = Advance();
            BinaryOp op = opToken.Kind == TokenKind.EqualEqual ? BinaryOp.Equal : BinaryOp.NotEqual;

            Expr right = ParseComparison();
            left = new BinaryExpr(left, op, right);
        }

        return left;
    }

    private Expr ParseOr()
    {
        Expr left = ParseAnd();
        while (Match(TokenKind.PipePipe))
        {
            Expr right = ParseAnd();
            left = new BinaryExpr(left, BinaryOp.Or, right);
        }
        return left;
    }

    private Expr ParseUnary()
    {
        if (Match(TokenKind.Bang))
        {
            Expr operand = ParseUnary();   // 递归！支持 !!!x
            return new UnaryExpr(UnaryOp.Not, operand);
        }
        if (Match(TokenKind.Minus))
        {
            Expr operand = ParseUnary();   // 递归！支持 -(-1)
            return new UnaryExpr(UnaryOp.Negate, operand);
        }
        return ParsePrimary();
    }

    private Expr ParseAnd()
    {
        Expr left = ParseEquality();
        while (Match(TokenKind.AmpAmp))
        {
            Expr right = ParseEquality();
            left = new BinaryExpr(left, BinaryOp.And, right);
        }
        return left;
    }

    /* public Expr Parse()
       {
           Expr expr = ParseEquality();
           if (!IsAtEnd())
    //     {
    //         throw new Exception("还有多余的");
    //     }
           return expr;
       }
    */

    private Stmt ParseStatement()
    {
        // 变量声明分支
        if (Match(TokenKind.Var))
        {
            if (Peek().Kind != TokenKind.Identifier)
            {
                throw new Exception("var 后面必须是变量名");
            }
            string name = Advance().Text;

            if (!Match(TokenKind.Equals))
            {
                throw new Exception("变量声明缺少 = 号");
            }

            Expr initializer = ParseOr();

            if (!Match(TokenKind.Semicolon))
            {
                throw new Exception($"变量声明必须以分号结尾，当前 Token: {Peek().Kind}");
            }

            return new VarStmt(name, initializer);
        }

        if (Match(TokenKind.If))
        {
            if (!Match(TokenKind.LeftParen))
            {
                throw new Exception("if后面必须是(");
            }
            Expr cond = ParseOr();

            if (!Match(TokenKind.RightParen))
            {
                throw new Exception($"(未闭合");
            }

            Stmt thenBranch = ParseBlock();
            if (Match(TokenKind.Else))
            {
                Stmt elseBranch = ParseBlock();
                return new IfStmt(cond, thenBranch, elseBranch);
            }
            return new IfStmt(cond, thenBranch);
        }

        // 表达式语句分支
        Expr expr = ParseOr();
        if (!Match(TokenKind.Semicolon))
        {
            throw new Exception($"语句必须以分号结尾，当前 Token: {Peek().Kind}");
        }
        return new ExprStmt(expr);
    }

    private BlockStmt ParseBlock()
    {
        if (!Match(TokenKind.LeftBrace))
        {
            throw new Exception("if语句必须要{");
        }
        List<Stmt> list = [];
        while (Peek().Kind != TokenKind.RightBrace)
        {
            if (IsAtEnd())
            {
                throw new Exception("{未闭合");
            }

            list.Add(ParseStatement());
        }
        Match(TokenKind.RightBrace);
        return new BlockStmt(list);
    }

    private IReadOnlyList<Expr> ParseArgs()
    {
        List<Expr> args = [];
        if (Match(TokenKind.RightParen))
        {
            return args;
        }
        do
        {
            args.Add(ParseOr());
            if (Match(TokenKind.Comma)) { }
            else if (Match(TokenKind.RightParen))
            {
                return args;
            }
            else
            {
                throw new Exception($"Token: {Peek()}");
            }
        } while (true);
    }

}