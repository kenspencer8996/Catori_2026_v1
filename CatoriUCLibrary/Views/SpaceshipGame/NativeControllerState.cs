using System.Runtime.InteropServices;

namespace CatoriUCLibrary.Views.SpaceshipGame.GameObjects;

[StructLayout(LayoutKind.Sequential)]
internal struct NativeControllerState
{

    internal uint Packet;

    internal NativeControllerPad Pad;

}

