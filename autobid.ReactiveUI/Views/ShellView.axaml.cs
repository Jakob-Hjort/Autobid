using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace autobid.ReactiveUI.Views
{
    public partial class ShellView : UserControl
    {
        public ShellView()
        {
            InitializeComponent();
        }

        // Brug runtime loader – fungerer som InitializeComponent
        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}
