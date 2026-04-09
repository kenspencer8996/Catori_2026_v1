using CatoriCity2025WPF.Objects.Arguments;
using CatoriCity2025WPF.Objects.DragDrop;
using CatoriCity2025WPF.Objects.Messages;
using CommunityToolkit.Mvvm.Messaging;
using System.IO;
using System.Windows.Input;
using System.Windows.Threading;

namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for PersonControl.xaml
    /// </summary>
    public partial class PersonControl : UserControl, IDraggable
    {
        public event EventHandler<PrimaryPrsonDragArgg> PersonMouseDown;
        public event EventHandler<PrimaryPrsonDragArgg> PersonMouseUp;
        public event EventHandler<PrimaryPrsonDragArgg> MovePersonStart;
        public event EventHandler<PrimaryPrsonDragArgg> MovePersonStop;
        public event EventHandler<DiggerCompleteCycleArgs> DiggerCycleComplete;
        public List<string> WalkingRightImages = new List<string>();
        public List<string> WalkingLeftImages = new List<string>();
        public List<string> WalkingLeftShovelImages = new List<string>();
        public List<string> WalkingRightShovelImages = new List<string>();
        public List<string> CurrentImages = new List<string>();
        public List<string> PickupTreasureImages = new List<string>();
        PersonService personservice;
        PersonMouseTargetenum _PersonMouseTargetenum;
        public PersonViewModel _person;
        FactoryControl _factoryControl;
        int _imageindex;
        int _dirtindex = 0;
        public bool MovePerson = false;
        readonly DispatcherTimer _animation_timer;
        readonly DispatcherTimer _animationDigger_timer;
        readonly DispatcherTimer _animationLiftChest_timer;
        public bool _isDragging = false;
        List<string> _diggerpositions = new List<string>();
        int _currentIndex = 0;
        int _currentDiggerIndex = 0;
        bool _digMode = false;
        DragManager _dragManager;
        Canvas _hostCanvas;
        public Point ClickOffset;
        public bool isCarryingChest = false;
        public TimeSpan AnimationFrameInterval
        {
            get => _animation_timer.Interval;
            set => _animation_timer.Interval = value;
        }

        public UIElement Visual 
            { 
            get
            { 
                return this; 
            }
        }

        public Point OriginalPosition     {
            get
            {
                double left = Canvas.GetLeft(this);
                double top = Canvas.GetTop(this);
                return new Point(left, top);
            }
        }

        public PersonControl(PersonViewModel person, 
            DragManager dragManager,Canvas hostCanvas)
        {
            InitializeComponent();
            _dragManager = dragManager; 
            _hostCanvas = hostCanvas;
            cLogger.Log("event hit");
            _person = person;
            personservice = new PersonService();
            if (CityScapeGlobal.ShowAllBordersIfAvailable)
            {
                MainBorder.BorderThickness = new Thickness(1);
                MainBorder.BorderBrush = Brushes.Red;
            }
            LoadImageLists();
            CurrentImages= WalkingRightImages;
            _animation_timer = new DispatcherTimer(DispatcherPriority.Normal);
            LoadDiggerImagesList();
            _animation_timer.Tick += _animation_timer_Tick;
            AnimationFrameInterval = new TimeSpan(0, 0, 0,0,200);
            _animationDigger_timer = new DispatcherTimer(DispatcherPriority.Normal);
            _animationDigger_timer.Interval = new TimeSpan(0, 0, 0, 0, 999);
            _animationDigger_timer.Tick += _animationDigger_timer_Tick;

            _animationLiftChest_timer = new DispatcherTimer(DispatcherPriority.Normal);
            _animationLiftChest_timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            _animationLiftChest_timer.Tick  += _animationLiftChest_timer_Tick;
            PersonImage.Source = UIUtility.GetImageControl(_person.StaticImageFilePath, 50, 50, 0).Source;
            WeakReferenceMessenger.Default.Register<LeaveWorkArg>(this, (r, m) =>
            {
                //check if the person is in this house
                if (m.Person.Name == _person.Name)
                {
                   
                    this.Opacity = 1;
                    personservice.UpsertPerson(m.Person);
                }
            });
            WeakReferenceMessenger.Default.Register<PersonShowMessage>(this, (r, m) =>
            {
                Canvas.SetZIndex(this, 4001);

                Canvas.SetLeft(this, 450);
                Canvas.SetTop(this, 250);
                PersonImage.Opacity = 1;
            }); 
            WeakReferenceMessenger.Default.Register<FactoryMouseMessage>(this, (r, m) =>
            {
                cLogger.Log(this.Name + " WeakReferenceMessenger called : " + "LeaveEnter " + m.LeaveEnter);
                switch (m.LeaveEnter)
                {
                    case LeaveEnerEnum.Leave:
                        _PersonMouseTargetenum = PersonMouseTargetenum.None;
                        break;
                    case LeaveEnerEnum.Enter:
                        _PersonMouseTargetenum = PersonMouseTargetenum.Factory;
                        _factoryControl = m.FactoryControlInstance;
                        break;
                    default:
                        break;
                }
            });
            WeakReferenceMessenger.Default.Register<PersonDroppedMessage>(this, (r, m) =>
            {
                cLogger.Log(this.Name + " WeakReferenceMessenger called : ");
               
            });

            WeakReferenceMessenger.Default.Register<PersonValuablesMessage>(this, (r, m) =>
            {
                person.Funds += m.ValuableAmount;
            });
        }

        public void ShowPerson()
        {
            this.Opacity = 1;
        }
        public void HidePerson()
        {
            this.Opacity = 0;
        }
        private void LoadDiggerImagesList()
        {
            _diggerpositions.Clear();
            string path;
            //path = System.IO.Path.Combine(GlobalStuff.ImageFolder, "PrimaryPeople\\Digging", "GirlDigDownDirt1.png");
            //_diggerpositions.Add(path);
            path = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "PrimaryPeople\\Digging", "GirlDigShovelDown.png");
            _diggerpositions.Add(path);
            path = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "PrimaryPeople\\Digging", "GirlDigDownShovelDirt1.png");
            _diggerpositions.Add(path);
            path = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "PrimaryPeople\\Digging", "GirlDigShovelDownDirt2.png");
            _diggerpositions.Add(path);
            path = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "PrimaryPeople\\Digging", "GirlDigShovelDownDIrt3.png");
            _diggerpositions.Add(path);
            path = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "PrimaryPeople\\Digging", "GirlDigShovelDownDirt4.png");
            _diggerpositions.Add(path);
            path = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "PrimaryPeople\\Digging", "GIrlDigShovelPartialDownDirt.png");
            _diggerpositions.Add(path);
            path = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "PrimaryPeople\\Digging", "GirlDigBackDirt1.png");
            _diggerpositions.Add(path);
            path = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "PrimaryPeople\\Digging", "GirlDigBackDirt2.png");
            _diggerpositions.Add(path);
            path = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "PrimaryPeople\\Digging", "GirlDigShovelOverDhoulderDirt.png");
            _diggerpositions.Add(path);
            path = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "PrimaryPeople\\Digging", "GirlDigShovelOverDhoulderDirt2.png");
            _diggerpositions.Add(path);
            path = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "PrimaryPeople\\Digging", "GirlDigShovelOverDhoulderDirt3.png");
            _diggerpositions.Add(path);
            path = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "PrimaryPeople\\Digging", "GirlDigShovelOverDhoulderDirt4.png");
            _diggerpositions.Add(path);


        }
        private void _animation_timer_Tick(object? sender, EventArgs e)
        {
           // cLogger.Log("event hit");
            if (_imageindex >= CurrentImages.Count)
                _imageindex = 0;
            try
            {
                PersonImage.Source = UIUtility.GetImageControl(CurrentImages[_imageindex], 10, 5, 0).Source;
                _imageindex = (_imageindex + 1) % CurrentImages.Count;

            }
            catch (Exception ex)
            {
                cLogger.Log("Image Error : " + ex.Message);
            }
        }

        private void DepositMenu_Click(object sender, RoutedEventArgs e)
        {

            cLogger.Log("event hit");
            FundsDetailView fundsDetailView = new FundsDetailView();
            fundsDetailView.Owner = CityScapeGlobal.CityScapeView;
            fundsDetailView.ShowDialog();
        }

        private void UserControl_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            cLogger.Log("event hit");
            if (_isDragging)
            {
                _isDragging = false;
                //ReleaseMouseCapture();
                //_dragManager.EndDrag(e.GetPosition(_hostCanvas));
            }
            cLogger.Log(this.Name + " UserControl_MouseUp called : ");
            if (PersonMouseUp != null)
            {
                PrimaryPrsonDragArgg arg = new PrimaryPrsonDragArgg();
                PersonMouseUp(this, arg);
            }
        }
       
    

        private void UserControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
           cLogger.Log("UserControl_MouseEnter hit");

        }



        private void PickupShovelMenu_Click(object sender, RoutedEventArgs e)
        {
            cLogger.Log("event hit");

        }

       
        public void StartDiggingAnimation(int dirtindex)
        {
            _dirtindex = dirtindex;
            isCarryingChest = true;
            _digMode = true;
            _currentDiggerIndex = 0;
            Opacity = 1;
            _animationDigger_timer.Start();
        }
        public void StopDigging()
        {
            _digMode = false;
            _animationDigger_timer.Stop();
        }
        private void _animationDigger_timer_Tick(object? sender, EventArgs e)
        {
            int totalPositions = _diggerpositions.Count();  
            if (_currentDiggerIndex <= totalPositions - 1  )
            {
                try
                {
                    string filePath = Path.Combine(GlobalAllApps.ImageFolder, "PrimaryPeople\\Digging", _diggerpositions[_currentDiggerIndex]);

                    PersonImage.Source = UIUtility.GetImageControl(filePath, 200, 80, 300).Source;

                }
                catch (Exception ex)
                {
                    cLogger.Log("Image Error : " + ex.Message);
                }
                if (_currentDiggerIndex == totalPositions - 1)
                {
                    _animationDigger_timer.Stop();    
                     _currentDiggerIndex = 0;
                   _animationLiftChest_timer.Start();
                    if (DiggerCycleComplete != null)
                    {
                        DiggerCompleteCycleArgs args = new DiggerCompleteCycleArgs();
                        DiggerCycleComplete(this, args);
                    }
                }
            }

            _currentDiggerIndex++;
        }
        private void _animationLiftChest_timer_Tick(object? sender, EventArgs e)
        {
            if (_currentDiggerIndex <= PickupTreasureImages.Count() - 1)
            {
                try
                {
                    string filePath = Path.Combine(GlobalAllApps.ImageFolder, "PrimaryPeople", "Digging", PickupTreasureImages[_currentDiggerIndex]);
                    PersonImage.Source = UIUtility.GetImageControl(filePath, 200, 80, 300).Source;
                }
                catch (Exception ex)
                {
                    cLogger.Log("Image Error : " + ex.Message);
                }            
            }
            else
            {
                _animationLiftChest_timer.Stop();
            }
            _currentDiggerIndex++;
        }
        internal void WalkLeft()
        {
           // cLogger.Log("event hit");
            CurrentImages = WalkingLeftImages;
            _animation_timer.Start();
        }
        
        public void StopDiggerAnimation()
        {
            //cLogger.Log("event hit");
            _animationDigger_timer.Stop();
        }
        internal void StopAnimation()
        {
            //cLogger.Log("event hit");
            _animation_timer.Stop();
        }

        internal void WalkRight()
        {
            //cLogger.Log("event hit");
            CurrentImages = WalkingRightImages;
            _animation_timer.Start();
        }

        private void LoadImageLists()
        {
            try
            {
                WalkingRightImages.Clear();
                WalkingLeftImages.Clear();
                WalkingLeftShovelImages.Clear();
                WalkingRightShovelImages.Clear();
                string filePath = Path.Combine(GlobalAllApps.ImageFolder, "PrimaryPeople/WalkingRight");
                string[] images = Directory.GetFiles(filePath, "*.png");
                WalkingRightImages.AddRange(images);

                filePath = Path.Combine(GlobalAllApps.ImageFolder, "PrimaryPeople/WalkingLeft");
                string[] images2 = Directory.GetFiles(filePath, "*.png");
                WalkingLeftImages.AddRange(images2);

                filePath = Path.Combine(GlobalAllApps.ImageFolder, "PrimaryPeople/WalkingShovelLeft");
                string[] images3 = Directory.GetFiles(filePath, "*.png");
                WalkingLeftImages.AddRange(images3);

                filePath = Path.Combine(GlobalAllApps.ImageFolder, "PrimaryPeople/WalkingShovelRight");
                string[] images4 = Directory.GetFiles(filePath, "*.png");
                WalkingLeftImages.AddRange(images4);

                PickupTreasureImages = new List<string>();
                filePath = Path.Combine(GlobalAllApps.ImageFolder, "PrimaryPeople", "Digging", "GirlStandingChest1.png");
                PickupTreasureImages.Add(filePath);
                filePath = Path.Combine(GlobalAllApps.ImageFolder, "PrimaryPeople", "Digging", "GirlStandingChest2.png");
                PickupTreasureImages.Add(filePath);
                filePath = Path.Combine(GlobalAllApps.ImageFolder, "PrimaryPeople", "Digging", "GirlStandingChest3.png");
                PickupTreasureImages.Add(filePath);
                filePath = Path.Combine(GlobalAllApps.ImageFolder, "PrimaryPeople", "Digging", "GirlStandingChest4.png");
                PickupTreasureImages.Add(filePath);
                filePath = Path.Combine(GlobalAllApps.ImageFolder, "PrimaryPeople", "Digging", "GirlStandingChest.png");
                PickupTreasureImages.Add(filePath);


            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void UserControl_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //var control = (UserControl)sender; 
            //var cursor = e.GetPosition(_hostCanvas); 
            //_dragManager.StartDrag(control, e.GetPosition(_hostCanvas)); 
            //control.CaptureMouse();
            //if (MovePersonStop != null)
            //{
            //    PrimaryPrsonDragArgg args = new PrimaryPrsonDragArgg();
            //    MovePersonStop(this, args);
            //}
        }
        public Point _grabOffset;
        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            cLogger.Log("Canvas pos: " + e.GetPosition(_hostCanvas)); 
            //var element = (UIElement)sender; 
            //element.BeginAnimation(Canvas.LeftProperty, null); 
            //element.BeginAnimation(Canvas.TopProperty, null);
            //_hostCanvas.CaptureMouse();
            //CaptureMouse();
            _isDragging = true;
            // 1. Get click offset relative to the control
            //var clickOffset = e.GetPosition(this);
            //clickOffset = new Point(clickOffset.X - (0), clickOffset.Y - (0));
            //// 2. Convert mouse position to canvas coordinates
            ////var mouseOnCanvas = e.GetPosition(GlobalStuff.MainView.MainLayout);
            ////cLogger.Log($"mouseOnCanvas x {mouseOnCanvas.X} y {mouseOnCanvas.Y} isdragging {_isDragging}");
            //// 3. Start dragging THIS control, not the canvas
            //var screenPos = Mouse.GetPosition(null);
            //var mouseOnCanvas = _hostCanvas.PointFromScreen(screenPos);
            //var element = (UIElement)sender;
            //// mouse position relative to the REAL canvas
            //Point mouseOnCanvas = e.GetPosition(_hostCanvas);

            //// element's current position on that canvas
            //double left = Canvas.GetLeft(element);
            //double top = Canvas.GetTop(element);

            //if (double.IsNaN(left)) left = 0;
            //if (double.IsNaN(top)) top = 0;

            //// compute offset between mouse and element
            //ClickOffset = new Point(
            //    mouseOnCanvas.X - left,
            //    mouseOnCanvas.Y - top
            //);
            //Debug.WriteLine($"Left={Canvas.GetLeft(element)}, Top={Canvas.GetTop(element)}");

            //_dragManager.BeginDrag(this, mouseOnCanvas, ClickOffset);
            //var mouseOnCanvas = e.GetPosition(_hostCanvas);
            //var elementTopLeft = new Point(Canvas.GetLeft(sender as UIElement),
            //                               Canvas.GetTop(sender as UIElement));

            //_grabOffset = new Point(mouseOnCanvas.X - elementTopLeft.X,
            //                        mouseOnCanvas.Y - elementTopLeft.Y);

            ////_dragManager.BeginDrag(this, mouseOnCanvas, _grabOffset);
            //PersonMouseDown?.Invoke(this, new PrimaryPrsonDragArgg());
        }

        private void UserControl_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            cLogger.Log("event hit");
        }

        private void MovePersonMenu_Click(object sender, RoutedEventArgs e)
        {
            cLogger.Log("event hit");
            if (MovePersonStart != null)
            {
                PrimaryPrsonDragArgg args = new PrimaryPrsonDragArgg();
                MovePersonStart(this, args);
            }
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            //cLogger.Log("IsDragging " + _dragManager.IsDragging);
           
        }

        private void UserControl_MouseEnter_1(object sender, MouseEventArgs e)
        {

        }
        public void ShowPersonStandingNoChest()
        {
            try
            {
                string filePath = Path.Combine(GlobalAllApps.ImageFolder, "PrimaryPeople", "GirlStandingLeft.png");
                PersonImage.Source = UIUtility.GetImageControl(filePath, 200, 80, 300).Source;
            }
            catch (Exception ex)
            {
                cLogger.Log("Image Error : " + ex.Message);
            }
        }

        public void OnDragMouseup()
        {
        }
    }
}
