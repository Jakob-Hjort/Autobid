using autobid.ReactiveUI.Views;
using Avalonia.Controls.Chrome;
using ReactiveUI;

namespace autobid.ReactiveUI.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		private static MainWindowViewModel _currentViewModel { get; set; } = null!;

		public static void ChangeContent(ViewModelBase? content)
		{

			_currentViewModel.Content = content;
			_currentViewModel.Title = content.Title;
		}

		object? _content = new LoginViewModel();
		public object? Content
		{
			get => _content;
			set => this.RaiseAndSetIfChanged(ref _content, value, nameof(Content));
		}

		public MainWindowViewModel() : base("Main window")
		{
			_currentViewModel = this;
		}

       
    }
}
