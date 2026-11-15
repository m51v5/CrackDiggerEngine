using System.Windows;
using System.Windows.Media;
using CrackDigger.Sample.ViewModels;

namespace CrackDigger.Sample.Views
{
    public partial class MainWindow : Window
    {
        // Radius of the dark panel's bottom-right curve, where the gradient shows through.
        private const double CurveRadius = 160;

        private readonly MainViewModel viewModel = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = viewModel;

            // A new filter or a new search starts at the top of the results.
            viewModel.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName is nameof(MainViewModel.SelectedSite) or nameof(MainViewModel.IsSearching))
                {
                    resultsScroll.ScrollToTop();
                }
            };

            Loaded += async (_, _) =>
            {
                searchBox.Focus();
                await viewModel.StartAsync();
            };
        }

        // Cuts the panel's bottom-right corner round, clipping the results with it.
        private void Panel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double w = panel.ActualWidth, h = panel.ActualHeight;
            double r = Math.Min(CurveRadius, Math.Min(w, h));
            StreamGeometry shape = new();
            using (StreamGeometryContext c = shape.Open())
            {
                c.BeginFigure(new Point(0, 0), true, true);
                c.LineTo(new Point(w, 0), false, false);
                c.LineTo(new Point(w, h - r), false, false);
                c.ArcTo(new Point(w - r, h), new Size(r, r), 0, false, SweepDirection.Clockwise, false, false);
                c.LineTo(new Point(0, h), false, false);
            }
            shape.Freeze();
            panel.Clip = shape;
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow(viewModel.Update) { Owner = this }.ShowDialog();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void Maximize_Click(object sender, RoutedEventArgs e) =>
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}
