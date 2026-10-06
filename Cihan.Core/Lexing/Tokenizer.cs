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
        while (PeekChar(0) == ' ' || PeekChar(0) == '\t' || PeekChar(0) == '\r' || PeekChar(0) == '\n')
        {
            AdvanceChar();
        }
        if (PeekChar(0) == null)
        {
            return new Token(TokenKind.Eof, "");
        }

        char? current = PeekChar(0);
        if (current != null && (char.IsLetter(current.Value) || current.Value == '_'))
        {
            int start = _index;
            while (PeekChar(0) is char c && (char.IsLetterOrDigit(c) || c == '_'))
            {
                AdvanceChar();
            }
            string text = Source[start.._index];
            return new Token(TokenKind.Identifier, text);
        }

        switch (AdvanceChar())
        {
            case '.': return new Token(TokenKind.Dot, ".");
            case '(': return new Token(TokenKind.LeftParen, "(");
            case ')': return new Token(TokenKind.RightParen, ")");
            case ',': return new Token(TokenKind.Comma, ",");
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
            default:
                throw new Exception($"Unexpected character: {current}");
        }

    }

}