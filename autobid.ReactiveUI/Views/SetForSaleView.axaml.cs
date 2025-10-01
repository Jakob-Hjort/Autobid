using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using autobid.ReactiveUI.ViewModels;

namespace autobid.ReactiveUI.Views;

public partial class SetForSaleView : ReactiveUserControl<SetForSaleViewModel>
{
    public SetForSaleView() => InitializeComponent();
    private void InitializeComponent() => AvaloniaXamlLoader.Load(this); // robust loader
}
