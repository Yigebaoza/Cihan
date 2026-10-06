namespace Cihan.Core.Ast;

public sealed class IdentifierExpr(string name) : Expr
{
    public override ExprTag Tag => ExprTag.Identifier;
    public string Name { get; } = name;
}