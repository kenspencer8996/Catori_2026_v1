namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for DroneDeliveryControl.xaml
    /// </summary>
    public partial class DroneDeliveryControl : UserControl
    {
        public DroneDeliveryControl()
        {
            InitializeComponent();

            if (GlobalStuff.ShowAllBordersIfAvailable)
            {
                MainBorder.BorderThickness = new Thickness(1);
                MainBorder.BorderBrush = Brushes.Red;
            }
        }
    }
}
