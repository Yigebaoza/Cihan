namespace Cihan.Runtime.Objects;

public sealed class FileHandle(string path)
{
    public string Path { get; } = path;

    public string Read()
    {
        return File.ReadAllText(Path);
    }

    public string Read(int length)
    {
        using var reader = new StreamReader(Path);
        char[] buffer = new char[length];
        reader.Read(buffer, 0, length);
        return new string(buffer);
    }

    public void Write(string content)
    {
        File.WriteAllText(Path, content);
    }

    public void Append(string content)
    {
        File.AppendAllText(Path, content);
    }

    public void Delete()
    {
        File.Delete(Path);
    }

    public bool Exists()
    {
        return File.Exists(Path);
    }

    public long Size()
    {
        return new FileInfo(Path).Length;
    }

    public DateTime LastModified()
    {
        return File.GetLastWriteTime(Path);
    }

    public void Copy(string destinationPath)
    {
        File.Copy(Path, destinationPath);
    }

    public void Move(string destinationPath)
    {
        File.Move(Path, destinationPath);
    }

    public void Close()
    {
        // No resources to release in this implementation, but method is provided for API consistency.
    }
}