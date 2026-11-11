using System.Globalization;
using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Airliner3D;

public partial class Airplane3DUC
{
    /// <summary>Clockwise screen angle of the rendered nose, including pitch, camera and path-host rotation.</summary>
    public double? CurrentScreenHeading
    {
        get
        {
            var forward = AircraftModel.Transform.Transform(new Vector3D(1, 0, 0));
            if (forward.LengthSquared < double.Epsilon)
            {
                return null;
            }
            forward.Normalize();
            var look = FixedCamera.LookDirection;
            look.Normalize();
            var right = Vector3D.CrossProduct(look, FixedCamera.UpDirection);
            right.Normalize();
            var up = Vector3D.CrossProduct(right, look);
            up.Normalize();
            double x = Vector3D.DotProduct(forward, right);
            double y = -Vector3D.DotProduct(forward, up);
            if (x * x + y * y < 0.0000000001)
            {
                // A nose pointing along the view direction has no screen bearing.
                return null;
            }
            double outer = (_pathRotation?.Angle ?? 0)
                + Math.Atan2(RenderTransform.Value.M12, RenderTransform.Value.M11) * 180 / Math.PI;
            double heading = Math.Atan2(y, x) * 180 / Math.PI + outer;
            return (heading % 360 + 360) % 360;
        }
    }

    public string ScreenHeadingText => CurrentScreenHeading is double heading
        ? heading.ToString("0.0", CultureInfo.CurrentCulture) + "°" : "undefined (along view)";

    public string CurrentBankDirection
    {
        get
        {
            return DescribeBank(Roll);
        }
    }

    private static string DescribeBank(double roll)
    {
        if (Math.Abs(roll) < 0.05)
        {
            return "wings level";
        }
        if (Math.Abs(roll) > 179.95)
        {
            return "inverted";
        }
        return roll > 0 ? "right bank" : "left bank";
    }
}
