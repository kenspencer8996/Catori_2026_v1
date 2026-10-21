using System.Runtime.InteropServices;
namespace CatoriUCLibrary.Views.Flight;
[StructLayout(LayoutKind.Sequential)]
internal struct FlightPad
{
    internal ushort Buttons;
    internal byte LeftTrigger;
    internal byte RightTrigger;
    internal short LeftX;
    internal short LeftY;
    internal short RightX;
    internal short RightY;
}
