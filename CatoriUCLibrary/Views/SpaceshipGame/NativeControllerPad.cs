using System.Runtime.InteropServices;

namespace CatoriUCLibrary.Views.SpaceshipGame.GameObjects;

[StructLayout(LayoutKind.Sequential)]
internal struct NativeControllerPad
{

    internal ushort Buttons;

    internal byte LeftTrigger;

    internal byte RightTrigger;

    internal short X;

    internal short Y;

    internal short RightX;

    internal short RightY;

}

