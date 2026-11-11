using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace CatoriApp.Game.Objects.AnimationOnPath;

public sealed partial class PathAnimationHandle
{
    private bool _isPaused;
    private bool _hasSteppedSincePause;
    private Clock? _debugClock;
    private List<(Point Start, Point End, double Offset, double Length)>? _debugSegments;
    private bool _completionProcessed;
    private bool _seeking;
    private bool _reverseTraversal;
    private long _traversal;
    private PlaybackDelay? _startDelay;
    public event Action? PlaybackChanged;
    public bool IsPaused => _isPaused;
    public bool IsComplete => _completionProcessed;
    public bool IsActive => !_isDisposed && !_completionProcessed && (_isStarted || _isStartPending);
    private double? _debugPathLength;
    public double PathLength => _debugPathLength ??= CatoriShared.Keyframes.SceneKeyframeStore.PathLength(_path);
    public double CurrentDistance => (_isPaused && _hasSteppedSincePause
        ? Math.Clamp(_previousKeyframeProgress, 0, 1) : RenderedProgress()) * PathLength;


    private double RenderedProgress()
    {
        if (_canvas.FindName(_translateName) is not TranslateTransform translation || PathLength <= 0)
        {
            return Math.Clamp(_previousKeyframeProgress, 0, 1);
        }
        if (_debugSegments is null)
        {
            _debugSegments = new();
            double offset = 0;
            foreach (var figure in Geometry.Parse(_path).GetFlattenedPathGeometry().Figures)
            {
                Point previous = figure.StartPoint;
                void Add(Point end)
                {
                    double length = (end - previous).Length;
                    if (length > 0)
                    {
                        _debugSegments.Add((previous, end, offset, length));
                        offset += length;
                    }
                    previous = end;
                }
                foreach (var segment in figure.Segments)
                {
                    if (segment is LineSegment line)
                    {
                        Add(line.Point);
                    }
                    else if (segment is PolyLineSegment polyline)
                    {
                        foreach (Point end in polyline.Points)
                        {
                            Add(end);
                        }
                    }
                }
                if (figure.IsClosed)
                {
                    Add(figure.StartPoint);
                }
            }
        }
        Point position = new(translation.X, translation.Y);
        double best = double.PositiveInfinity;
        double distance = 0;
        double estimate = Math.Clamp(_debugClock?.CurrentProgress ?? _previousKeyframeProgress, 0, 1) * PathLength;
        foreach (var segment in _debugSegments)
        {
            Vector vector = segment.End - segment.Start;
            double fraction = Math.Clamp(Vector.Multiply(position - segment.Start, vector) / vector.LengthSquared, 0, 1);
            double separation = (position - (segment.Start + vector * fraction)).LengthSquared;
            double candidate = segment.Offset + segment.Length * fraction;
            if (separation < best - 0.000001 || (Math.Abs(separation - best) <= 0.000001 && Math.Abs(candidate - estimate) < Math.Abs(distance - estimate)))
            {
                best = separation;
                distance = candidate;
            }
        }
        return Math.Clamp(distance / PathLength, 0, 1);
    }

    private double ProgressAtTime(double time)
    {
        double acceleration = _options.AccelerationRatio;
        double deceleration = _options.DecelerationRatio;
        double rate = 1 / (1 - (acceleration + deceleration) / 2);
        if (acceleration > 0 && time < acceleration)
        {
            return rate * time * time / (2 * acceleration);
        }
        if (deceleration > 0 && time > 1 - deceleration)
        {
            return 1 - rate * (1 - time) * (1 - time) / (2 * deceleration);
        }
        return rate * (time - acceleration / 2);
    }

    public void StepForward(double distance, Action<TimeSpan>? advanceAnimations = null)
    {
        ThrowIfDisposed();
        if (!_isPaused || !IsActive || !double.IsFinite(distance) || distance <= 0)
        {
            throw new InvalidOperationException("Step requires paused playback and a finite distance greater than zero.");
        }
        double length = PathLength;
        if (length <= 0)
        {
            return;
        }
        if (_isStartPending)
        {
            TimeSpan delayStep = TimeSpan.FromSeconds(distance / length * _options.Duration.TotalSeconds);
            advanceAnimations?.Invoke(delayStep);
            _startDelay?.Advance(delayStep);
            PlaybackChanged?.Invoke();
            return;
        }
        double currentProgress = _hasSteppedSincePause ? _previousKeyframeProgress : RenderedProgress();
        double previous = _previousKeyframeProgress;
        double target = Math.Clamp(Math.Max(0, currentProgress) + (_reverseTraversal ? -distance : distance) / length, 0, 1);
        double low = 0;
        double high = 1;
        for (int index = 0; index < 52; index++)
        {
            double middle = (low + high) / 2;
            if (ProgressAtTime(middle) < target)
            {
                low = middle;
            }
            else
            {
                high = middle;
            }
        }
        double localTime = (low + high) / 2;
        double seconds = (_traversal + (_reverseTraversal ? 1 - localTime : localTime)) * _options.Duration.TotalSeconds;
        TimeSpan targetTime = TimeSpan.FromSeconds(seconds);
        TimeSpan elapsed = targetTime - (Storyboard.GetCurrentTime(_canvas) ?? TimeSpan.Zero);
        if (elapsed < TimeSpan.Zero)
        {
            elapsed = TimeSpan.Zero;
        }
        advanceAnimations?.Invoke(elapsed);
        _seeking = true;
        try
        {
            Storyboard.SeekAlignedToLastTick(_canvas, targetTime, TimeSeekOrigin.BeginTime);
        }
        finally
        {
            _seeking = false;
        }
        if (Host.Content is FrameworkElement control)
        {
            _keyframes?.Advance(target * 100, _traversal, _reverseTraversal, control);
        }
        _previousKeyframeProgress = target;
        _hasSteppedSincePause = true;
        _options.ProgressChanged?.Invoke(previous, target);
        double total = _options.RepeatBehavior.HasCount
            ? _options.Duration.TotalSeconds * (_options.AutoReverse ? 2 : 1) * _options.RepeatBehavior.Count
            : _options.RepeatBehavior.HasDuration ? _options.RepeatBehavior.Duration.TotalSeconds : double.PositiveInfinity;
        if (seconds >= total - 0.000001)
        {
            StoryBoardComplete();
        }
        else if ((!_reverseTraversal && target >= 1) || (_reverseTraversal && target <= 0))
        {
            _traversal++;
            _reverseTraversal = _options.AutoReverse && _traversal % 2 == 1;
        }
        PlaybackChanged?.Invoke();
    }
}
