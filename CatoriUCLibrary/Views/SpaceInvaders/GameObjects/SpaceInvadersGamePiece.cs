using System;
using System.Collections.Generic;
using System.Text;

namespace CatoriUCLibrary.Views.SpaceInvaders.GameObjects
{
    public class SpaceInvadersGamePiece
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; init; }
        public double Height { get; init; }
        public int Type { get; init; }
        public int Health { get; set; } = 1;
        public double Velocity { get; init; }

        public bool Intersects(double x, double y, double width, double height)
        {
            return X < x + width && X + Width > x && Y < y + height && Y + Height > y;
        }
    }
}
