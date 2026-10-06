namespace Cihan.Core.Ast;

public sealed class StringLiteralExpr(string value) : Expr
{
    public override ExprTag Tag => ExprTag.StringLiteral;
    public string Value { get; } = value;
}