namespace KristofferStrube.StaticBlog.NET.Shared;

public class Settings
{
    public string Name { get; set; } = string.Empty;
    public List<Author> Authors { get; set; } = new();
}
