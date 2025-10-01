using autobid.ReactiveUI.Views;
using ReactiveUI;

namespace autobid.ReactiveUI.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		private static MainWindowViewModel _currentViewModel { get; set; } = null!;

		public static void ChangeContent(object? content)
		{
			_currentViewModel.Content = content;
		}

		object? _content = new LoginViewModel();
		public object? Content
		{
			get => _content;
			set => this.RaiseAndSetIfChanged(ref _content, value, nameof(Content));
		}

		public MainWindowViewModel()
		{
			_currentViewModel = this;
		}
	}
}
