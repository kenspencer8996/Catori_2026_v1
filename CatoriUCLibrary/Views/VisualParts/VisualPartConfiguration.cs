using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CatoriUCLibrary.Views.VisualParts;

public sealed class VisualPartConfiguration
{
    public required string Name { get; init; }
    public string? ImagePath { get; init; }
    public double X { get; init; }
    public double Y { get; init; }
    public double Width { get; init; }
    public double Height { get; init; }
    public double PivotX { get; init; }
    public double PivotY { get; init; }
    public double InitialAngle { get; init; }
    public int ZOrder { get; init; }
    public string? ParentPartName { get; init; }
}
