
using CatoriCity2025WPF.Objects.Arguments;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace CatoriCity2025WPF.Controllers
{
    public class StoreHardwareInteriorControlController : StoreInteriorControllerBase
    {
        StoreHardwareInteriorControl _view;
        int boxheight = 100;
        DroneDeliveryControl DroneDeliveryUC;
        internal CardboardBoxUC cardboardBoxUC;

        CardboardBoxUC box;
     public StoreHardwareInteriorControlController(StoreHardwareInteriorControl view)
        {
            _view = view;
            LoadControls();
        }
        private void LoadControls()
        {
            cardboardBoxUC = new CardboardBoxUC();

            DroneDeliveryUC = new DroneDeliveryControl();
            DroneDeliveryUC.Width = 50;
            DroneDeliveryUC.Height = 50;

            //stage drone on shelf
            _view.MainLayout.Children.Add(DroneDeliveryUC);
            Canvas.SetLeft(DroneDeliveryUC, 1350);
            Canvas.SetTop(DroneDeliveryUC, 170);
            Canvas.SetZIndex(DroneDeliveryUC, 3005);

            DroneDeliveryUC.Width = 120;
            DroneDeliveryUC.Height = 120;
            DroneDeliveryUC.DroneAtPickup += DroneDeliveryUC_DroneAtPickup;
            //stage drone on shelf
            _view.MainLayout.Children.Add(cardboardBoxUC);
            cardboardBoxUC.Height = 0;
            cardboardBoxUC.Width = 100;
            Canvas.SetLeft(cardboardBoxUC, 1320);
            Canvas.SetTop(cardboardBoxUC, 500);
            Canvas.SetZIndex(cardboardBoxUC, 3005);

            cardboardBoxUC.BoxOpenFinished += BoxclosedUC_BoxOpenFinished;

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
            _view.MainLayout.Children.Add(personShopperControl);
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
    }
}