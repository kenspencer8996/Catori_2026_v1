using System.Diagnostics;
using System.Text;

namespace CatoriShared.Diagnostics;

public static class GameSafetyLog
{
    private static readonly object Sync = new();
    private static readonly string LogDirectory = System.IO.Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Catori", "Logs");

    public static string CurrentLogPath => System.IO.Path.Combine(LogDirectory,
        $"game-{DateTime.Now:yyyyMMdd}.log");

    public static void Warning(string subsystem, string message)
    {
        Write("WARN", subsystem, message, null);
    }

    public static void Error(string subsystem, string message, Exception exception)
    {
        Write("ERROR", subsystem, message, exception);
    }

    private static void Write(string level, string subsystem, string message, Exception? exception)
    {
        var entry = new StringBuilder()
            .Append(DateTimeOffset.Now.ToString("O")).Append(" [").Append(level).Append("] [")
            .Append(subsystem).Append("] ").AppendLine(message);
        if (exception != null)
            entry.AppendLine(exception.ToString());

        string text = entry.ToString();
        Debug.Write(text);
        try
        {
            lock (Sync)
            {
                System.IO.Directory.CreateDirectory(LogDirectory);
                System.IO.File.AppendAllText(CurrentLogPath, text, Encoding.UTF8);
            }
        }
        catch (Exception logFailure)
        {
            Debug.WriteLine($"Catori logging failed: {logFailure}");
        }
    }
}
