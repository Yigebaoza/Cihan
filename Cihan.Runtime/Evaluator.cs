using Cihan.Core.Ast;
using Cihan.Core;
using Cihan.Runtime.Objects;

namespace Cihan.Runtime;

class BreakSign : Exception { }
class ContinueSign : Exception { }
class ReturnSign(object? v) : Exception
{
	public object? Value { get; } = v;
}

public sealed class Evaluator
{
	private readonly Dictionary<string, object> _globals = new()
	{
		["Console"] = new ConsoleObject(),
		["File"] = new FileObject(),
	};

	private readonly Core.Environment _global = new();   // 全局层
	private Core.Environment _current;                   // 当前层
	private readonly Dictionary<string, FuncDecl> _funcs = [];

	public Evaluator()
	{
		_current = _global;
	}

	public object? Evaluate(Expr expr)
	{
		switch (expr)
		{
			case StringLiteralExpr str:
				return str.Value;
			case IdentifierExpr id:
				if (_current.TryGet(id.Name, out var variable))
				{
					return variable.Value;
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
				// 3. 调用
				return InvokeMethod(callee!, args);
			case NumberLiteralExpr number:
				return number.Value;
			case BoolLiteralExpr boolean:
				return new Bool(boolean.Value);
			case UnaryExpr unary:
				if (unary.Op is UnaryOp.PrefixIncrement or UnaryOp.PrefixDecrement
					or UnaryOp.PostfixIncrement or UnaryOp.PostfixDecrement)
				{
					if (unary.Operand is not IdentifierExpr identifier)
					{
						throw new Exception("自增和自减运算符只能用于变量");
					}
					if (!_current.TryGet(identifier.Name, out var incrementVariable))
					{
						throw new Exception($"未定义的变量: {identifier.Name}");
					}
					if (incrementVariable.IsConst)
					{
						throw new Exception($"无法对常量 '{identifier.Name}' 重新赋值");
					}

					double oldValue = ToNumber(incrementVariable.Value);
					double newValue = unary.Op is UnaryOp.PrefixIncrement or UnaryOp.PostfixIncrement
						? oldValue + 1
						: oldValue - 1;
					incrementVariable.Value = newValue;

					return unary.Op is UnaryOp.PostfixIncrement or UnaryOp.PostfixDecrement
						? oldValue
						: newValue;
				}

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
			case CallExpr callExpr:
				if (!_funcs.TryGetValue(callExpr.Name, out var fn))
				{
					throw new Exception($"未定义的函数: {callExpr.Name}");
				}

				Core.Environment saved = _current;

				object? result;
				try
				{
					Core.Environment env = new(fn.ClosureEnv);
					_current = env;
					for (int i = 0; i < fn.Params.Count; i++)
					{
						object? val = i < callExpr.Args.Count
							? Evaluate(callExpr.Args[i])
							: fn.Defaults[i] != null
								? Evaluate(fn.Defaults[i]!)
								: throw new Exception($"函数 '{fn.Name}' 的参数 '{fn.Params[i]}' 缺少实参");
						env.Declare(fn.Params[i], new Variable(val, false));
					}
					try
					{
						ExecuteBlockStmt(fn.Body);
						result = null;
					}
					catch (ReturnSign rs)
					{
						result = rs.Value;
					}
				}
				finally
				{
					_current = saved;
				}
				return result;
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

	private void ExecuteStmt(Stmt stmt)
	{
		switch (stmt)
		{
			case ExprStmt exprStmt:
				Evaluate(exprStmt.Expression); // 求值但丢弃结果
				break;
			case VarStmt varStmt:
				if (_current.IsDeclared(varStmt.Name))
					throw new Exception($"变量 '{varStmt.Name}' 重复声明");
				_current.Declare(varStmt.Name, new Variable(Evaluate(varStmt.Initializer), varStmt.IsConst));
				break;
			case DelStmt delStmt:
				if (!_current.IsDeclared(delStmt.Expr.Name) && !_funcs.ContainsKey(delStmt.Expr.Name))
				{
					throw new Exception($"未定义的标识符: {delStmt.Expr.Name}");
				}
				else if (_funcs.ContainsKey(delStmt.Expr.Name))
				{
					_funcs.Remove(delStmt.Expr.Name);
				}
				else
				{
					_current.Declare(delStmt.Expr.Name);
				}
				break;
			case AssignStmt assignStmt:
				if (_globals.ContainsKey(assignStmt.Name))
				{
					throw new Exception($"内置对象不可更改: {assignStmt.Name}");
				}
				if (!_current.TryGet(assignStmt.Name, out var v))
					throw new Exception($"未定义的变量: {assignStmt.Name}");
				if (v.IsConst)
					throw new Exception($"无法对常量 '{assignStmt.Name}' 重新赋值");
				v.Value = Evaluate(assignStmt.Value);
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
			case WhileStmt whileStmt:
				while (IsTruthy(Evaluate(whileStmt.Condition)))
				{
					try { ExecuteBlockStmt(whileStmt.Body); }
					catch (BreakSign) { break; }
					catch (ContinueSign) { continue; }
				}
				break;
			case ForStmt forStmt:
				Core.Environment saved = _current;
				_current = new Core.Environment(_current);
				ExecuteStmt(forStmt.Init);
				while (IsTruthy(Evaluate(forStmt.Condition)))
				{
					try { ExecuteBlockStmt(forStmt.Body); }
					catch (BreakSign) { break; }
					catch (ContinueSign) { }
					ExecuteStmt(forStmt.Increment);
				}
				_current = saved;
				break;
			case FuncDecl funcDecl:
				funcDecl.ClosureEnv = _current;
				_funcs[funcDecl.Name] = funcDecl;
				break;
			case BreakStmt: throw new BreakSign();
			case ContinueStmt: throw new ContinueSign();
			case ReturnStmt ret: throw new ReturnSign(ret.Value is null ? null : Evaluate(ret.Value));
			default: throw new NotImplementedException($"未实现的语句类型: {stmt.GetType().Name}");
		}
	}

	public void ExecuteBlockStmt(BlockStmt blockStmt)
	{
		_current = new Core.Environment(_current);
		try
		{
			foreach (var stmt in blockStmt.Statements)
			{
				ExecuteStmt(stmt);
			}
		}
		finally
		{
			_current = _current.Parent!;
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
		var methods = target.GetType().GetMethods()
			.Where(method => method.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
			.ToArray();
		if (methods.Length == 0)
		{
			throw new Exception("找不到方法");
		}
		return new BoundMethod(target, methods);
	}

	private static object? InvokeMethod(object callee, object?[] args)
	{
		// 如果 callee 是 BoundMethod，调用它：
		if (callee is BoundMethod boundMethod)
		{
			foreach (var method in boundMethod.Methods)
			{
				var parameters = method.GetParameters();
				if (args.Length > parameters.Length)
				{
					continue;
				}

				bool hasAllArguments = true;
				object?[] convertedArgs = new object?[parameters.Length];
				for (int i = 0; i < parameters.Length; i++)
				{
					if (i < args.Length)
					{
						if (!TryConvertArgument(args[i], parameters[i].ParameterType, out convertedArgs[i]))
						{
							hasAllArguments = false;
							break;
						}
					}
					else if (parameters[i].IsOptional)
					{
						convertedArgs[i] = parameters[i].DefaultValue;
					}
					else
					{
						hasAllArguments = false;
						break;
					}
				}

				if (hasAllArguments)
				{
					return method.Invoke(boundMethod.Target, convertedArgs);
				}
			}

			throw new Exception("找不到匹配的方法参数");
		}
		throw new Exception();
	}

	private static bool TryConvertArgument(object? value, Type targetType, out object? converted)
	{
		if (targetType == typeof(string))
		{
			converted = value?.ToString();
			return true;
		}
		if (targetType == typeof(double))
		{
			if (value is double number)
			{
				converted = number;
				return true;
			}
			if (value is Bool boolean)
			{
				converted = boolean.Value ? 1.0 : 0.0;
				return true;
			}
		}
		if (targetType == typeof(int))
		{
			if (value is double number)
			{
				converted = checked((int)number);
				return true;
			}
		}
		if (targetType == typeof(long))
		{
			if (value is double number)
			{
				converted = checked((long)number);
				return true;
			}
		}
		if (targetType == typeof(bool))
		{
			if (value is Bool boolean)
			{
				converted = boolean.Value;
				return true;
			}
		}
		if (value is null && (!targetType.IsValueType || Nullable.GetUnderlyingType(targetType) != null))
		{
			converted = null;
			return true;
		}
		if (value != null && targetType.IsInstanceOfType(value))
		{
			converted = value;
			return true;
		}

		converted = null;
		return false;
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