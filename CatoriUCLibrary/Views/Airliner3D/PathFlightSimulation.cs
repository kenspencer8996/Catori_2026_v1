using System.Windows.Media;

namespace CatoriUCLibrary.Views.Airliner3D;

/// <summary>Deterministic simulation driven by elapsed seconds, independent of the UI render rate.</summary>
public sealed class PathFlightSimulation
{
    private readonly PathFlightSampler sampler;
    private double speed = 100;
    private double maximumBank = 30;
    public double Progress { get; private set; }
    public double Length => sampler.Length;
    public FlightPose Pose { get; private set; }
    public double Speed
    {
        get => speed;
        set
        {
            if (!double.IsFinite(value) || value <= 0 || value > 2000)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Speed must be between 0 and 2000 logical units/second.");
            }
            speed = value;
        }
    }
    public double MaximumBankAngle
    {
        get => maximumBank;
        set
        {
            if (!double.IsFinite(value) || value < 0 || value > 75)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Bank must be between 0 and 75 degrees.");
            }
            maximumBank = value;
        }
    }

    public PathFlightSimulation(PathGeometry path)
    {
        sampler = new PathFlightSampler(path);
        Pose = sampler.Sample(0, Speed, MaximumBankAngle);
    }

    public FlightPose Advance(double elapsedSeconds)
    {
        if (!double.IsFinite(elapsedSeconds) || elapsedSeconds < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(elapsedSeconds));
        }
        Progress = Math.Min(1, Progress + Speed * elapsedSeconds / sampler.Length);
        var next = sampler.Sample(Progress, Speed, MaximumBankAngle);
        Pose = next with { Heading = AngleMath.Unwrap(Pose.Heading, next.Heading) };
        return Pose;
    }
}
