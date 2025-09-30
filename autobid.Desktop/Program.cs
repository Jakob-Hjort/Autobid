using autobid;                // <- vigtigt: App-klassen ligger i UI-projektet
using Avalonia;
using Avalonia;
using Avalonia.ReactiveUI;

internal static class Program
{
    public static void Main(string[] args) =>
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<autobid.ReactiveUI.App>()      // <- autobid.App
                     .UsePlatformDetect()
                     .LogToTrace()
                     .UseReactiveUI();
}