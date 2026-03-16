using System.Windows.Media.Animation;

namespace CatoriCity2025WPF.Controllers
{
    public class DroneDeliveryController
    {
        private readonly DroneDeliveryControl _view;
        private string _imagePath;
        double _originalLeft = 1350;
        double _originalTop = 170;
        public DroneDeliveryController(DroneDeliveryControl view)
        {
            _view = view;
            _imagePath = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "DroneShipping");
            string pathtofile = System.IO.Path.Combine(_imagePath, "DoneEmpty.png");
            SetImage(pathtofile);

        }
        public string ImagePath
        {
            get
            {
                return _imagePath;
            }
            set
            {
                _imagePath = value;
            }
        }
       
        public void FlyUpFromHome()
        {
            cLogger.Log("Animation starting");
            double topMoveto = _originalTop - 50;

            string pathtofile  = System.IO.Path.Combine(_imagePath, "DoneEmpty.png");
            SetImage(pathtofile);
            //show side view
            int seconds = 2;
            Storyboard sb = new Storyboard();
            DoubleAnimation daleft = AnimationHelper.GetDoubleAnimationWIthEasing(_originalTop, topMoveto, seconds * 1000);
           
            Storyboard.SetTarget(daleft, _view);
            Storyboard.SetTargetProperty(daleft, new PropertyPath("(Canvas.Top)"));
            sb.Children.Add(daleft);

            sb.Completed += (s, e) =>
            {
                //show rear view
                Task.Delay(500).ContinueWith(t =>
                {
                    _view.Dispatcher.Invoke(() =>
                    {
                        //show front view
                        FlyToLeftFromHome();
                    });
                });
            };
            sb.Begin();
        }
        private void FlyToLeftFromHome()
        {
            cLogger.Log("Animation continues");
            double topMoveto = _originalTop - 28;
            double leftMoveto = _originalLeft - 50;

            //show side view
            int seconds = 2;
            Storyboard sb = new Storyboard();
            DoubleAnimation daleft = AnimationHelper.GetDoubleAnimationWIthEasing(_originalLeft, leftMoveto, seconds * 1000);

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
                        FlyToPickup();
                    });
                });
            };
            sb.Begin();
        }
       
        public void FlyToPickup()
        {
            cLogger.Log("Animation continues");
            double topMoveto = 450;

            //show side view
            int seconds = 2;
            Storyboard sb = new Storyboard();
            DoubleAnimation daleft = AnimationHelper.GetDoubleAnimationWIthEasing(_originalTop - 50, topMoveto, seconds * 1000);

            Storyboard.SetTarget(daleft, _view);
            Storyboard.SetTargetProperty(daleft, new PropertyPath("(Canvas.Top)"));
            sb.Children.Add(daleft);

            sb.Completed += (s, e) =>
            {
                //show rear view
                Task.Delay(500).ContinueWith(t =>
                {
                    _view.Dispatcher.Invoke(() =>
                    {
                        //show front view
                        string pathtofile = System.IO.Path.Combine(_imagePath, "DroneWithBox.png");
                        SetImage(pathtofile);
                        _view.FireDroneAtPickup();
                        Task.Delay(3000);
                        LiftOff();
                    });
                });
            };
            sb.Begin();

        }
        public void FlyThruPortal()
        {
            cLogger.Log("Animation continues");
            double leftMoveTo = 1900;

            //show side view
            int seconds = 2;
            Storyboard sb = new Storyboard();
            DoubleAnimation daleft = AnimationHelper.GetDoubleAnimationWIthEasing(_originalLeft, leftMoveTo, seconds * 1000);

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

        }
        public void FlyToDroneParking()
        {
            string pathtofile  = System.IO.Path.Combine(_imagePath, "DroneEmpty.png");
            SetImage(pathtofile);
        }
        public void LiftOff()
        {
            cLogger.Log("Animation continues");
            double topMoveto = _originalTop - 20;
            double topFrom = 430;

            //show side view
            int seconds = 2;
            Storyboard sb = new Storyboard();
            DoubleAnimation daleft = AnimationHelper.GetDoubleAnimationWIthEasing(topFrom, topMoveto, seconds * 1000);

            Storyboard.SetTarget(daleft, _view);
            Storyboard.SetTargetProperty(daleft, new PropertyPath("(Canvas.Top)"));
            sb.Children.Add(daleft);

            sb.Completed += (s, e) =>
            {
                //show rear view
                Task.Delay(500).ContinueWith(t =>
                {
                    _view.Dispatcher.Invoke(() =>
                    {
                        //show front view
                        FlyThruPortal();
                    });
                });
            };
            sb.Begin();
        }

 
        public void FlyToHouse()
        {
            string pathtofile = System.IO.Path.Combine(_imagePath, "dronewithbox.png");
            SetImage(pathtofile);
        }

        public void DeliverPackage()
        {
            string pathtofile = System.IO.Path.Combine(_imagePath, "dronewithbox.png");
            SetImage(pathtofile);
        }
        /// <summary>
        /// Set image source. Accepts absolute/relative file path or pack URI.
        /// Uses existing UIUtility helper to build the image control source.
        /// </summary>
        public void SetImage(string? path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    // clear image
                    _view.DroneImage.Source = null;
                    return;
                }

                // UIUtility.GetImageControl returns an Image; use its Source for consistency
                var img = UIUtility.GetImageControl(path, _view.DroneImage.Width, _view.DroneImage.Height, 0);
                if (img != null)
                {
                    _view.DroneImage.Source = img.Source;
                }
                else
                {
                    _view.DroneImage.Source = null;
                }
            }
            catch (Exception)
            {
                _view.DroneImage.Source = null;
                throw;
            }
        }
    }
}
