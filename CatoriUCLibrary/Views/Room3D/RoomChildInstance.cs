using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Room3D;

public sealed class RoomChildInstance
{
    public RoomChildModel Data { get; }
    public Model3DGroup Model { get; } = new();
    public TranslateTransform3D Position { get; } = new();

    public RoomChildInstance(RoomChildModel data)
    {
        Data = data;
        if (!double.IsFinite(data.Scale) || data.Scale <= 0 || !double.IsFinite(data.Rotation))
        {
            throw new ArgumentException("Child scale and rotation must be valid.");
        }
        var transform = new Transform3DGroup();
        transform.Children.Add(new ScaleTransform3D(data.Scale, data.Scale, data.Scale));
        transform.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), data.Rotation)));
        transform.Children.Add(Position);
        Model.Transform = transform;
        Model.Children.Add(new Room3DModelLoader().Load(data.ModelPath, data.SourceYUp, 1));
        Position.OffsetX = data.X;
        Position.OffsetY = data.Y;
        Position.OffsetZ = data.Z;
    }
}
