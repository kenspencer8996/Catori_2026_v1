using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Threading;

namespace CatoriUCLibrary.Views.Airliner3D;

/// <summary>Owns the render subscription and cancellation lifetime; never constructs UI.</summary>
public sealed class PathFlightPlayer : IDisposable
{
    private readonly Dispatcher dispatcher;
    private readonly Action<FlightPose> update;
    private TaskCompletionSource? completion;
    private CancellationTokenRegistration cancellation;
    private bool subscribed;
    private long lastTick;
    private bool disposed;
    public PathFlightSimulation Simulation { get; }
    public bool IsPaused => completion != null && !subscribed;
    public bool IsActive => completion != null;

    public PathFlightPlayer(PathFlightSimulation simulation, Action<FlightPose> update, Dispatcher dispatcher)
    {
        Simulation = simulation;
        this.update = update;
        this.dispatcher = dispatcher;
    }

    public Task RunAsync(CancellationToken token)
    {
        dispatcher.VerifyAccess();
        ObjectDisposedException.ThrowIf(disposed, this);
        if (completion != null)
        {
            throw new InvalidOperationException("This path is already active. Resume it or reset it first.");
        }
        token.ThrowIfCancellationRequested();
        completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var task = completion.Task;
        cancellation = token.Register(() => dispatcher.BeginInvoke(() => Finish(true)));
        try
        {
            update(Simulation.Pose);
            Resume();
        }
        catch (Exception exception)
        {
            Finish(false, exception);
        }
        return task;
    }

    public void Pause()
    {
        dispatcher.VerifyAccess();
        if (subscribed)
        {
            CompositionTarget.Rendering -= OnRendering;
            subscribed = false;
        }
    }

    public void Resume()
    {
        dispatcher.VerifyAccess();
        if (completion == null || subscribed)
        {
            return;
        }
        lastTick = Stopwatch.GetTimestamp();
        CompositionTarget.Rendering += OnRendering;
        subscribed = true;
    }

    private void OnRendering(object? sender, EventArgs e)
    {
        long now = Stopwatch.GetTimestamp();
        double seconds = Stopwatch.GetElapsedTime(lastTick, now).TotalSeconds;
        lastTick = now;
        // A suspended/minimized window must resume from its last pose, not teleport across the scene.
        if (seconds > .25)
        {
            return;
        }
        try
        {
            update(Simulation.Advance(seconds));
            if (Simulation.Progress >= 1)
            {
                Finish(false);
            }
        }
        catch (Exception exception)
        {
            Finish(false, exception);
        }
    }

    private void Finish(bool canceled, Exception? error = null)
    {
        Pause();
        cancellation.Dispose();
        var pending = completion;
        completion = null;
        if (error != null)
        {
            pending?.TrySetException(error);
        }
        else if (canceled)
        {
            pending?.TrySetCanceled();
        }
        else
        {
            pending?.TrySetResult();
        }
    }

    public void Dispose()
    {
        dispatcher.VerifyAccess();
        if (!disposed)
        {
            Finish(true);
            disposed = true;
        }
    }
}
