namespace Cihan.Core.Ast;

public sealed class DelStmt(IdentifierExpr expr) : Stmt
{
    public IdentifierExpr Expr { get; } = expr;
}