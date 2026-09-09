public sealed class PhysicsDragController
{
    private readonly CancellationTokenSource _cts = new();
    private readonly Canvas _canvas;

    private Action<Point> _setPosition;
    private Point _current;
    private Point _target;

    public PhysicsDragController(Canvas canvas, Action<Point> setPosition)
    {
        _canvas = canvas;
        _setPosition = setPosition;
    }

    public void SetTarget(Point target)
    {
        _target = target;
    }

    public void Start(Point initialPosition)
    {
        _current = initialPosition;
        _target = initialPosition;

        var token = _cts.Token;

        Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                // Smooth spring movement
                _current.X += (_target.X - _current.X) * 0.35;
                _current.Y += (_target.Y - _current.Y) * 0.35;

                await _canvas.Dispatcher.InvokeAsync(() =>
                {
                    if (!token.IsCancellationRequested)
                        _setPosition(_current);
                });

                await Task.Delay(16, token);
            }
        }, token);
    }

    public void Stop()
    {
        _cts.Cancel();
        _setPosition = _ => { };
    }
}
