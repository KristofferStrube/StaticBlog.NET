using KristofferStrube.Blazor.FileSystem;
using KristofferStrube.StaticBlog.NET.Editor.Infrastructure;
using Microsoft.AspNetCore.Components;

namespace KristofferStrube.StaticBlog.NET.Editor.Shared;

public partial class EditBlog : ComponentBase
{
    private IFileSystemHandleInProcess[] entries = Array.Empty<IFileSystemHandleInProcess>();
    private FileSystemFileHandleInProcess? settingsEntry;
    private FileSystemDirectoryHandleInProcess? postsEntry;
    private FileSystemDirectoryHandleInProcess[] postEntries = Array.Empty<FileSystemDirectoryHandleInProcess>();
    private string newPostName = "My New Post";

    [Parameter, EditorRequired]
    public FileSystemDirectoryHandleInProcess BlogFolder { get; set; } = default!;

    [Inject]
    public HandleHolder Handles { get; set; } = default!;

    [Inject]
    NavigationManager NavigationManager { get; set; } = default!;

    protected override async Task OnParametersSetAsync()
    {
        entries = await BlogFolder.ValuesAsync();
        await DetectKeyEntriesAsync();
    }

    async Task DetectKeyEntriesAsync()
    {
        DetectSettings();
        await DetectPostsAsync();
    }

    void DetectSettings()
    {
        var settings = entries.FirstOrDefault(e => e.Name is "settings.json");
        if (settings is FileSystemFileHandleInProcess fileHandle)
        {
            settingsEntry = fileHandle;
        }
    }

    async Task DetectPostsAsync()
    {
        var posts = entries.FirstOrDefault(e => e.Name is "posts");
        if (posts is FileSystemDirectoryHandleInProcess fileHandle)
        {
            postsEntry = fileHandle;
            postEntries = (await postsEntry.ValuesAsync())
                .Where(e => e is FileSystemDirectoryHandleInProcess)
                .Select(e => (FileSystemDirectoryHandleInProcess)e)
                .ToArray();
        }
    }

    async Task CreateSettingsEntryAsync()
    {
        try
        {
            settingsEntry = await BlogFolder.GetFileHandleAsync("settings.json", new() { Create = true });
        }
        catch
        {
            // TODO: Actually log these
        }
    }
    async Task CreatePostsEntryAsync()
    {
        try
        {
            postsEntry = await BlogFolder.GetDirectoryHandleAsync("posts", new() { Create = true });
        }
        catch
        {
            // TODO: Actually log these
        }
    }
    async Task AddPostEntryAsync()
    {
        if (postsEntry is null) return;
        try
        {
            await postsEntry.GetDirectoryHandleAsync(newPostName, new() { Create = true });
            newPostName = "My new Post";
            await DetectPostsAsync();
        }
        catch
        {
            // TODO: Actually log these
        }
    }
    void EditPost(FileSystemDirectoryHandleInProcess postEntry)
    {
        Handles.CurrentPostEntry = postEntry;
        NavigationManager.NavigateTo("./post");
    }
}