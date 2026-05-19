using Microsoft.Extensions.Primitives;
namespace CatoriApp.Views.Controls.Locations.Factory
{
    /// <summary>
    /// Interaction logic for PartSimpleUserControl.xaml
    /// </summary>
    public partial class PartSimpleUserControl : UserControl
    {
        double _width;
        double _height;
        double _left; 
        double _top;
        int _rotation;
        public PartSimpleUserControl(double width, double height,
            double left, double top,int rotation, string imageName)
        {
            InitializeComponent();
            _width = width;
            _height = height;
            _left = left;
            _top = top;
            _rotation = rotation;
            _width = width;
            _height = height;
            this.Width = width;
            this.Height = height;
            Canvas.SetLeft(PartImage, left);
            Canvas.SetTop(PartImage, top);
            PartImage.Width = width;
            PartImage.Height = height;
            if (rotation > -1)
            {
                RotateTransform rotateTransform = new RotateTransform(rotation);
                this.RenderTransform = rotateTransform;
                rotateTransform.CenterX = width / 2;
                rotateTransform.CenterY = height / 2;
                rotateTransform.Angle = rotation;
            }
            string imagesPath = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "Locations", imageName); 
            PartImage.Source = UIUtility.GetImageControl(imagesPath, width, height, 1).Source;
        }
    }
}


