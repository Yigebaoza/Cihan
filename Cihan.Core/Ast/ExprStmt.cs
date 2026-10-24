namespace Cihan.Core.Ast;

public sealed class ExprStmt(Expr expr) : Stmt
{
    public Expr Expression { get; } = expr;
}