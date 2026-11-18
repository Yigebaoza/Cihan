namespace Cihan.Runtime.Objects;

public sealed class ConsoleObject
{
    public static void Writeln(object? value = null)
    {
        Console.WriteLine(value);
    }

    public static void Write(object? value = null)
    {
        Console.Write(value);
    }

    public static string? Readln(object value)
    {
        Console.Write(value);
        return Console.ReadLine();
    }

    public static string? Readln()
    {
        return Console.ReadLine();
    }

    public static string? ReadKey(object value)
    {
        Console.Write(value);
        if (Console.IsInputRedirected)
        {
            return Console.ReadLine();
        }
        return Console.ReadKey().KeyChar.ToString();
    }

    public static string? ReadKey()
    {
        if (Console.IsInputRedirected)
        {
            return Console.ReadLine();
        }
        return Console.ReadKey().KeyChar.ToString();
    }

    public static void Clear()
    {
        if (!Console.IsOutputRedirected)
        {
            Console.Clear();
        }
    }

    public static void SetCursorPosition(int left, int top)
    {
        if (!Console.IsOutputRedirected)
        {
            Console.SetCursorPosition(left, top);
        }
    }

}