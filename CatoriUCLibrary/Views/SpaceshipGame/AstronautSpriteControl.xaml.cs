using System.Windows.Controls;

using System.Windows.Media;


namespace CatoriUCLibrary.Views.SpaceshipGame.GameObjects;


public partial class AstronautSpriteControl : UserControl
{

    public AstronautSpriteControl() => InitializeComponent();


    public void FaceDirection(double dx, double dy)
    {

        if (Math.Abs(dx) < .01 && Math.Abs(dy) < .01) return;

        RenderTransform = new RotateTransform(Math.Atan2(dy, dx) * 180 / Math.PI);

    }

}

