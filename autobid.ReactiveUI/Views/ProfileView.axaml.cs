using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using autobid.ReactiveUI.ViewModels;

namespace autobid.ReactiveUI.Views;
public partial class ProfileView : ReactiveUserControl<ProfileViewModel>
{
    public ProfileView() => InitializeComponent();
    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
