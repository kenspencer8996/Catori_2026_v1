using System.IO;
using Path = System.IO.Path;
using System.Text.Json;
using System.Windows.Threading;

namespace CatoriShared.Keyframes;

/// <summary>Reloads only the scene envelope; ActionJson remains opaque.</summary>
public sealed class SceneKeyframeWatcher : IDisposable
{
    private readonly string _file;
    private readonly Dispatcher _dispatcher;
    private readonly DispatcherTimer _timer;
    private readonly FileSystemWatcher? _watcher;
    private string? _lastJson;
    private int _attempts;
    private volatile bool _disposed;
    public Keyframes Current { get; private set; } = new();
    public event Action<Keyframes>? DefinitionsChanged;

    public SceneKeyframeWatcher(string definitionFile, Dispatcher dispatcher)
    {
        _file = Path.GetFullPath(definitionFile);
        _dispatcher = dispatcher;
        _dispatcher.VerifyAccess();
        _timer = new DispatcherTimer(DispatcherPriority.Background, dispatcher)
        {
            Interval = TimeSpan.FromMilliseconds(350)
        };
        _timer.Tick += ReloadTick;
        string directory = Path.GetDirectoryName(_file)!;
        string watchDirectory = Directory.Exists(directory) ? directory : Path.GetDirectoryName(directory)!;
        if (Directory.Exists(watchDirectory) && (string.Equals(directory, watchDirectory, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(watchDirectory, Path.GetPathRoot(watchDirectory), StringComparison.OrdinalIgnoreCase)))
        {
            _watcher = new FileSystemWatcher(watchDirectory, Path.GetFileName(_file))
            {
                IncludeSubdirectories = !string.Equals(directory, watchDirectory, StringComparison.OrdinalIgnoreCase),
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size
            };
            _watcher.Changed += FileChanged;
            _watcher.Created += FileChanged;
            _watcher.Deleted += FileChanged;
            _watcher.Renamed += FileRenamed;
            _watcher.Error += WatcherError;
            _watcher.EnableRaisingEvents = true;
        }
        TryReload();
        // Also covers a change between enabling the watcher and the first read.
        ScheduleReload();
    }

    private void FileChanged(object sender, FileSystemEventArgs e)
    {
        if (string.Equals(Path.GetFullPath(e.FullPath), _file, StringComparison.OrdinalIgnoreCase))
        {
            QueueReload();
        }
    }

    private void FileRenamed(object sender, RenamedEventArgs e)
    {
        if (string.Equals(Path.GetFullPath(e.FullPath), _file, StringComparison.OrdinalIgnoreCase)
            || string.Equals(Path.GetFullPath(e.OldFullPath), _file, StringComparison.OrdinalIgnoreCase))
        {
            QueueReload();
        }
    }

    private void WatcherError(object sender, ErrorEventArgs e)
    {
        QueueReload();
    }

    private void QueueReload()
    {
        if (!_disposed && !_dispatcher.HasShutdownStarted)
        {
            _dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                if (!_disposed)
                {
                    ScheduleReload();
                }
            }));
        }
    }

    private void ScheduleReload()
    {
        _timer.Stop();
        _attempts = 0;
        _timer.Start();
    }

    private void ReloadTick(object? sender, EventArgs e)
    {
        if (_disposed)
        {
            return;
        }
        if (TryReload() || ++_attempts >= 6)
        {
            _timer.Stop();
        }
    }

    private bool TryReload()
    {
        try
        {
            using var stream = new FileStream(_file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            using var reader = new StreamReader(stream);
            string json = reader.ReadToEnd();
            if (json == _lastJson)
            {
                return true;
            }
            var definitions = JsonSerializer.Deserialize<Keyframes>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? throw new InvalidDataException("Scene keyframe definition is null.");
            if (definitions.Frames is null || definitions.Frames.Any(frame => frame is null
                || !double.IsFinite(frame.KeyframeDistance) || frame.KeyframeDistance < 0
                || frame.Actions is null || frame.Actions.Any(action => action is null)))
            {
                throw new InvalidDataException("Invalid scene keyframe envelope.");
            }
            Current = definitions;
            _lastJson = json;
            DefinitionsChanged?.Invoke(definitions);
            return true;
        }
        catch (Exception exception) when (exception is IOException or InvalidDataException or UnauthorizedAccessException or JsonException)
        {
            if (_attempts == 5 && exception is not FileNotFoundException and not DirectoryNotFoundException)
            {
                CatoriShared.Diagnostics.GameSafetyLog.Warning("Keyframe reload", $"Keeping the last valid definitions for '{_file}': {exception.Message}");
            }
            return false;
        }
    }

    public void Dispose()
    {
        _dispatcher.VerifyAccess();
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        _timer.Stop();
        _timer.Tick -= ReloadTick;
        _watcher?.Dispose();
        DefinitionsChanged = null;
    }
}
