namespace KristofferStrube.StaticBlog.NET.Shared;

public class Settings
{
    public string Name { get; set; } = string.Empty;
    public string URL { get; set; } = string.Empty;
    public string Teaser { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public Author Author { get; set; } = new();
}
