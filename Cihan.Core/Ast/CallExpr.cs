namespace Cihan.Core.Ast;

public sealed class CallExpr : Expr
{
    public string Name { get; }
    public IReadOnlyList<Expr> Args { get; }

    public override ExprTag Tag => ExprTag.Call;

    public CallExpr(string name, IReadOnlyList<Expr> args) { Name = name; Args = args; }
}