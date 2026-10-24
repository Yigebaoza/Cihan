namespace Cihan.Core.Ast;

public sealed class BlockStmt(IReadOnlyList<Stmt> stmts) : Stmt
{
    public IReadOnlyList<Stmt> Statements { get; } = stmts;
}