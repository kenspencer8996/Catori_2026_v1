using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace CatoriUCLibrary.Views.Room3D;

public class Shelf3D : ModelVisual3D
{
    private const double BoardThickness = 0.08;
    private readonly Model3DGroup frame = new();
    private readonly Model3DGroup root = new();
    private readonly Model3DGroup contents = new();
    private readonly AxisAngleRotation3D orientation = new(new Vector3D(0, 0, 1), 0);
    private readonly List<ShelfItem> items = new();
    private static readonly DependencyPropertyKey ItemCountPropertyKey = DependencyProperty.RegisterReadOnly(nameof(ItemCount), typeof(int), typeof(Shelf3D), new PropertyMetadata(0));
    public static readonly DependencyProperty ItemCountProperty = ItemCountPropertyKey.DependencyProperty;
    public int ItemCount => (int)GetValue(ItemCountProperty);
    public static readonly DependencyProperty ShelfWidthProperty = DependencyProperty.Register(nameof(ShelfWidth), typeof(double), typeof(Shelf3D), new PropertyMetadata(1.6, DimensionsChanged), value => ValidDimension(value, 0.6, 10));
    public static readonly DependencyProperty ShelfHeightProperty = DependencyProperty.Register(nameof(ShelfHeight), typeof(double), typeof(Shelf3D), new PropertyMetadata(2.1, DimensionsChanged), value => ValidDimension(value, 0.9, 5));
    public static readonly DependencyProperty ShelfDepthProperty = DependencyProperty.Register(nameof(ShelfDepth), typeof(double), typeof(Shelf3D), new PropertyMetadata(0.6, DimensionsChanged), value => ValidDimension(value, 0.25, 3));
    public static readonly DependencyProperty ShelfColorProperty = DependencyProperty.Register(nameof(ShelfColor), typeof(Color), typeof(Shelf3D), new PropertyMetadata(Color.FromRgb(174, 122, 69), ColorChanged));

    public Color ShelfColor
    {
        get => (Color)GetValue(ShelfColorProperty);
        set => SetValue(ShelfColorProperty, value);
    }

    public double RotationDegrees => orientation.Angle;

    public double ShelfWidth
    {
        get => (double)GetValue(ShelfWidthProperty);
        set => SetValue(ShelfWidthProperty, value);
    }

    public double ShelfHeight
    {
        get => (double)GetValue(ShelfHeightProperty);
        set => SetValue(ShelfHeightProperty, value);
    }

    public double ShelfDepth
    {
        get => (double)GetValue(ShelfDepthProperty);
        set => SetValue(ShelfDepthProperty, value);
    }

    public Shelf3D()
    {
        root.Children.Add(frame);
        root.Children.Add(contents);
        root.Transform = new RotateTransform3D(orientation);
        Content = root;
        RebuildFrame();
    }

