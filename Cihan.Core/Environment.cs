namespace Cihan.Core;

public sealed class Variable(object? value, bool isConst)
{
    public object? Value { get; set; } = value;
    public bool IsConst { get; } = isConst;
}

public sealed class Environment(Environment? parent = null)
{
    private readonly Dictionary<string, Variable> _vars = [];
    public Environment? Parent { get; } = parent;

    public bool IsDeclared(string name) => _vars.ContainsKey(name);
    public void Declare(string name, Variable v) => _vars[name] = v;

    public void Declare(string name) => _vars.Remove(name);

    public bool TryGet(string name, out Variable v)
    {
        if (_vars.TryGetValue(name, out v!)) return true;
        if (Parent != null) return Parent.TryGet(name, out v);
        v = null!; return false;
    }
}