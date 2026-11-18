using System.Reflection;

namespace Cihan.Runtime.Objects;

public sealed record BoundMethod(object Target, IReadOnlyList<MethodInfo> Methods);