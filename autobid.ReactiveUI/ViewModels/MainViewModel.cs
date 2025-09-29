using ReactiveUI;

namespace autobid.ReactiveUI.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private static MainViewModel _currentViewModel { get; set; } = null!;

    public static void ChangeContent(object? content)
    {
        _currentViewModel.Content = content;
    }

    object? _content;
    public object? Content
    {
        get => _content;
        set => this.RaiseAndSetIfChanged(ref _content, value, nameof(Content));
    }

	public MainViewModel()
	{
        _currentViewModel = this;
	}
}
