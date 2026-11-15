using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Room3D;

public static class Room3DMaterialFallback
{
    public static int Apply(Model3D model)
    {
        if (model is Model3DGroup group)
        {
            return group.Children.Sum(Apply);
        }
        if (model is not GeometryModel3D geometry || HasDiffuse(geometry.Material))
        {
            return 0;
        }
        geometry.Material = AddDiffuse(geometry.Material);
        geometry.BackMaterial = geometry.BackMaterial is null ? geometry.Material : HasDiffuse(geometry.BackMaterial) ? geometry.BackMaterial : AddDiffuse(geometry.BackMaterial);
        return 1;
    }

    private static bool HasDiffuse(Material? material)
    {
        return material switch
        {
            DiffuseMaterial { Brush: ImageBrush image } => image.ImageSource is not null,
            DiffuseMaterial { Brush: not null } => true,
            EmissiveMaterial { Brush: not null } => true,
            MaterialGroup group => group.Children.Any(HasDiffuse),
            _ => false
        };
    }

    private static Material AddDiffuse(Material? source)
    {
        var material = new MaterialGroup();
        material.Children.Add(new DiffuseMaterial(Brushes.LightSteelBlue));
        if (source is not null)
        {
            material.Children.Add(source);
        }
        return material;
    }
}
