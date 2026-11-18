namespace Cihan.Core.Lexing;

public readonly record struct Token(TokenKind Kind, string Text, int Line = 1, int Column = 1);