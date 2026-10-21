using System.Runtime.InteropServices;

namespace CatoriUCLibrary.Views.SuperMario.GameObjects;

[StructLayout(LayoutKind.Sequential)]
public struct MarioPad {
 public ushort Buttons;
 public byte LeftTrigger, RightTrigger;
 public short LX, LY, RX, RY;
 }

