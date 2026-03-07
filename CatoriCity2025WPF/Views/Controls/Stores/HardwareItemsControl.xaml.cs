using CatoriCity2025WPF.Objects.DragDrop;

namespace CatoriCity2025WPF.Views.Controls.House
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
                string path = System.IO.Path.Combine(CityScapeGlobal.ImageFolder, model.ImageName);
                thisimage.Source = UIUtility.GetImageControl(path, 20, 50, 0).Source;
                thisimage.Width = 120;
                thisimage.Height = 120;
                ItemsStackPanel.Children.Add(thisimage);
            }
        }



        void IDropTarget.OnDrop(UIElement element)
        {
            throw new NotImplementedException();
        }
        bool IDropTarget.CanDrop(UIElement element)
        {
            return true;
        }
        
    }
}
