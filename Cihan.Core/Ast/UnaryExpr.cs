namespace Cihan.Core.Ast;

public sealed class UnaryExpr(UnaryOp op, Expr operand) : Expr
{
    public override ExprTag Tag => ExprTag.Unary;
    public UnaryOp Op { get; } = op;
    public Expr Operand { get; } = operand;
}