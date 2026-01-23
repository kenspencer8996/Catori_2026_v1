
using System.Windows.Media.Animation;

namespace CatoriCity2025WPF.Controllers
{
    public class StoreHardwareInteriorControlController : StoreInteriorControllerBase
    {
        StoreHardwareInteriorControl _view;
        int boxheight = 100;
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

    }
}