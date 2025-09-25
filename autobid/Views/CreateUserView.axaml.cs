using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using autobid.Services;

namespace autobid.Views
{
    public partial class CreateUserView : UserControl
    {
        public CreateUserView()
        {
            AvaloniaXamlLoader.Load(this);

            this.FindControl<Button>("CreateButton")!.Click += OnCreateClick;
            this.FindControl<Button>("BackButton")!.Click += OnBackClick;
        }

        private void OnBackClick(object? s, RoutedEventArgs e)
            => (this.VisualRoot as MainWindow)?.ShowLogin();

        private void OnCreateClick(object? s, RoutedEventArgs e)
        {
            var uBox = this.FindControl<TextBox>("UsernameBox");
            var pBox = this.FindControl<TextBox>("PasswordBox");
            var rBox = this.FindControl<TextBox>("RepeatBox");
            var corpBx = this.FindControl<CheckBox>("CorporateBox");
            var error = this.FindControl<TextBlock>("ErrorText");

            var u = (uBox?.Text ?? "").Trim();
            var p = pBox?.Text ?? "";
            var r = rBox?.Text ?? "";
            var corp = corpBx?.IsChecked == true;

            error!.Text = "";
            if (u.Length < 2) { error.Text = "Ugyldigt brugernavn."; return; }
            if (p.Length < 4) { error.Text = "Kode skal være mindst 4 tegn."; return; }
            if (p != r) { error.Text = "Passwords matcher ikke."; return; }

            if (!MockUsers.TryCreate(u, p, corp))
            {
                error.Text = "Brugernavn findes allerede.";
                return;
            }
            (this.VisualRoot as MainWindow)?.ShowLogin();
        }
    }
}
