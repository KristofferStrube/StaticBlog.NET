using KristofferStrube.StaticBlog.NET.Shared;
using System.CodeDom.Compiler;
using System.Text;

namespace KristofferStrube.StaticBlog.NET.Templater;

internal static class PostTemplater
{
    public static Directory Directory(Settings settings, List<Post> posts, List<string> scripts, List<string> stylesheets)
    {
        var files = posts.Select(p => TemplatePost(settings, p, scripts, stylesheets)).ToList();

        return new Directory(Constants.POST_DIRECTORY, new(), files);
    }

    public static File TemplatePost(Settings settings, Post post, List<string> scripts, List<string> stylesheets)
    {
        using var resultingWriter = new StringWriterWithEncoding(Encoding.UTF8);
        using var writer = new IndentedTextWriter(resultingWriter);

        writer.WriteLine("""<!DOCTYPE html>""");
        writer.WriteLine("""<html lang="en">""");
        writer.Indent++;
        writer.WriteHead(settings, scripts, stylesheets, post);
        writer.WritePostBody(settings, post);
        writer.Indent--;
        writer.WriteLine("</html>");

        var content = Encoding.UTF8.GetBytes(resultingWriter.ToString());
        return new File($"{post.UrlPath}.html", content);
    }

    public static void WritePostBody(this IndentedTextWriter writer, Settings settings, Post post)
    {
        writer.WriteLine("<body>");
        writer.Indent++;
        writer.WriteHeader(settings, 1);
        writer.WriteLine("""<main class="container">""");
        writer.Indent++;
        writer.WriteLine("<article>");
        writer.Indent++;

        writer.WriteLine("""<div class="title-container">""");
        writer.Indent++;
        writer.WriteLine("""<div class="title-item">""");
        writer.Indent++;
        writer.WriteLine($"<h2>{post.Title}</h1>");
        writer.Indent--;
        writer.WriteLine("</div>");
        writer.WriteLine("""<div class="title-item date">""");
        writer.Indent++;
        writer.WriteLine(post.PublishDate.ToShortDateString());
        writer.Indent--;
        writer.WriteLine("</div>");
        writer.Indent--;
        writer.WriteLine("</div>");

        writer.WriteLine("""<div class="cover-image-container">""");
        writer.Indent++;
        writer.WriteLine($"""<img class="cover-image" alt="{post.Title}" src="../{post.ImagePath}" />""");
        writer.Indent--;
        writer.WriteLine("</div>");

        writer.WriteLine($"<p>{post.Description}</p>");
        writer.WriteLine($"<hr />");
        writer.WriteLine(post.Content);
        writer.Indent--;
        writer.WriteLine("</article>");
        writer.Indent--;
        writer.WriteLine("</main>");
        writer.WriteFooter(settings);
        writer.Indent--;
        writer.WriteLine("</body>");
    }
}
