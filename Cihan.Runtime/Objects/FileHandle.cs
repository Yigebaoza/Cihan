namespace Cihan.Runtime.Objects;

public sealed class FileHandle
{
    public string Path { get; }

    public FileHandle(string path)
    {
        Path = path;
    }

    public string Read()
    {
        return System.IO.File.ReadAllText(Path);
    }
}