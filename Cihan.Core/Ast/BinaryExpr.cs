namespace Cihan.Core.Ast;

public sealed class BinaryExpr(Expr left, BinaryOp op, Expr right) : Expr
{
    public override ExprTag Tag => ExprTag.Binary;
    public Expr Left { get; } = left;
    public BinaryOp Op { get; } = op;
    public Expr Right { get; } = right;
}