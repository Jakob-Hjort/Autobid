using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;                // vores UI-services
using autobid.ViewModels;
using autobid.Views;

namespace autobid;

public partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {

            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindow()
            };
        }
        base.OnFrameworkInitializationCompleted();
    }
}