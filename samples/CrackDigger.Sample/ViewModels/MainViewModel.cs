using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrackDigger.Sample.Services;
using static CrackDigger.CrackDiggerEngine;

namespace CrackDigger.Sample.ViewModels
{
    /// <summary>The main window: search box, site chips, SITES list and result cards.</summary>
    public partial class MainViewModel : ObservableObject
    {
        // Bumped by each new search or stop, so late answers from an old search are dropped.
        private int generation;
        private bool hasSearched;
        private readonly Stopwatch clock = new();

        public MainViewModel()
        {
            SiteOptions = new ObservableCollection<SiteOptionViewModel>(SearchService.Sites.Select(site => new SiteOptionViewModel(site)));
            foreach (SiteOptionViewModel option in SiteOptions)
            {
                option.PropertyChanged += (_, _) => SearchCommand.NotifyCanExecuteChanged();
            }

            CardsView = CollectionViewSource.GetDefaultView(Cards);
            CardsView.Filter = item => SelectedSite == null || ((GameCardViewModel)item).Domain == SelectedSite.Domain;
            UpdateMessage();
        }

        public ObservableCollection<SiteOptionViewModel> SiteOptions { get; }

        public ObservableCollection<SiteStatusViewModel> SiteStatuses { get; } = new();

        public ObservableCollection<GameCardViewModel> Cards { get; } = new();

        public ICollectionView CardsView { get; }

        public UpdateState Update { get; } = new();

        public string LibraryLabel => $"Sample for CrackDiggerEngine {AppLinks.LibraryVersion}";

        public string RepositoryLabel => AppLinks.RepositoryShort;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
        private string keyword = "";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsAllSelected))]
        private SiteStatusViewModel? selectedSite;

        [ObservableProperty]
        private bool isSearching;

        [ObservableProperty]
        private string summary = "";

        [ObservableProperty]
        private string? message;

        public bool IsAllSelected => SelectedSite == null;

        /// <summary>Runs the quiet update check once, on launch.</summary>
        public Task StartAsync() => Update.CheckAsync();

        partial void OnSelectedSiteChanged(SiteStatusViewModel? value)
        {
            CardsView.Refresh();
            UpdateMessage();
        }

        private bool CanSearch() => !string.IsNullOrWhiteSpace(Keyword) && SiteOptions.Any(option => option.IsSelected);

        [RelayCommand(CanExecute = nameof(CanSearch))]
        private async Task Search()
        {
            int run = ++generation;
            string key = Keyword.Trim();
            hasSearched = true;

            Cards.Clear();
            SiteStatuses.Clear();
            SelectedSite = null;
            foreach (SiteOptionViewModel option in SiteOptions.Where(option => option.IsSelected))
            {
                SiteStatuses.Add(new SiteStatusViewModel(option.Info));
            }

            IsSearching = true;
            clock.Restart();
            UpdateSummary();
            UpdateMessage();

            // Each site on its own, so a slow site doesn't hold the others.
            await Task.WhenAll(SiteStatuses.ToList().Select(status => SearchSiteAsync(status, key, run)));

            if (run == generation)
            {
                Finish();
            }
        }

        [RelayCommand]
        private void Stop()
        {
            generation++;
            foreach (SiteStatusViewModel status in SiteStatuses.Where(status => status.State == SiteState.Searching))
            {
                status.State = SiteState.Stopped;
            }
            Finish();
        }

        [RelayCommand]
        private void ShowAll()
        {
            SelectedSite = null;
            // The All toggle flips itself on click; this puts it back in step.
            OnPropertyChanged(nameof(IsAllSelected));
        }

        [RelayCommand]
        private void SelectAllSites() => SetAllSites(true);

        [RelayCommand]
        private void SelectNoSites() => SetAllSites(false);

        [RelayCommand]
        private void OpenLink(string url) => LinkOpener.Open(url);

        private async Task SearchSiteAsync(SiteStatusViewModel status, string key, int run)
        {
            clsGames result = await SearchService.SearchAsync(status.Info.Site, key);
            if (run != generation)
            {
                return;
            }

            status.Apply(result);
            foreach (clsGameDataObject game in result.Data ?? [])
            {
                Cards.Add(new GameCardViewModel(game, status.Domain));
            }
            UpdateSummary();
            UpdateMessage();
        }

        private void Finish()
        {
            clock.Stop();
            IsSearching = false;
            UpdateSummary();
            UpdateMessage();
        }

        private void SetAllSites(bool selected)
        {
            foreach (SiteOptionViewModel option in SiteOptions)
            {
                option.IsSelected = selected;
            }
        }

        private void UpdateSummary()
        {
            int done = SiteStatuses.Count(status => status.State != SiteState.Searching);
            int withResults = SiteStatuses.Count(status => status.State == SiteState.Done);
            string results = Cards.Count == 1 ? "1 result" : $"{Cards.Count} results";

            Summary = IsSearching
                ? $"{results} so far · {done} of {SiteStatuses.Count} sites answered"
                : $"{results} from {withResults} {(withResults == 1 ? "site" : "sites")} · {clock.Elapsed.TotalSeconds:0.0} s";
        }

        private void UpdateMessage()
        {
            bool anyVisible = !CardsView.IsEmpty;
            if (anyVisible)
            {
                Message = null;
            }
            else if (!hasSearched)
            {
                Message = $"Search a game name across {SiteOptions.Count} sites.";
            }
            else if (IsSearching)
            {
                Message = "Searching…";
            }
            else if (SelectedSite != null)
            {
                Message = SelectedSite.State == SiteState.Failed ? $"{SelectedSite.Domain} failed: {SelectedSite.Error}" : $"No results from {SelectedSite.Domain}.";
            }
            else if (SiteStatuses.Count > 0 && SiteStatuses.All(status => status.State == SiteState.Failed))
            {
                Message = "No site answered. Check your connection.";
            }
            else
            {
                Message = $"No results for \"{Keyword.Trim()}\".";
            }
        }
    }
}
