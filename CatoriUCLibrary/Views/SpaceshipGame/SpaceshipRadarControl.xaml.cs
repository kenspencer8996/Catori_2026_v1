using System.Windows;

using System.Windows.Controls;


namespace CatoriUCLibrary.Views.SpaceshipGame.GameObjects;


public partial class SpaceshipRadarControl : UserControl
{

    public SpaceshipRadarControl() => InitializeComponent();


    public void Track(Point shipCenter, Size playArea, double distanceFromCanvas)
    {

        var center = new Point(playArea.Width / 2, playArea.Height / 2);

        var dx = shipCenter.X - center.X;

        var dy = shipCenter.Y - center.Y;

        var length = Math.Max(1, Math.Sqrt(dx * dx + dy * dy));

        const double radius = 70;

        ContactDot.Margin = new Thickness(
            86 + dx / length * radius - ContactDot.Width / 2,
            86 + dy / length * radius - ContactDot.Height / 2, 0, 0);

        RangeText.Text = $"SHIP {Math.Ceiling(distanceFromCanvas)} px";

    }

}

