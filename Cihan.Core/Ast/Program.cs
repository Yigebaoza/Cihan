namespace Cihan.Core.Ast;

public sealed class Program(IReadOnlyList<Stmt> stmts)
{
    public IReadOnlyList<Stmt> Statements { get; } = stmts;
}