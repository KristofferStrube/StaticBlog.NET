using KristofferStrube.StaticBlog.NET.Shared;
using System.CodeDom.Compiler;

namespace KristofferStrube.StaticBlog.NET.Templater;

public static class BlogTemplater
{
    public static Directory TemplateBlogDirectory(Settings settings, List<Post> posts, List<Directory> extraDirectories)
    {
        var scripts = new List<string>();
        var stylesheets = new List<string>();
        foreach (var directory in extraDirectories)
        {
            foreach (var file in directory.ChildFiles)
            {
                if (file.Name.EndsWith(".js"))
                {
                    scripts.Add($"{directory.Name}/{file.Name}");
                }
                if (file.Name.EndsWith(".css"))
                {
                    stylesheets.Add($"{directory.Name}/{file.Name}");
                }
            }
        }

        posts = posts.OrderByDescending(p => p.PublishDate).ToList();

        var indexPage = IndexTemplater.File(settings, posts, scripts, stylesheets);
        var rssFeed = RSSFeedTemplater.File(settings, posts);

        var taggedRssFeeds = posts
            .SelectMany(post => post.Tags)
            .Distinct()
            .Select(tag => RSSFeedTemplater.File(settings, posts, tag));

        var postDirectory = PostTemplater.Directory(settings, posts, scripts, stylesheets);
        var files = new List<File>() { indexPage, rssFeed };
        files.AddRange(taggedRssFeeds);

        return new Directory("root", extraDirectories.Append(postDirectory).ToList(), files);
    }

    public static void WriteHead(this IndentedTextWriter writer, Settings settings, List<string> scripts, List<string> stylesheets, Post? post = null)
    {
        writer.WriteLine("<head>");
        writer.Indent++;
        writer.WriteLine($"""
            <meta charset="UTF-8" />
            <meta http-equiv="content-language" content="en" />
            <meta name="viewport" content="width=device-width, initial-scale=1" />
            <title>{(post is null ? "" : $"{post.Title} - ")}{settings.Name}</title>
            <link rel="icon" type="image/x-icon" href="{settings.URL}/{settings.Logo}">
            <meta name="description" content="{(post is null ? "" : $"{post.Teaser} - ")}{settings.Description}" />
            <meta name="author" content="{settings.Author.Name}" />
            <meta property="og:author" content="{settings.Author.Name}" />
            <link rel="alternate" type="application/rss+xml" title="{settings.Name} RSS feed" href="{settings.URL}/{Constants.RSS_FEED_FILE_NAME_START}.{Constants.RSS_FEED_FILE_NAME_EXTENSION}" />
            """);

        if (post is not null)
        {
            writer.WriteLine($"""
            <meta name="twitter:card" content="summary" />
            <meta property="og:url" content="{settings.URL}/{Constants.POST_DIRECTORY}/{post.UrlPath}/" />
            <meta property="og:title" content="{post.Title} - {settings.Name}" />
            <meta property="og:type" content="article" />
            <meta property="og:description" content="{post.Teaser}" />
            <meta name="publish_date" property="og:publish_date" content="{post.PublishDate.ToDateTime(new TimeOnly()):yyyy-MM-ddTHH:mm:sszzz}" />
            <meta property="og:image" content="{settings.URL}/{post.ImagePath}" />
            """);
            if (!string.IsNullOrEmpty(post.CanonicalPostOrigin))
            {
                writer.WriteLine($"<link rel=\"canonical\" href=\"{post.CanonicalPostOrigin}\" />");
            }
        }

        foreach (var script in scripts)
        {
            writer.WriteLine($"""<script src="{(post is null ? "" : "../../")}{script}"></script>""");
        }
        foreach (var stylesheet in stylesheets)
        {
            writer.WriteLine($"""<link href="{(post is null ? "" : "../../")}{stylesheet}" rel="Stylesheet" type="text/css">""");
        }
        writer.Indent--;
        writer.WriteLine("</head>");
    }

    public static void WriteHeader(this IndentedTextWriter writer, Settings settings, int subpage = 0)
    {
        writer.WriteLine("<header>");
        writer.Indent++;
        writer.WriteLine("""<div class="container header-container">""");
        writer.Indent++;

        writer.WriteLine("""<div>""");
        writer.Indent++;
        writer.WriteLine($"""<h1><a class="no-link-style" href="{(string.Join("", Enumerable.Range(0, subpage).Select(_ => "../")))}">{settings.Name}</a></h1>""");
        writer.WriteLine($"<span>{settings.Teaser}</span>");
        writer.Indent--;
        writer.WriteLine("</div>");

        writer.WriteLine("""<div class="header-item">""");
        writer.Indent++;
        writer.WriteLine($"""<span><a href="{(string.Join("", Enumerable.Range(0, subpage).Select(_ => "../")))}">Home</a></span>""");
        writer.Indent--;
        writer.WriteLine("</div>");

        writer.WriteLine("""<div class="header-item">""");
        writer.Indent++;
        writer.WriteLine($"""<span><a href="{(string.Join("", Enumerable.Range(0, subpage).Select(_ => "../")))}{Constants.RSS_FEED_FILE_NAME_START}.{Constants.RSS_FEED_FILE_NAME_EXTENSION}">RSS Feed 🔖</a></span>""");
        writer.Indent--;
        writer.WriteLine("</div>");

        writer.Indent--;
        writer.WriteLine("</div>");
        writer.Indent--;
        writer.WriteLine("</header>");
    }

    public static void WriteFooter(this IndentedTextWriter writer, Settings settings)
    {
        writer.WriteLine("<footer>");
        writer.Indent++;
        writer.WriteLine("""<div class="container footer-container">""");
        writer.Indent++;

        writer.WriteLine("""<div>""");
        writer.Indent++;
        writer.WriteLine($"{settings.Name}<br />{settings.Teaser}");
        writer.Indent--;
        writer.WriteLine("</div>");

        foreach (var link in settings.Author.Contact)
        {
            writer.WriteLine("""<div class="footer-item">""");
            writer.Indent++;
            writer.WriteLine($"""<a class="no-link-style" href="{link.URL}">{link.DisplayText}</a>""");
            writer.Indent--;
            writer.WriteLine("</div>");
        }

        writer.Indent--;
        writer.WriteLine("</div>");
        writer.Indent--;
        writer.WriteLine("</footer>");
    }
}
