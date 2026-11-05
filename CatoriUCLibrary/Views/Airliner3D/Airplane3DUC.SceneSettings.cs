using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Airliner3D;

public partial class Airplane3DUC
{
    private bool initialLandingGearRetracted;
    private bool alignWithPath = true;
    private bool preserveSizeOnPath = true;
    private double? sceneCameraWidth;
    private double sceneReferenceWidth;

    public bool InitialLandingGearRetracted
    {
        get => initialLandingGearRetracted;
        set
        {
            Dispatcher.VerifyAccess();
            _ = SetLandingGearAsync(!value, TimeSpan.Zero);
            initialLandingGearRetracted = value;
        }
    }

    public bool AlignWithPath
    {
        get => alignWithPath;
        set
        {
            Dispatcher.VerifyAccess();
            alignWithPath = value;
            UpdatePathOrientation();
        }
    }

    public bool PreserveSizeOnPath
    {
        get => preserveSizeOnPath;
        set
        {
            Dispatcher.VerifyAccess();
            preserveSizeOnPath = value;
            UpdateCameraView();
        }
    }

    public void ConfigureSceneCamera(double width)
    {
        Dispatcher.VerifyAccess();
        if (!double.IsFinite(width) || width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width));
        }
        sceneCameraWidth = width;
        sceneReferenceWidth = double.IsFinite(Width) && Width > 0 ? Width : SceneProjection.Width;
        CancelCameraTransition();
        SetValue(CameraElevationProperty, 0d);
        CurrentCameraPreset = AirplaneCameraPreset.Side;
        UpdateCameraView();
    }

    private double CameraViewWidth()
    {
        double width = sceneCameraWidth ?? SceneProjection.Width;
        if (
            sceneCameraWidth.HasValue
            && preserveSizeOnPath
            && Parent is ContentControl
            && ActualWidth > 0
        )
        {
            width *= ActualWidth / sceneReferenceWidth;
        }
        return width / zoom;
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        if (sceneCameraWidth.HasValue)
        {
            UpdateCameraView();
        }
    }
}
