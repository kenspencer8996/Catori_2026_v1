using System.Windows;

namespace CatoriUCLibrary.Views.SuperMario.GameObjects;

public sealed class Enemy(double x, double y) {
 public double X=x,Y=y,VX=-55;
 public bool Alive=true;
 public Rect Bounds => new(X,Y,30,28);
 }

