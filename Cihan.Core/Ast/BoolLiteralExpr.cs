namespace Cihan.Core.Ast;

public sealed class BoolLiteralExpr(bool value) : Expr
{
    public override ExprTag Tag => ExprTag.BoolLiteral;
    public bool Value { get; } = value;
}