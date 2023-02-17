namespace KristofferStrube.StaticBlog.NET.Templater;

public record Directory(string Name, List<Directory> ChildDirectories, List<File> ChildFiles);
