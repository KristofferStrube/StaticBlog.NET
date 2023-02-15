# StaticBlog.NET
This is a project for generating and editing a static blog. It stores the necessary information in a folder structure and uses MarkDown files for the main content of each blog post to enabled a mix between simplicity and customizability.

## Solution structure
The solution is separated in three main projects.
### StaticBlog.NET.Generator
This part is what actually generates the main page, blog post list, header tags, tree-map, and each individual blog post in simple HTML files.
### StaticBlog.NET.Editor
This part is a Blazor WASM project that uses the [Blazor.FileSystemAccess](https://github.com/KristofferStrube/Blazor.FileSystemAccess) project to open a local directory where you store your blog data and edit it.
### StaticBlog.NET.Analytics
This part is still to be decided. But the main idea is that this will be an API that can be used for tracking user behavior i.e. how many visits each post has (we don't need a cookie consent for that).

Later this data might be used to order some lists or show specific posts as "What to read next" in the bottom of each page.

## Future priorities
To use the generator in something like an CI/CD pipeline it would be cool to have it as either an GitHub Actions package that can be added there or the more versatile approach, a `dotnet tool`.