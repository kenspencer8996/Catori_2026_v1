using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CatoriUCLibrary.Views.VisualParts;

public sealed record VisualPartAnimationStep(
    VisualPartPose Pose,
    TimeSpan Duration,
    TimeSpan HoldDuration
);
