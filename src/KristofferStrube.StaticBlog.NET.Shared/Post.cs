namespace KristofferStrube.StaticBlog.NET.Shared;

public class Post
{
    public string Title { get; set; } = string.Empty;
    public string UrlPath { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Teaser { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public List<string> AdditionalMetaTags { get; set; } = new();
    public string CanonicalPostOrigin { get; set; } = string.Empty;
    public string Content { get; set; } = "";
    public DateOnly PublishDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly LastUpdatedDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    public Post(string name)
    {
        name = name.Trim();
        Title = name;
        UrlPath = name.Trim().Replace(" ", "-");
        Description = name + " Description";
        Teaser = name + " Teaser";
        Tags = name.Split(" ").ToList();
        Content = name + " content";
    }

    public Post() { }
}
