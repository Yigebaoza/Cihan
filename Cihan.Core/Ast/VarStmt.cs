namespace Cihan.Core.Ast;

public sealed class VarStmt(string name, Expr initializer, bool isConst) : Stmt
{
    public string Name { get; } = name;
    public Expr Initializer { get; } = initializer;
    public bool IsConst { get; } = isConst;
}