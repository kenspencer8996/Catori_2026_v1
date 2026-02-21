using CatoriCity2025WPF.Controllers;
using CatoriCity2025WPF.Objects.DragDrop;
using CatoriCity2025WPF.Views;
using System.Diagnostics.Eventing.Reader;
using System.Windows.Input;
using System.Windows.Threading;

namespace CatoriCity2025WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowController _controller;
        LandscapeObjectService _landscapeObjectService = new LandscapeObjectService();
        SettingService _settingService = new SettingService();
        internal bool ispagedirty = false;
        DepositService _depositService = new DepositService();
        StatusControl1 statusUC = new StatusControl1();
        Point mouseOffset;
        public bool isdragging = false;
        public bool isMouseDown = false;
        private Point _previousPosition; // Stores the last mouse position
        private bool _isFirstMove = true; // To skip direction check on first move
        private readonly DispatcherTimer _mouseStopTimer;

        internal Brush MainLayoutBackground
        {
            get { return MainLayout.Background as SolidColorBrush; }
            set
            {
                if (value != null)
                {
                    MainLayout.Background = UIUtility.GetSolidColorBrush(value.ToString());
                }
                else
                {
                    MainLayout.Background = UIUtility.GetSolidColorBrush("White");
                }
            }
        }
        System.Windows.Threading.DispatcherTimer statusUpdatedispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            this.Width = 1820;
            this.Height = 980;
            DateTime now = DateTime.Now;
            statusUpdatedispatcherTimer.Tick += new EventHandler(statusUpdatedispatcherTimer_Tick);
            statusUpdatedispatcherTimer.Interval = new TimeSpan(0, 0, 30);

            string timestamp = now.ToString("yyyy-MM-dd_HH_mm_ss");

            cLogger.LogFilePath = System.IO.Path.Combine("c:\\Logs", "CatoriCity2026WPF" + timestamp + ".Log");
            GlobalStuff.mainWindowViewModel = new MainWindowViewModel();
            DataContext = GlobalStuff.mainWindowViewModel;
            GlobalStuff.MainView = this;
            Title = "Catori City Game 2026";
            _controller = new MainWindowController(this);

            string tooltipimagechest = System.IO.Path.Combine(GlobalStuff.ImageFolder, "Treasure","CHestClosed.png");

            ImageTextToolTip toolTip = new ImageTextToolTip
            {
                Title = "Treasure Field",
                Description = "View the treasure field to see the clues and information about the bad guys hidden treasure.",
                Icon = UIUtility.GetImageControl(tooltipimagechest, 32, 32, 0).Source
            };
            TreasureFieldViewButton.ToolTip = toolTip;
            
            RegisterDragDropHandlers();

            _mouseStopTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500) // delay after last movement
            };
            _mouseStopTimer.Tick += MouseStopped;

            statusUpdatedispatcherTimer.Start();
        }

        private void RegisterDragDropHandlers()
        {
            DragManager.RegisterDropHandler<StoreHardwareControl>(new StoreHardwareDropHandler());
            DragManager.RegisterDropHandler<FactoryControl>(new FactoryControlDropHandler());
            DragManager.RegisterDropHandler<LandscapeObjectControl>(new LandscapeToCanvasDropHandler());
        }

        private void MouseStopped(object? sender, EventArgs e)
        {
            _controller.primaryPerson.StopAnimation();
        }
        /// <summary>
        /// Update status with current funds and pay.  This runs every 30 seconds 
        /// to keep the status updated with any changes to funds or pay that may have occurred from other views or actions in the game.  It calculates the current funds by summing the user's deposits, current pay, and existing funds, then updates the status control's FundsLabel with the formatted currency value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void statusUpdatedispatcherTimer_Tick(object? sender, EventArgs e)
        {
            statusUpdatedispatcherTimer.Stop();
            decimal currentFunds = 0m;
            var foundfunds = from d in GlobalStuff.Deposits
                             where d.PersonId == GlobalStuff.CurrentUserPerson.PersonId
                             select d;
            if (foundfunds.Count() > 0)
            {
                currentFunds = foundfunds.First().Amount;
            }
            currentFunds += GlobalStuff.CurrentUserPerson.CurrentPay + GlobalStuff.CurrentUserPerson.Funds;
            statusUC.FundsLabel.Content = currentFunds.ToString("C");
            statusUpdatedispatcherTimer.Start();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GlobalStuff.Screenwidth = this.ActualWidth;
            GlobalStuff.Screenheight = this.ActualHeight;
            GlobalStuff.WriteDebugOutput("screen  " + $"Width: {GlobalStuff.Screenwidth}, Height: {GlobalStuff.Screenheight}");

            GlobalServices.LoadSettings();
            GlobalStuff.CurrentUserPerson = new PersonViewModel();
            SetCurrentUserPerson();
            GlobalStuff.CurrentHouseName = GlobalServices.GetSetting("CurrentHouseName").StringSetting;
            if (GlobalStuff.CurrentHouseName == null || GlobalStuff.CurrentHouseName == "")
            {
                // get 1st isuser
                var founduser = from h in GlobalStuff.Houses
                                select h;
                if (founduser.Count() > 0)
                {
                   GlobalStuff.CurrentHouseName = founduser.First().Name;
                   statusUC.NameLabel.Content = founduser.First().Name;
                }
            }
            _controller.Startup((int)YouStRect.Height);
            LoadPersonsOnCanvas();
            AddBadGuysToCanvas();
            SetSettings();
            SetTravelSpeed();
            _controller.SetupPaths();
            CreateStreetPaths();
            ispagedirty = false ;
        }
        private void SetCurrentUserPerson()
        {
            PersonService personService = new PersonService();
            var persons = personService.GetPersonsAsync().Result;
            int currentUser = GlobalServices.GetSetting("CurrentUserPersonId").IntSetting;
            if (currentUser > 0)
            {
                var foundperson = from p in persons
                                  where p.PersonId == currentUser
                                  select p;
                if (foundperson.Count() > 0)
                {
                    GlobalStuff.CurrentUserPerson = foundperson.First();
                }
            }
            else
            {
                var foundperson = from p in persons
                                  where p.IsUser == true
                                  select p;
                if (foundperson.Count() > 0)
                {
                    GlobalStuff.CurrentUserPerson = foundperson.First();
                }
            }
            statusUC.NameLabel.Content = GlobalStuff.CurrentUserPerson.Name;
        }

        private void SetTravelSpeed()
        {
            GlobalStuff.mainWindowViewModel.PolicecarToravelSpeed = GlobalServices.GetSetting("PoliceCarTravelSpeed").IntSetting;
            GlobalStuff.mainWindowViewModel.BadGuyTravelSpeed = GlobalServices.GetSetting("BadGuyTravelSpeed").IntSetting;
        }

        private void SetSettings()
        {
            var foundscreen = GlobalServices.GetSetting("ScreenBackgroundColor");
            if (foundscreen != null )
            {
                var screen = foundscreen;
                if (screen.StringSetting != null && screen.StringSetting != "")
                {
                    MainLayoutBackground = UIUtility.GetSolidColorBrush(screen.StringSetting);
                }
            }
        }

        private void LoadPersonsOnCanvas()
        {
             //PeopleSelectorList
            try
            {
                MainLayout.Children.Add(statusUC);
                statusUC.Width = 400;
                statusUC.Height = 100;
                statusUpdatedispatcherTimer.Start();
                double leftStatus = this.Width - statusUC.Width - 20;
                Canvas.SetLeft(statusUC, leftStatus);
                Canvas.SetTop(statusUC, 10);
                //string imagePath = GlobalStuff.ImageFolder + "\\PrimaryPeople";
                //string[] images = System.IO.Directory.GetFiles(imagePath, "*.*");
                List<PersonViewModel> persons = new List<PersonViewModel>();
                var personsPrimary = from p in GlobalStuff.AllPersons
                                     where p.IsUser == true
                                     select p;
                //persons = personsPrimary.ToList();
                foreach (var item in personsPrimary)
                {
                    item.StaticImageFilePath = System.IO.Path.Combine(item.ImagesFolder, item.FileNameOptional);
                    persons.Add(item);
                }

                var foundhousesimages = from i in GlobalStuff.AllPersons
                                        where i.IsUser == false
                                        select i;
              
                foreach (var item in foundhousesimages)
                {
                    item.Images = GlobalStuff.GetImagesForPerson(item.ImagesFolder);
                    item.SetupImageLists();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        
        }
        private void AddBadGuysToCanvas()
        {
         //   Canvas.SetZIndex(PeopleSelectorList, 1);
            //PeopleSelectorList
            try
            {

                var LandscapeChildren = MainLayout.Children.OfType<LandscapeObjectControl>().ToList();
                ispagedirty = false;
                LandscapeObjectViewModel model = new LandscapeObjectViewModel();
                LandscapeObjectControl foundUC = null;
                var models = from m in GlobalStuff.LandscapeObjects
                             where m.HomeObject == true
                             select m;
                if (models.Count() > 0)
                    model = models.First();

                foreach (var child in LandscapeChildren)
                {
                   if (model.Name == child.Name)
                    {
                        foundUC = child;
                        break;
                    }
                }
                var personsBad = from p in GlobalStuff.AllPersons
                                     where p.IsUser == false
                                     && p.PersonRole == PersonEnum.BadPerson
                                 select p;

                cLogger.Log("Adding bad guys to canvas count " + personsBad.Count().ToString());
                foreach (var item in personsBad)
                {
                   // if (GlobalStuff.bad)
                    BadPersonControl badPersonControl = new BadPersonControl();
                    badPersonControl.AddPerson(item);
                    //double width = foundUC.Width;
                    //double height = foundUC.Height;
                    double width = GlobalStuff.BadGuyWith;
                    double height = GlobalStuff.BadGuyHeight;
                    badPersonControl.Width = width;
                    badPersonControl.Height = height;
                    var locx = Canvas.GetLeft(foundUC);
                    var locy = Canvas.GetTop(foundUC);
                    locx += width / 2;
                    locy += height / 2;
                    badPersonControl.Opacity = .2;
                    Canvas.SetZIndex(badPersonControl, 1100);
                    Canvas.SetLeft(badPersonControl, locx);
                    Canvas.SetTop(badPersonControl, locy);
                    MainLayout.Children.Add(badPersonControl);
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }
      
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GlobalStuff.MainViewHeight = e.NewSize.Height - 50;
            GlobalStuff.MainViewWidth = e.NewSize.Width;
            SetStreetSizes();
            _controller.ResizeLots(e.PreviousSize, e.NewSize);
            //double groupsLabelTop = e.NewSize.Height;
            //groupsLabelTop -= 50;
            //Canvas.SetTop(GroupIdsPanel, groupsLabelTop);
        }
        private void SetStreetSizes()
        {
            //YouStreet; top
            //MikStreet; bottom
            //MooStreet; middle
            //YodelStreet; left
            //TeeaStreet; right

            //Intersections
            //youYodel
            //YouTea
            //YouMoo
            //MikYodel
            //MikMoo
            //MikTea

            StreetHelper.SetGlobalStreetvalues(this.Width, YouStRect.Height, MikStRect.Height, TeaStRect.Height,TeaStRect.Width);
            double horizstwidth = GlobalGeo.YouStreetloc.LocationEndXY.x - GlobalGeo.YouStreetloc.LocationStartXY.x;
            horizstwidth -= 5;
            YouStRect.Width = horizstwidth;
            MikStRect.Width = horizstwidth;
            GlobalGeo.YouStreetloc.LocationEndXY.x = GlobalGeo.YouStreetloc.LocationStartXY.x + horizstwidth;

            Canvas.SetTop(YouStRect, GlobalGeo.YouStreetloc.LocationStartXY.y);
            Canvas.SetTop(MikStRect, GlobalGeo.MikAveLoc.LocationStartXY.y);
            YodelStRect.Height = GlobalGeo.MikAveLoc.LocationStartXY.y - GlobalGeo.YouStreetloc.LocationEndXY.y  ;
            MooStRect.Height = GlobalGeo.MooDrLoc.LocationEndXY.y - GlobalGeo.MooDrLoc.LocationStartXY.y;
            Canvas.SetLeft(MooStRect, GlobalGeo.MooDrLoc.LocationStartXY.x);
            TeaStRect.Height = YodelStRect.Height + YouStRect.Height;// GlobalGeo.MooDrLoc.LocationEndXY.y - GlobalGeo.MooDrLoc.LocationStartXY.y ;
            Canvas.SetLeft(TeaStRect, GlobalGeo.TeaStreetLoc.LocationStartXY.x);
            Canvas.SetTop(TeaStRect, GlobalGeo.TeaStreetLoc.LocationStartXY.y);
        }

        private void CreateStreetPaths()
        {
            //sb events
            //https://learn.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/timing-events-overview
            //GlobalStuff.PoliceStationLocation.;
              // GlobalStuff.te
            //PathFigure startpathFigure = new PathFigure();

            //         LocationXYEntity nextLocationN = GlobalStuff.ApproachPointN;
            //        LineSegment nextLocationNlineSegment = new LineSegment
            //        {
            //            Point = new System.Windows.Point(Convert.ToInt32(nextLocationN.x), Convert.ToInt32(nextLocationN.y))
            //        };
            //        startpathFigure.Segments.Add(nextLocationNlineSegment);

            }

    
        private void SettingsViewButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsView view = new SettingsView();
            view.Owner = this;
            view.OnLandscapeGroupChange += View_OnLandscapeGroupChange;
            view.ShowDialog();
            if (view.ScreenBackgroundColor != null)
            {
                MainLayoutBackground = UIUtility.GetSolidColorBrush(view.ScreenBackgroundColor);
                SettingEntity setting = new SettingEntity();
                setting.Name = "ScreenBackgroundColor";
                setting.StringSetting = view.ScreenBackgroundColor;
                GlobalServices.Settings.Add(setting);
            }
        }

        private void View_OnLandscapeGroupChange(object? sender, Objects.Arguments.ReloadLandscapeSettingsArg e)
        {
            var childrenToRemove = MainLayout.Children.OfType<LandscapeObjectControl>().ToList();

            // Remove each child from the canvas
            foreach (var child in childrenToRemove)
            {
                MainLayout.Children.Remove(child);
            }
            _controller.LoadLandscapeObjects();
        }

        private void SaveLayoutButton_Click(object sender, RoutedEventArgs e)
        {
            SaveChanges();
           
        }
        private void SaveChanges()
        {
            var LandscapeChildren = MainLayout.Children.OfType<LandscapeObjectControl>().ToList();
            ispagedirty = false;
            // Remove each child from the canvas
            List<LandscapeObjectControl> ControlsToDelete = new List<LandscapeObjectControl>()  ;
            foreach (var child in LandscapeChildren)
            {
                double childleft = Canvas.GetLeft(child);
                double childtop = Canvas.GetTop(child);
                var models = from m in GlobalStuff.LandscapeObjects
                             where m.Name == child.Name
                             select m;
                if (models.Count() > 0)
                {

                    LandscapeObjectViewModel model = models.First();
                    if (model.DeleteModel == false)
                    {
                        model.Entity.xActual = childleft;
                        model.Entity.yActual = childtop;
                        _landscapeObjectService.Upsert(model.Entity);
                    }
                    else
                    {
                        _landscapeObjectService.Delete(model.Entity);
                        GlobalStuff.LandscapeObjects.Remove(model);
                        ControlsToDelete.Add(child);
                    }
                }
            }
              RemoveLandscapeObjects(ControlsToDelete);
                
                foreach (var item in GlobalServices.Settings)
            {
                _settingService.UpsertSetting(item);
            }
        }
        private void RemoveLandscapeObjects(List<LandscapeObjectControl> ControlsToDelete)
        {
            foreach (var item in ControlsToDelete)
            {
                MainLayout.Children.Remove(item);
            }
        }
        private void MainWin_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ispagedirty)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save your layout changes?", "Save Changes", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    SaveChanges();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true; // Cancel the closing event
                }
            }
        }
        internal void RunPoliceCars()
        {
            _controller.RunPoliceCars();
        }
        private void PoliceCarTravelSpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ispagedirty = true;
            GlobalStuff.mainWindowViewModel.PolicecarToravelSpeed = PoliceCarTravelSpeedSlider.Value;
        }
        private void BadGuyTravelSpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ispagedirty = true;
            GlobalStuff.mainWindowViewModel.BadGuyTravelSpeed = BadGuyTravelSpeedSlider.Value;
        }

        private void MainWin_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void AboutViewButton_Click(object sender, RoutedEventArgs e)
        {
            AboutView aboutView = new AboutView();
            aboutView.Owner = this;
            aboutView.ShowDialog();
        }

        private void AboutButtonImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AboutView aboutView = new AboutView();
            aboutView.Owner = this;
            aboutView.ShowDialog();

        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.ResetCharacters();
        }

        private void TreasureFile1ViewButton_Click(object sender, RoutedEventArgs e)
        {
            TreasureFieldView view = new TreasureFieldView(Width,Height);
            view.Owner = this;
            view.ShowDialog();
        }
        #region Mouse events 
      
        private void MainWin_MouseMove(object sender, MouseEventArgs e)
        {
            var pt = e.GetPosition(this);
            _mouseStopTimer.Stop();  // reset timer to detect if mouse stopped
            _mouseStopTimer.Start();

            //cLogger.Log("MainWin_MouseMove main x y" + pt.X.ToString() + "  " + pt.Y.ToString());
            // Get mouse position relative to the window
            Point relativePoint = e.GetPosition(MainLayout);

            double xpos = relativePoint.X;
            double toppos = relativePoint.Y;
            Point personpoint = e.GetPosition(_controller.primaryPerson);
            Point currentPosition = e.GetPosition(this);

            double deltaX = currentPosition.X - _previousPosition.X;
            double deltaY = currentPosition.Y - _previousPosition.Y;
            // Skip direction detection on the very first move
            if (_isFirstMove)
            {
                _previousPosition = currentPosition;
                _isFirstMove = false;
                deltaX = 0;
                deltaY = 0;
                _controller.primaryPerson.StopAnimation();
                return;
            }
 

            if (isdragging== false && isMouseDown)
                isdragging = UIUtility.CheckMouseMoveForDrag(relativePoint, personpoint);
            if (_controller.MovePerson)
            { 
            //if (isMouseDown == false)
            //    isdragging = false;
            //if (isdragging == true && _controller.primaryPerson != null)
            //{
                double personwidth = _controller.primaryPerson.PersonImage.ActualWidth;
                double personheight = _controller.primaryPerson.PersonImage.ActualHeight;
                double halfwidth = personwidth / 2;
                double halfheight = personheight / 2;
                double finalx = xpos - halfwidth;
                double finaltop = toppos - halfheight;
                //_controller._draggedShopItemControl.SetLocation(xpos, ypos);
                //cLogger.Log("isdragging  finalx " + finalx + " finaltop " + finaltop);
                //cLogger.Log("  xpos " + xpos + " ypos " + toppos);
                //cLogger.Log("  halfwidth " + halfwidth + " halfheight " + halfheight);
                Canvas.SetLeft(_controller.primaryPerson, finalx);
                Canvas.SetTop(_controller.primaryPerson, finaltop);
                MouseDirectionEnum direction = GetDirection(deltaX, deltaY);
                switch (direction)
                {
                    case MouseDirectionEnum.Up:
                    case MouseDirectionEnum.Down:
                    case MouseDirectionEnum.Left:
                        _controller.primaryPerson.WalkLeft();
                        break;
                    case MouseDirectionEnum.Right:
                        _controller.primaryPerson.WalkRight();
                        break;
                    case MouseDirectionEnum.UpLeft:
                        _controller.primaryPerson.WalkLeft();
                        break;
                    case MouseDirectionEnum.UpRight:
                        _controller.primaryPerson.WalkRight();
                        break;
                    case MouseDirectionEnum.DownLeft:
                        _controller.primaryPerson.WalkLeft();
                        break;
                    case MouseDirectionEnum.DownRight:
                        _controller.primaryPerson.WalkRight();
                        break;
                    case MouseDirectionEnum.None:
                        _controller.primaryPerson.StopAnimation();
                        break;
                    default:
                        break;
                }
            }

        }
        /// <summary>
        /// Determines the direction based on X and Y deltas.
        /// </summary>
        private MouseDirectionEnum GetDirection(double deltaX, double deltaY)
        {
            MouseDirectionEnum result = MouseDirectionEnum.None;
            const double threshold = 1.0; // Ignore tiny movements
            bool isset = false;
            if (Math.Abs(deltaX) < threshold && Math.Abs(deltaY) < threshold)
            {
                result = MouseDirectionEnum.None;
                return result; // No significant movement
            }

            if (Math.Abs(deltaX) > Math.Abs(deltaY))
            {
                isset = true;
                if (deltaX > 0)
                    result = MouseDirectionEnum.Right;
                else
                {
                    result = MouseDirectionEnum.Left;
                }
            }
            else if( isset == false)
            {
                if (deltaY > 0)
                    result = MouseDirectionEnum.Down;
                else
                {
                    result = MouseDirectionEnum.Up;
                }
            }
            if (isset == false)
            {
                // Diagonal movement
                if (deltaX > 0 && deltaY > 0) result = MouseDirectionEnum.DownRight;
                if (deltaX < 0 && deltaY > 0) result = MouseDirectionEnum.DownLeft;
                if (deltaX > 0 && deltaY < 0) result = MouseDirectionEnum.UpRight ;
                if (deltaX < 0 && deltaY < 0) result = MouseDirectionEnum.UpLeft;
            }
            return result;
        }
        private void MainWin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var pt = e.GetPosition(this);
            isMouseDown = true;
            mouseOffset = e.GetPosition(_controller.primaryPerson);

            //cLogger.Log("MainWin_MouseDown main x y" + pt.X.ToString() + "  " + pt.Y.ToString());

        }
        #endregion

      

        private void MainWin_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = false;

        }

        private void MainWin_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            

        }
    }

}