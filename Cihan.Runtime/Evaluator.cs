using Cihan.Core.Ast;
using Cihan.Runtime.Objects;

namespace Cihan.Runtime;

public sealed class Evaluator
{
    // 内置对象表：把语言里的标识符映射到 C# 对象
    private readonly Dictionary<string, object> _globals = new()
    {
        ["Console"] = new ConsoleObject(),
        ["File"] = new FileObject(),
    };

    public object? Evaluate(Expr expr)
    {
        switch (expr)
        {
            case StringLiteralExpr str:
                return str.Value;
            case IdentifierExpr id:
                if (_globals.TryGetValue(id.Name, out var obj))
                {
                    return obj;
                }
                throw new Exception($"未定义的标识符: {id.Name}");
            case MemberAccessExpr member:
                object target = Evaluate(member.Target)!;
                // 2. 找到对应的 C# 方法，返回一个 BoundMethod
                return ResolveMember(target, member.MemberName);

            case MethodCallExpr call:
                // 1. 求值 Callee（拿到 BoundMethod）
                var callee = Evaluate(call.Callee);
                // 2. 求值所有参数
                var args = call.Args.Select(Evaluate).ToArray();
                // 3. 调用它！
                return InvokeMethod(callee!, args);
            default:
                throw new NotImplementedException($"暂未实现的表达式类型: {expr.Tag}");
        }
    }

    private object ResolveMember(object target, string name)
    {
        // 把语言里的名字映射到 C# 的方法名
        string csharpName = name[0].ToString().ToUpper() + name[1..];
        // 然后用反射找到方法：
        var method = target.GetType().GetMethod(csharpName);
        if (method is null)
        {
            throw new Exception("找不到方法");
        }
        return new BoundMethod(target, method);
    }

    private object? InvokeMethod(object callee, object?[] args)
    {
        // 如果 callee 是 BoundMethod，调用它：
        if (callee is BoundMethod)
        {
            BoundMethod method = (BoundMethod)callee;

            return method.Method.Invoke(method.Target, args);
        }
        throw new Exception();
    }

}