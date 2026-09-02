using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace CatoriUCLibrary.Views.Vehicles;

public partial class VehicleTracked_UC : UserControl
{
    public VehicleTracked_UC()
    {
        InitializeComponent();
    }

    public void SpinTracks(double speed, TimeSpan duration)
    {
        var animation = new DoubleAnimation
        {
            By = speed * 24,
            Duration = duration,
            RepeatBehavior = RepeatBehavior.Forever
        };
        TrackTranslateTransform.BeginAnimation(System.Windows.Media.TranslateTransform.XProperty, animation);
    }

    public void StopTracks()
    {
        TrackTranslateTransform.BeginAnimation(System.Windows.Media.TranslateTransform.XProperty, null);
    }

    public void SetShovelAngle(double angle, TimeSpan duration)
    {
        ShovelRotateTransform.BeginAnimation(System.Windows.Media.RotateTransform.AngleProperty, new DoubleAnimation(angle, duration));
    }

    public void SetCraneAngle(double angle, TimeSpan duration)
    {
        CraneRotateTransform.BeginAnimation(System.Windows.Media.RotateTransform.AngleProperty, new DoubleAnimation(angle, duration));
    }
}
