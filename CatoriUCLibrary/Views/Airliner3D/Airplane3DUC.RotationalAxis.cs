using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace CatoriUCLibrary.Views.Airliner3D;

public partial class Airplane3DUC
{
    public static readonly DependencyProperty ShowRotationalAxisProperty = DependencyProperty.Register(
        nameof(ShowRotationalAxis), typeof(bool), typeof(Airplane3DUC),
        new PropertyMetadata(false, RotationalAxisVisibilityChanged));

    public bool ShowRotationalAxis
    {
        get => (bool)GetValue(ShowRotationalAxisProperty);
        set => SetValue(ShowRotationalAxisProperty, value);
    }

    private static void RotationalAxisVisibilityChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
    {
        var airplane = (Airplane3DUC)control;
        if (airplane.RotationalAxisOverlay is not null)
        {
            airplane.RotationalAxisOverlay.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
            airplane.UpdateRotationalAxis();
        }
    }

    private void InitializeRotationalAxis()
    {
        AircraftModel.Transform.Changed += (_, _) => UpdateRotationalAxis();
        _pathOrientationCorrection.Changed += (_, _) => UpdateRotationalAxis();
        SizeChanged += (_, _) => UpdateRotationalAxis();
        FixedCamera.Changed += (_, _) => UpdateRotationalAxis();
        Loaded += (_, _) => UpdateRotationalAxis();
    }

    private void UpdateRotationalAxis()
    {
        if (!ShowRotationalAxis || RotationalAxisOverlay is null || ActualWidth <= 0 || ActualHeight <= 0)
        {
            return;
        }
        var transform = AircraftModel.Transform;
        Vector3D forward = transform.Transform(new Vector3D(1, 0, 0));
        Vector3D lateral = transform.Transform(new Vector3D(0, 1, 0));
        Vector3D vertical = transform.Transform(new Vector3D(0, 0, 1));
        forward.Normalize();
        lateral.Normalize();
        vertical.Normalize();
        double outerAngle = (_pathRotation?.Angle ?? 0) + Math.Atan2(RenderTransform.Value.M12, RenderTransform.Value.M11) * 180 / Math.PI;
        double pitch = Pitch;
        double roll = Roll;
        double heading = Yaw;
        Vector3D cameraLook = FixedCamera.LookDirection;
        cameraLook.Normalize();
        Vector3D cameraRight = Vector3D.CrossProduct(cameraLook, FixedCamera.UpDirection);
        cameraRight.Normalize();
        Vector3D cameraUp = Vector3D.CrossProduct(cameraRight, cameraLook);
        cameraUp.Normalize();
        Vector3D offset = transform.Transform(new Point3D()) - FixedCamera.Position;
        double pixelsPerUnit = ActualWidth / FixedCamera.Width;
        Point origin = new(ActualWidth / 2 + Vector3D.DotProduct(offset, cameraRight) * pixelsPerUnit,
            ActualHeight / 2 - Vector3D.DotProduct(offset, cameraUp) * pixelsPerUnit);
        UpdateMomentArm(RollAxisLine, RollAxisArrow, RollAxisLabel, forward, origin, 110, outerAngle,
            "Roll: " + roll.ToString("+0.0;-0.0;0.0", CultureInfo.CurrentCulture) + "° (" + DescribeBank(roll) + ")");
        UpdateMomentArm(PitchAxisLine, PitchAxisArrow, PitchAxisLabel, GetRotationAxis(RotationAxis.Pitch), origin, 100, outerAngle,
            "Pitch: " + pitch.ToString("+0.0;-0.0;0.0", CultureInfo.CurrentCulture) + "°");
        UpdateMomentArm(HeadingAxisLine, HeadingAxisArrow, HeadingAxisLabel, GetRotationAxis(RotationAxis.Heading), origin, 155, outerAngle,
            "Screen heading: " + ScreenHeadingText + Environment.NewLine
            + "Physical yaw: " + heading.ToString("0.0", CultureInfo.CurrentCulture) + "°");
    }

    private void UpdateMomentArm(Line line, Polygon arrow, Border label, Vector3D axis, Point origin,
        double length, double outerAngle, string text)
    {
        Vector3D look = FixedCamera.LookDirection;
        look.Normalize();
        Vector3D right = Vector3D.CrossProduct(look, FixedCamera.UpDirection);
        right.Normalize();
        Vector3D up = Vector3D.CrossProduct(right, look);
        up.Normalize();
        Vector direction = new(Vector3D.DotProduct(axis, right), -Vector3D.DotProduct(axis, up));
        // Foreshortening is retained so a camera-facing axis is not shown as a full-length arm.
        Vector delta = direction * length;
        Point end = origin + delta;
        line.X1 = origin.X;
        line.Y1 = origin.Y;
        line.X2 = end.X;
        line.Y2 = end.Y;
        if (direction.LengthSquared > 0.000001)
        {
            direction.Normalize();
        }
        else
        {
            direction = new Vector(0, -1);
        }
        Vector normal = new(-direction.Y, direction.X);
        arrow.Points[0] = end;
        arrow.Points[1] = end - direction * 9 + normal * 4;
        arrow.Points[2] = end - direction * 9 - normal * 4;
        ((TextBlock)label.Child).Text = text;
        Point labelPosition = new(end.X + 6, end.Y - 10);
        if (ReferenceEquals(label, RollAxisLabel))
        {
            // Keep the label outside the aircraft viewport even when the roll
            // axis points into the camera and its projected arm becomes short.
            label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double clearance = Math.Sqrt(ActualWidth * ActualWidth + ActualHeight * ActualHeight) / 2 + 16;
            double radians = outerAngle * Math.PI / 180;
            double cosine = Math.Cos(radians);
            double sine = Math.Sin(radians);
            Vector screenDirection = new(direction.X * cosine - direction.Y * sine,
                direction.X * sine + direction.Y * cosine);
            double offsetX = screenDirection.X < 0 ? -label.DesiredSize.Width - 6 : 6;
            double offsetY = screenDirection.Y < 0 ? -label.DesiredSize.Height - 6 : 6;
            Vector localOffset = new(offsetX * cosine + offsetY * sine,
                -offsetX * sine + offsetY * cosine);
            labelPosition = origin + direction * clearance + localOffset;
        }
        Canvas.SetLeft(label, labelPosition.X);
        Canvas.SetTop(label, labelPosition.Y);
        ((RotateTransform)label.RenderTransform).Angle = -outerAngle;
    }
}
