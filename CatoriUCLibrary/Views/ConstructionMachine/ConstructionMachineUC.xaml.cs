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

public partial class ConstructionMachineUC : UserControl
{
    private ConstructionMachineSettings _settings = new();

    public ConstructionMachineUC()
    {
        InitializeComponent();
    }

    public VisualPartAnimatorUC Animator => MachinePartAnimator;

    public void ApplySettings(ConstructionMachineSettings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        MachineRoot.Width = settings.DesignWidth;
        MachineRoot.Height = settings.DesignHeight;
        MachinePartAnimator.ClearParts();
        Configure(BasePartImage, settings.Base);
        ConfigureOptional(BoomPartImage, BoomPartRoot, settings.Boom);
        ConfigureOptional(ArmPartImage, ArmPartRoot, settings.Arm);
        ConfigureOptional(BucketPartImage, BucketPartRoot, settings.Bucket);
    }

    public void Move(double x, double y)
    {
        Canvas.SetLeft(this, x);
        Canvas.SetTop(this, y);
    }

    public Task Raise1(TimeSpan duration, CancellationToken cancellationToken = default)
    {
        return AnimateOptionalPart(_settings.Boom?.Name, _settings.Raise1Angle, duration, cancellationToken);
    }

    public Task Raise1()
    {
        return Raise1(TimeSpan.FromMilliseconds(300));
    }

    public Task Raise2(TimeSpan duration, CancellationToken cancellationToken = default)
    {
        return AnimateOptionalPart(_settings.Arm?.Name, _settings.Raise2Angle, duration, cancellationToken);
    }

    public Task Raise2()
    {
        return Raise2(TimeSpan.FromMilliseconds(300));
    }

    public Task Raise3(TimeSpan duration, CancellationToken cancellationToken = default)
    {
        return AnimateOptionalPart(_settings.Bucket?.Name, _settings.Raise3Angle, duration, cancellationToken);
    }

    public Task Raise3()
    {
        return Raise3(TimeSpan.FromMilliseconds(300));
    }

    public Task Lower1(TimeSpan duration, CancellationToken cancellationToken = default)
    {
        return AnimateToInitialAngle(_settings.Boom, duration, cancellationToken);
    }

    public Task Lower1()
    {
        return Lower1(TimeSpan.FromMilliseconds(300));
    }

    public Task Lower2(TimeSpan duration, CancellationToken cancellationToken = default)
    {
        return AnimateToInitialAngle(_settings.Arm, duration, cancellationToken);
    }

    public Task Lower2()
    {
        return Lower2(TimeSpan.FromMilliseconds(300));
    }

    public Task Lower3(TimeSpan duration, CancellationToken cancellationToken = default)
    {
        return AnimateToInitialAngle(_settings.Bucket, duration, cancellationToken);
    }

    public Task Lower3()
    {
        return Lower3(TimeSpan.FromMilliseconds(300));
    }

    public void Reset()
    {
        MachinePartAnimator.ResetParts();
    }

    private void Configure(Image image, VisualPartConfiguration configuration)
    {
        image.Visibility = Visibility.Visible;
        MachinePartAnimator.ConfigurePart(image, configuration);
    }

    private void ConfigureOptional(
        Image image,
        Canvas rotationTarget,
        VisualPartConfiguration? configuration
    )
    {
        if (configuration == null)
        {
            rotationTarget.Visibility = Visibility.Collapsed;
            return;
        }
        rotationTarget.Visibility = Visibility.Visible;
        MachinePartAnimator.ConfigurePart(image, configuration, rotationTarget);
        Canvas.SetLeft(image, 0);
        Canvas.SetTop(image, 0);
    }

    private Task AnimateOptionalPart(
        string? partName,
        double targetAngle,
        TimeSpan duration,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(partName) || !MachinePartAnimator.ContainsPart(partName))
        {
            return Task.CompletedTask;
        }
        VisualPartPose pose = new(new Dictionary<string, double>
        {
            [partName] = targetAngle,
        });
        return MachinePartAnimator.AnimateToPoseAsync(pose, duration, cancellationToken);
    }

    private Task AnimateToInitialAngle(
        VisualPartConfiguration? configuration,
        TimeSpan duration,
        CancellationToken cancellationToken
    )
    {
        return configuration == null
            ? Task.CompletedTask
            : AnimateOptionalPart(
                configuration.Name,
                configuration.InitialAngle,
                duration,
                cancellationToken
            );
    }
}
