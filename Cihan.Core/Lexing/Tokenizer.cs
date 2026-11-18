namespace Cihan.Core.Lexing;

public sealed class Tokenizer(string source)
{
    private string Source { get; } = source;

    private int _index = 0;
    private int _line = 1;
    private int _column = 1;

    public List<Token> Tokenize()
    {
        List<Token> list = [];
        while (true)
        {
            Token token = NextToken();
            list.Add(token);
            if (token.Kind is TokenKind.Eof)
            {
                break;
            }
        }
        return list;
    }

    private char AdvanceChar()
    {
        char cur = Source[_index];
        _index++;
        if (cur == '\n')
        {
            _line++;
            _column = 1;
        }
        else if (cur == '\r' && PeekChar(0) != '\n')
        {
            _line++;
            _column = 1;
        }
        else
        {
            _column++;
        }
        return cur;
    }

    private char? PeekChar(int offset = 0)
    {
        int index = _index + offset;
        if (index >= Source.Length)
        {
            return null;
        }
        else
        {
            return Source[index];
        }
    }

    private Token NextToken()
    {
        while (true)
        {
            if (PeekChar(0) == null)
            {
                return new Token(TokenKind.Eof, "", _line, _column);
            }
            if (PeekChar(0) == ' ' || PeekChar(0) == '\t' || PeekChar(0) == '\r' || PeekChar(0) == '\n')
            {
                AdvanceChar();
                continue;
            }
            if (PeekChar(0) == '#')
            {
                while (PeekChar(0) != '\n' && PeekChar(0) != null)
                {
                    AdvanceChar();
                }
                continue;
            }
            break;
        }

        char? current = PeekChar(0);
        int tokenLine = _line;
        int tokenColumn = _column;
        Token Create(TokenKind kind, string text) => new(kind, text, tokenLine, tokenColumn);

        if (current != null && char.IsDigit(current.Value))
        {
            int start = _index;
            while (PeekChar(0) is char c && (char.IsDigit(c) || c == '.'))
            {
                AdvanceChar();
            }
            string num = Source[start.._index];
            return Create(TokenKind.NumberLiteral, num);
        }

        if (current != null && (char.IsLetter(current.Value) || current.Value == '_'))
        {
            int start = _index;
            while (PeekChar(0) is char c && (char.IsLetterOrDigit(c) || c == '_'))
            {
                AdvanceChar();
            }
            string text = Source[start.._index];
            return text switch
            {
                "var" => Create(TokenKind.Var, text),
                "const" => Create(TokenKind.Const, text),
                "del" => Create(TokenKind.Del, text),
                "if" => Create(TokenKind.If, text),
                "else" => Create(TokenKind.Else, text),
                "while" => Create(TokenKind.While, text),
                "for" => Create(TokenKind.For, text),
                "break" => Create(TokenKind.Break, text),
                "continue" => Create(TokenKind.Continue, text),
                "func" => Create(TokenKind.Func, text),
                "return" => Create(TokenKind.Return, text),
                "True" => Create(TokenKind.True, text),
                "False" => Create(TokenKind.False, text),
                _ => Create(TokenKind.Identifier, text)
            };
        }

        switch (AdvanceChar())
        {
            case '.': return Create(TokenKind.Dot, ".");
            case '(': return Create(TokenKind.LeftParen, "(");
            case ')': return Create(TokenKind.RightParen, ")");
            case ',': return Create(TokenKind.Comma, ",");
            case ';': return Create(TokenKind.Semicolon, ";");
            case '+':
                if (PeekChar(0) == '+') { AdvanceChar(); return Create(TokenKind.PlusPlus, "++"); }
                return Create(TokenKind.Plus, "+");
            case '-':
                if (PeekChar(0) == '-') { AdvanceChar(); return Create(TokenKind.MinusMinus, "--"); }
                return Create(TokenKind.Minus, "-");
            case '*': return Create(TokenKind.Star, "*");
            case '/': return Create(TokenKind.Slash, "/");
            case '{': return Create(TokenKind.LeftBrace, "{");
            case '}': return Create(TokenKind.RightBrace, "}");
            case '=':
                if (PeekChar(0) == '=') { AdvanceChar(); return Create(TokenKind.EqualEqual, "=="); }
                return Create(TokenKind.Equals, "=");  // 单个 = 还是赋值号
            case '!':
                if (PeekChar(0) == '=') { AdvanceChar(); return Create(TokenKind.BangEquals, "!="); }
                return Create(TokenKind.Bang, "!");
            case '<':
                if (PeekChar(0) == '=') { AdvanceChar(); return Create(TokenKind.LessOrEqual, "<="); }
                return Create(TokenKind.Less, "<");
            case '>':
                if (PeekChar(0) == '=') { AdvanceChar(); return Create(TokenKind.GreaterOrEqual, ">="); }
                return Create(TokenKind.Greater, ">");
            case '"':
                int stringLiteralStart = _index;

                while (PeekChar(0) is char c && c != '"')
                {
                    AdvanceChar();
                }

                string value = Source[stringLiteralStart.._index];
                if (PeekChar(0) == '"')
                {
                    AdvanceChar();
                }

                return Create(TokenKind.StringLiteral, value);
            case '&':
                if (PeekChar(0) == '&')
                {
                    AdvanceChar();
                    return new Token(TokenKind.AmpAmp, "&&");
                }
                throw new Exception($"第 {tokenLine} 行，第 {tokenColumn} 列：暂不支持单个&");
            case '|':
                if (PeekChar(0) == '|')
                {
                    AdvanceChar();
                    return new Token(TokenKind.PipePipe, "||");
                }
                throw new Exception($"第 {tokenLine} 行，第 {tokenColumn} 列：暂不支持单个|");
            default:
                throw new Exception($"第 {tokenLine} 行，第 {tokenColumn} 列：无法识别的字符 '{current}'");
        }

    }

}
