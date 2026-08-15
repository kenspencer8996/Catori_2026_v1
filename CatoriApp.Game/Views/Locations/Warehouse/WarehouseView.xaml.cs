using CatoriApp.Game.Controllers.Locations.Warehouse;
using System.Windows;

namespace CatoriApp.Game.Views.Locations.Warehouse
{
    public partial class WarehouseView : Window
    {
        private readonly WarehouseViewController _controller;

        public WarehouseView()
        {
            InitializeComponent();
            _controller = new WarehouseViewController(this);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
            => _controller.Close();
    }
}
