namespace Cihan.Core.Ast;

public sealed class VarStmt(string name, Expr initializer) : Stmt
{
    public string Name { get; } = name;
    public Expr Initializer { get; } = initializer;
}