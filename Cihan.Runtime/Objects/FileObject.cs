namespace Cihan.Runtime.Objects;

public sealed class FileObject
{
    public FileHandle Open(string path)
    {
        return new FileHandle(path);
    }
}