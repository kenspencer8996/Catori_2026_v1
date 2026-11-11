using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Airliner3D;

public partial class Airplane3DUC
{
    private FlapConfiguration? flapConfiguration;

    private void InitializeFlapConfiguration()
    {
        string path = System.IO.Path.Combine(AppContext.BaseDirectory, "flaps.json");
        if (System.IO.File.Exists(path))
        {
            ConfigureFlaps(FlapConfiguration.Load(path));
            return;
        }
        using var stream =
            typeof(Airplane3DUC).Assembly.GetManifestResourceStream(
                "CatoriUCLibrary.AirlinerFlaps.json"
            ) ?? throw new System.IO.InvalidDataException("Default flap configuration is missing.");
        var configuration =
            System.Text.Json.JsonSerializer.Deserialize<FlapConfiguration>(stream)
            ?? throw new System.IO.InvalidDataException("Default flap configuration is empty.");
        ConfigureFlaps(configuration);
    }

    public void LoadFlapConfiguration(string path)
    {
        ConfigureFlaps(FlapConfiguration.Load(path));
    }

    public FlapSetting CurrentFlapSetting { get; private set; }
    public double FlapSettingDegrees { get; private set; }

    public void ConfigureFlaps(FlapConfiguration configuration)
    {
        Dispatcher.VerifyAccess();
        ArgumentNullException.ThrowIfNull(configuration);
        configuration.Validate();
        flapConfiguration = configuration;
        SetFlaps(CurrentFlapSetting);
    }

    public void NoFlap()
    {
        SetFlaps(FlapSetting.NoFlap);
    }

    public void HalfFlap()
    {
        SetFlaps(FlapSetting.HalfFlap);
    }

    public void FullFlap()
    {
        SetFlaps(FlapSetting.FullFlap);
    }

    public void SetFlaps(FlapSetting setting)
    {
        Dispatcher.VerifyAccess();
        double degrees = setting switch
        {
            FlapSetting.NoFlap => 0,
            FlapSetting.HalfFlap => (
                flapConfiguration ?? throw new InvalidOperationException("Configure flaps first.")
            ).HalfFlapDegrees,
            FlapSetting.FullFlap => (
                flapConfiguration ?? throw new InvalidOperationException("Configure flaps first.")
            ).FullFlapDegrees,
            _ => throw new ArgumentOutOfRangeException(nameof(setting)),
        };
        ApplyFlaps(degrees);
        CurrentFlapSetting = setting;
    }

    public double FlapAngleDegrees => -LeftFlapRotation.Angle;

    public void SetFlaps(double degrees)
    {
        Dispatcher.VerifyAccess();
        if (degrees == 0)
            SetFlaps(FlapSetting.NoFlap);
        else if (degrees == flapConfiguration?.HalfFlapDegrees)
            SetFlaps(FlapSetting.HalfFlap);
        else if (degrees == flapConfiguration?.FullFlapDegrees)
            SetFlaps(FlapSetting.FullFlap);
        else
            throw new ArgumentOutOfRangeException(
                nameof(degrees),
                "Use NoFlap, HalfFlap or FullFlap with configured angles."
            );
    }

    private void ApplyFlaps(double degrees)
    {
        FlapSettingDegrees = degrees;
        AnimateFlap(LeftFlapRotation, -degrees);
        AnimateFlap(RightFlapRotation, degrees);
    }

    private void AnimateFlap(AxisAngleRotation3D rotation, double target)
    {
        var animation = new DoubleAnimation(rotation.Angle, target, TimeSpan.FromSeconds(0.6))
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut },
        };
        ApplyPlaybackAnimation(rotation, 
            AxisAngleRotation3D.AngleProperty,
            animation,
            HandoffBehavior.SnapshotAndReplace
        );
    }
}
