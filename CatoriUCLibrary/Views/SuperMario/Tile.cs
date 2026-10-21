using System.Windows;

namespace CatoriUCLibrary.Views.SuperMario.GameObjects;

public sealed class Tile(int x, int y, char kind) {
 public double X = x * 40, Y = y * 40;
 public char Kind = kind;
 public Rect Bounds => new(X,Y,40,40);
 }

