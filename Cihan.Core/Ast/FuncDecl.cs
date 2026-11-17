namespace Cihan.Core.Ast;

using Cihan.Core;   // 引用 Environment

public sealed class FuncDecl(string name, IReadOnlyList<string> @params,
                IReadOnlyList<Expr?> defaults, BlockStmt body) : Stmt
{
    public string Name { get; } = name; public IReadOnlyList<string> Params { get; } = @params; public IReadOnlyList<Expr?> Defaults { get; } = defaults; public BlockStmt Body { get; } = body; public Environment? ClosureEnv { get; set; }
}