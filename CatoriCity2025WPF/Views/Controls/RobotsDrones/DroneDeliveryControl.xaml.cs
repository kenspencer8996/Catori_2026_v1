using CatoriCity2025WPF.Controllers;
using CatoriCity2025WPF.Objects.Arguments;
using System.Xml.Serialization;

namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for DroneDeliveryControl.xaml
    /// </summary>
    public partial class DroneDeliveryControl : UserControl
    {
        public event EventHandler<DeliveryArgs> DroneAtPickup;
        DroneDeliveryController _controller;
        public DroneDeliveryControl()
        {
            InitializeComponent();
            _controller = new DroneDeliveryController(this);
            if (CityScapeGlobal.ShowAllBordersIfAvailable)
            {
                MainBorder.BorderThickness = new Thickness(1);
                MainBorder.BorderBrush = Brushes.Red;
            }
        }
        public void FlyToPickup()
        {
             _controller.FlyUpFromHome();

        }
        public void FireDroneAtPickup()
        {
            if (DroneAtPickup != null)
            {
                DroneAtPickup(this, new Objects.Arguments.DeliveryArgs());
            }

        }
    }
}
