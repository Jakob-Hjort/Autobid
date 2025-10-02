using autobid.ReactiveUI.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace autobid.ReactiveUI.Views
{
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
			AvaloniaXamlLoader.Load(this);
        }
        public void SetUsername(string username)
            => this.FindControl<TextBlock>("WelcomeText")!.Text = $"Welcome, {username}";

		protected async override void OnLoaded(RoutedEventArgs e)
		{
			base.OnLoaded(e);

            if (DataContext is HomeViewModel viewModel)
                await viewModel.LoadAuctions();
		}
    }
}
