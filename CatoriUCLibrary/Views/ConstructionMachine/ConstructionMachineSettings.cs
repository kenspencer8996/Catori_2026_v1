using System.Windows;
using System.Windows.Controls;
using CatoriUCLibrary.Views.VisualParts;

namespace CatoriUCLibrary.Views.ConstructionMachine;

public sealed class ConstructionMachineSettings
{
    public double DesignWidth { get; init; } = 300;
    public double DesignHeight { get; init; } = 220;
    public VisualPartConfiguration Base { get; init; } = CreatePart("Base");
    public VisualPartConfiguration? Boom { get; init; }
    public VisualPartConfiguration? Arm { get; init; }
    public VisualPartConfiguration? Bucket { get; init; }
    public double Raise1Angle { get; init; } = -30;
    public double Raise2Angle { get; init; } = -35;
    public double Raise3Angle { get; init; } = 25;

    private static VisualPartConfiguration CreatePart(string name)
    {
        return new VisualPartConfiguration
        {
            Name = name,
        };
    }
}
