using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace autobid.ReactiveUI.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            AvaloniaXamlLoader.Load(this);

            // hook events in code
            //this.FindControl<Button>("LoginButton")!.Click += OnLoginClick;
            //this.FindControl<Button>("CreateUserButton")!.Click += OnGoCreateClick;
        }

        //private void OnGoCreateClick(object? s, RoutedEventArgs e)
        //    => (this.VisualRoot as MainWindow)?.ShowCreateUser();

        //private void OnLoginClick(object? s, RoutedEventArgs e)
        //{
        //    var username = this.FindControl<TextBox>("UsernameBox")?.Text ?? "";
        //    var password = this.FindControl<TextBox>("PasswordBox")?.Text ?? "";
        //    var error = this.FindControl<TextBlock>("ErrorText");

        //    error!.Text = "";
        //    var user = MockUsers.Login(username.Trim(), password);
        //    if (user is null)
        //    {
        //        error.Text = "Forkert brugernavn eller kode.";
        //        return;
        //    }
        //    (this.VisualRoot as MainWindow)?.ShowHome(user.Username);
        //}
    }
}
