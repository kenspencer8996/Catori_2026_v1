using System.Runtime.InteropServices;
namespace CatoriUCLibrary.Views.Flight;
[StructLayout(LayoutKind.Sequential)]
internal struct FlightControllerState
{
    internal uint Packet;
    internal FlightPad Pad;
}
