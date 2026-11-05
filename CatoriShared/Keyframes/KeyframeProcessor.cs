namespace CatoriShared.Keyframes;
// Prototype processor: distance crossing -> event -> controller dispatch.
public sealed class KeyframeProcessor
{
    public event Action<KeyframeAction>? KeyframeActionTriggered;
    private readonly Keyframes _keyframes;
    private double _pathLength;
    private readonly HashSet<SceneKeyframeEntity> _executed = new();
    public KeyframeProcessor(Keyframes keyframes, double pathLength)
    {
        _keyframes = keyframes;
        _pathLength = pathLength;
    }
    public bool HasFrames => _keyframes.Frames.Count > 0;
    public void SetPathLength(double length)
    {
        _pathLength = length;
        Reset();
    }
    public void Reset()
    {
        _executed.Clear();
    }
    public void ProcessKeyframes(double previousProgress, double progress)
    {
        if (!double.IsFinite(previousProgress) || !double.IsFinite(progress))
        {
            return;
        }
        if (previousProgress < 0)
        {
            Reset();
        }
        double previousDistance = previousProgress < 0 ? double.NegativeInfinity : previousProgress * _pathLength;
        double currentDistance = progress * _pathLength;
        foreach (var keyframe in _keyframes.Frames.OrderBy(frame => frame.KeyframeDistance))
        {
            if (currentDistance > previousDistance
                && keyframe.KeyframeDistance > previousDistance
                && keyframe.KeyframeDistance <= currentDistance
                && _executed.Add(keyframe))
            {
                foreach (var action in keyframe.Actions)
                {
                    KeyframeActionTriggered?.Invoke(action);
                }
            }
        }
    }
}
