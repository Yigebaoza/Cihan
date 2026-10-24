namespace Cihan.Runtime.Objects;

public sealed class Bool(bool value)
{
    public bool Value { get; } = value;

    public override string ToString()
    {
        if (Value == false)
        {
            return "False";
        }
        return "True";
    }
}