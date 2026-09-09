using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace CatoriUCLibrary.Views.Vehicles;

public partial class VehicleWheeled_UC : UserControl
{
    public VehicleWheeled_UC()
    {
        InitializeComponent();
    }

    public void SpinWheels(double revolutions, TimeSpan duration)
    {
        var animation = new DoubleAnimation
        {
            By = revolutions * 360,
            Duration = duration,
            RepeatBehavior = RepeatBehavior.Forever
        };
        LeftWheelRotateTransform.BeginAnimation(System.Windows.Media.RotateTransform.AngleProperty, animation);
        RightWheelRotateTransform.BeginAnimation(System.Windows.Media.RotateTransform.AngleProperty, animation);
    }

    public void StopWheels()
    {
        LeftWheelRotateTransform.BeginAnimation(System.Windows.Media.RotateTransform.AngleProperty, null);
        RightWheelRotateTransform.BeginAnimation(System.Windows.Media.RotateTransform.AngleProperty, null);
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
