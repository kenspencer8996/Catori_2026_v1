using System.Windows;
namespace CatoriShared.AnimationHelpers;
public sealed class PathKeyframeTrigger
{
    public double PositionPercent { get; }
    public Func<FrameworkElement, Task> Execute { get; }
    public PathKeyframeTrigger(double positionPercent, Func<FrameworkElement, Task> execute)
    {
        if (!double.IsFinite(positionPercent) || positionPercent < 0 || positionPercent > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(positionPercent));
        }
        PositionPercent = positionPercent;
        Execute = execute ?? throw new ArgumentNullException(nameof(execute));
    }
}
