using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace autobid.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);  // runtime load
            ShowLogin();
        }

        public void ShowLogin()
        {
            var header = this.FindControl<TextBlock>("HeaderText");
            var host = this.FindControl<ContentControl>("MainContent");
            if (header != null) header.Text = "autobid — Login";
            if (host != null) host.Content = new LoginView();
        }

        public void ShowCreateUser()
        {
            var header = this.FindControl<TextBlock>("HeaderText");
            var host = this.FindControl<ContentControl>("MainContent");
            if (header != null) header.Text = "autobid — Create user";
            if (host != null) host.Content = new CreateUserView();
        }

        public void ShowHome(string username)
        {
            var header = this.FindControl<TextBlock>("HeaderText");
            var host = this.FindControl<ContentControl>("MainContent");
            if (header != null) header.Text = "autobid — Home";
            if (host != null)
            {
                var hv = new HomeView();
                hv.SetUsername(username);
                host.Content = hv;
            }
        }
    }
}
