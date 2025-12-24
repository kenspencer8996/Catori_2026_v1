using CatoriCity2025WPF.ExtensionMethods;
using System.Windows.Threading;

namespace CatoriCity2025WPF.Controllers
{
    internal class MainWindowController
    {
        MainWindow _view;

        internal CityscapeStreetsViewModel Model = new CityscapeStreetsViewModel();
        public bool instartupFlag = true;
        bool VisualContentLoaded = false;
        MapPositionEntity _mapPosition = new MapPositionEntity();
        LotHelper lotHelper;
        int _streetwidth;
        private List<LotControl> _lots = new List<LotControl>();
        DispatcherTimer _updatePathsTimerTimer;
        System.Timers.Timer interestAddTimer;
        internal MainWindowController(MainWindow view)
        {
            _view = view;
            //NlogSetup.Configure();

            cLogger.Log("MainWindowController Constructor");
            GlobalStuff.TimingsRandom = new List<int>();
            Random rnd = new Random();
            for (int i = 1; i <= 50; i++)
            {
                int rInt = rnd.Next(0, 5);
                GlobalStuff.TimingsRandom.Add(rInt);
            }

        }
        internal void Startup(int streetwidth)
        {
            try
            {
                cLogger.Log("MainWindowController Startup");
                _streetwidth = streetwidth;
                CreateLots();
                GlobalStuff.Houses = ImageFileHelper.GetHouses();
                GlobalStuff.Banks = ImageFileHelper.GetBanks();
                LoadPersons();
                GlobalStuff.Businesses.AddRange(ImageFileHelper.GetFactories());
                LoadHouses();
                LoadBanks();
                LoadFactories();
                AddPoliceStation();
                loadPoliceCars();
                //AddToLandscapeItems();
                LoadLandscapeObjects();
                GetLandscapeObjectsGroupIds();
                _view.BaloonHelpUC.Visibility = Visibility.Collapsed;
                double mainwidth = _view.Width;
                double mainheight = _view.Height;
                Canvas.SetTop(_view.BadGuySpeedStackPanel, mainheight - _view.BadGuySpeedStackPanel.Height);
                Canvas.SetLeft(_view.BadGuySpeedStackPanel, mainwidth - _view.BadGuySpeedStackPanel.Width - 20);
                GlobalStuff.mainWindowViewModel.BadGuyTravelSpeed = GlobalServices.GetSettingByName("BadGuyTravelSpeed").IntSetting;
                GlobalStuff.mainWindowViewModel.PolicecarToravelSpeed = GlobalServices.GetSettingByName("PoliceCarTravelSpeed").IntSetting;
                _view.BaloonHelpUC.MoveBaloon += BaloonHelpUC_MoveBaloon;
                _view.BaloonHelpUC.Visibility = Visibility.Collapsed;
                _view.BadGuyTravelSpeedSlider.Value = GlobalStuff.mainWindowViewModel.BadGuyTravelSpeed;
                _updatePathsTimerTimer = new DispatcherTimer();
                _updatePathsTimerTimer.Tick += new EventHandler(__updatePathsTimerTimer_Tick);
                _updatePathsTimerTimer.Interval = new TimeSpan(0, 0, 30);

                interestAddTimer = new System.Timers.Timer();
                interestAddTimer.Interval = 60000 * 10; // 10 minutes
                interestAddTimer.Elapsed += InterestAddTimer_Elapsed;
                interestAddTimer.Start();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void InterestAddTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            interestAddTimer.Stop();
            DepositService  depositService = new DepositService();  
            BankService bankService = new BankService();
            var banks = bankService.GetAllAsync().Result;   
            depositService.CalculateDeposits(banks).Wait();

            interestAddTimer.Start();

        }

        private void AddfactoryIntrnalView(double leftFactory,double topFactory)
        {
            double width = 300;
            double height = 200;
            GlobalStuff.factoryInteriorControl.Width = width;
            GlobalStuff.factoryInteriorControl.Height = height;
            GlobalStuff.factoryInteriorControl.Visibility = Visibility.Collapsed;
            _view.MainLayout.Children.Add(GlobalStuff.factoryInteriorControl);
            Canvas.SetLeft(GlobalStuff.factoryInteriorControl, leftFactory - width);
            Canvas.SetTop(GlobalStuff.factoryInteriorControl, topFactory + 100 );
            Canvas.SetZIndex(GlobalStuff.factoryInteriorControl, 2000);
        }

        private void BaloonHelpUC_MoveBaloon(object? sender, Objects.Arguments.BaloonLocationInfoArgument e)
        {
            SetbaloonPosition(e.Left, e.Top);
        }

        private void SetbaloonPosition(double baloonLeft, double baloonTop)
        {
            Canvas.SetLeft(_view.BaloonHelpUC, baloonLeft);
            Canvas.SetTop(_view.BaloonHelpUC, baloonTop);
            Canvas.SetZIndex(_view.BaloonHelpUC, 1250);

        }
        private void loadPoliceCars()
        {
            PoliceCarRepository policeCarRepo = new PoliceCarRepository();
            GlobalStuff.PoliceCars = (List<PoliceCarEntity>) policeCarRepo.GetPoliceCars();
            LotControl lotControl = GlobalStuff.PoliceStationLocation;
            int policecarincrementer = 0;
            int carNumber = 1;
            foreach (var car in GlobalStuff.PoliceCars)
            {

                if (car.CarType.ToLower() == "normal")
                {
                    AddPoliceCar(car, policecarincrementer, lotControl, carNumber);
                }
                else 
                {
                    AddCarPoliiceOffRoad(car,policecarincrementer, lotControl);
                }
                policecarincrementer += 0;
                carNumber++;
            }
        }

        private void __updatePathsTimerTimer_Tick(object? sender, EventArgs e)
        {
            _updatePathsTimerTimer.Stop();
            SetupPaths();
            _updatePathsTimerTimer.Start();
        }

        internal void SetupPaths()
        {
            _updatePathsTimerTimer.Start();

        }

        private void LoadPersons()
        {
            PersonService personService = new PersonService();
            GlobalStuff.AllPersons = personService.GetPersonsAsync().Result;

        }
        #region Add methods

        internal void AddCarPoliiceOffRoad(PoliceCarEntity car,int policecarincrementer, LotControl lotControl)
        {
            CarPoliiceOffRoadControl carControl = new CarPoliiceOffRoadControl();
            carControl.Visibility = Visibility.Visible;
            carControl.Name = car.CarName + policecarincrementer;
            carControl.Width = 60;
            carControl.Height = 25;
            string imagepath = Imagehelper.GetImagePath(car.ImageName);
            carControl.CarImage.Source = UIUtility.GetImageControl(imagepath, carControl.Width, carControl.Height, 0).Source;
            Canvas.SetZIndex(carControl, 1210);
            _view.MainLayout.Children.Add(carControl);

            Canvas.SetLeft(carControl, Canvas.GetLeft(lotControl) );
            Canvas.SetTop(carControl, Canvas.GetTop(lotControl) + lotControl.ActualHeight );
        }
        internal void AddPoliceCar(PoliceCarEntity policeCar, int policecarincrementer, LotControl lotControl,int carNumber)
        {
            try
            {
                CarPoliiceControl carControl = new CarPoliiceControl(policeCar.ImageName, 60, 25, carNumber);
                carControl.Visibility = Visibility.Visible;
                carControl.Name = policeCar.CarName + policecarincrementer;
                Canvas.SetZIndex(carControl, 1210);
                _view.MainLayout.Children.Add(carControl);
                Canvas.SetLeft(carControl, Canvas.GetLeft(lotControl));
                Canvas.SetTop(carControl, Canvas.GetTop(lotControl) + lotControl.ActualHeight);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        internal void AddPoliceStation()
        {
            try
            {
                PoliceStationUC ps = new PoliceStationUC();
                ps.Width = GlobalStuff.buildingsize - 10;
                ps.Height = GlobalStuff.buildingsize - 10;
                Canvas.SetZIndex(ps, 4);
                var found = from lotPS in _lots
                            where lotPS.LotOccupied == false
                            && lotPS.Street == StreetsEnum.Teastreet
                            select lotPS;
                LotControl lotControl = found.First();
                if (found.Any())
                {
                    try
                    {
                        double x = Canvas.GetLeft(lotControl);
                        double y = Canvas.GetTop(lotControl);
                        LotEntity policeLot = new LotEntity();
                        policeLot.x = x;
                        policeLot.y = y;
                        policeLot.lotPosition = LotPositionEnum.RightSide;
                        lotControl.AddBuilding(ps);
                        GlobalGeo.PoliceStationLocationEntity = policeLot;
                        GlobalStuff.PoliceStationLocation = lotControl;
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }

                //Canvas.SetLeft(ps, left);
                //Canvas.SetTop(ps, top);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion
        #region

        internal void CreateLots()
        {
            int ewlotcount = GlobalGeo.StLengthEW / GlobalGeo.LotSize;
            _lots = new List<LotControl>();
            int nslotcount = GlobalGeo.StLengthNS / GlobalGeo.LotSize;
            int lotloccounter = 100;
            //layout EW lots
            string info2 = "--------------------------------------------------Lots------------------------------" + Environment.NewLine;
            cLogger.Log(info2);

            for (int i = 0; i < ewlotcount; i++)
            {
                LotControl lotControl = GetLotControl();
                lotControl.Width = GlobalGeo.LotSize;
                lotControl.Height = GlobalGeo.LotSize - 22;
                Canvas.SetTop(lotControl, 10);
                Canvas.SetLeft(lotControl, lotloccounter);
                lotControl.Street = StreetsEnum.YouStreet;
                _lots.Add(lotControl);
                _view.MainLayout.Children.Add(lotControl);

                LotControl lotControlbottom = GetLotControl();
                Canvas.SetTop(lotControlbottom, GlobalGeo.MikAveLoc.LocationStartXY.y + _streetwidth);
                Canvas.SetLeft(lotControlbottom, lotloccounter);
                lotControlbottom.Street = StreetsEnum.MikAve;
                _lots.Add(lotControlbottom);
                _view.MainLayout.Children.Add(lotControlbottom);

                double leftlot = Canvas.GetLeft(lotControlbottom);
                double toplot = Canvas.GetLeft(lotControlbottom);
                cLogger.Log($" bottom lot loc " + "x " + leftlot + " y " + toplot + " lotlocationconter " + lotControl); // HouseControl: /CatoriCity2025WPF; 100 100

                lotloccounter += GlobalGeo.LotSize;
            }
            lotloccounter = (int)GlobalGeo.YouStreetloc.LocationStartXY.y;
            //layout NS lots
            for (int i = 0; i < nslotcount; i++)
            {
                LotControl lotControl = GetLotControl();
                Canvas.SetTop(lotControl, lotloccounter);
                Canvas.SetLeft(lotControl, 0);
                _view.MainLayout.Children.Add(lotControl);

                double left = GlobalGeo.TeaStreetLoc.LocationStartXY.x + 50;
                LotControl lotControlRight = GetLotControl();
                Canvas.SetTop(lotControlRight, lotloccounter);
                Canvas.SetLeft(lotControlRight, left);
                lotControlRight.Street = StreetsEnum.Teastreet;
                _lots.Add(lotControlRight);
                _view.MainLayout.Children.Add(lotControlRight);
                //double left = Canvas.GetLeft(lotControlRight);
                lotloccounter += GlobalGeo.LotSize;
            }
        }
        private LotControl GetLotControl()
        {
            LotControl lotControl = new LotControl();
            lotControl.Width = GlobalGeo.LotSize;
            lotControl.Height = GlobalGeo.LotSize;
            return lotControl;
        }
        #endregion



        #region Load Routines
        private void LoadHouses()
        {
            int top = 100;
            int left = 100;
            cLogger.Log($"houses : {GlobalStuff.Houses.Count}"); // HouseControl: /CatoriCity2025WPF; 100 100

            PersonViewModel currentPerson = new PersonViewModel();
            if (GlobalStuff.CurrentUserPerson.PersonId > 0)
            {
                var person = from i in GlobalStuff.AllPersons
                                    where i.PersonId == GlobalStuff.CurrentUserPerson.PersonId
                                    select i;
                currentPerson = person.First();
            }
            try
            {
                foreach (var house in GlobalStuff.Houses)
                {
                    try
                    {
                        HouseControl houseControl = new HouseControl(house);
                        if (house.Name.Trim().ToLower() == GlobalStuff.CurrentHouseName.Trim().ToLower()
                            && currentPerson.Name != null && currentPerson.Name != "")
                        {
                            houseControl.AddPersonModel(currentPerson);
                        }
                        houseControl.Width = GlobalStuff.buildingsize;
                        houseControl.Height = GlobalStuff.buildingsize - 10;
                        Canvas.SetZIndex(houseControl, 100);
                        Canvas.SetLeft(houseControl, left);
                        Canvas.SetTop(houseControl, top);
                        var found = from lot in _lots
                                    where lot.LotOccupied == false
                                    && lot.Street == StreetsEnum.YouStreet
                                    select lot;
                        if (found.Any())
                        {
                            found.First().AddBuilding(houseControl);
                        }
                        left += 90;
                    }
                    catch (Exception exi)
                    {

                        throw;
                    }
                    cLogger.Log($"HouseControl: {house.HouseImageFileName}"); // HouseControl: /CatoriCity2025WPF; 100 100
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private void LoadBanks()
        {
            try
            {
                int top = 100;
                int left = 100;
                double Width = GlobalStuff.buildingsize; //  double Width
                double Height = GlobalStuff.buildingsize; //  double Width
                var banks = from b in GlobalStuff.Banks
                            select b;
                cLogger.Log($"houses : {GlobalStuff.Houses.Count}"); // HouseControl: /CatoriCity2025WPF; 100 100
                string info = "---------------------------------Banks-----------------------------------------------" + Environment.NewLine;
                cLogger.Log(info);

                var foundlots = from lot in _lots
                                where lot.LotOccupied == false
                                && lot.Street == StreetsEnum.MikAve
                                select lot;
                LotControl firstlot = new LotControl();
                if (foundlots.Any())
                {
                    firstlot = foundlots.First();
                }
                int x = (int)Canvas.GetLeft(firstlot);
                int y = (int)Canvas.GetTop(firstlot);
                List<BankViewModel> bankViewModels = new List<BankViewModel>();
                BankService bankService = new BankService();
                bankViewModels = bankService.GetAllAsync().Result;
                foreach (var item in banks)
                {
                    try
                    {
                        var foundbankmodel = from bvm in bankViewModels
                                             where bvm.Bankkey == item.Bankkey
                                             select bvm;
                        BankControl bankUC = new BankControl(left, top);
                        item.BankUC = bankUC;
                        if (foundbankmodel.Any())
                        {
                            item.BankId = foundbankmodel.First().BankId;
                        }
                        else
                        {

                        }
                        bankUC.Model = item;
                        bankUC.BusinessImage.Source = UIUtility.GetImageControl(item.ImageFileName, Width, Height, 0).Source;
                        bankUC.BankKey = item.Bankkey;
                        if (item.Name == "")
                        {
                            string busname = System.IO.Path.GetFileNameWithoutExtension(item.Bankkey);
                            item.Name = busname;
                        }
                        bankUC.Width = GlobalStuff.buildingsize;
                        bankUC.Height = GlobalStuff.buildingsize;
                        bankUC.businessName = item.Name;
                        Canvas.SetZIndex(bankUC, 100);
                        Canvas.SetLeft(bankUC, left);
                        Canvas.SetTop(bankUC, top);
                        var found = from lot in _lots
                                    where lot.LotOccupied == false
                                    && lot.Street == StreetsEnum.MikAve
                                    select lot;
                        LotControl lotControl = new LotControl();
                        if (found.Any())
                        {
                            lotControl = found.First();
                            lotControl.AddBuilding(bankUC);
                            GlobalStuff.FinancialLotCobtrols.Add(found.First());
                            bankUC.Funds.Y = Canvas.GetTop(lotControl);
                            bankUC.Funds.X = Canvas.GetLeft(lotControl);
                            bankUC.ParentLeft = bankUC.Funds.X;
                            bankUC.Parentop = bankUC.Funds.Y;
                            item.X = bankUC.Funds.X;
                            item.Y = bankUC.Funds.Y;
                        }
                        //_view.MainLayout.Children.Add(houseControl);
                        left += 90;
                        int lotLeft = (int)Canvas.GetLeft(lotControl);
                        int lotTop = (int)Canvas.GetTop(lotControl);
                        cLogger.Log($"BankControl: {item.ImageFileName}" + "x " + lotLeft + " y " + lotTop);

                    }
                    catch (Exception exinner)
                    {

                        throw;
                    }
                }
                string info2 = "--------------------------------------------------------------------------------" + Environment.NewLine;
                cLogger.Log(info2);

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        
        private void LoadFactories()
        {
            int top = 100;
            int left = 100;
            double Width = GlobalStuff.buildingsize - 20; //  double Width
            double Height = GlobalStuff.buildingsize - 20; //  double Width
            var business = from b in GlobalStuff.Businesses
                           where b.BusinessType == BusinessTypeEnum.Factory
                           select b;
            var foundlots = from lot in _lots
                        where lot.LotOccupied == false
                        && lot.Street == StreetsEnum.Teastreet
                        select lot;
            LotControl firstlot = new LotControl();
            if (foundlots.Any())
            {
                firstlot = foundlots.First();
            }
            int x = (int)Canvas.GetLeft(firstlot);
            int y = (int)Canvas.GetTop(firstlot);
            Canvas.SetLeft(_view.FactoryHelp, x - 75);
            Canvas.SetTop(_view.FactoryHelp, y + 50 );
            Canvas.SetZIndex(_view.FactoryHelp, 100);
            int I = 0;
            double leftFistFactory = Canvas.GetLeft( firstlot);
            double topFistFactory = Canvas.GetTop(firstlot);
            foreach (var item in business)
            {
                FactoryControl factoryControl = new FactoryControl();
                factoryControl.BusinessImage.Source = UIUtility.GetImageControl(item.ImageName, Width, Height, 0).Source; ;
                factoryControl.Width = GlobalStuff.buildingsize;
                factoryControl.Height = GlobalStuff.buildingsize;
                Canvas.SetZIndex(factoryControl, 100);
                Canvas.SetLeft(factoryControl, left);
                Canvas.SetTop(factoryControl, top);
                if (I == 0)
                {
                    I++;
                }
                var found = from lot in _lots
                            where lot.LotOccupied == false
                            && lot.Street == StreetsEnum.Teastreet
                            select lot;
                if (found.Any())
                {
                    found.First().AddBuilding(factoryControl);
                }
                string leftpos = Canvas.GetLeft(factoryControl).ToString();
                // cLogger.Log($"BusinessControl: {businessControl.BusinessImage.Source}  {left} {top}");
                //_view.MainLayout.Children.Add(businessControl);
                left += 90;
                cLogger.Log($"BusinessControl: {item.ImageName}"); // HouseControl: /CatoriCity2025WPF; 100 100
            }
            AddfactoryIntrnalView(leftFistFactory,topFistFactory);
        }

        #endregion

        #region Landscape loading
        public void GetLandscapeObjectsGroupIds()
        {
            double mainwidth = _view.Width;
            double mainheight = _view.Height;

            LandscapeObjectRepository repository = new LandscapeObjectRepository();
            GlobalStuff.landscapeObjectGroupIds = repository.GetLandscapeObjectsGroupIds();
        }

        private void LandscapeObjectButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int groupid = (int)button.Content;
            SwitchLandscape(groupid);
            cLogger.Log($"GroupId: {groupid}");
        }

        private void SwitchLandscape(int groupid)
        {
        }

        public async void LoadLandscapeObjects()
        {
            double mainwidth = _view.Width;
            double mainheight = _view.Height;
            GlobalStuff.LandscapeUCs = new List<LandscapeObjectControl>();
            LandscapeObjectService landscapeservice = new LandscapeObjectService();
            GlobalStuff.LandscapeObjects = await landscapeservice.GetLandscapeObjectsAsync();
            LandscapeObjectViewModel featureModel = new LandscapeObjectViewModel();
            foreach (var landscapeObject in GlobalStuff.LandscapeObjects)
            {
                if (landscapeObject.FeatureNote == null || landscapeObject.FeatureNote == "")
                {
                    LandscapeObjectControl thisUC = new LandscapeObjectControl();
                    double x, y;
                    thisUC = GetLandscapeObject(landscapeObject,landscapeObject.Name);
                    x = landscapeObject.xActual;
                    y = landscapeObject.yActual;

                    GlobalStuff.LandscapeUCs.Add(thisUC);
                    Canvas.SetZIndex(thisUC, 1101);
                    _view.MainLayout.Children.Add(thisUC);
                    Canvas.SetLeft(thisUC, x);
                    Canvas.SetTop(thisUC, y);
                    thisUC.SetCenter(x, y);
                    if (landscapeObject.NextFromHomeObject)
                        GlobalStuff.NextFromHomeObject = landscapeObject;
                    if (landscapeObject.HomeObject)
                        GlobalStuff.HomeLandscapeObject = landscapeObject;
                }
                else
                {
                    cLogger.Log("LoadLandscapeObjects - feature note for " + landscapeObject.Name);
                    featureModel = landscapeObject; 
                }
            }
            SetupApproachPoints();
            SetupApproachSettingPoints(featureModel);
        }

        private LandscapeObjectControl GetLandscapeObject(LandscapeObjectViewModel landscapeObject,
            string name)
        {
            LandscapeObjectControl thisUC = new LandscapeObjectControl();
            thisUC.Name = name;
            thisUC.Location = new LocationXYEntity() { x = GlobalStuff.Tentx, y = GlobalStuff.Tenty };
            thisUC.Height = landscapeObject.Height;
            thisUC.Width = landscapeObject.Width;
            //thisUC.Height = 60;
            //thisUC.Width = 40;
            thisUC.AddImage(landscapeObject.ImageName, (int)thisUC.Width, (int)thisUC.Height);
            thisUC.OnDragDropChange += ThisUC_OnDragDropChange;

            return thisUC;
        }

        private void SetupApproachSettingPoints(LandscapeObjectViewModel featureModel)
        {
            if (featureModel != null  && featureModel.Name != "")
            {
                featureModel.Height = 20;
                featureModel.Width = 20;
                cLogger.Log("SetupApproachSettingPoints - featureModel is null");
                string name = "NorthApproach";
                LandscapeObjectControl thisUCN = GetLandscapeObject(featureModel,name);
                _view.MainLayout.Children.Add(thisUCN);
                name = "EastApproach";
                LandscapeObjectControl thisUCE = GetLandscapeObject(featureModel,name);
                _view.MainLayout.Children.Add(thisUCE);
                name = "SouthApproach";
                LandscapeObjectControl thisUCS = GetLandscapeObject(featureModel, name);
                _view.MainLayout.Children.Add(thisUCS);
                name = "WestApproach";
                LandscapeObjectControl thisUCW = GetLandscapeObject(featureModel, name);
                _view.MainLayout.Children.Add(thisUCW);

                //thisUCN.RenderTransform = GetRenderTransform(0,featureModel.Width,featureModel.Height);
                thisUCE.RenderTransform = GetRenderTransform(90, featureModel.Width, featureModel.Height);
                thisUCS.RenderTransform = GetRenderTransform(180, featureModel.Width, featureModel.Height);
                thisUCW.RenderTransform = GetRenderTransform(270, featureModel.Width, featureModel.Height);

                GlobalStuff.LandscapeObjectApproachNextControls = new List<LandscapeObjectControl>()
                {
                    thisUCN,
                    thisUCE,
                    thisUCS,
                    thisUCW
                };
                var offsetsetting = GlobalServices.GetSettingByName("nextfrmhomeoffset");
                int offsetsettingval = (int)offsetsetting.IntSetting;

                int offset = offsetsettingval;
                Canvas.SetZIndex(thisUCN, 1101);
                Canvas.SetLeft(thisUCN, GlobalStuff.ApproachPointN.x);
                Canvas.SetTop(thisUCN, GlobalStuff.ApproachPointN.y - offset);

                Canvas.SetZIndex(thisUCE, 1101);
                Canvas.SetLeft(thisUCE, GlobalStuff.ApproachPointE.x);
                Canvas.SetTop(thisUCE, GlobalStuff.ApproachPointE.y);
                
                Canvas.SetZIndex(thisUCS, 1101);
                Canvas.SetLeft(thisUCS, GlobalStuff.ApproachPointS.x);
                Canvas.SetTop(thisUCS, GlobalStuff.ApproachPointS.y+ offset);

                Canvas.SetZIndex(thisUCW, 1101);
                Canvas.SetLeft(thisUCW, GlobalStuff.ApproachPointW.x- offset);
                Canvas.SetTop(thisUCW, GlobalStuff.ApproachPointW.y);
            }
        }
        private RotateTransform GetRenderTransform(int angle,double width,double height)
        {
            RotateTransform rotateTransform = new RotateTransform
            {
                Angle = angle, // Rotation angle in degrees
                CenterX = width / 2, // Center of the UserControl
                CenterY = height / 2
            };

            return rotateTransform;
        }
        private void SetupApproachPoints()
        {
           var nextuc = GlobalStuff.GetNextFromHomeObject(); ;
            double nextx = Canvas.GetLeft(nextuc);
            double nexty = Canvas.GetTop(nextuc);
            double width = nextuc.Width;
            double height = nextuc.Height;
            double centerx = nextx + (width / 2);
            double centery = nexty + (height / 2);
            double distancedivsor = 1.4;
            GlobalStuff.ApproachPointN = new LocationXYEntity()
            {
                x = centerx ,
                y = centery - (height / distancedivsor)
            };
            GlobalStuff.ApproachPointE = new LocationXYEntity()
            {
                x = centerx + (width + (width / distancedivsor)),
                y = centery + (height / distancedivsor)
            };
            GlobalStuff.ApproachPointS = new LocationXYEntity()
            {
                x = centerx ,
                y = centery + (height / 2 + (height / distancedivsor))
            };
            GlobalStuff.ApproachPointW = new LocationXYEntity()
            {
                x = centerx - (width / distancedivsor),
                y = centery 
            };
        }

        private void ThisUC_OnDragDropChange(object? sender, Objects.Arguments.DragDropChangeArgs e)
        {
           _view.ispagedirty = true;
        }

        internal void RunPoliceCars()
        {
            //get financials and set below
            var banks = GlobalStuff.Banks;
            BankViewModel bank = banks.First();
            CarPoliiceControl foundPolice = UIHelper.FindChild<CarPoliiceControl>(_view,null);
            RobberyMessage robberyMessage = new RobberyMessage("Robber1",bank );
            if ( foundPolice != null)
            {
                foundPolice.StartanimationToBank(robberyMessage);
            }
        }

        #endregion



    }

}
