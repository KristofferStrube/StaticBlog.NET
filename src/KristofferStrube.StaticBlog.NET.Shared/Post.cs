namespace KristofferStrube.StaticBlog.NET.Shared;

public class Post
{
    public string Title { get; set; } = string.Empty;
    public string UrlPath { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Teaser { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
    public string Author { get; set; } = "Anonymous";
    public List<string> Tags { get; set; } = new();
    public List<string> AdditionalMetaTags { get; set; } = new();

    public Post(string name)
    {
        name = name.Trim();
        Title = name;
        UrlPath = name;
        Description = name + " Description";
        Description = name + " Teaser";
        Tags = name.Split(" ").ToList();
    }

    public Post() { }
}
