using Avalonia;
using autobid;                // <- vigtigt: App-klassen ligger i UI-projektet

internal static class Program
{
    public static void Main(string[] args) =>
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()      // <- autobid.App
                     .UsePlatformDetect()
                     .LogToTrace();
}