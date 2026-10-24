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

    private readonly Dictionary<string, object?> _variables = [];

    public object? Evaluate(Expr expr)
    {
        switch (expr)
        {
            case StringLiteralExpr str:
                return str.Value;
            case IdentifierExpr id:
                if (_variables.TryGetValue(id.Name, out var variable))
                {
                    return variable;
                }
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
            case NumberLiteralExpr number:
                return number.Value;
            case BoolLiteralExpr boolean:
                return new Bool(boolean.Value);
            case UnaryExpr unary:
                object? value = Evaluate(unary.Operand);
                if (unary.Op == UnaryOp.Not)
                {
                    return new Bool(!IsTruthy(value));
                }
                else if (unary.Op == UnaryOp.Negate)
                {
                    return -ToNumber(value);
                }
                throw new Exception($"不支持的Unary类型: {unary.Op}");
            case BinaryExpr binary:
                object? left = Evaluate(binary.Left);

                if (binary.Op == BinaryOp.And)
                {
                    if (!IsTruthy(left))
                    {
                        return left;
                    }
                    return Evaluate(binary.Right);
                }
                else if (binary.Op == BinaryOp.Or)
                {
                    if (IsTruthy(left))
                    {
                        return left;
                    }
                    return Evaluate(binary.Right);
                }

                object? right = Evaluate(binary.Right);
                return binary.Op switch
                {
                    BinaryOp.Add => AddValues(left, right),
                    BinaryOp.Subtract => ToNumber(left) - ToNumber(right),
                    BinaryOp.Multiply => ToNumber(left) * ToNumber(right),
                    BinaryOp.Divide => ToNumber(left) / ToNumber(right),
                    BinaryOp.Equal => new Bool(ValueEquals(left, right)),
                    BinaryOp.NotEqual => new Bool(!ValueEquals(left, right)),
                    BinaryOp.Less => new Bool(ToNumber(left) < ToNumber(right)),
                    BinaryOp.Greater => new Bool(ToNumber(left) > ToNumber(right)),
                    BinaryOp.LessOrEqual => new Bool(ToNumber(left) <= ToNumber(right)),
                    BinaryOp.GreaterOrEqual => new Bool(ToNumber(left) >= ToNumber(right)),
                    _ => throw new Exception($"未知运算符: {binary.Op}")
                };
            default:
                throw new NotImplementedException($"暂未实现的表达式类型: {expr.Tag}");
        }
    }

    public void Execute(Program program)
    {
        foreach (var stmt in program.Statements)
        {
            ExecuteStmt(stmt);
        }
    }

    public void ExecuteBlockStmt(BlockStmt blockStmt)
    {
        foreach (var stmt in blockStmt.Statements)
        {
            ExecuteStmt(stmt);
        }
    }

    private void ExecuteStmt(Stmt stmt)
    {
        switch (stmt)
        {
            case ExprStmt exprStmt:
                Evaluate(exprStmt.Expression); // 求值但丢弃结果
                break;
            case VarStmt varStmt:
                _variables[varStmt.Name] = Evaluate(varStmt.Initializer);
                break;
            case BlockStmt blockStmt:
                ExecuteBlockStmt(blockStmt);
                break;
            case IfStmt ifStmt:
                object? cond = Evaluate(ifStmt.Condition);
                if (IsTruthy(cond))
                {
                    ExecuteStmt(ifStmt.ThenBranch);
                }
                else
                {
                    if (ifStmt.ElseBranch is not null) ExecuteStmt(ifStmt.ElseBranch);
                }
                break;
            default:
                throw new NotImplementedException($"未实现的语句类型: {stmt.GetType().Name}");
        }
    }

    public static object? AddValues(object? left, object? right)
    {
        if (left is double d1 && right is double d2)
        {
            return d1 + d2;
        }
        else if (left is string s1 && right is string s2)
        {
            return s1 + s2;
        }
        else if (left is Bool b1 && right is Bool b2)
        {
            return (b1.Value ? 1.0 : 0.0) + (b2.Value ? 1.0 : 0.0);
        }
        else
        {
            throw new Exception($"{left}和{right}不可相加");
        }
    }

    private static BoundMethod ResolveMember(object target, string name)
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

    private static object? InvokeMethod(object callee, object?[] args)
    {
        // 如果 callee 是 BoundMethod，调用它：
        if (callee is BoundMethod)
        {
            BoundMethod method = (BoundMethod)callee;

            return method.Method.Invoke(method.Target, args);
        }
        throw new Exception();
    }

    private static double ToNumber(object? value)
    {
        return value switch
        {
            double d => d,
            Bool b => b.Value ? 1 : 0,
            _ => throw new Exception($"{value}不可转换为数字")
        };
    }

    private static bool TryToNumber(object? value, out double result)
    {
        switch (value)
        {
            case double d:
                result = d;
                return true;
            case Bool b:
                result = ToNumber(b);
                return true;
            default:
                result = 0;
                return false;
        }
    }

    private static bool ValueEquals(object? left, object? right)
    {
        // 情况 1：两边都能转成数字 → 按数字比较
        if (TryToNumber(left, out double l) && TryToNumber(right, out double r))
        {
            return l == r;
        }

        // 情况 2：两边都是字符串 → 按字符串比较
        if (left is string s1 && right is string s2)
        {
            return s1 == s2;
        }

        // 其他情况 → 不能比较
        throw new Exception($"{left} 和 {right} 不可比较");
    }

    private static bool IsTruthy(object? value)
    {
        return value switch
        {
            Bool b => b.Value,
            double d => d != 0,
            string s => s != "",
            null => false,
            _ => throw new Exception($"无法判断真假: {value}")
        };
    }

}