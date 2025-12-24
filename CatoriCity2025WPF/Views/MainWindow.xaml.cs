using CatoriCity2025WPF.Controllers;
using CatoriCity2025WPF.Views;
using System.Windows.Input;

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
            DateTime now = DateTime.Now;
            statusUpdatedispatcherTimer.Tick += new EventHandler(statusUpdatedispatcherTimerr_Tick);
            statusUpdatedispatcherTimer.Interval = new TimeSpan(0, 5, 0);

            string timestamp = now.ToString("yyyy-MM-dd_HH");

            cLogger.LogFilePath = System.IO.Path.Combine("c:\\Logs", "CatoriCity2025WPF" + timestamp + ".Log");
            GlobalStuff.mainWindowViewModel = new MainWindowViewModel();
            DataContext = GlobalStuff.mainWindowViewModel;
            GlobalStuff.MainView = this;
            Title = "Catori City Game 2025";
            _controller = new MainWindowController(this);
        }

        private void statusUpdatedispatcherTimerr_Tick(object? sender, EventArgs e)
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
            currentFunds += GlobalStuff.CurrentUserPerson.CurrentPay;
            statusUC.FundsLabel.Content = currentFunds.ToString();
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
            LoadPersons();
            AddBadGuysToCanvas();
            SetSettings();
            SetTravelSpeed();
            _controller.SetupPaths();
            CreateStreetPaths();
            PeopleSelectorList.Visibility = Visibility.Collapsed;
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

        private void LoadPersons()
        {
            Canvas.SetZIndex(PeopleSelectorList, 200);
            double left = this.Width - PeopleSelectorList.Width - 20;
            Canvas.SetLeft(PeopleSelectorList, left);
            
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
                PeopleSelectorList.ItemsSource = persons;

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
        private ListBoxItem GetListBoxItemForPerson(string imagename)
        {
            string imagePath = Imagehelper.GetImagePath(imagename);
            ListBoxItem item = new ListBoxItem();
            Image image = new Image();
            image.Source = UIUtility.GetImageControl(imagePath, 50, 50, 0).Source;
            image.Width = 20;
            image.Height = 30;
            image.AllowDrop = false;
            StackPanel stack = new StackPanel();
            stack.Children.Add(image);
            item.Content = stack;

            return item;
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GlobalStuff.MainViewHeight = e.NewSize.Height - 50;
            GlobalStuff.MainViewWidth = e.NewSize.Width;
            SetStreetSizes();

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

        private void PeopleSelectorImage_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void PeopleSelectorList_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
           
        }

        private void PeopleSelectorList_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }
       private void PeopleListSelectDrag()
        {
            if (PeopleSelectorList.SelectedItem != null)
            {
                PersonViewModel selected = (PersonViewModel)PeopleSelectorList.SelectedItem;
                string modelstring = GenericSerializer.Serializer<PersonViewModel>(selected);
                DataObject data = new DataObject();
                data.SetText(modelstring);
                DragDrop.DoDragDrop(PeopleSelectorList, modelstring, DragDropEffects.Move);
                PeopleSelectorList.Visibility = Visibility.Collapsed;
                statusUC.NameLabel.Content = selected.Name;
                GlobalStuff.CurrentUserPerson = selected;
                GlobalServices.UpdateSetting("CurrentUserPersonId","", selected.PersonId);
            }
        }
        private void PeopleSelectorList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PeopleListSelectDrag();
            PeopleSelectorList.SelectedItem = null;
        }

        private void ShowSelectorButton_Click(object sender, RoutedEventArgs e)
        {
            PeopleSelectorList.Visibility = Visibility.Visible;
        }

        private void MainWin_MouseMove(object sender, MouseEventArgs e)
        {
            var pt = e.GetPosition(this);
            //cLogger.Log("MainWin_MouseMove main x y" + pt.X.ToString() + "  " + pt.Y.ToString());

        }
        private void MainWin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var pt = e.GetPosition(this);
            //cLogger.Log("MainWin_MouseDown main x y" + pt.X.ToString() + "  " + pt.Y.ToString());

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
    }
    
}