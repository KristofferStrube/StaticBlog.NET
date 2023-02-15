using KristofferStrube.Blazor.FileSystem;

namespace KristofferStrube.StaticBlog.NET.Editor.Infrastructure;

public class HandleHolder
{
    public FileSystemDirectoryHandleInProcess? BlogEntry { get; set; }
    public FileSystemDirectoryHandleInProcess? CurrentPostEntry { get; set; }
}
