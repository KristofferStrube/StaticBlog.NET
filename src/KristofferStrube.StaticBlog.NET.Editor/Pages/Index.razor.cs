using KristofferStrube.Blazor.FileSystemAccess;
using KristofferStrube.StaticBlog.NET.Editor.Infrastructure;
using Microsoft.AspNetCore.Components;

namespace KristofferStrube.StaticBlog.NET.Editor.Pages;

public partial class Index : ComponentBase
{
    private bool notSupported;

    [Inject]
    public IFileSystemAccessServiceInProcess FileSystemAccessService { get; set; } = default!;

    [Inject]
    public HandleHolder Handles { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        notSupported = !await FileSystemAccessService.IsSupportedAsync();
    }

    async Task OpenDirectoryAsync()
    {
        try
        {
            Handles.BlogEntry = await FileSystemAccessService.ShowDirectoryPickerAsync();
        }
        catch
        {
            // TODO: Actually log these
        }
    }
}