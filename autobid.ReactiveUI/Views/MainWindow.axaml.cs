using autobid.ReactiveUI.ViewModels;
using Avalonia.Controls;

namespace autobid.ReactiveUI.Views
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			DataContext = new MainWindowViewModel();
		}
	}
}