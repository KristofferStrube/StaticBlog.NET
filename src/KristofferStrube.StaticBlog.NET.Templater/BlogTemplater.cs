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

        var indexPage = IndexTemplater.File(settings, posts, scripts, stylesheets);
        var postDirectory = PostTemplater.Directory(settings, posts, scripts, stylesheets);

        return new Directory("root", extraDirectories.Append(postDirectory).ToList(), new() { indexPage });
    }

    public static void WriteHead(this IndentedTextWriter writer, Settings settings, List<string> scripts, List<string> stylesheets, Post? post = null)
    {
        writer.WriteLine("<head>");
        writer.Indent++;
        writer.WriteLine("""<meta charset="UTF-8" />""");
        writer.WriteLine("""<meta http-equiv="content-language" content="en" />""");
        writer.WriteLine("""<meta name="viewport" content="width=device-width, initial-scale=1" />""");
        writer.WriteLine($"<title>{(post is null ? "" : $"{post.Title} - ")}{settings.Name}</title>");
        writer.WriteLine($"""<meta name="description" content="{(post is null ? "" : $"{post.Teaser} - ")}{settings.Description}" />""");
        writer.WriteLine($"""<meta name="author" content="{settings.Author.Name}" />""");
        writer.WriteLine($"""<meta property="og:author" content="{settings.Author.Name}" />""");

        // Twitter things
        if (post is not null)
        {
            writer.WriteLine($"""
            <meta name="twitter:card" content="summary" />
            <meta property="og:url" content="https://kristoffer-strube.dk/posts/{post.UrlPath}.html" />
            <meta property="og:title" content="{post.Title} - {settings.Name}" />
            <meta property="og:type" content="article" />
            <meta property="og:description" content="{post.Teaser}" />
            <meta name="publish_date" property="og:publish_date" content="{post.Date.ToDateTime(new TimeOnly()).ToString("yyyy-MM-ddTHH:mm:sszzz")}" />
            <meta property="og:image" content="https://kristoffer-strube.dk/{post.ImagePath}" />
            """);
        }

        foreach (var script in scripts)
        {
            writer.WriteLine($"""<script src="{(post is null ? "" : "../")}{script}"></script>""");
        }
        foreach (var stylesheet in stylesheets)
        {
            writer.WriteLine($"""<link href="{(post is null ? "" : "../")}{stylesheet}" rel="Stylesheet" type="text/css">""");
        }
        writer.Indent--;
        writer.WriteLine("</head>");
    }

    public static void WriteHeader(this IndentedTextWriter writer, Settings settings, int subpage = 0)
    {
        writer.WriteLine("<header>");
        writer.Indent++;
        writer.WriteLine("""<div class="container">""");
        writer.Indent++;
        writer.WriteLine($"""<h1><a class="no-link-style" href="{(string.Join("", Enumerable.Range(0, subpage).Select(_ => "../")))}index.html">{settings.Name}</a></h1>""");
        writer.WriteLine($"<span>{settings.Teaser}</span>");
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
