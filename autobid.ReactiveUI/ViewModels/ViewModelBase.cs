using ReactiveUI;
namespace autobid.ReactiveUI.ViewModels;

public class ViewModelBase : ReactiveObject
{
    string _title = "Autobid - Login";

    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);

    }
    public ViewModelBase(string title = "")
    {
        Title = title;
    }
}
