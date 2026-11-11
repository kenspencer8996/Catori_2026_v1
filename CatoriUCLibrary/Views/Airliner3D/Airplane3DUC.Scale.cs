using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Airliner3D;

public partial class Airplane3DUC
{
    /// <summary>Animate to an absolute uniform scale relative to the normal airplane size.</summary>
    public void SetScale(double scale, TimeSpan duration)
    {
        Dispatcher.VerifyAccess();
        if (!double.IsFinite(scale) || scale <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(scale), "Scale must be finite and greater than zero.");
        }
        if (duration < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(duration));
        }
        double target = 1.5 * scale;
        if (!double.IsFinite(target))
        {
            throw new ArgumentOutOfRangeException(nameof(scale));
        }
        foreach (var property in new[] { ScaleTransform3D.ScaleXProperty, ScaleTransform3D.ScaleYProperty, ScaleTransform3D.ScaleZProperty })
        {
            double current = (double)AircraftScale.GetValue(property);
            ApplyPlaybackAnimation(AircraftScale, property, null);
            AircraftScale.SetValue(property, target);
            if (duration > TimeSpan.Zero)
            {
                ApplyPlaybackAnimation(AircraftScale, property, new DoubleAnimation(current, target, duration)
                {
                    FillBehavior = FillBehavior.Stop
                }, HandoffBehavior.SnapshotAndReplace);
            }
        }
    }
}

