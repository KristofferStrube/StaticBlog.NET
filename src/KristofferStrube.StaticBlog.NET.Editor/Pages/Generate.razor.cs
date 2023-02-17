using KristofferStrube.Blazor.FileSystem;
using KristofferStrube.Blazor.FileSystemAccess;
using KristofferStrube.StaticBlog.NET.Editor.Infrastructure;
using KristofferStrube.StaticBlog.NET.Shared;
using KristofferStrube.StaticBlog.NET.Templater;
using Markdig;
using Microsoft.AspNetCore.Components;
using static System.Text.Json.JsonSerializer;

namespace KristofferStrube.StaticBlog.NET.Editor.Pages;

public partial class Generate : ComponentBase
{
    private bool notSupported;
    private FileSystemDirectoryHandleInProcess? blogDestinationEntry;
    private IFileSystemHandleInProcess[] entries = Array.Empty<IFileSystemHandleInProcess>();
    private FileSystemFileHandleInProcess? settingsEntry;
    private FileSystemDirectoryHandleInProcess? postsEntry;
    private FileSystemDirectoryHandleInProcess[] postEntries = Array.Empty<FileSystemDirectoryHandleInProcess>();

    [Inject]
    public IFileSystemAccessServiceInProcess FileSystemAccessService { get; set; } = default!;

    [Inject]
    public HandleHolder Handles { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        notSupported = !await FileSystemAccessService.IsSupportedAsync();
    }

    async Task OpenGenerationDirectoryAsync()
    {
        if (Handles.BlogEntry is null) return;
        try
        {
            blogDestinationEntry = await FileSystemAccessService.ShowDirectoryPickerAsync();
            await GenerateAsync();
        }
        catch
        {
            // TODO: Actually log these
        }
    }

    public async Task GenerateAsync()
    {
        try
        {
            entries = await Handles.BlogEntry!.ValuesAsync();
            if (entries.FirstOrDefault(e => e.Name is "settings.json") is FileSystemFileHandleInProcess settingsFileHandle)
            {
                settingsEntry = settingsFileHandle;
            }
            else
            {
                return;
            }

            if (entries.FirstOrDefault(e => e.Name is "posts") is FileSystemDirectoryHandleInProcess postFileHandle)
            {
                postsEntry = postFileHandle;
                postEntries = (await postsEntry.ValuesAsync())
                    .Where(e => e is FileSystemDirectoryHandleInProcess)
                    .Select(e => (FileSystemDirectoryHandleInProcess)e)
                    .ToArray();
            }
            else
            {
                postEntries = Array.Empty<FileSystemDirectoryHandleInProcess>();
            }

            try
            {
                var settingsFile = await settingsEntry.GetFileAsync();
                var settings = Deserialize<Settings>(await settingsFile.TextAsync())!;
                var posts = new List<NET.Shared.Post>();
                foreach (var postEntry in postEntries)
                {
                    try
                    {
                        var metaDataEntry = await postEntry.GetFileHandleAsync("metaData.json");
                        var metaDataFile = await metaDataEntry.GetFileAsync();
                        var metaData = Deserialize<NET.Shared.Post>(await metaDataFile.TextAsync())!;
                        var contentEntry = await postEntry.GetFileHandleAsync("content.md");
                        var contentFile = await contentEntry.GetFileAsync();
                        metaData.Content = Markdown.ToHtml(await contentFile.TextAsync());
                        posts.Add(metaData);
                    }
                    catch
                    {
                        // TODO: Actually log these
                    }
                }

                var extraDirectories = new List<Templater.Directory>();
                foreach (var entry in entries)
                {
                    if (entry is not FileSystemDirectoryHandleInProcess directoryHandle) continue;
                    if (entry.Name is "posts") continue;
                    try
                    {
                        var files = new List<Templater.File>();
                        foreach (var childEntry in await directoryHandle.ValuesAsync())
                        {
                            if (childEntry is not FileSystemFileHandleInProcess fileHandle) continue;
                            var file = await fileHandle.GetFileAsync();
                            files.Add(new Templater.File(childEntry.Name, await file.TextAsync()));
                        }
                        extraDirectories.Add(new Templater.Directory(directoryHandle.Name, new(), files));
                    }
                    catch
                    {
                        // TODO: Actually log these
                    }
                }

                var blogDirectory = BlogTemplater.TemplateBlogDirectory(settings, posts, extraDirectories);
                await GenerateFilesAndDirectoriesAsync(blogDestinationEntry, blogDirectory);
            }
            catch
            {
                // TODO: Actually log these
            }
        }
        catch
        {
            // TODO: Actually log these
        }
    }

    public async Task GenerateFilesAndDirectoriesAsync(FileSystemDirectoryHandleInProcess parentHandle, Templater.Directory directory)
    {
        foreach (var childDirectory in directory.ChildDirectories)
        {
            var directoryHandle = await parentHandle.GetDirectoryHandleAsync(childDirectory.Name, new() { Create = true });
            await GenerateFilesAndDirectoriesAsync(directoryHandle, childDirectory);
        }

        foreach (var childFile in directory.ChildFiles)
        {
            var fileHandle = await parentHandle.GetFileHandleAsync(childFile.Name, new() { Create = true });
            await using var writer = await fileHandle.CreateWritableAsync();
            await writer.WriteAsync(childFile.Content);
        }
    }
}