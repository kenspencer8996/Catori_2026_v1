
using CatoriApp.Objects.Arguments;
using CatoriApp.Views;
using System.Windows.Media.Animation;
namespace CatoriApp.Controllers.Stores
{
    public class StoreHardwareInteriorViewController : StoreInteriorControllerBase
    {
        StoreHardwareInteriorView _view;
        int boxheight = 100;
        DroneDeliveryControl DroneDeliveryUC;
        internal CardboardBoxUC cardboardBoxUC;

        CardboardBoxUC box;
        public StoreHardwareInteriorViewController(
            StoreHardwareInteriorView view)
        {
            _view = view;
            LoadControls();
            LoadShelves();

            LoadShopItems();
        }
        private void LoadControls()
        {
            cardboardBoxUC = new CardboardBoxUC();

            DroneDeliveryUC = new DroneDeliveryControl();
            DroneDeliveryUC.Width = 50;
            DroneDeliveryUC.Height = 50;

            //stage drone on shelf
            _view.MainLayoutHardwareStoreInterior.Children.Add(DroneDeliveryUC);
            Canvas.SetLeft(DroneDeliveryUC, 1350);
            Canvas.SetTop(DroneDeliveryUC, 170);
            Canvas.SetZIndex(DroneDeliveryUC, 3005);

            DroneDeliveryUC.Width = 120;
            DroneDeliveryUC.Height = 120;
            DroneDeliveryUC.DroneAtPickup += DroneDeliveryUC_DroneAtPickup;
            //stage drone on shelf
            _view.MainLayoutHardwareStoreInterior.Children.Add(cardboardBoxUC);
            cardboardBoxUC.Height = 0;
            cardboardBoxUC.Width = 100;
            Canvas.SetLeft(cardboardBoxUC, 1320);
            Canvas.SetTop(cardboardBoxUC, 500);
            Canvas.SetZIndex(cardboardBoxUC, 3005);

            cardboardBoxUC.BoxOpenFinished += BoxclosedUC_BoxOpenFinished;

        }
        private void LoadShopItems()
        {
            ShopItemService shopItemService = new ShopItemService();
            CityScapeGlobal.ShopItems = shopItemService.GetAllAsync().Result;
            foreach (var item in CityScapeGlobal.ShopItems)
            {
                var found = from sh in CityScapeGlobal.ShelfUCs
                            where sh.Model.ShopItemId == item.ShopItemId
                            select sh;
                if (found.Any())
                {
                    ShelfItemControl shelfUC = found.First();
                    shelfUC.AddSHopItemToShelfLocation(GetShopItemControl(item));
                }
            }
        }
        private ShopItemControl GetShopItemControl(ShopItemViewModel model)
        {
            ShopItemControl shopitemtemp = new ShopItemControl();
            shopitemtemp.Width = model.Width;
            shopitemtemp.Height = model.Height;
            shopitemtemp.Name = model.Name;
            shopitemtemp.Width = model.Width;
            shopitemtemp.Height = model.Height;
            shopitemtemp.Model = model;
            shopitemtemp.ShopItemMouseDown += _view.ShopItemMouseDown;
            shopitemtemp.ShopItemMouseUp += _view.ShopItemMouseUp; ;
            if (model.RotationDegree > 0)
            {
                RotateTransform rotateTransform = new RotateTransform(model.RotationDegree);
                shopitemtemp.RenderTransform = rotateTransform;

            }
            shopitemtemp.Visibility = Visibility.Visible;

            //shopitem.ItemCost = item.Price.ToString("C");
            string filepath = Imagehelper.GetImagePath(model.ImageName);
            shopitemtemp.MainImage.Source = UIUtility.GetImageControl(filepath, 100, 100, 0).Source;
            return shopitemtemp;
        }
        private void DroneDeliveryUC_DroneAtPickup(object? sender, DeliveryArgs e)
        {
            cardboardBoxUC.Visibility = Visibility.Collapsed;
            //_view.TrapazoidImage.Visibility = Visibility.Visible;
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
        private void BoxclosedUC_BoxOpenFinished(object? sender, BoxOpenedArg e)
        {
            BoxOpened();
        }

        public void LoadPerson(double width, double height)
        {
            PersonShopperControl personShopperControl = new PersonShopperControl();
            personShopperControl.Model = _view.Model;
            personShopperControl.MainImage.Source = UIUtility.GetImageControl(_view.Model.StaticImageFilePath,
                width, height, 0).Source;
            _view.MainLayoutHardwareStoreInterior.Children.Add(personShopperControl);
            Canvas.SetLeft(personShopperControl, 900);
            Canvas.SetTop(personShopperControl, 500);
            _view.POSUC.SetupPOC(_view.Model.PersonId);

        }

        internal void MoveUpBox()
        {
            //make box rise from floor
            int seconds = 1;
            Storyboard sb = new Storyboard();
            DoubleAnimation daleft = AnimationHelper.GetDoubleAnimation(0, boxheight, seconds * 1000);
            daleft.EasingFunction = new ExponentialEase
            {
                EasingMode = EasingMode.EaseOut,
                Exponent = 5
            };
            Storyboard.SetTarget(daleft, _view);
            Storyboard.SetTargetProperty(daleft, new PropertyPath("(Canvas.Left)"));
            sb.Children.Add(daleft);
            sb.Completed += (s, e) =>
            {
                //show rear view
                Task.Delay(500).ContinueWith(t =>
                {
                    _view.Dispatcher.Invoke(() =>
                    {
                        //show front view
                    });
                });
            };
            sb.Begin();
            //fire up drone
            //fly to box
            //lift it        }
        }

        internal void BoxOpened()
        {
            DroneDeliveryUC.FlyToPickup();
        }
   
       private void LoadShelves()
        {
            ShelfLocationService shelfLocationService = new ShelfLocationService();
            CityScapeGlobal.ShelfViewModels = shelfLocationService.GetAllAsync().Result;
            var groupByStoreQuery = from s in CityScapeGlobal.ShelfViewModels
                                    group s by s.StoreType into storeTypesKey
                                    select new
                                    {
                                        groupkey = storeTypesKey.Key
                                    };

            foreach (var grp in groupByStoreQuery)
            {
                CityScapeGlobal.Stores.Add(grp.groupkey);
                var foundShelf = from sh in CityScapeGlobal.ShelfViewModels
                                 where sh.StoreType == grp.groupkey
                                 select sh;
                CityScapeGlobal.ShelfUCs = new List<ShelfItemControl>();
                foreach (var shelf in foundShelf)
                {
                    ShelfItemControl shelfUC = GetShelfControl(shelf);
                    shelfUC.Model = shelf;
                    cLogger.Log($"Placing shelf {shelf.ShelfLocationID} at X:{shelf.PositionX} Y:{shelf.PositionY}");
                    Canvas.SetLeft(shelfUC, Convert.ToDouble(shelf.PositionX));
                    Canvas.SetTop(shelfUC, Convert.ToDouble(shelf.PositionY));
                    Canvas.SetZIndex(shelfUC, 2002);
                    CityScapeGlobal.ShelfUCs.Add(shelfUC);
                    switch (shelf.StoreType)
                    {
                        case "Hardware1":
                            shelfUC.Name = "HardwareShelf" + shelf.ShelfLocationID;
                            _view.MainLayoutHardwareStoreInterior.Children.Add(shelfUC);
                            break;
                        default:
                            shelfUC.Name = "Shelf" + shelf.ShelfLocationID;
                            break;
                    }
                }
            }
        }
        private ShelfItemControl GetShelfControl(ShelfLocationViewModel model)
        {
            ShelfItemControl shelftemp = new ShelfItemControl();
            shelftemp.Width = model.Width;
            shelftemp.Height = model.Height;

            return shelftemp;
        }
    }
}

