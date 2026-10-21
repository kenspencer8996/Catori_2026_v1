namespace CatoriUCLibrary.Views.Flight;
public sealed class FlightModel
{
    public double Bank { get; private set; }
    public double Pitch { get; private set; }
    public double Throttle { get; private set; } = 0.55;
    public double Speed { get; private set; } = 100;
    public double Altitude { get; private set; } = 1800;
    public double Heading { get; private set; } = 270;
    public void Update(double seconds, FlightInput input)
    {
        double dt = Math.Clamp(seconds, 0, 0.05);
        Throttle = Math.Clamp(Throttle + input.Throttle * dt * 0.35, 0, 1);
        double desiredBank = Math.Clamp(input.Bank, -1, 1) * 55;
        double desiredPitch = Math.Clamp(input.Pitch, -1, 1) * 25;
        Bank += (desiredBank - Bank) * Math.Min(1, dt * 3);
        Pitch += (desiredPitch - Pitch) * Math.Min(1, dt * 2);
        Speed += (45 + Throttle * 145 - Speed) * Math.Min(1, dt * 0.5);
        Altitude = Math.Max(10, Altitude + Math.Sin(Pitch * Math.PI / 180) * Speed * 0.514444 * dt);
        Heading = (Heading + Math.Tan(Bank * Math.PI / 180) * dt * 12 + 360) % 360;
    }
}
