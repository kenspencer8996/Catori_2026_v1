using System.Windows;
namespace CatoriShared.AnimationHelpers;
/// <summary>Crossing-based, metadata-free keyframe execution. Each direction is a traversal.</summary>
public sealed class PathKeyframeRunner
{
    private readonly PathKeyframeTrigger[] _frames;
    private readonly HashSet<int> _executed = [];
    private readonly bool _alternating;
    private long _traversal;
    private double _previous;
    private bool _reverse;
    public PathKeyframeRunner(IEnumerable<PathKeyframeTrigger> frames, bool alternating = false)
    {
        _alternating = alternating;
        _frames = frames.OrderBy(frame => frame.PositionPercent).ToArray();
        Reset();
    }
    public void Reset()
    {
        _traversal = 0;
        _previous = -double.Epsilon;
        _reverse = false;
        _executed.Clear();
    }
    public void Advance(double percent, long traversal, bool reverse, FrameworkElement control)
    {
        if (!double.IsFinite(percent) || traversal < _traversal)
        {
            return;
        }
        percent = Math.Clamp(percent, 0, 100);
        while (_traversal < traversal)
        {
            Cross(_reverse ? 0 : 100, control);
            _traversal++;
            _reverse = _traversal == traversal ? reverse : _alternating && _traversal % 2 == 1;
            _previous = _reverse ? double.PositiveInfinity : -double.Epsilon;
            _executed.Clear();
        }
        _reverse = reverse;
        Cross(percent, control);
    }
    public void Complete(FrameworkElement control)
    {
        Cross(_reverse ? 0 : 100, control);
    }
    private void Cross(double percent, FrameworkElement control)
    {
        double lower = Math.Min(_previous, percent);
        double upper = Math.Max(_previous, percent);
        for (int index = 0; index < _frames.Length; index++)
        {
            var frame = _frames[index];
            if (frame.PositionPercent >= lower && frame.PositionPercent <= upper && _executed.Add(index))
            {
                Invoke(frame, control);
            }
        }
        _previous = percent;
    }
    private static async void Invoke(PathKeyframeTrigger frame, FrameworkElement control)
    {
        try
        {
            await frame.Execute(control);
        }
        catch (OperationCanceledException)
        {
            // A superseded control animation may legitimately cancel its preceding action.
        }
        catch (Exception exception)
        {
            CatoriShared.Diagnostics.GameSafetyLog.Error("Keyframe", $"Action failed at {frame.PositionPercent}%.", exception);
        }
    }
}
