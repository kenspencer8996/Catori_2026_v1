using System.Windows.Shapes;

namespace CatoriUCLibrary.Views.SpaceshipGame.GameObjects;

public sealed class Projectile(Shape shape, double x, double y, double dx, double dy, bool friendly)
    {

        public Shape Shape {
 get;
 }
 = shape;

        public double X {
 get;
 set;
 }
 = x;

        public double Y {
 get;
 set;
 }
 = y;

        public double Dx {
 get;
 }
 = dx;

        public double Dy {
 get;
 }
 = dy;

        public bool Friendly {
 get;
 }
 = friendly;

    }

