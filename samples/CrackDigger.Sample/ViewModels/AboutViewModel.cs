using CommunityToolkit.Mvvm.Input;
using CrackDigger.Sample.Services;

namespace CrackDigger.Sample.ViewModels
{
    /// <summary>The About window: links, and the update check shared with the main window.</summary>
    public partial class AboutViewModel(UpdateState update)
    {
        public UpdateState Update { get; } = update;

        public string VersionText => $"Library version {AppLinks.LibraryVersion} · Sample app";

        [RelayCommand]
        private void Open(string url) => LinkOpener.Open(url);

        [RelayCommand]
        private async Task CheckUpdate()
        {
            if (Update.IsAvailable)
            {
                LinkOpener.Open(Update.Result!.ReleaseUrl);
                return;
            }
            await Update.CheckAsync();
        }
    }
}
