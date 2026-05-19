using CatoriApp.Objects.DragDrop;
using System.Windows.Media.Effects;
namespace CatoriApp.Views.Controls.Stores
{
    /// <summary>
    /// Interaction logic for HardwareItemsControl.xaml
    /// </summary>
    public partial class HardwareItemsControl : UserControl,IDropTarget
    {
        List<PersonProductsOwnedViewModel> _models;
        public HardwareItemsControl(List<PersonProductsOwnedViewModel> models)
        {
            InitializeComponent();
            _models = models;
            if (CityScapeGlobal.ShowAllBordersIfAvailable)
            {
                MainBorder.BorderBrush = Brushes.Red;
                MainBorder.BorderThickness = new Thickness(3);
            }
            foreach (var model in _models)
            {
                Image thisimage =  new Image();
                string path = System.IO.Path.Combine(GlobalAllApps.ImageFolder, model.ImageName);
                thisimage.Source = UIUtility.GetImageControl(path, 20, 50, 0).Source;
                thisimage.Width = 120;
                thisimage.Height = 120;
                ItemsStackPanel.Children.Add(thisimage);
            }
        }



        void IDropTarget.OnDrop(IDraggable element)
        {
        }
        bool IDropTarget.CanDrop(IDraggable element)
        {
            return true;
        }
        public void HighlightOn()
        {
            // Example glow
            this.Effect = new DropShadowEffect
            {
                Color = Colors.Gold,
                BlurRadius = 25,
                ShadowDepth = 0,
                Opacity = 0.8
            };
        }

        public void HighlightOff()
        {
            this.Effect = null;
        }

        public Point GetSnapPoint(UIElement dragged)
        {
            var feDragged = (FrameworkElement)dragged;
            double x = Canvas.GetLeft(this) + (this.ActualWidth - feDragged.ActualWidth) / 2;
            double y = Canvas.GetTop(this) + (this.ActualHeight - feDragged.ActualHeight) / 2;
            return new Point(x, y);
        }

        public Point GetSnapPoint(IDraggable dragged)
        {
            var feDragged = (FrameworkElement)dragged;
            double x = Canvas.GetLeft(this) + (this.ActualWidth - feDragged.ActualWidth) / 2;
            double y = Canvas.GetTop(this) + (this.ActualHeight - feDragged.ActualHeight) / 2;
            return new Point(x, y);
        }
    }
}


