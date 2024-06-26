using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace MemoryMosaic;

public static class Startup {
    private static readonly AnsiConsoleTheme CustomTheme = new(new Dictionary<ConsoleThemeStyle, string> {
        // [ConsoleThemeStyle.Text] = "\x1b[37m", // Default text color
        // [ConsoleThemeStyle.SecondaryText] = "\x1b[30m", // Dark gray
        // [ConsoleThemeStyle.TertiaryText] = "\x1b[90m", // Gray
        [ConsoleThemeStyle.Invalid] = "\x1b[31m", // Red
        [ConsoleThemeStyle.Null] = "\x1b[35m", // Magenta
        [ConsoleThemeStyle.Name] = "\x1b[36m", // Cyan
        [ConsoleThemeStyle.String] = "\x1b[32m", // Green
        [ConsoleThemeStyle.Number] = "\x1b[33m", // Yellow
        [ConsoleThemeStyle.Boolean] = "\x1b[33m", // Yellow
        [ConsoleThemeStyle.Scalar] = "\x1b[33m", // Yellow
        [ConsoleThemeStyle.LevelVerbose] = "\x1b[37m", // White
        [ConsoleThemeStyle.LevelDebug] = "\x1b[37m", // White
        [ConsoleThemeStyle.LevelInformation] = "\x1b[38;5;45m", // Cyan
        [ConsoleThemeStyle.LevelWarning] = "\x1b[33m", // Yellow
        [ConsoleThemeStyle.LevelError] = "\x1b[31m", // Red
        [ConsoleThemeStyle.LevelFatal] = "\x1b[31m", // Red
    });

    public static void ConfigureSerilogLogger() {
        // https://stackoverflow.com/questions/816566/how-do-you-get-the-current-project-directory-from-c-sharp-code-when-creating-a-c
        string workingDirectory = Environment.CurrentDirectory;
        string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;

        var logFilePath = Path.Combine(projectDirectory, "logs", $"{DateTime.Now:yyyy-MM-dd}-.log");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .Enrich.WithProcessId()
            .Enrich.WithProcessName()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.WithComputed("SourceContextName", "Substring(SourceContext, LastIndexOf(SourceContext, '.') + 1)")
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u5}] | {ProcessId} | {SourceContextName} | {Message:lj}{NewLine}{Exception}", theme: CustomTheme)
             .WriteTo.Async(a => a.File(
                logFilePath,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u5}] | {Message:lj}{NewLine}{Exception}",
                shared: true,
                 // https://stackoverflow.com/questions/60228026/serilog-how-to-customize-date-in-rolling-file-name 
                rollingInterval: RollingInterval.Day
                 ))
            .CreateLogger();
    }
}