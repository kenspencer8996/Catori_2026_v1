using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace CatoriApp.Game.Objects.AnimationOnPath;

/// <summary>A delay on WPF's existing animation clock, so playback pause also suspends pending handoffs.</summary>
public sealed class PlaybackDelay : IDisposable
{
    private readonly AnimationClock _clock;
    private readonly TimeSpan _duration;
    private readonly TaskCompletionSource _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly CancellationTokenRegistration _registration;
    private bool _disposed;
    public Task Completion => _completion.Task;

    public PlaybackDelay(TimeSpan duration, bool paused = false, CancellationToken cancellationToken = default)
    {
        _duration = duration;
        _clock = (AnimationClock)new DoubleAnimation(0, 1, duration).CreateClock(true);
        _clock.Completed += (_, _) => _completion.TrySetResult();
        _clock.Controller!.Begin();
        if (paused)
        {
            Pause();
        }
        var dispatcher = Dispatcher.CurrentDispatcher;
        _registration = cancellationToken.Register(() =>
        {
            if (dispatcher.CheckAccess())
            {
                Dispose();
            }
            else
            {
                dispatcher.BeginInvoke(new Action(Dispose));
            }
        });
    }

    public void Pause()
    {
        if (!_disposed)
        {
            _clock.Controller!.Pause();
        }
    }

    public void Resume()
    {
        if (!_disposed)
        {
            _clock.Controller!.Resume();
        }
    }

    public void Advance(TimeSpan elapsed)
    {
        if (_disposed)
        {
            return;
        }
        TimeSpan target = (_clock.CurrentTime ?? TimeSpan.Zero) + elapsed;
        _clock.Controller!.SeekAlignedToLastTick(target, TimeSeekOrigin.BeginTime);
        if (target >= _duration)
        {
            _completion.TrySetResult();
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        _clock.Controller!.Remove();
        _registration.Dispose();
        _completion.TrySetCanceled();
    }
}
