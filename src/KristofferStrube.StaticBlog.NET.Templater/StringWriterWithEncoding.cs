using System.Text;

namespace KristofferStrube.StaticBlog.NET.Templater;

internal sealed class StringWriterWithEncoding : StringWriter
{
    public override Encoding Encoding { get; }

    public StringWriterWithEncoding(Encoding encoding)
    {
        Encoding = encoding;
    }
}