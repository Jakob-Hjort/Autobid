using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace autobid.Views
{
    public partial class HomeView : UserControl
    {
        public HomeView() { AvaloniaXamlLoader.Load(this); }
        public void SetUsername(string username)
            => this.FindControl<TextBlock>("WelcomeText")!.Text = $"Welcome, {username}";
    }
}
