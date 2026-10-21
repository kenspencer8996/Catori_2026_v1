using System.Runtime.InteropServices;

namespace CatoriUCLibrary.Views.Pacman.GameObjects;

[StructLayout(LayoutKind.Sequential)]
public struct PacmanState
{

 public uint Packet;

 public PacmanPad Pad;

}

