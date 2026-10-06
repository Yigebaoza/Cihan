namespace Cihan.Core.Lexing;

public enum TokenKind
{
    StringLiteral,
    Identifier,
    Dot,
    LeftParen,
    RightParen,
    // Special
    Comma,
    Eof,
}