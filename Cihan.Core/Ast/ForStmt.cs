namespace Cihan.Core.Ast;

public sealed class ForStmt(Stmt? init, Expr? condition, Stmt? increment, BlockStmt? body) : Stmt
{
    public Stmt Init { get; } = init ?? new BlockStmt([]);
    public Expr Condition { get; } = condition ?? new BoolLiteralExpr(true);
    public Stmt Increment { get; } = increment ?? new BlockStmt([]);
    public BlockStmt Body { get; } = body ?? new BlockStmt([]);
}