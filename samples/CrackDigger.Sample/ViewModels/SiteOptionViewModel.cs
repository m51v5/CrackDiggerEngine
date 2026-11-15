using CommunityToolkit.Mvvm.ComponentModel;
using CrackDigger.Sample.Services;

namespace CrackDigger.Sample.ViewModels
{
    /// <summary>A site chip above the results: searched when selected.</summary>
    public partial class SiteOptionViewModel(SiteInfo info) : ObservableObject
    {
        public SiteInfo Info { get; } = info;

        public string Domain => Info.Domain;

        [ObservableProperty]
        private bool isSelected = true;
    }
}
