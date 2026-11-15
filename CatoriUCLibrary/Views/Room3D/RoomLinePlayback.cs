using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Room3D;

public sealed class RoomLinePlayback
{
    private readonly HashSet<RoomChildInstance> running = new();

    public void Run(RoomChildInstance instance, RoomNamedLine line)
    {
        if (!double.IsFinite(line.Duration) || line.Duration < 0.05 || line.Duration > 3600)
        {
            throw new ArgumentException("Duration must be 0.05–3600 seconds.");
        }
        Stop();
        Animate(instance.Position, TranslateTransform3D.OffsetXProperty, line.StartX, line.EndX, line.Duration);
        Animate(instance.Position, TranslateTransform3D.OffsetYProperty, line.StartY, line.EndY, line.Duration);
        running.Add(instance);
    }

    public void Stop()
    {
        foreach (var instance in running)
        {
            double x = instance.Position.OffsetX;
            double y = instance.Position.OffsetY;
            instance.Position.BeginAnimation(TranslateTransform3D.OffsetXProperty, null);
            instance.Position.BeginAnimation(TranslateTransform3D.OffsetYProperty, null);
            instance.Position.OffsetX = x;
            instance.Position.OffsetY = y;
        }
        running.Clear();
    }

    private static void Animate(TranslateTransform3D target, DependencyProperty property, double start, double end, double seconds)
    {
        var animation = new DoubleAnimationUsingKeyFrames { Duration = TimeSpan.FromSeconds(seconds), FillBehavior = FillBehavior.HoldEnd };
        animation.KeyFrames.Add(new LinearDoubleKeyFrame(start, KeyTime.FromTimeSpan(TimeSpan.Zero)));
        animation.KeyFrames.Add(new LinearDoubleKeyFrame(end, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(seconds))));
        target.BeginAnimation(property, animation, HandoffBehavior.SnapshotAndReplace);
    }
}
