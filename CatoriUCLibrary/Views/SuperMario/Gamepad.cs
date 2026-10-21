using System.Runtime.InteropServices;

namespace CatoriUCLibrary.Views.SuperMario.GameObjects;

// Polls every frame, supports all four XInput slots and reconnects automatically.
public sealed class Gamepad
{

    [DllImport("xinput1_4.dll", EntryPoint="XInputGetState")] private static extern uint Read(uint index, out MarioState state);

    [DllImport("xinput9_1_0.dll", EntryPoint="XInputGetState")] private static extern uint ReadLegacy(uint index, out MarioState state);

    private bool legacy;

    public bool Connected {
 get;
 private set;
 }

    public MarioPad Current {
 get;
 private set;
 }

    public void Poll()
    {

        Connected = false;
 Current = default;

        for (uint i = 0;
 i < 4;
 i++)
        {

            try
            {

                MarioState s;
 uint result;

                try {
 result = legacy ? ReadLegacy(i, out s) : Read(i, out s);
 }

                catch (DllNotFoundException) {
 legacy = true;
 result = ReadLegacy(i, out s);
 }

                if (result == 0) {
 Connected = true;
 Current = s.MarioPad;
 return;
 }

            }

            catch (DllNotFoundException) {
 return;
 }

            catch (EntryPointNotFoundException) {
 return;
 }

        }

    }

    public bool Down(ushort button) => (Current.Buttons & button) != 0;

    public bool Left => Down(0x4) || Current.LX < -8000;

    public bool Right => Down(0x8) || Current.LX > 8000;

}

