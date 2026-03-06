
using System.Diagnostics;

public sealed class PhysicsDragController
{
    private readonly CancellationTokenSource _cts = new();
    private readonly Canvas _canvas;

    private Action<Point> _setPosition;
    private Point _current;
    private Point _velocity;

    public PhysicsDragController(Canvas canvas, Action<Point> setPosition)
    {
        _canvas = canvas;
        _setPosition = setPosition;
    }

    public void SetTarget(Point target)
    {
        // You can add smoothing or velocity logic here
        _velocity = new Point(target.X - _current.X, target.Y - _current.Y);
    }

    //public async void Start()
    //{
    //    var token = _cts.Token;

    //    while (!token.IsCancellationRequested)
    //    {
    //        Debug.WriteLine("Physics update on thread: " + Thread.CurrentThread.ManagedThreadId);

    //        // Simple velocity integration
    //        _current.X += Convert.ToInt32( _velocity.X * 0.25);
    //        _current.Y += Convert.ToInt32(_velocity.Y * 0.25);

    //        if (token.IsCancellationRequested)
    //            break;

    //        await _canvas.Dispatcher.BeginInvoke(() =>
    //        {
    //            if (!token.IsCancellationRequested)
    //                _setPosition(_current);
    //        });

    //        if (token.IsCancellationRequested)
    //            break;

    //        try
    //        {
    //            await Task.Delay(1, token);
    //        }
    //        catch (TaskCanceledException)
    //        {
    //            break;
    //        }
    //    }
    //}
    public void Start(Point initialPosition)
    {
        _current = initialPosition; // ← THIS FIXES THE JUMP
        _velocity = new Point(0, 0);
        var token = _cts.Token;

        Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                Debug.WriteLine("Physics update on thread: " + Thread.CurrentThread.ManagedThreadId);  // Physics update on background thread
                _current.X += _velocity.X * 0.25;
                _current.Y += _velocity.Y * 0.25;

                // UI update on dispatcher
                await _canvas.Dispatcher.BeginInvoke(() =>
                {
                    if (!token.IsCancellationRequested)
                        _setPosition(_current);
                });

                await Task.Delay(16, token); // ~60 FPS
            }
        }, token);
    }

    public void Stop()
    {
        // 1. Cancel loop immediately
        _cts.Cancel();

        // 2. Disable all future UI updates
        _setPosition = _ => { };
    }
}
