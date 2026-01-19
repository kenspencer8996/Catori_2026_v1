namespace CatoriCity2025WPF.Controllers
{
    public class StoreHardwareInteriorControlController : StoreInteriorControllerBase
    {
        StoreHardwareInteriorControl _view;
        public StoreHardwareInteriorControlController(StoreHardwareInteriorControl view)
        {
            _view = view;
            
        }
        public string HardwareStoreName { get; set; } = "Hardware Store";
        public ShoppingCartUtility shoppingCartUtility = new ShoppingCartUtility();
        public ShopItemControl? _draggedShopItemControl;
        public bool _isShopItemDragging = false;
        public bool _isShopItemMouseDown = false;
        public Point _mouseOffset;
        public bool _isMouseInDropOnCOunterares = false;
    
        public void CheckOut()
        {

        }
        public void LoadPerson(double width, double height)
        {
            PersonShopperControl personShopperControl = new PersonShopperControl();
            personShopperControl.Model = _view.Model;
            personShopperControl.MainImage.Source = UIUtility.GetImageControl(_view.Model.StaticImageFilePath,
                width, height, 0).Source;
            _view.MainLayout.Children.Add(personShopperControl);
            Canvas.SetLeft(personShopperControl, 900);
            Canvas.SetTop(personShopperControl, 500);
        }
    }
}