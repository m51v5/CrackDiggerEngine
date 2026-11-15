using System.Windows;
using System.Windows.Input;
using CrackDigger.Sample.ViewModels;

namespace CrackDigger.Sample.Views
{
    public partial class AboutWindow : Window
    {
        public AboutWindow(UpdateState update)
        {
            InitializeComponent();
            DataContext = new AboutViewModel(update);
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, (_, _) => Close()));
        }

        // Borderless, so the whole window drags.
        private void Drag_Window(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}
