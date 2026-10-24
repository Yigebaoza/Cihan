namespace Cihan.Core.Ast;

public sealed class NumberLiteralExpr(double value) : Expr
{
    public override ExprTag Tag => ExprTag.NumberLiteral;
    public double Value { get; } = value;
}