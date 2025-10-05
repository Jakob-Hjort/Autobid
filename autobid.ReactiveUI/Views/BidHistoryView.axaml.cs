using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace autobid.ReactiveUI.Views
{
    // Arv fra ReactiveUserControl<TViewModel> når du bruger ReactiveUI
    public partial class BidHistoryView : ReactiveUserControl<autobid.ReactiveUI.ViewModels.BidHistoryViewModel>
    {
        public BidHistoryView()
        {
            InitializeComponent(); // <-- generatorens metode, ingen manuel AvaloniaXamlLoader her
        }
    }
}
