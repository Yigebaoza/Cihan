namespace Cihan.Runtime.Objects;

public sealed class ConsoleObject
{
    public static void Writeln(object? value)
    {
        Console.WriteLine(value);
    }

    public static void Write(object? value)
    {
        Console.Write(value);
    }

    public static string? Readln(object? value)
    {
        Console.Write(value);
        return Console.ReadLine();
    }
}