namespace Cihan.Runtime.Objects;

public sealed class ConsoleObject
{
    public void Writeln(object? value)
    {
        Console.WriteLine(value);
    }
}