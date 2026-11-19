using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Room3D;

public sealed class RoomChildInstance
{
    public RoomChildModel Data { get; }
    public Model3DGroup Model { get; } = new();
    public ScaleTransform3D Size { get; } = new();
    public AxisAngleRotation3D Orientation { get; } = new(new Vector3D(0, 0, 1), 0);
    public TranslateTransform3D Position { get; } = new();

    public RoomChildInstance(RoomChildModel data)
    {
        Data = data;
        if (!double.IsFinite(data.Scale) || data.Scale <= 0 || !double.IsFinite(data.Rotation))
        {
            throw new ArgumentException("Child scale and rotation must be valid.");
        }
        var transform = new Transform3DGroup();
        Size.ScaleX = data.Scale;
        Size.ScaleY = data.Scale;
        Size.ScaleZ = data.Scale;
        transform.Children.Add(Size);
        Orientation.Angle = data.Rotation;
        transform.Children.Add(new RotateTransform3D(Orientation));
        transform.Children.Add(Position);
        Model.Transform = transform;
        Model.Children.Add(string.IsNullOrEmpty(data.VisualType) ? new Room3DModelLoader().Load(data.ModelPath, data.SourceYUp, 1) : RoomVisualCatalog.Create(data.VisualType, data.ShelfContents, data.Room));
        Position.OffsetX = data.X;
        Position.OffsetY = data.Y;
        Position.OffsetZ = data.Z;
    }
}
