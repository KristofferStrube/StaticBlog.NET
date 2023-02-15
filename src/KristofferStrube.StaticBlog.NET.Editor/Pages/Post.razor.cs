using KristofferStrube.Blazor.FileSystem;
using KristofferStrube.StaticBlog.NET.Editor.Infrastructure;
using Microsoft.AspNetCore.Components;

namespace KristofferStrube.StaticBlog.NET.Editor.Pages;

public partial class Post : ComponentBase
{
    private FileSystemFileHandleInProcess[] entries = Array.Empty<FileSystemFileHandleInProcess>();
    private FileSystemFileHandleInProcess? markDownEntry;
    private string markDown = string.Empty;

    [Inject]
    public HandleHolder Handles { get; set; } = default!;

    [Inject]
    NavigationManager NavigationManager { get; set; } = default!;

    protected override async Task OnParametersSetAsync()
    {
        if (Handles.CurrentPostEntry is null)
        {
            NavigationManager.NavigateTo("./");
            return;
        }
        entries = (await Handles.CurrentPostEntry.ValuesAsync())
                .Where(e => e is FileSystemFileHandleInProcess)
                .Select(e => (FileSystemFileHandleInProcess)e)
                .ToArray();
        DetectKeyEntries();
    }

    void DetectKeyEntries()
    {
        DetectMarkDown();
    }
    async void DetectMarkDown()
    {
        var settings = entries.FirstOrDefault(e => e.Name == "content.md");
        if (settings is FileSystemFileHandleInProcess fileHandle)
        {
            markDownEntry = fileHandle;
            await GetMarkDownContentAsync();
        }
    }

    async Task GetMarkDownContentAsync()
    {
        var file = await markDownEntry!.GetFileAsync();
        markDown = await file.TextAsync();
        StateHasChanged();
    }

    async Task CreateMarkDownEntryAsync()
    {
        if (Handles.CurrentPostEntry is null) return;
        try
        {
            markDownEntry = await Handles.CurrentPostEntry.GetFileHandleAsync("content.md", new() { Create = true });
            await GetMarkDownContentAsync();
        }
        catch
        {
            // TODO: Actually log these
        }
    }

    async Task SaveMarkDownAsync()
    {
        await using var writer = await markDownEntry!.CreateWritableAsync(new () { KeepExistingData = false });
        await writer.WriteAsync(markDown);
    }
}