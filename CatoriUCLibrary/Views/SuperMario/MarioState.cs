using System.Runtime.InteropServices;

namespace CatoriUCLibrary.Views.SuperMario.GameObjects;

[StructLayout(LayoutKind.Sequential)]
public struct MarioState {
 public uint Packet;
 public MarioPad MarioPad;
 }

