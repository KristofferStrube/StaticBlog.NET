# StaticBlog.NET
This is a project for generating and editing a static blog. It stores the necessary information in a folder structure and uses MarkDown files for the main content of each blog post to enable a mix between simplicity and customizability.

*This project is still under development, so some of these detail might not be implemented yet.*

An example of a generated site can be seen at https://kristoffer-strube.dk/ which is generated from the files at https://github.com/KristofferStrube/StaticBlog.NET

## Solution structure
The solution is separated in 4 main projects.
### StaticBlog.NET.Templater
This part is what actually generates the main page, blog post list, header tags, and each individual blog post in simple HTML files.
### StaticBlog.NET.Editor
This part is a Blazor WASM project that uses the [Blazor.FileSystemAccess](https://github.com/KristofferStrube/Blazor.FileSystemAccess) project to open a local directory where you store your blog data and edit it.
The editor can likewise call the templater to generate the output files to some specific folder.
### StaticBlog.NET.Generator
This is a CLI tool that simply is able to open the files in a directory like the editor and then use the templating project to generate the necessary files before writing them to some output path.
### StaticBlog.NET.Analytics
This part is still to be decided. But the main idea is that this will be an API that can be used for tracking user behavior i.e. how many visits each post has (we don't need a cookie consent for that).

Later this data might be used to order some lists or show specific posts as "What to read next" in the bottom of each page.

## Future priorities
For using the generator in something like an CI/CD pipeline it would be cool to have it as either an GitHub Actions package that can be added there or the more versatile approach, a `dotnet tool`.

## Related articles
- [Welcome to the blog - Kristoffer Strube’s Blog](https://kristoffer-strube.dk/post/welcome-to-the-blog.html)
