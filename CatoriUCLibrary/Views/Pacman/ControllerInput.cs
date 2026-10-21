using System.Runtime.InteropServices;

namespace CatoriUCLibrary.Views.Pacman.GameObjects;

public sealed class ControllerInput
{


    [DllImport("xinput1_4.dll", EntryPoint="XInputGetState")] private static extern uint GetState(uint index, out PacmanState state);

    public bool Connected {
 get;
 private set;
 }

    public int Direction {
 get;
 private set;
 }
 = -1;

    public bool PausePressed {
 get;
 private set;
 }

    public bool RestartPressed {
 get;
 private set;
 }

    private ushort previous;

    public void Poll()
    {

        Connected = false;
 Direction = -1;
 PausePressed = RestartPressed = false;

        try
        {

            for (uint i = 0;
 i < 4;
 i++)
            {

                if (GetState(i, out PacmanState state) != 0) continue;

                Connected = true;

                ushort b = state.Pad.Buttons;

                PausePressed = (b & 0x10) != 0 && (previous & 0x10) == 0;

                RestartPressed = (b & 0x1000) != 0 && (previous & 0x1000) == 0;

                previous = b;

                if ((b & 1) != 0) Direction = 0;

                else if ((b & 8) != 0) Direction = 1;

                else if ((b & 2) != 0) Direction = 2;

                else if ((b & 4) != 0) Direction = 3;

                else if (Math.Max(Math.Abs((int)state.Pad.LeftX), Math.Abs((int)state.Pad.LeftY)) > 9000)
                    Direction = Math.Abs((int)state.Pad.LeftX) > Math.Abs((int)state.Pad.LeftY) ? (state.Pad.LeftX > 0 ? 1 : 3) : (state.Pad.LeftY > 0 ? 0 : 2);

                return;

            }

        }

        catch (DllNotFoundException) {
 }

        catch (EntryPointNotFoundException) {
 }

        previous = 0;

    }

}

