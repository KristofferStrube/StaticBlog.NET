using KristofferStrube.StaticBlog.NET.Shared;
using System.CodeDom.Compiler;
using System.Text;

namespace KristofferStrube.StaticBlog.NET.Templater;

public static class IndexTemplater
{
    public static File File(Settings settings, List<Post> posts, List<string> scripts, List<string> stylesheets)
    {
        var resultingWriter = new StringWriterWithEncoding(Encoding.UTF8);
        var writer = new IndentedTextWriter(resultingWriter);

        writer.WriteLine("""<!DOCTYPE html>""");
        writer.WriteLine("""<html lang="en">""");
        writer.Indent++;
        writer.WriteHead(settings, scripts, stylesheets);
        writer.WriteIndexBody(settings, posts);
        writer.Indent--;
        writer.WriteLine("</html>");

        var content = resultingWriter.ToString();
        return new File(Constants.INDEX_FILE_NAME, content);
    }

    public static void WriteIndexBody(this IndentedTextWriter writer, Settings settings, List<Post> posts)
    {
        writer.WriteLine("<body>");
        writer.Indent++;
        writer.WriteHeader(settings);
        writer.WriteLine("""<main class="container">""");
        writer.Indent++;
        foreach (var post in posts)
        {
            writer.WritePostTeaser(post);
        }
        writer.Indent--;
        writer.WriteLine("</main>");
        writer.WriteFooter(settings);
        writer.Indent--;
        writer.WriteLine("</body>");
    }

    public static void WritePostTeaser(this IndentedTextWriter writer, Post post)
    {
        writer.WriteLine("""<div class="box">""");
        writer.Indent++;
        writer.WriteLine("""<div class="title-container">""");
        writer.Indent++;

        writer.WriteLine("""<div class="title-item">""");
        writer.Indent++;
        writer.WriteLine($"""<a href="{Constants.POST_DIRECTORY}/{post.UrlPath}.html"><img alt="{post.Title}" src="{post.ImagePath}" height="100px" /></a>""");
        writer.Indent--;
        writer.WriteLine("</div>");

        writer.WriteLine("""<div class="title-item">""");
        writer.Indent++;
        writer.WriteLine($"""<h3><a class="no-link-style" href="{Constants.POST_DIRECTORY}/{post.UrlPath}.html">{post.Title}</a></h3>""");
        writer.Indent--;
        writer.WriteLine("</div>");

        writer.WriteLine("""<div class="title-item date">""");
        writer.Indent++;
        writer.WriteLine(post.Date.ToShortDateString());
        writer.Indent--;
        writer.WriteLine("</div>");

        writer.Indent--;
        writer.WriteLine("</div>");
        writer.WriteLine($"<p>{post.Teaser}</p>");
        writer.Indent--;
        writer.WriteLine("</div>");
    }
}
