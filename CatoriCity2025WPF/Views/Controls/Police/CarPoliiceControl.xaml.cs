using CatoriCity2025WPF.Controllers;
using CatoriCity2025WPF.Objects;
using CatoriServices.Objects;
using CityAppServices.Objects.Entities;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for CarPo_ice.xaml
    /// </summary>
    public partial class CarPoliiceControl : UserControl
    {
        RobberyMessage _robberyMessage;
        CarPoliceContentController _controller;
        System.Windows.Controls.Image _eastImage;
        System.Windows.Controls.Image _westImage;
        System.Windows.Controls.Image _northImage;
        System.Windows.Controls.Image _southImage;
        System.Windows.Controls.Image _rawImage;
        string _imagepath;
        int _movedelaytime;
        private DispatcherTimer _startMoveTimer;

        int _carNumber = -1;
        public CarPoliiceControl(string imageName,double width,double height, int carNumber)
        {
            InitializeComponent();
            _carNumber = carNumber; 
            double x = Canvas.GetLeft(this);
            double y = Canvas.GetTop(this);
            _imagepath = Imagehelper.GetImagePath(imageName);
            _controller = new CarPoliceContentController(this,x,y);
            this.Width = width;
            this.Height = height;
            
            _movedelaytime= GlobalStuff.TimingsRandom[_carNumber -1];
            
            BorderImage.BorderThickness = new Thickness(0);
            BorderImage.BorderBrush = Brushes.Red;

            SetupImages();
            WeakReferenceMessenger.Default.Register<RobberyMessage>(this, (r, m) =>
            {
                cLogger.Log(this.Name + " WeakReferenceMessenger called : " + " ");
                _robberyMessage = m;
                
                _controller.bank = _robberyMessage.Bank;
                _startMoveTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(_movedelaytime) // Set the long press duration (1 second here)
                };
                _startMoveTimer.Tick += _startMoveTimer_Tick; ;
                _startMoveTimer.Start();
            });
        }

        private void _startMoveTimer_Tick(object? sender, EventArgs e)
        {
            _startMoveTimer.Stop();
            StartanimationToBank(_robberyMessage);
        }

        private void SetupImages()
        {
            _rawImage =  UIUtility.GetImageControl(_imagepath, this.Width, this.Height, 0);
            _westImage = _rawImage;
            _eastImage = _rawImage;
            _northImage = _rawImage;
            _southImage = _rawImage;
            this.CarImage.Source = _westImage.Source;

            RotateTransform rotateTransform;
            rotateTransform = new RotateTransform(90);
            _southImage.RenderTransform = rotateTransform;

            rotateTransform = new RotateTransform(180);
            _eastImage.RenderTransform = rotateTransform;

            rotateTransform = new RotateTransform(270);
            _northImage.RenderTransform = rotateTransform;

            FlipControlToEast(_eastImage);
        }

       
        public void StartanimationToBank(RobberyMessage m)
        {
            _controller.StartAnimation(m);
            System.Threading.Thread.Sleep(2);
            RobberyMessage m2 = m;
            m2.RobberName = "robber43";
            _controller.StartAnimation(m2);
        }

        public string RobberName { get; internal set; }
        internal void ChangeDirection(PositionsEWNSEnum direction)
        {
            RotateTransform rotateTransform;
            switch (direction)
            {
                case PositionsEWNSEnum.North:
                    rotateTransform = new RotateTransform(90);
                    this.RenderTransform = rotateTransform;
                    this.CarImage.Source = _northImage.Source;
                    break;
                case PositionsEWNSEnum.East:
                    this.CarImage.Source = _eastImage.Source;
                    rotateTransform = new RotateTransform(05);
                    this.RenderTransform = rotateTransform;
                    break;
                case PositionsEWNSEnum.South:
                    rotateTransform = new RotateTransform(90);
                    this.RenderTransform = rotateTransform;
                    this.CarImage.Source = _southImage.Source;
                    break;
                case PositionsEWNSEnum.West:
                    this.CarImage.Source = _westImage.Source;
                    rotateTransform = new RotateTransform(0);
                    this.RenderTransform = rotateTransform;
                    break;
                default:
                    break;
            }
        }
        private void FlipControlToEast(System.Windows.Controls.Image image)
        {
            ScaleTransform flipTrans = new ScaleTransform();
            flipTrans.ScaleX = -1; // Flip horizontally
            flipTrans.ScaleY = 1;  // No vertical flip
            image.RenderTransform = flipTrans;
        }
      
    }
}