    public void AddItem(Model3D item, int shelfIndex)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (shelfIndex < 0 || shelfIndex >= 3)
        {
            throw new ArgumentOutOfRangeException(nameof(shelfIndex), "Shelf indices are 0 (bottom), 1 (middle) and 2 (top).");
        }
        if (items.Any(entry => ReferenceEquals(entry.Source, item)))
        {
            throw new ArgumentException("This model is already on the shelf. Use a separate model instance for another item.", nameof(item));
        }
        if (items.Count(entry => entry.Level == shelfIndex) >= 12)
        {
            throw new InvalidOperationException("This prototype supports up to 12 items per level.");
        }
        var snapshot = item.CloneCurrentValue();
        var bounds = snapshot.Bounds;
        if (bounds.IsEmpty || !double.IsFinite(bounds.X + bounds.Y + bounds.Z + bounds.SizeX + bounds.SizeY + bounds.SizeZ) || bounds.SizeX <= 0 || bounds.SizeY <= 0 || bounds.SizeZ <= 0)
        {
            throw new ArgumentException("Items need nonempty, finite 3D bounds on all axes.", nameof(item));
        }
        snapshot.Freeze();
        var visual = new Model3DGroup();
        visual.Children.Add(snapshot);
        var entry = new ShelfItem(item, visual, bounds, shelfIndex);
        items.Add(entry);
        contents.Children.Add(entry.Visual);
        LayoutItems();
        SetValue(ItemCountPropertyKey, items.Count);
    }

    public bool RemoveItem(Model3D item)
    {
        ArgumentNullException.ThrowIfNull(item);
        var entry = items.FirstOrDefault(entry => ReferenceEquals(entry.Source, item));
        if (entry is null)
        {
            return false;
        }
        items.Remove(entry);
        contents.Children.Remove(entry.Visual);
        LayoutItems();
        SetValue(ItemCountPropertyKey, items.Count);
        return true;
    }

    public bool RemoveLastItem()
    {
        return items.Count > 0 && RemoveItem(items[^1].Source);
    }

    public void ClearItems()
    {
        items.Clear();
        contents.Children.Clear();
        SetValue(ItemCountPropertyKey, 0);
    }

    public void SetShelfColor(Color color)
    {
        ShelfColor = color;
    }

    public void Rotate(double degrees)
    {
        if (!double.IsFinite(degrees))
        {
            throw new ArgumentOutOfRangeException(nameof(degrees));
        }
        orientation.Angle = (orientation.Angle + degrees) % 360;
    }

    public int GetItemCount(int shelfIndex)
    {
        if (shelfIndex < 0 || shelfIndex >= 3)
        {
            throw new ArgumentOutOfRangeException(nameof(shelfIndex));
        }
        return items.Count(entry => entry.Level == shelfIndex);
    }

    private void LayoutItems()
    {
        double innerWidth = ShelfWidth - 2 * BoardThickness - 0.08;
        double innerDepth = ShelfDepth - 0.08;
        for (int level = 0; level < 3; level++)
        {
            var levelItems = items.Where(entry => entry.Level == level).ToArray();
            if (levelItems.Length == 0)
            {
                continue;
            }
            double gap = Math.Min(0.04, innerWidth / (4 * levelItems.Length));
            double cellWidth = (innerWidth - gap * (levelItems.Length - 1)) / levelItems.Length;
            double ceiling = level == 2 ? ShelfHeight - BoardThickness - 0.04 : BoardTop(level + 1) - BoardThickness - 0.04;
            double usableHeight = ceiling - BoardTop(level) - 0.01;
            for (int index = 0; index < levelItems.Length; index++)
            {
                var entry = levelItems[index];
                var bounds = entry.Bounds;
                double scale = Math.Min(1, Math.Min(cellWidth / bounds.SizeX, Math.Min(innerDepth / bounds.SizeY, usableHeight / bounds.SizeZ)));
                double x = -innerWidth / 2 + cellWidth / 2 + index * (cellWidth + gap);
                var transform = new Transform3DGroup();
                transform.Children.Add(new TranslateTransform3D(-bounds.X - bounds.SizeX / 2, -bounds.Y - bounds.SizeY / 2, -bounds.Z));
                transform.Children.Add(new ScaleTransform3D(scale, scale, scale));
                transform.Children.Add(new TranslateTransform3D(x, 0, BoardTop(level) + 0.01));
                entry.Visual.Transform = transform;
            }
        }
    }

    private void RebuildFrame()
    {
        var material = new DiffuseMaterial(new SolidColorBrush(ShelfColor));
        material.Freeze();
        frame.Children.Clear();
        frame.Children.Add(Box(new Point3D(-(ShelfWidth - BoardThickness) / 2, 0, ShelfHeight / 2), BoardThickness, ShelfDepth, ShelfHeight, material));
        frame.Children.Add(Box(new Point3D((ShelfWidth - BoardThickness) / 2, 0, ShelfHeight / 2), BoardThickness, ShelfDepth, ShelfHeight, material));
        for (int level = 0; level < 3; level++)
        {
            frame.Children.Add(Box(new Point3D(0, 0, BoardTop(level) - BoardThickness / 2), ShelfWidth - 2 * BoardThickness, ShelfDepth, BoardThickness, material));
        }
        frame.Children.Add(Box(new Point3D(0, 0, ShelfHeight - BoardThickness / 2), ShelfWidth, ShelfDepth, BoardThickness, material));
    }

    private double BoardTop(int shelfIndex)
    {
        return 0.12 + shelfIndex * (ShelfHeight - 0.18) / 3;
    }

    private static Model3D Box(Point3D center, double width, double depth, double height, Material material)
    {
        var mesh = new HelixToolkit.Geometry.MeshBuilder(false, false);
        mesh.AddBox(new System.Numerics.Vector3((float)center.X, (float)center.Y, (float)center.Z), (float)width, (float)depth, (float)height);
        var geometry = new MeshGeometry3D { Positions = new Point3DCollection(mesh.Positions.Select(p => new Point3D(p.X, p.Y, p.Z))), TriangleIndices = new Int32Collection(mesh.TriangleIndices) };
        var model = new GeometryModel3D(geometry, material) { BackMaterial = material };
        model.Freeze();
        return model;
    }

    private static bool ValidDimension(object value, double minimum, double maximum)
    {
        return value is double dimension && double.IsFinite(dimension) && dimension >= minimum && dimension <= maximum;
    }

    private static void DimensionsChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
        ((Shelf3D)target).RebuildFrame();
        ((Shelf3D)target).LayoutItems();
    }

    private static void ColorChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
        ((Shelf3D)target).RebuildFrame();
    }
}
