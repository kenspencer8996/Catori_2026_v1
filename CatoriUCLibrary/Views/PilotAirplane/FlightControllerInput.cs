using System.Runtime.InteropServices;
namespace CatoriUCLibrary.Views.Flight;
public static class FlightControllerInput
{
    public static FlightInput Read()
    {
        try
        {
            for (uint index = 0; index < 4; index++)
            {
                if (GetState(index, out var state) == 0)
                {
                    return new FlightInput(Normalize(state.Pad.RightX), -Normalize(state.Pad.RightY), Normalize(state.Pad.LeftY), (state.Pad.Buttons & 0x2000) != 0);
                }
            }
        }
        catch (DllNotFoundException)
        {
        }
        catch (EntryPointNotFoundException)
        {
        }
        return default;
    }
    private static double Normalize(short value)
    {
        const double deadZone = 8500;
        double magnitude = Math.Abs((double)value);
        return magnitude <= deadZone ? 0 : Math.Sign(value) * Math.Min(1, (magnitude - deadZone) / (32767 - deadZone));
    }
    [DllImport("xinput1_4.dll", EntryPoint = "XInputGetState")]
    private static extern uint GetState(uint index, out FlightControllerState state);
}
