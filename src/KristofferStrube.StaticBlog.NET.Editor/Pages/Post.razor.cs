using KristofferStrube.Blazor.FileSystem;
using KristofferStrube.StaticBlog.NET.Editor.Infrastructure;
using Microsoft.AspNetCore.Components;
using System.Text.Json;
using static System.Text.Json.JsonSerializer;

namespace KristofferStrube.StaticBlog.NET.Editor.Pages;

public partial class Post : ComponentBase
{
    private FileSystemFileHandleInProcess[] entries = Array.Empty<FileSystemFileHandleInProcess>();
    private FileSystemFileHandleInProcess? markDownEntry;
    private FileSystemFileHandleInProcess? metaDataEntry;
    private string markDown = string.Empty;
    private NET.Shared.Post? post;
    private string tags = string.Empty;

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
        await DetectKeyEntriesAsync();
    }

    async Task DetectKeyEntriesAsync()
    {
        await DetectMarkDownAsync();
        await DetectMetaDataAsync();
    }

    async Task DetectMarkDownAsync()
    {
        if (entries.FirstOrDefault(e => e.Name == "content.md") is FileSystemFileHandleInProcess fileHandle)
        {
            markDownEntry = fileHandle;
            await GetMarkDownContentAsync();
        }
    }

    async Task DetectMetaDataAsync()
    {
        if (entries.FirstOrDefault(e => e.Name == "metaData.json") is FileSystemFileHandleInProcess fileHandle)
        {
            metaDataEntry = fileHandle;
            await GetMetaDataAsync();
        }
    }

    async Task GetMarkDownContentAsync()
    {
        var file = await markDownEntry!.GetFileAsync();
        markDown = await file.TextAsync();
        StateHasChanged();
    }

    async Task GetMetaDataAsync()
    {
        try
        {
            var file = await metaDataEntry!.GetFileAsync();
            post = Deserialize<NET.Shared.Post>(await file.TextAsync());
        }
        catch
        {
            post = new(Handles.CurrentPostEntry!.Name);
        }
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

    async Task CreateMetaDataEntryAsync()
    {
        if (Handles.CurrentPostEntry is null) return;
        try
        {
            metaDataEntry = await Handles.CurrentPostEntry.GetFileHandleAsync("metaData.json", new() { Create = true });
            await GetMetaDataAsync();
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

    async Task SaveMetaDataAsync()
    {
        await using var writer = await metaDataEntry!.CreateWritableAsync(new() { KeepExistingData = false });
        await writer.WriteAsync(Serialize(post, new JsonSerializerOptions() { WriteIndented = true }));
    }
}