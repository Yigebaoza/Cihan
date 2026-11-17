namespace Cihan.Core.Ast;

public sealed class ReturnStmt(Expr? value) : Stmt
{
    public Expr? Value { get; } = value;
}