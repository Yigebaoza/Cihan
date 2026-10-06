namespace Cihan.Core.Ast;

public sealed class MemberAccessExpr(Expr target, string memberName) : Expr
{
    public override ExprTag Tag => ExprTag.MemberAccess;
    public string MemberName { get; } = memberName;
    public Expr Target = target;
}