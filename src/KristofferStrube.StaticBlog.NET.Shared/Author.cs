namespace KristofferStrube.StaticBlog.NET.Shared;

public class Author
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string WebSite { get; set; } = string.Empty;
    public List<MediaLink> Contact { get; set; } = new();
}
