using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Airliner3D;

public partial class Airplane3DUC
{
    private RotationAxis? _dragAxis;
    private Vector3D _dragWorldAxis;
    private Vector3D _dragVector;
    private GeneralTransform? _dragCoordinates;
    private Window? _dragWindow;
    private Point _dragOrigin;
    public string RotationDragStatus { get; private set; } = "Drag a colored axis to rotate; Add saves, Cancel restores.";

    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        if (_rotationPreview is null || e.OriginalSource is not FrameworkElement { Tag: string tag }
            || !Enum.TryParse<RotationAxis>(tag, out var axis))
        {
            base.OnPreviewMouseLeftButtonDown(e);
            return;
        }
        e.Handled = true;
        _dragWindow = Window.GetWindow(this);
        _dragCoordinates = TransformToAncestor(_dragWindow).Inverse;
        _dragOrigin = new Point(RollAxisLine.X1, RollAxisLine.Y1);
        _dragWorldAxis = GetRotationAxis(axis);
        _dragWorldAxis.Normalize();
        if (TryDragVector(e, out _dragVector) && CaptureMouse())
        {
            _dragAxis = axis;
            RotationDragStatus = "Dragging " + axis;
        }
        else
        {
            RotationDragStatus = "This grab is too close to the rotation pole. Grab farther along the arm.";
        }
        RotationPreviewChanged?.Invoke(this, EventArgs.Empty);
    }

    private bool TryDragVector(MouseEventArgs e, out Vector3D vector)
    {
        Point point = _dragCoordinates!.Transform(e.GetPosition(_dragWindow));
        double dx = point.X - _dragOrigin.X;
        double dy = point.Y - _dragOrigin.Y;
        Vector displacement = new(dx, dy);
        return RotationTrackball.TryProject(displacement, _dragWorldAxis, FixedCamera.LookDirection, FixedCamera.UpDirection, out vector);
    }

    protected override void OnPreviewMouseMove(MouseEventArgs e)
    {
        if (_dragAxis is RotationAxis axis && e.LeftButton == MouseButtonState.Pressed && TryDragVector(e, out var vector))
        {
            double delta = RotationTrackball.Angle(_dragVector, vector, _dragWorldAxis);
            PreviewBodyRotation(axis, delta);
            _dragVector = vector;
            e.Handled = true;
        }
        base.OnPreviewMouseMove(e);
    }

    protected override void OnLostMouseCapture(MouseEventArgs e)
    {
        _dragAxis = null;
        base.OnLostMouseCapture(e);
    }

    protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        if (_dragAxis is not null)
        {
            _dragAxis = null;
            ReleaseMouseCapture();
            e.Handled = true;
        }
        base.OnPreviewMouseLeftButtonUp(e);
    }
}
