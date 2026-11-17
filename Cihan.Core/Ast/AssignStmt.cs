namespace Cihan.Core.Ast;

public sealed class AssignStmt(string name, Expr value) : Stmt
{
    public string Name { get; } = name;
    public Expr Value { get; } = value;
}