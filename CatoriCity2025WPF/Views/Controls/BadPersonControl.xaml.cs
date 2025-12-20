using CatoriCity2025WPF.Controllers;
using CatoriCity2025WPF.Objects;
using CatoriCity2025WPF.ViewModels;
using CatoriServices.Objects;
using CatoriServices.Objects.Entities;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for BadPersonControl.xaml
    /// </summary>
    public partial class BadPersonControl : UserControl
    {
        public DispatcherTimer _moveOutToBankTimer;
        public DispatcherTimer _moveFromHomeToApproachTimer;
        private DispatcherTimer _moveToHomeTimer;
        internal PersonViewModel Person { get; set; }
        FundsViewModel _robberFunds;
        Random rndOut = new Random();
        BadPersonController _controller;
        int randomInt ;
        readonly DispatcherTimer _animation_timer;
        internal BadPersonImageTypeEnum _currentImageType = BadPersonImageTypeEnum.WalkingLeft;
        int _imageindex;
        BankViewModel CurrentTargetBank;
        /// <summary>
        /// Interval between frames. Default 500ms.
        /// </summary>
        public TimeSpan AnimationFrameInterval
        {
            get => _animation_timer.Interval;
            set => _animation_timer.Interval = value;
        }
        public BadPersonControl()
        {
            InitializeComponent();
            _controller = new BadPersonController(this);
            randomInt = rndOut.Next(4, 9);
            Random rndToHome = new Random();
            int randomToHomeInt = rndToHome.Next(14, 32);
            MainBorder.BorderThickness = new Thickness(0);

            _moveOutToBankTimer = new DispatcherTimer();
            _moveOutToBankTimer.Tick += new EventHandler(MoveOutToBankTimer_Tick);
            _moveOutToBankTimer.Interval = new TimeSpan(0, 0, randomInt); // Set initial interval to 5 seconds

            _moveFromHomeToApproachTimer = new DispatcherTimer();
            _moveFromHomeToApproachTimer.Tick += new EventHandler(MoveOutFromHomeToApproachTimer_Tick);
           
            _moveToHomeTimer = new DispatcherTimer();
            _moveToHomeTimer.Tick += new EventHandler(MoveToHomeTimer_Tick);
            _moveToHomeTimer.Interval = new TimeSpan(0, 0, randomInt); // Set initial interval to 5 seconds
            _moveFromHomeToApproachTimer.Interval = new TimeSpan(0, 0, randomInt);

            _animation_timer = new DispatcherTimer(DispatcherPriority.Normal);
           
            _animation_timer.Tick += _animation_timer_Tick;
            AnimationFrameInterval = new TimeSpan(0,0,1);
            _moveFromHomeToApproachTimer.Start();
            
 

            //WeakReferenceMessenger.Default.Register<MessageToRobbersOnDeposit>(this, (r, m) =>
            //{
            //    _robberFunds = new FundsViewModel();
            //   _robberFunds.Money = m.Amount;
            //    _robberFunds.X = m.X;
            //    _robberFunds.Y = m.Y;
            //    cLogger.Log(this.Name + " WeakReferenceMessenger called : " +  " ");
            //    _controller.BankLocationX = m.X;
            //    _controller.BankLocationY = m.Y;
            //    _controller.CreateGeometry();
            //    _moveOutToBankTimer.Start();
            //});
        }

        private void _animation_timer_Tick(object? sender, EventArgs e)
        {

            GlobalStuff.WriteDebugOutput("_animation_timer_Tick " + _currentImageType.ToString() + "  (" + _imageindex + ")");
            switch (_currentImageType)
            {
                case BadPersonImageTypeEnum.WalkingRight:
                    if (_imageindex >= Person.WalkingRightImages.Count )
                        _imageindex = 0;
                    MainImage.Source = UIUtility.GetImageControl(Person.WalkingRightImages[_imageindex], 10, 5, 0).Source;
                    _imageindex = (_imageindex + 1) % Person.WalkingRightImages.Count;
                    break;
                case BadPersonImageTypeEnum.WalkingLeft:
                    if (_imageindex >= Person.WalkingLeftImages.Count)
                        _imageindex = 0;
                    MainImage.Source = UIUtility.GetImageControl(Person.WalkingLeftImages[_imageindex], 10, 5, 0).Source; 
                    _imageindex = (_imageindex + 1) % Person.WalkingLeftImages.Count;
                    break;
                case BadPersonImageTypeEnum.WalkingRightBag:
                    if (_imageindex >= Person.WalkingRightBagImages.Count)
                        _imageindex = 0;
                    MainImage.Source = UIUtility.GetImageControl(Person.WalkingRightBagImages[_imageindex], 10, 5, 0).Source; 
                    _imageindex = (_imageindex + 1) % Person.WalkingRightBagImages.Count;
                    break;
                case BadPersonImageTypeEnum.WalkingLeftBag:
                    if (_imageindex >= Person.SittingImages.Count)
                        _imageindex = 0;
                    MainImage.Source = UIUtility.GetImageControl(Person.WalkingLeftBagImages[_imageindex], 10, 5, 0).Source; 
                    _imageindex = (_imageindex + 1) % Person.SittingImages.Count;
                    break;
                case BadPersonImageTypeEnum.Sitting:
                    if (_imageindex >= Person.SittingImages.Count)
                        _imageindex = 0;
                    MainImage.Source = UIUtility.GetImageControl(Person.SittingImages[_imageindex], 10, 5, 0).Source; 
                    _imageindex = (_imageindex + 1) % Person.SittingImages.Count;
                    break;
                case BadPersonImageTypeEnum.LayingDown:
                    if (_imageindex >= Person.LayingDownImages.Count)
                        _imageindex = 0;
                    MainImage.Source = UIUtility.GetImageControl(Person.LayingDownImages[_imageindex], 10, 5, 0).Source; 
                    _imageindex = (_imageindex + 1) % Person.LayingDownImages.Count;
                    break;
                case BadPersonImageTypeEnum.Jumping:
                    if (_imageindex >= Person.JumpingImages.Count)
                        _imageindex = 0;
                    MainImage.Source = UIUtility.GetImageControl(Person.JumpingImages[_imageindex], 10, 5, 0).Source; 
                    _imageindex = (_imageindex + 1) % Person.JumpingImages.Count;
                    break;
                case BadPersonImageTypeEnum.Digging:
                    if (_imageindex >= Person.DiggingImages.Count)
                        _imageindex = 0;
                    MainImage.Source = UIUtility.GetImageControl(Person.DiggingImages[_imageindex], 10, 5, 0).Source; 
                    _imageindex = (_imageindex + 1) % Person.DiggingImages.Count;
                    break;
                default:
                    break;
            }

        }

        private void MoveToHomeTimer_Tick(object? sender, EventArgs e)
        {
            MoveBadPersonOnEscapeToHome();
        }

        private void MoveBadPersonOnEscapeToHome()
        {
            Storyboard storyboard = new Storyboard();
            storyboard.Completed += MoveBadPersonOnEscapeToHome_Completed;
            if (_controller._pathGeometryHome.Count == 0)
                return; 
            var current = _controller._pathGeometryHome.Pop();

            try
            {
                double lastendx ;
                double lastendy ;
                if (current.PathPositions.Count == 0)
                {
                    lastendx = Canvas.GetLeft(this);
                    lastendy = Canvas.GetTop(this);
                }
                else
                {
                    lastendx = current.PathPositions[0].startx;
                    lastendy = current.PathPositions[0].starty;
                }
                  StartImageAnimation(0, 10);//dummy up values for force direction right

                    int i = 0;

                    cLogger.Log("------------------------------- start storyboard ---------------------");
                    double startx = Canvas.GetLeft(this);
                    double starty = Canvas.GetTop(this);
                    double endx = 0;
                    double endy = 0;

                    var found = from l in GlobalStuff.LandscapeUCs
                                where l.Name.Contains("Forest")
                                || l.Name.Contains("Barn") 
                                || l.Name.Contains("Shed")
                                select l;
                    if (found != null && found.Count() > 0)
                    {
                        var landscape = found.First();
                        endx = landscape.xCenter + 20;
                        endy = landscape.yCenter + 20;
                    }
                    DoubleAnimation leftAnimation = new DoubleAnimation();
                    leftAnimation.From = startx;
                    leftAnimation.To = endx;
                    leftAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(3000));
                    DoubleAnimation topAnimation = new DoubleAnimation();
                    topAnimation.From = starty;
                    topAnimation.To = endy;
                    topAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(3000));
                    Storyboard.SetTarget(leftAnimation, this);
                    Storyboard.SetTarget(topAnimation, this);
                    Storyboard.SetTargetProperty(leftAnimation, new PropertyPath("(Canvas.Left)"));
                    Storyboard.SetTargetProperty(topAnimation, new PropertyPath("(Canvas.Top)"));
                    storyboard.Children.Add(leftAnimation);
                    storyboard.Children.Add(topAnimation);
                   
                    storyboard.Begin();
                    cLogger.Log("------------------------------- end MoveBadPersonOnEscapeToHome ---------------------");
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void MoveBadPersonOnEscapeToHome_Completed(object? sender, EventArgs e)
        {
        }

        private void MoveOnEscapeToHomeStoryboard_CurrentStateInvalidated(object? sender, EventArgs e)
        {

            StopImageAnimation();
        }

        private void MoveOutFromHomeToApproachTimer_Tick(object? sender, EventArgs e)
        {
            cLogger.Log(this.Name + " MoveOutFromHomeTimerTimer_Tick called : " + " ");
            _moveFromHomeToApproachTimer.Stop();
            _controller.MovePersonOutToApproachStartAndSetupAnimation();
        }

        private void MoveOutToBankTimer_Tick(object? sender, EventArgs e)
        {
            Random rnd = new Random();

            // Generate random number in range [min, max]
            int randomNumber = rnd.Next(0, GlobalStuff.Banks.Count -1); // max is exclusive, so add 1
            CurrentTargetBank = GlobalStuff.Banks[randomNumber];
            _robberFunds = new FundsViewModel();
            _robberFunds.X = CurrentTargetBank.X;
            _robberFunds.Y = CurrentTargetBank.Y;
            cLogger.Log(this.Name + " WeakReferenceMessenger called : " + " ");
            _controller.BankLocationX = CurrentTargetBank.X;
            _controller.BankLocationY = CurrentTargetBank.Y;
            _controller.CreateGeometry();

            _moveOutToBankTimer.Stop();
           // _controller.SitCharacterOnLog_ApproachItem();

            MovePersonToBank();
        }
        internal void AddPerson(PersonViewModel person)
        {
            if (person != null)
            {
                Person = person;
                //set default image
                MainImage.Source = UIUtility.GetImageControl(Person.WalkingLeftImages[0], 10, 5, 0).Source;

            }
        }
        internal void MovePersonToBank()
        {
            
             cLogger.Log(this.Name + " MovePersonToBank called with count: " + _controller._pathGeometryToBank.Count +  " ");
            if (_controller._pathGeometryToBank.Count > 0)
            {
                var current = _controller._pathGeometryToBank.Pop();
                cLogger.Log(this.Name + " MovePersonToBank name: " + current.PositionName + " ");
                cLogger.Log(this.Name + " MovePersonToBank path pos: " + current.PathPositions.Count + " ");
                Storyboard storyboard = new Storyboard();
                storyboard.Completed += MovePersonTobankStoryboard_Completed;
                storyboard.CurrentStateInvalidated += MovePersonTobankStoryboard_CurrentStateInvalidated;
                int stopat = 900;
               
                int lastindex = current.PathPositions.Count - 1;
                double startx = current.PathPositions[0].startx;
                double endx = current.PathPositions[lastindex].endx;
                StartImageAnimation(startx,endx);

                //end test code
                cLogger.Log("---------------- current.PathPositions -----------------");
                foreach (var item in current.PathPositions)
                {
                    cLogger.Log("MovePersonToBank startx " + item.startx + " starty " + item.startx + " endx " + item.endx + " endy " + item.endy);
                    cLogger.Log("  MovePersonToBank centerx " + item.centerx + " centerx " + item.centery);

                }
                cLogger.Log("------------------------------- end ---------------------");

                if (current.PathPositions.Count > 0)
                {
                    double lastendx = current.PathPositions[0].startx;
                    double lastendy = current.PathPositions[0].starty; 
                    int i = 0;
                    cLogger.Log("MovePersonToBank Storyboard lastendx " + lastendx + " lastendy " + lastendy);
                    cLogger.Log("  MovePersonToBank Storyboard duration " + current.PathPositions[0].durationseconds);

                    cLogger.Log("------------------------------- start storyboard ---------------------");
                    foreach (var position in current.PathPositions)
                    {
                        cLogger.Log("MovePersonToBank Storyboard lastendx " + lastendx + " lastendy " + lastendy);
                        cLogger.Log("  MovePersonToBank centerx " + position.centerx + " centery " + position.centery);
                        //if (lastendy > GlobalStuff.Screenheight)
                        //    lastendy = GlobalStuff.Screenheight - 50;
                        //if (lastendx > GlobalStuff.Screenwidth)
                        //    lastendx = GlobalStuff.Screenwidth - 50;
                        //if (lastendy < 0)
                        //    lastendy =  50;
                        //if (lastendx < 0)
                        //    lastendx = 50;
                        position.SetAnimations(lastendx, lastendy);
                        DoubleAnimation leftAnimation = position.LeftAnimation;
                        DoubleAnimation topAnimation = position.TopAnimation;
                        Storyboard.SetTarget(leftAnimation, this);
                        Storyboard.SetTarget(topAnimation, this);
                        Storyboard.SetTargetProperty(leftAnimation, new PropertyPath("(Canvas.Left)"));
                        Storyboard.SetTargetProperty(topAnimation, new PropertyPath("(Canvas.Top)"));
                        storyboard.Children.Add(leftAnimation);
                        storyboard.Children.Add(topAnimation);
                        lastendx = position.centerx;
                        lastendy = position.centery;
                        if (stopat <= 0)
                            break;
                        stopat--;
                        i++;
                    }
                    storyboard.Begin();
                    cLogger.Log("------------------------------- end ---------------------");

                }
                else
                {
                    SendRobberyMessage();
                }
            }
        }

  
        private void MovePersonTobankStoryboard_CurrentStateInvalidated(object? sender, EventArgs e)
        {
            //cLogger.Log("MoveTobankStoryboard_CurrentStateInvalidated Timer elapsed!");
            SendRobberyMessage();
            Random rnd = new Random();
            _moveToHomeTimer.Interval = new TimeSpan(0, 0, randomInt);
            _moveToHomeTimer.Start();
        }

        private void MovePersonTobankStoryboard_Completed(object? sender, EventArgs e)
        {
            cLogger.Log("MoveTobankStoryboard_CompletedAsync start " + _controller._pathGeometryToBank.Count);
            StopImageAnimation();
            //AnimanteFinalStepToBank();
        }

        

        private void AnimateFinalStepToBank_Completed(object? sender, EventArgs e)
        {
            Thread.Sleep(1000); // Simulate a delay
            
            SendRobberyMessage();
            Random rnd = new Random();
            _moveToHomeTimer.Interval = new TimeSpan(0, 0, randomInt);
            _moveToHomeTimer.Start();
        }

        private void SendRobberyMessage()
        {
            if (_controller.ReturnStackEntity.BankVM.X == 0 || 
                _controller.ReturnStackEntity.BankVM.Y == 0)
            {
                _controller.ReturnStackEntity.BankVM.X = _robberFunds.X;
                _controller.ReturnStackEntity.BankVM.Y = _robberFunds.Y;
            }

            WeakReferenceMessenger.Default.Send(new RobberyMessage(
                this.Name,
                _controller.ReturnStackEntity.BankVM));
        }

       

        private void BadGuyUC_Loaded(object sender, RoutedEventArgs e)
        {
            _controller.ControlLoaded();
            
        }
        private void BadPersonControlUC_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            cLogger.Log("BadPersonControlUC_SizeChanged called: " + e.NewSize.Width + " " + e.NewSize.Height);
       
        }

        private void MainBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            cLogger.Log("BadPersonControlUC_SizeChanged called: " + e.NewSize.Width + " " + e.NewSize.Height);

        }

        internal void StartImageAnimation(double start,double end)
        {
            BadPersonImageTypeEnum currentImageType;
           if (start > end)
                currentImageType = BadPersonImageTypeEnum.WalkingLeft;
            else
                currentImageType = BadPersonImageTypeEnum.WalkingRight;
            _currentImageType = currentImageType;
            _animation_timer.Start();
        }
        internal void StopImageAnimation()
        {
            _animation_timer.Stop();
        }
    }
}
