using CommunityToolkit.Mvvm.ComponentModel;
using CrackDigger.Sample.Services;

namespace CrackDigger.Sample.ViewModels
{
    /// <summary>The last update check, shared by the main window and the About window.</summary>
    public partial class UpdateState : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Text), nameof(IsAvailable), nameof(ToolTip))]
        private UpdateResult? result;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Text))]
        private bool isChecking;

        public bool IsAvailable => Result?.Status == UpdateStatus.Available;

        public string Text => IsChecking ? "Checking…" : Result?.Status switch
        {
            UpdateStatus.Available => $"{Result.LatestTag} is out",
            UpdateStatus.UpToDate => "Up to date",
            UpdateStatus.Unreachable => "Couldn't reach GitHub",
            _ => "",
        };

        public string ToolTip => IsAvailable
            ? $"Opens {Result!.ReleaseUrl}. Pull and rebuild to use it."
            : "Asks GitHub for the library's latest release";

        public async Task CheckAsync()
        {
            if (IsChecking)
            {
                return;
            }
            IsChecking = true;
            Result = await UpdateChecker.CheckAsync();
            IsChecking = false;
        }
    }
}
