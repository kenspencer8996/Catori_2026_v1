using CatoriApp.Controllers;
using CatoriApp.Views;
using System.Windows.Input;
using System.Windows.Threading;
namespace CatoriApp.Views.City
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class CityScapeView : Window
    {
        CityScapeViewController _controller;
        LandscapeObjectService _landscapeObjectService = new LandscapeObjectService();
        SettingService _settingService = new SettingService();
        internal bool ispagedirty = false;
        DepositService _depositService = new DepositService();
        StatusControl1 statusUC = new StatusControl1();
        Point mouseOffset;
        private Point _previousPosition; // Stores the last mouse position
        private bool _isFirstMove = true; // To skip direction check on first move
        private readonly DispatcherTimer _mouseStopTimer;
        DragManager _dragManager;
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
        public CityScapeView()
        {
            InitializeComponent();
            this.Width = 1820;
            this.Height = 980;
            DateTime now = DateTime.Now;
            Left = 0;
            Top = 0;
            _dragManager = GlobalCode.GetDragmanager(MainLayout);
            statusUpdatedispatcherTimer.Tick += new EventHandler(statusUpdatedispatcherTimer_Tick);
            statusUpdatedispatcherTimer.Interval = new TimeSpan(0, 0, 30);

            string timestamp = now.ToString("yyyy-MM-dd_HH_mm_ss");

            CityScapeGlobal.mainWindowViewModel = new MainWindowViewModel();
            DataContext = CityScapeGlobal.mainWindowViewModel;
            CityScapeGlobal.CityScapeView = this;
            Title = "Catori City Game 2026";
            _controller = new CityScapeViewController(this);


             
            _mouseStopTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500) // delay after last movement
            };
            _mouseStopTimer.Tick += MouseStopped;

            statusUpdatedispatcherTimer.Start();
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
            var foundfunds = from d in CityScapeGlobal.Deposits
                             where d.PersonId == CityScapeGlobal.CurrentUserPerson.PersonId
                             select d;
            if (foundfunds.Count() > 0)
            {
                currentFunds = foundfunds.First().Amount;
            }
            currentFunds += CityScapeGlobal.CurrentUserPerson.CurrentPay + CityScapeGlobal.CurrentUserPerson.Funds;
            statusUC.FundsLabel.Content = currentFunds.ToString("C");
            statusUpdatedispatcherTimer.Start();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CityScapeGlobal.Screenwidth = this.ActualWidth;
            CityScapeGlobal.Screenheight = this.ActualHeight;
            CityScapeGlobal.WriteDebugOutput("screen  " + $"Width: {CityScapeGlobal.Screenwidth}, Height: {CityScapeGlobal.Screenheight}");

            GlobalServices.LoadSettings();
            CityScapeGlobal.CurrentUserPerson = new PersonViewModel();
            SetCurrentUserPerson();
            CityScapeGlobal.CurrentHouseName = GlobalServices.GetSetting("CurrentHouseName").StringSetting;
            if (CityScapeGlobal.CurrentHouseName == null || CityScapeGlobal.CurrentHouseName == "")
            {
                // get 1st isuser
                var founduser = from h in CityScapeGlobal.Houses
                                select h;
                if (founduser.Count() > 0)
                {
                   CityScapeGlobal.CurrentHouseName = founduser.First().Name;
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
                    CityScapeGlobal.CurrentUserPerson = foundperson.First();
                }
            }
            else
            {
                var foundperson = from p in persons
                                  where p.IsUser == true
                                  select p;
                if (foundperson.Count() > 0)
                {
                    CityScapeGlobal.CurrentUserPerson = foundperson.First();
                }
            }
            statusUC.NameLabel.Content = CityScapeGlobal.CurrentUserPerson.Name;
        }

        private void SetTravelSpeed()
        {
            CityScapeGlobal.mainWindowViewModel.PolicecarToravelSpeed = GlobalServices.GetSetting("PoliceCarTravelSpeed").IntSetting;
            CityScapeGlobal.mainWindowViewModel.BadGuyTravelSpeed = GlobalServices.GetSetting("BadGuyTravelSpeed").IntSetting;
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
                var personsPrimary = from p in GlobalAllApps.AllPersons
                                     where p.IsUser == true
                                     select p;
                //persons = personsPrimary.ToList();
                foreach (var item in personsPrimary)
                {
                    item.StaticImageFilePath = System.IO.Path.Combine(item.ImagesFolder, item.FileNameOptional);
                    persons.Add(item);
                }

                var foundhousesimages = from i in GlobalAllApps.AllPersons
                                        where i.IsUser == false
                                        select i;
              
                foreach (var item in foundhousesimages)
                {
                    item.Images = CityScapeGlobal.GetImagesForPerson(item.ImagesFolder);
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
                var models = from m in CityScapeGlobal.LandscapeObjects
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
                var personsBad = from p in GlobalAllApps.AllPersons
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
                    double width = CityScapeGlobal.BadGuyWith;
                    double height = CityScapeGlobal.BadGuyHeight;
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
            CityScapeGlobal.CityScapeViewHeight = e.NewSize.Height - 50;
            CityScapeGlobal.CityScapeViewWidth = e.NewSize.Width;
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
                var models = from m in CityScapeGlobal.LandscapeObjects
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
                        CityScapeGlobal.LandscapeObjects.Remove(model);
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
            CityScapeGlobal.mainWindowViewModel.PolicecarToravelSpeed = PoliceCarTravelSpeedSlider.Value;
        }
        private void BadGuyTravelSpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ispagedirty = true;
            CityScapeGlobal.mainWindowViewModel.BadGuyTravelSpeed = BadGuyTravelSpeedSlider.Value;
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
        }
        private void MainWin_MouseDown(object sender, MouseButtonEventArgs e)
        {
 
        }
        #endregion

      

        private void MainWin_MouseUp(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void MainWin_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            

        }

        private void MainLayout_MouseMove(object sender, MouseEventArgs e)
        {
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

}

