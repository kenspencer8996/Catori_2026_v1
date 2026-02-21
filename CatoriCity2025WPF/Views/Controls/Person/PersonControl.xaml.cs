using CatoriCity2025WPF.Objects.Arguments;
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
    public partial class PersonControl : UserControl
    {
        public event EventHandler<PrimaryPrsonDragArgg> PersonMouseDown;
        public event EventHandler<PrimaryPrsonDragArgg> PersonMouseUp;
        public event EventHandler<PrimaryPrsonDragArgg> MovePersonStart;
        public event EventHandler<PrimaryPrsonDragArgg> MovePersonStop;
        public List<string> WalkingRightImages = new List<string>();
        public List<string> WalkingLeftImages = new List<string>();
        public List<string> WalkingLeftShovelImages = new List<string>();
        public List<string> WalkingRightShovelImages = new List<string>();
        public List<string> CurrentImages = new List<string>();
        PersonService personservice;
        PersonMouseTargetenum _PersonMouseTargetenum;
        public PersonViewModel _person;
        FactoryControl _factoryControl;
        int _imageindex;
        public bool MovePerson = false;
        readonly DispatcherTimer _animation_timer;
        bool _isDragging = false;
        public TimeSpan AnimationFrameInterval
        {
            get => _animation_timer.Interval;
            set => _animation_timer.Interval = value;
        }
        public PersonControl(PersonViewModel person)
        {
            InitializeComponent();
            cLogger.Log("event hit");
            _person = person;
            personservice = new PersonService();
            if (GlobalStuff.ShowAllBordersIfAvailable)
            {
                MainBorder.BorderThickness = new Thickness(1);
                MainBorder.BorderBrush = Brushes.Red;
            }
            LoadImageLists();
            CurrentImages= WalkingRightImages;
            _animation_timer = new DispatcherTimer(DispatcherPriority.Normal);

            _animation_timer.Tick += _animation_timer_Tick;
            AnimationFrameInterval = new TimeSpan(0, 0, 0,0,200);
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
            
        }
       

        private void _animation_timer_Tick(object? sender, EventArgs e)
        {
           // cLogger.Log("event hit");
            if (_imageindex >= CurrentImages.Count)
                _imageindex = 0;
            PersonImage.Source = UIUtility.GetImageControl(CurrentImages[_imageindex], 10, 5, 0).Source;
            _imageindex = (_imageindex + 1) % CurrentImages.Count;

        }

        private void DepositMenu_Click(object sender, RoutedEventArgs e)
        {

            cLogger.Log("event hit");
            FundsDetailView fundsDetailView = new FundsDetailView(_person);
            fundsDetailView.Owner = GlobalStuff.MainView;
            fundsDetailView.ShowDialog();
        }

        private void UserControl_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            cLogger.Log("event hit");
            if (_isDragging)
            {
                _isDragging = false;
                ReleaseMouseCapture();
                DragManager.EndDrag();
            }
            cLogger.Log(this.Name + " UserControl_MouseUp called : ");
            if (PersonMouseUp != null)
            {
                PrimaryPrsonDragArgg arg = new PrimaryPrsonDragArgg();
                PersonMouseUp(this, arg);
            }
        }
        private void CaptureMouse(bool capture)
        {
            if (capture)
            {
                this.CaptureMouse();
            }
            else
            {
                this.ReleaseMouseCapture();
            }
        }
        private void UserControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            cLogger.Log("event hit");
            bool mouseIsDown = System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed;
            CaptureMouse();
            _isDragging = true;

            var clickOffset = e.GetPosition(this);

            DragManager.StartDrag(this, GlobalStuff.MainView.MainLayout, clickOffset);
            if (mouseIsDown)
            {
                //cLogger.Log("LandscapeUC_MouseDown ");
                //    cLogger.Log("LandscapeUC_MouseDown mouse is down");
            }
            if (PersonMouseDown != null)
            {
                PrimaryPrsonDragArgg arg = new PrimaryPrsonDragArgg();
                PersonMouseDown(this, arg);
                FactoryMouseMessage args = new FactoryMouseMessage();
                args.LeaveEnter = LeaveEnerEnum.Enter;
                args.Person = _person;
              //  WeakReferenceMessenger.Default.Send< FactoryMouseMessage>(args);
            }
        }

        private void UserControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
           // cLogger.Log("event hit");

        }



        private void PickupShovelMenu_Click(object sender, RoutedEventArgs e)
        {
            cLogger.Log("event hit");

        }

        private void DigMenu_Click(object sender, RoutedEventArgs e)
        {
            cLogger.Log("event hit");

        }

        internal void WalkLeft()
        {
           // cLogger.Log("event hit");
            CurrentImages = WalkingLeftImages;
            _animation_timer.Start();
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
                string filePath = Path.Combine(GlobalStuff.ImageFolder, "PrimaryPeople/WalkingRight");
                string[] images = Directory.GetFiles(filePath, "*.png");
                WalkingRightImages.AddRange(images);

                filePath = Path.Combine(GlobalStuff.ImageFolder, "PrimaryPeople/WalkingLeft");
                string[] images2 = Directory.GetFiles(filePath, "*.png");
                WalkingLeftImages.AddRange(images2);

                filePath = Path.Combine(GlobalStuff.ImageFolder, "PrimaryPeople/WalkingShovelLeft");
                string[] images3 = Directory.GetFiles(filePath, "*.png");
                WalkingLeftImages.AddRange(images3);

                filePath = Path.Combine(GlobalStuff.ImageFolder, "PrimaryPeople/WalkingShovelRight");
                string[] images4 = Directory.GetFiles(filePath, "*.png");
                WalkingLeftImages.AddRange(images4);

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void UserControl_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (MovePersonStop != null)
            {
                PrimaryPrsonDragArgg args = new PrimaryPrsonDragArgg();
                MovePersonStop(this, args);
            }
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
            if (_isDragging)
                DragManager.UpdateDrag();
        }
    }
}
