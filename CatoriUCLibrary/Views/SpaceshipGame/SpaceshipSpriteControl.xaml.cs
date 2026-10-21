using System.Windows.Controls;

using System.Windows.Media;


namespace CatoriUCLibrary.Views.SpaceshipGame.GameObjects;


public partial class SpaceshipSpriteControl : UserControl
{

    public SpaceshipSpriteControl() => InitializeComponent();


    public void FaceDirection(double dx, double dy)
    {

        if (Math.Abs(dx) < .01 && Math.Abs(dy) < .01) return;

        // The source artwork points left. Mirror it for rightward travel and only
        // bank toward vertical movement so the ship never rolls upside down.
        var bank = Math.Atan2(dy, Math.Max(1, Math.Abs(dx))) * 180 / Math.PI;

        bank = Math.Clamp(bank, -70, 70);

        var transforms = new TransformGroup();

        transforms.Children.Add(new ScaleTransform(dx >= 0 ? -1 : 1, 1));

        transforms.Children.Add(new RotateTransform(dx >= 0 ? bank : -bank));

        RenderTransform = transforms;

    }

}

