using System.Runtime.InteropServices;

namespace CatoriUCLibrary.Views.SpaceshipGame.GameObjects;

public static class ControllerInput
{

    public static ControllerReading Read()
    {

        for (uint index = 0;
 index < 4;
 index++)
        {

            try
            {

                if (GetState(index, out var state) == 0)
                {

                    var pad = state.Pad;

                    double x = Normalize(pad.X);

                    double y = -Normalize(pad.Y);

                    if ((pad.Buttons & 12) != 0)
                        x = ((pad.Buttons & 8) != 0 ? 1 : 0) - ((pad.Buttons & 4) != 0 ? 1 : 0);

                    if ((pad.Buttons & 3) != 0)
                        y = ((pad.Buttons & 2) != 0 ? 1 : 0) - ((pad.Buttons & 1) != 0 ? 1 : 0);

                    double length = Math.Sqrt(x * x + y * y);

                    if (length > 1)
                    {

                        x /= length;

                        y /= length;

                    }

                    return new(true, x, y, (pad.Buttons & 0x1000) != 0 || pad.RightTrigger > 30 || pad.LeftTrigger > 30, pad.Buttons);

                }

            }

            catch (DllNotFoundException) {
 return default;
 }

            catch (EntryPointNotFoundException) {
 return default;
 }

        }

        return default;

    }

    public static double Normalize(short value)
    {

        const double deadZone = 7849;

        double magnitude = Math.Abs((double)value);

        return magnitude <= deadZone ? 0 : Math.Sign(value) * Math.Min(1, (magnitude - deadZone) / (32767 - deadZone));

    }

    [DllImport("xinput1_4.dll", EntryPoint = "XInputGetState")]
    private static extern uint GetState(uint index, out NativeControllerState state);

}

