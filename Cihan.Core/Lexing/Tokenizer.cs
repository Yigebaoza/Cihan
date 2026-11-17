namespace Cihan.Core.Lexing;

public sealed class Tokenizer(string source)
{
    private string Source { get; } = source;

    private int _index = 0;

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
                return new Token(TokenKind.Eof, "");
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

        if (current != null && char.IsDigit(current.Value))
        {
            int start = _index;
            while (PeekChar(0) is char c && (char.IsDigit(c) || c == '.'))
            {
                AdvanceChar();
            }
            string num = Source[start.._index];
            return new Token(TokenKind.NumberLiteral, num);
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
                "var" => new Token(TokenKind.Var, text),
                "const" => new Token(TokenKind.Const, text),
                "if" => new Token(TokenKind.If, text),
                "else" => new Token(TokenKind.Else, text),
                "while" => new Token(TokenKind.While, text),
                "break" => new Token(TokenKind.Break, text),
                "continue" => new Token(TokenKind.Continue, text),
                "func" => new Token(TokenKind.Func, text),
                "return" => new Token(TokenKind.Return, text),
                "True" => new Token(TokenKind.True, text),
                "False" => new Token(TokenKind.False, text),
                _ => new Token(TokenKind.Identifier, text)
            };
        }

        switch (AdvanceChar())
        {
            case '.': return new Token(TokenKind.Dot, ".");
            case '(': return new Token(TokenKind.LeftParen, "(");
            case ')': return new Token(TokenKind.RightParen, ")");
            case ',': return new Token(TokenKind.Comma, ",");
            case ';': return new Token(TokenKind.Semicolon, ";");
            case '+':
                if (PeekChar(0) == '+') { AdvanceChar(); return new Token(TokenKind.PlusPlus, "++"); }
                return new Token(TokenKind.Plus, "+");
            case '-':
                if (PeekChar(0) == '-') { AdvanceChar(); return new Token(TokenKind.MinusMinus, "--"); }
                return new Token(TokenKind.Minus, "-");
            case '*': return new Token(TokenKind.Star, "*");
            case '/': return new Token(TokenKind.Slash, "/");
            case '{': return new Token(TokenKind.LeftBrace, "{");
            case '}': return new Token(TokenKind.RightBrace, "}");
            case '=':
                if (PeekChar(0) == '=') { AdvanceChar(); return new Token(TokenKind.EqualEqual, "=="); }
                return new Token(TokenKind.Equals, "=");  // 单个 = 还是赋值号
            case '!':
                if (PeekChar(0) == '=') { AdvanceChar(); return new Token(TokenKind.BangEquals, "!="); }
                return new Token(TokenKind.Bang, "!");
            case '<':
                if (PeekChar(0) == '=') { AdvanceChar(); return new Token(TokenKind.LessOrEqual, "<="); }
                return new Token(TokenKind.Less, "<");
            case '>':
                if (PeekChar(0) == '=') { AdvanceChar(); return new Token(TokenKind.GreaterOrEqual, ">="); }
                return new Token(TokenKind.Greater, ">");
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

                return new Token(TokenKind.StringLiteral, value);
            case '&':
                if (PeekChar(0) == '&')
                {
                    AdvanceChar();
                    return new Token(TokenKind.AmpAmp, "&&");
                }
                throw new Exception("暂不支持单个&");
            case '|':
                if (PeekChar(0) == '|')
                {
                    AdvanceChar();
                    return new Token(TokenKind.PipePipe, "||");
                }
                throw new Exception("暂不支持单个|");
            default:
                throw new Exception($"Unexpected character: {current}");
        }

    }

}
