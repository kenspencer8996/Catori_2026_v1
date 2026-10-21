using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using CatoriUCLibrary.Views.Flight;
namespace CatoriUCLibrary.Views;
public partial class PilotCockpitWindow : Window
{
    private readonly DispatcherTimer timer = new() { Interval = TimeSpan.FromMilliseconds(16) };
    private readonly Stopwatch clock = new();
    private readonly HashSet<Key> keys = new();
    private double previous;
    private bool exitArmed;
    public FlightModel Flight { get; } = new();
    public PilotCockpitWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Closed += OnClosed;
        Deactivated += (_, _) => keys.Clear();
        timer.Tick += OnTick;
        UpdateInstruments();
    }
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        clock.Restart();
        previous = 0;
        timer.Start();
        Focus();
    }
    private void OnClosed(object? sender, EventArgs e)
    {
        timer.Stop();
        clock.Stop();
        keys.Clear();
    }
    private void OnTick(object? sender, EventArgs e)
    {
        double now = clock.Elapsed.TotalSeconds;
        double elapsed = now - previous;
        previous = now;
        if (!IsActive)
        {
            return;
        }
        var reading = FlightControllerInput.Read();
        if (!reading.Exit)
        {
            exitArmed = true;
        }
        if (reading.Exit && exitArmed)
        {
            Close();
            return;
        }
        double bank = keys.Contains(Key.Right) ? 1 : keys.Contains(Key.Left) ? -1 : reading.Bank;
        double pitch = keys.Contains(Key.Up) ? 1 : keys.Contains(Key.Down) ? -1 : reading.Pitch;
        double throttle = keys.Contains(Key.PageUp) ? 1 : keys.Contains(Key.PageDown) ? -1 : reading.Throttle;
        Flight.Update(elapsed, new FlightInput(bank, pitch, throttle, false));
        UpdateInstruments();
    }
    private void UpdateInstruments()
    {
        HorizonBank.Angle = -Flight.Bank;
        HorizonPitch.Y = Flight.Pitch * 8;
        SpeedText.Text = $"{Flight.Speed:0}";
        AltitudeText.Text = $"{Flight.Altitude:0}";
        HeadingText.Text = $"{Flight.Heading:000}";
        ThrottleText.Text = $"{Flight.Throttle:P0}";
        ThrottleGauge.Value = Flight.Throttle * 100;
        AttitudeText.Text = $"BANK {Flight.Bank:0}°   PITCH {Flight.Pitch:0}°";
    }
    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Close();
            e.Handled = true;
            return;
        }
        if (e.Key is Key.Left or Key.Right or Key.Up or Key.Down or Key.PageUp or Key.PageDown)
        {
            keys.Add(e.Key);
            e.Handled = true;
        }
    }
    private void OnKeyUp(object sender, KeyEventArgs e)
    {
        keys.Remove(e.Key);
    }
}
