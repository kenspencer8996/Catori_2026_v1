
namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for StoreHardwareInteriorControl.xaml
    /// </summary>
    public partial class StoreHardwareInteriorControl : UserControl
    {
        public StoreHardwareInteriorControl()
        {
            InitializeComponent();
        }
        PersonViewModel _model;
        public PersonViewModel Model
        {
            get
            {
                return _model;
            }
            set
            {
                _model = value;
                LoadPerson();
            }
        }

        private void LoadPerson()
        {
            PersonShopperControl personShopperControl = new PersonShopperControl();
            personShopperControl.Model = _model;
            personShopperControl.MainImage.Source = UIUtility.GetImageControl(_model.StaticImageFilePath, Width, Height, 0).Source;
            MainLayout.Children.Add(personShopperControl);
            Canvas.SetLeft(personShopperControl, 900);
            Canvas.SetTop(personShopperControl, 500);
        }
    }
}
