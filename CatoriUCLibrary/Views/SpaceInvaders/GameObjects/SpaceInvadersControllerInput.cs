using System.Runtime.InteropServices;

namespace CatoriUCLibrary.Views.SpaceInvaders.GameObjects;

internal static class SpaceInvadersControllerInput
{
    // Native XInput masks differ from Windows.Gaming.Input.GamepadButtons.
    internal const ushort DPadLeft = 0x0004;
    internal const ushort DPadRight = 0x0008;
    internal const ushort A = 0x1000;
    private const short DeadZone = 7849;
    private const byte TriggerThreshold = 30;

    [StructLayout(LayoutKind.Sequential)]
    internal struct Gamepad
    {
        internal ushort Buttons;
        internal byte LeftTrigger;
        internal byte RightTrigger;
        internal short ThumbLX;
        internal short ThumbLY;
        internal short ThumbRX;
        internal short ThumbRY;

        internal readonly bool Left => ThumbLX < -DeadZone || (Buttons & DPadLeft) != 0;
        internal readonly bool Right => ThumbLX > DeadZone || (Buttons & DPadRight) != 0;
        internal readonly bool Fire => (Buttons & A) != 0
            || LeftTrigger > TriggerThreshold
            || RightTrigger > TriggerThreshold;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct State
    {
        internal uint PacketNumber;
        internal Gamepad Gamepad;
    }

    [DllImport("xinput1_4.dll", ExactSpelling = true)]
    private static extern uint XInputGetState(uint userIndex, out State state);

    internal static Gamepad Read()
    {
        // Poll all four slots: controllers can reconnect on a different slot.
        for (uint index = 0; index < 4; index++)
        {
            if (XInputGetState(index, out State state) == 0)
                return state.Gamepad;
        }
        // Disconnection releases every control.
        return default;
    }
}
