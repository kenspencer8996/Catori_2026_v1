using CatoriInterfaces;

namespace CatoriApp.Game.Objects.AnimationOnPath;

public sealed partial class PathAnimationSession
{
    private readonly HashSet<IAnimationPlaybackControl> _debugControls = new();
    private readonly List<PlaybackDelay> _debugDelays = new();
    private double _secondsPerUnit = 0.01;
    public bool IsPaused { get; private set; }
    public event Action? PlaybackChanged;
    public PathAnimationHandle? ActiveHandle => _handles.FirstOrDefault(handle => handle.IsActive);
    public bool HasPlayback => ActiveHandle is not null || _debugDelays.Count > 0
        || (IsPaused && _handles.Any(handle => handle.IsComplete));

    public void RegisterPlaybackControl(IAnimationPlaybackControl control)
    {
        _debugControls.Add(control);
        if (IsPaused)
        {
            control.PauseAnimations();
        }
    }

    private void HandlePlaybackChanged()
    {
        PlaybackChanged?.Invoke();
    }

    public void Pause()
    {
        IsPaused = true;
        foreach (var handle in _handles)
        {
            handle.Pause();
        }
        foreach (var control in _debugControls)
        {
            control.PauseAnimations();
        }
        foreach (var delay in _debugDelays)
        {
            delay.Pause();
        }
        PlaybackChanged?.Invoke();
    }

    public void Resume()
    {
        IsPaused = false;
        foreach (var control in _debugControls)
        {
            control.ResumeAnimations();
        }
        foreach (var handle in _handles)
        {
            handle.Resume();
        }
        foreach (var delay in _debugDelays)
        {
            delay.Resume();
        }
        PlaybackChanged?.Invoke();
    }

    public void StepForward(double distance)
    {
        if (!IsPaused || !double.IsFinite(distance) || distance <= 0)
        {
            throw new InvalidOperationException("Step requires paused playback and a finite distance greater than zero.");
        }
        var handle = ActiveHandle;
        if (handle is not null)
        {
            double actualStep = Math.Min(distance, Math.Max(0, handle.PathLength - handle.CurrentDistance));
            handle.StepForward(distance, elapsed =>
            {
                _secondsPerUnit = elapsed.TotalSeconds / Math.Max(actualStep, 0.000001);
                AdvanceControls(elapsed);
            });
        }
        else
        {
            AdvanceControls(TimeSpan.FromSeconds(distance * _secondsPerUnit));
        }
        PlaybackChanged?.Invoke();
    }

    private void AdvanceControls(TimeSpan elapsed)
    {
        foreach (var control in _debugControls)
        {
            control.AdvanceAnimations(elapsed);
        }
        foreach (var delay in _debugDelays.ToArray())
        {
            delay.Advance(elapsed);
        }
    }

    public async Task DelayAsync(TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
        {
            return;
        }
        var dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
        var delay = new PlaybackDelay(duration, IsPaused);
        _debugDelays.Add(delay);
        PlaybackChanged?.Invoke();
        try
        {
            await delay.Completion;
        }
        catch (OperationCanceledException)
        {
            // Session clear cancels pending work; callers still validate their captured handles.
        }
        finally
        {
            await dispatcher.InvokeAsync(() =>
            {
                _debugDelays.Remove(delay);
                delay.Dispose();
                PlaybackChanged?.Invoke();
            });
        }
    }
}
