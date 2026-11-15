using System.IO;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace CatoriUCLibrary.Views.Room3D;

public sealed class Room3DModelLoader
{
    public string? MaterialWarning { get; private set; }

    public Model3DGroup Load(string path, bool yUp, double longestSide)
    {
        MaterialWarning = null;
        if (!string.Equals(Path.GetExtension(path), ".obj", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Choose a Wavefront .obj file.");
        }
        var reader = new ObjReader { SwitchYZ = yUp, DefaultMaterial = new DiffuseMaterial(Brushes.LightSteelBlue), IgnoreErrors = false };
        var model = (reader.Read(path) ?? throw new InvalidDataException("OBJ has no usable geometry.")).CloneCurrentValue();
        int repaired = Room3DMaterialFallback.Apply(model);
        if (repaired > 0)
        {
            MaterialWarning = $"{repaired} mesh(es) had no diffuse material, often because textures are missing. A neutral material is used; keep textures at the OBJ/MTL's relative paths.";
        }
        var bounds = model.Bounds;
        double size = Math.Max(bounds.SizeX, Math.Max(bounds.SizeY, bounds.SizeZ));
        if (bounds.IsEmpty || !double.IsFinite(size) || size <= 0)
        {
            throw new InvalidDataException("OBJ has empty or nonfinite bounds.");
        }
        var transform = new Transform3DGroup();
        transform.Children.Add(new TranslateTransform3D(-bounds.X - bounds.SizeX / 2, -bounds.Y - bounds.SizeY / 2, -bounds.Z));
        double scale = longestSide / size;
        transform.Children.Add(new ScaleTransform3D(scale, scale, scale));
        var root = new Model3DGroup { Transform = transform };
        root.Children.Add(model);
        root.Freeze();
        return root;
    }
}
