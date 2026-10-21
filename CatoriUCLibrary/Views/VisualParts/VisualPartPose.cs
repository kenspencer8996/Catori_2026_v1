using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CatoriUCLibrary.Views.VisualParts;

public sealed record VisualPartPose(
    IReadOnlyDictionary<string, double> PartAngles,
    IReadOnlyDictionary<string, Point>? PartPositions = null
);
