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

    public Expr Parse()
    {
        Expr expr = ParsePrimary();
        if (!IsAtEnd())
        {
            throw new Exception("还有多余的");
        }
        return expr;
    }

    private Expr ParsePrimary()
    {
        switch (Peek().Kind)
        {
            case TokenKind.StringLiteral:
                return new StringLiteralExpr(Advance().Text);
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
                throw new Exception();
        }
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
            args.Add(ParsePrimary());
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