using KristofferStrube.StaticBlog.NET.Shared;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

namespace KristofferStrube.StaticBlog.NET.Templater;

internal static class RSSFeedTemplater
{
    public static File File(Settings settings, List<Post> posts)
    {
        SyndicationFeed feed = new(settings.Name, settings.Description, new Uri(settings.URL), settings.URL, DateTime.Now);

        SyndicationPerson author = new(settings.Author.Email, settings.Author.Email, settings.Author.Name);
        feed.Authors.Add(author);

        feed.Language = "en";
        feed.TimeToLive = new TimeSpan(2, 0, 0);

        List<SyndicationItem> postItems = new(posts.Count);
        foreach (var post in posts)
        {
            postItems.Add(
                new SyndicationItem(
                    post.Title,
                    post.Content,
                    new Uri($"{settings.URL}/{Constants.POST_DIRECTORY}/{post.UrlPath}/"),
                    post.UrlPath,
                    new DateTimeOffset(post.LastUpdatedDate.ToDateTime(TimeOnly.MinValue)))
                {
                    PublishDate = new DateTimeOffset(post.PublishDate.ToDateTime(TimeOnly.MinValue)),
                    Summary = new TextSyndicationContent(post.Description),
                    
                }
                );
        }
        feed.Items = postItems;

        var textWriter = new StringWriterWithEncoding(Encoding.UTF8);
        var xmlWriter = XmlWriter.Create(textWriter);
        feed.GetRss20Formatter().WriteTo(xmlWriter);
        xmlWriter.Close();

        var feedContent = Encoding.UTF8.GetBytes(textWriter.ToString());
        return new File(Constants.RSS_FEED_FILE_NAME, feedContent);
    }
}
