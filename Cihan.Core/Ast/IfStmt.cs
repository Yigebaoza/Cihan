namespace Cihan.Core.Ast;

public sealed class IfStmt(Expr condition, Stmt thenBranch, Stmt? elseBranch = null) : Stmt
{
    public Expr Condition { get; } = condition;
    public Stmt ThenBranch { get; } = thenBranch;
    public Stmt? ElseBranch { get; } = elseBranch;
}