using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using MeshBuilder = HelixToolkit.Geometry.MeshBuilder;
using System.Numerics;
using CatoriUCLibrary.Views.Room3D;

namespace CatoriUCLibrary.Controls;

public sealed class Room3D : ModelVisual3D
{
    public static readonly DependencyProperty WidthProperty = DependencyProperty.Register(nameof(Width), typeof(double), typeof(Room3D), new PropertyMetadata(8.0, GeometryChanged), value => value is double dimension && double.IsFinite(dimension) && dimension >= 0.5 && dimension <= 1000);
    public double Width
    {
        get => (double)GetValue(WidthProperty);
        set => SetValue(WidthProperty, value);
    }

    public static readonly DependencyProperty LengthProperty = DependencyProperty.Register(nameof(Length), typeof(double), typeof(Room3D), new PropertyMetadata(6.0, GeometryChanged), value => value is double dimension && double.IsFinite(dimension) && dimension >= 0.5 && dimension <= 1000);
    public double Length
    {
        get => (double)GetValue(LengthProperty);
        set => SetValue(LengthProperty, value);
    }

    public static readonly DependencyProperty HeightProperty = DependencyProperty.Register(nameof(Height), typeof(double), typeof(Room3D), new PropertyMetadata(3.0, GeometryChanged), value => value is double dimension && double.IsFinite(dimension) && dimension >= 0.5 && dimension <= 1000);
    public double Height
    {
        get => (double)GetValue(HeightProperty);
        set => SetValue(HeightProperty, value);
    }

    public static readonly DependencyProperty ShowNorthWallProperty = DependencyProperty.Register(nameof(ShowNorthWall), typeof(bool), typeof(Room3D), new PropertyMetadata(true, GeometryChanged));
    public bool ShowNorthWall
    {
        get => (bool)GetValue(ShowNorthWallProperty);
        set => SetValue(ShowNorthWallProperty, value);
    }

    public static readonly DependencyProperty ShowSouthWallProperty = DependencyProperty.Register(nameof(ShowSouthWall), typeof(bool), typeof(Room3D), new PropertyMetadata(true, GeometryChanged));
    public bool ShowSouthWall
    {
        get => (bool)GetValue(ShowSouthWallProperty);
        set => SetValue(ShowSouthWallProperty, value);
    }

    public static readonly DependencyProperty ShowEastWallProperty = DependencyProperty.Register(nameof(ShowEastWall), typeof(bool), typeof(Room3D), new PropertyMetadata(true, GeometryChanged));
    public bool ShowEastWall
    {
        get => (bool)GetValue(ShowEastWallProperty);
        set => SetValue(ShowEastWallProperty, value);
    }

    public static readonly DependencyProperty ShowWestWallProperty = DependencyProperty.Register(nameof(ShowWestWall), typeof(bool), typeof(Room3D), new PropertyMetadata(true, GeometryChanged));
    public bool ShowWestWall
    {
        get => (bool)GetValue(ShowWestWallProperty);
        set => SetValue(ShowWestWallProperty, value);
    }

    public static readonly DependencyProperty DoorProperty = DependencyProperty.Register(nameof(Door), typeof(RoomDoorSettings), typeof(Room3D), new PropertyMetadata(null, GeometryChanged));
    public RoomDoorSettings? Door
    {
        get => (RoomDoorSettings?)GetValue(DoorProperty);
        set => SetValue(DoorProperty, value);
    }

    public Room3D()
    {
        Rebuild();
    }

    private static void GeometryChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
        ((Room3D)target).Rebuild();
    }

    private void Rebuild()
    {
        Door?.Validate(Width, Length, Height);
        var model = new Model3DGroup();
        model.Children.Add(Box(0, 0, -0.08, Width, Length, 0.16, Brushes.Tan));
        if (ShowNorthWall)
        {
            AddWall(model, "North", Width, 0, Length / 2, false);
        }
        if (ShowSouthWall)
        {
            AddWall(model, "South", Width, 0, -Length / 2, false);
        }
        if (ShowEastWall)
        {
            AddWall(model, "East", Length, Width / 2, 0, true);
        }
        if (ShowWestWall)
        {
            AddWall(model, "West", Length, -Width / 2, 0, true);
        }
        Content = model;
    }

    private void AddWall(Model3DGroup model, string wall, double span, double x, double y, bool alongY)
    {
        if (Door is null || Door.Wall != wall)
        {
            model.Children.Add(Box(x, y, Height / 2, alongY ? 0.16 : span, alongY ? span : 0.16, Height, Brushes.LightSlateGray));
            return;
        }
        double side = (span - Door.Width) / 2;
        double offset = (span + Door.Width) / 4;
        model.Children.Add(Box(x - (alongY ? 0 : offset), y - (alongY ? offset : 0), Height / 2, alongY ? 0.16 : side, alongY ? side : 0.16, Height, Brushes.LightSlateGray));
        model.Children.Add(Box(x + (alongY ? 0 : offset), y + (alongY ? offset : 0), Height / 2, alongY ? 0.16 : side, alongY ? side : 0.16, Height, Brushes.LightSlateGray));
        model.Children.Add(Box(x, y, (Height + Door.Height) / 2, alongY ? 0.16 : Door.Width, alongY ? Door.Width : 0.16, Height - Door.Height, Brushes.LightSlateGray));
        var panel = Box(x, y, Door.Height / 2, alongY ? 0.06 : Door.Width - 0.03, alongY ? Door.Width - 0.03 : 0.06, Door.Height - 0.03, Brushes.SaddleBrown).Clone();
        if (Door.IsOpen)
        {
            double hingeX = x - (alongY ? 0 : Door.Width / 2);
            double hingeY = y - (alongY ? Door.Width / 2 : 0);
            double angle = wall is "North" or "West" ? -90 : 90;
            panel.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), angle), new Point3D(hingeX, hingeY, 0));
        }
        model.Children.Add(panel);
    }

    private static GeometryModel3D Box(double x, double y, double z, double width, double length, double height, Brush brush)
    {
        var mesh = new MeshBuilder(false, false);
        mesh.AddBox(new Vector3((float)x, (float)y, (float)z), (float)width, (float)length, (float)height);
        var geometry = new MeshGeometry3D
        {
            Positions = new Point3DCollection(mesh.Positions.Select(point => new Point3D(point.X, point.Y, point.Z))),
            TriangleIndices = new Int32Collection(mesh.TriangleIndices)
        };
        var material = new DiffuseMaterial(brush);
        var model = new GeometryModel3D(geometry, material) { BackMaterial = material };
        model.Freeze();
        return model;
    }
}
