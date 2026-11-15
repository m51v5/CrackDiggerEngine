using CommunityToolkit.Mvvm.ComponentModel;
using CrackDigger.Sample.Services;
using static CrackDigger.CrackDiggerEngine;

namespace CrackDigger.Sample.ViewModels
{
    public enum SiteState
    {
        Searching,
        Done,
        Empty,
        Failed,
        Stopped,
    }

    /// <summary>One row in the SITES list: how that site's search went.</summary>
    public partial class SiteStatusViewModel(SiteInfo info) : ObservableObject
    {
        public SiteInfo Info { get; } = info;

        public string Domain => Info.Domain;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CountText), nameof(ToolTip))]
        private SiteState state = SiteState.Searching;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CountText))]
        private int count;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ToolTip))]
        private string? error;

        public string CountText => State switch
        {
            SiteState.Searching => "···",
            SiteState.Failed => "n/a",
            SiteState.Stopped => "–",
            _ => Count.ToString(),
        };

        public string ToolTip => State switch
        {
            SiteState.Searching => $"Searching {Domain}…",
            SiteState.Failed => Error ?? "Failed",
            SiteState.Stopped => "Stopped before it answered",
            SiteState.Empty => "No results",
            _ => $"{Count} results. Click to show only these.",
        };

        public void Apply(clsGames result)
        {
            int found = result.Data?.Count() ?? 0;
            Count = found;
            if (!result.isSuccess)
            {
                Error = result.ErrorMessage;
                State = SiteState.Failed;
            }
            else
            {
                State = found > 0 ? SiteState.Done : SiteState.Empty;
            }
        }
    }
}
