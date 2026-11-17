namespace Cihan.Core.Ast;

public sealed class WhileStmt(Expr condition, BlockStmt body) : Stmt
{
    public Expr Condition { get; } = condition;
    public BlockStmt Body { get; } = body;
}