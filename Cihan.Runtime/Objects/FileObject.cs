namespace Cihan.Runtime.Objects;

public sealed class FileObject
{
    public static FileHandle Open(string path)
    {
        return new FileHandle(path);
    }
}