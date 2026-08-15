using CatoriApp.Game.Views.Locations.Warehouse;

namespace CatoriApp.Game.Controllers.Locations.Warehouse
{
    public sealed class WarehouseViewController : CatoriApp.Game.Controllers.ControllerAnimateLayoutsBase
    {
        private readonly WarehouseView _view;

        public WarehouseViewController(WarehouseView view)
            : base()
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            LoadWarehouseImage();
        }

        private void LoadWarehouseImage()
        {
            string imagePath = Imagehelper.GetImagePath(System.IO.Path.Combine(
                GlobalAllApps.ImageFolder, "Warehouses", "WarehouseInterior.png"));
            _view.WarehouseImage.Source = UIUtility.GetImageControl(imagePath, 100, 100, 0).Source;
        }

        public void Close()
        {
            Dispose();
            _view.Close();
        }
    }
}
