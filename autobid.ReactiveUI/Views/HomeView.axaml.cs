using autobid.ReactiveUI.ViewModels;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace autobid.ReactiveUI.Views
{
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
			InitializeComponent();
            DataContext = new HomeViewModel();
        }
        public void SetUsername(string username)
            => this.FindControl<TextBlock>("WelcomeText")!.Text = $"Welcome, {username}";
    }
}
