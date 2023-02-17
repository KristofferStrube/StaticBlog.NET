using KristofferStrube.Blazor.FileSystem;
using KristofferStrube.StaticBlog.NET.Shared;
using Microsoft.AspNetCore.Components;
using System.Text.Json;
using static System.Text.Json.JsonSerializer;

namespace KristofferStrube.StaticBlog.NET.Editor.Shared;

public partial class EditSettings : ComponentBase
{
    private Settings? settings;
    [Parameter, EditorRequired]
    public FileSystemFileHandleInProcess SettingsEntry { get; set; } = default !;
    protected override async Task OnParametersSetAsync()
    {
        var settingsFile = await SettingsEntry.GetFileAsync();
        try
        {
            if (Deserialize<Settings>(await settingsFile.TextAsync())is Settings stored)
            {
                settings = stored;
                return;
            }
        }
        catch
        {
        // TODO: Actually log these
        }

        settings = new();
    }

    async Task SaveAsync()
    {
        try
        {
            await using var writer = await SettingsEntry.CreateWritableAsync();
            await writer.WriteAsync(Serialize(settings, new JsonSerializerOptions() { WriteIndented = true }));
        }
        catch
        {
        // TODO: Actually log these
        }
    }
}