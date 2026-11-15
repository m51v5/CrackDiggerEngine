using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrackDigger.Sample.Services;
using static CrackDigger.CrackDiggerEngine;

namespace CrackDigger.Sample.ViewModels
{
    /// <summary>One result card: cover, title and site; opens the game's page.</summary>
    public partial class GameCardViewModel : ObservableObject
    {
        public GameCardViewModel(clsGameDataObject game, string domain)
        {
            Title = game.Title;
            Link = game.GameLink;
            Domain = domain;
            Initial = domain.Length > 0 ? char.ToUpperInvariant(domain[0]).ToString() : "?";

            if (!string.IsNullOrEmpty(game.ImageLink))
            {
                _ = LoadImageAsync(game.ImageLink);
            }
        }

        public string Title { get; }

        public string Link { get; }

        public string Domain { get; }

        /// <summary>Shown on the cover when the site gives no image.</summary>
        public string Initial { get; }

        public bool CanOpen => LinkOpener.IsWebLink(Link);

        public string ToolTip => CanOpen ? $"{Title}\n{Link}" : $"{Title}\nThe site gave no link.";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasImage))]
        private ImageSource? image;

        public bool HasImage => Image != null;

        [RelayCommand]
        private void Open() => LinkOpener.Open(Link);

        [RelayCommand]
        private void CopyLink() => LinkOpener.Copy(Link);

        private async Task LoadImageAsync(string url)
        {
            Image = await ImageLoader.LoadAsync(url);
        }
    }
}
