using System.Runtime.InteropServices;

namespace CatoriUCLibrary.Views.Pacman.GameObjects;

[StructLayout(LayoutKind.Sequential)]
public struct PacmanPad
{

 public ushort Buttons;

 public byte LeftTrigger, RightTrigger;

 public short LeftX, LeftY, RightX, RightY;

}

