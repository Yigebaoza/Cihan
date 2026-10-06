namespace Cihan.Core.Ast;

public sealed class MethodCallExpr(Expr callee, IReadOnlyList<Expr> args) : Expr
{
    public override ExprTag Tag => ExprTag.MethodCall;
    public Expr Callee { get; } = callee;
    public IReadOnlyList<Expr> Args { get; } = args;
}