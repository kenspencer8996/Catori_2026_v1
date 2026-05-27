using StreetsEnum = CatoriApp.Core.Objects.StreetsEnum;
using CatoriApp.Core.ExtensionMethods;
using CatoriApp.Core.Objects.Arguments;
using CommunityToolkit.Mvvm.Messaging;
using System.Diagnostics;
using System.Windows.Threading;
using CatoriApp.Core.Objects.Arguments;
namespace CatoriApp.Game.Controllers.City
{
    internal class CityScapeViewController
    {
        CityScapeView _view;
       // StoreHardwareInteriorControl storeHardwareInteriorUC = new StoreHardwareInteriorControl();
        internal CityscapeStreetsViewModel Model = new CityscapeStreetsViewModel();
        public bool instartupFlag = true;
        bool VisualContentLoaded = false;
        MapPositionEntity _mapPosition = new MapPositionEntity();
        int _streetwidth;
        private List<LotControl> _lots = new List<LotControl>();
        DispatcherTimer _updatePathsTimerTimer;
        DispatcherTimer _startupTimer;
        System.Timers.Timer interestAddTimer;
        public PersonControl primaryPerson;
        public bool MovePerson = false;
        DragManager _dragManager;
        public void WalkLeft ()
        {
            //_dropTargetManager.Register();
        }

        internal CityScapeViewController(CityScapeView view)
        { 
            _view = view;
            string startupmessage = "--------------------- Application Startup ------------------------" + Environment.NewLine;
            startupmessage += "Application Startup Time: " + DateTime.Now.ToString() + Environment.NewLine;
            startupmessage+= CityScapeGlobal.GetNameWithVersion() + Environment.NewLine;
            cLogger.Log(startupmessage);
            //NlogSetup.Configure();
            //CityScapeGlobal.LoadLocationInteriorControls();
            cLogger.Log("MainWindowController Constructor");
            CityScapeGlobal.TimingsRandom = new List<int>();
            Random rnd = new Random();
            for (int i = 1; i <= 50; i++)
            {
                int rInt = rnd.Next(0, 5);
                CityScapeGlobal.TimingsRandom.Add(rInt);
            }
            _dragManager = GlobalCode.GetDragmanager(_view.MainLayout);
            WeakReferenceMessenger.Default.Register<ShowHardwareStoreInteriorMessage>(this, (r, m) =>
            {
                try
                {
                    //ShowHardwareStoreInteriorMessage showHardwareInteriorMessage = m;
                    //storeHardwareInteriorUC.Width = _view.Width;
                    //storeHardwareInteriorUC.Height = _view.Height;
                    //storeHardwareInteriorUC.Model = m.Model;
                    //Canvas.SetZIndex(storeHardwareInteriorUC, 2000);
                    //storeHardwareInteriorUC.MoveCart();
                    //storeHardwareInteriorUC.Visibility = Visibility.Visible;
                }
                catch (Exception ex)
                {

                    throw;
                }
            });
            WeakReferenceMessenger.Default.Register<PostOfficeInteriorhowMessage>(this, (r, m) =>
            {
                try
                {
                    PostOfficeInteriorhowMessage shopItemShowMessage = m;
                    //xx.Width = _view.Width;
                    //storeHardwareInteriorUC.Height = _view.Height;
                    //storeHardwareInteriorUC.Model = m.Model;
                    //Canvas.SetZIndex(storeHardwareInteriorUC, 2000);
                    //storeHardwareInteriorUC.Visibility = Visibility.Visible;
                }
                catch (Exception ex)
                {

                    throw;
                }
            });
            WeakReferenceMessenger.Default.Register<ResetPersonMesssage>(this, (r, m) =>
            {
                try
                {
                    ResetPrimaryPerson(120,120);
                }
                catch (Exception ex)
                {

                    throw;
                }
            });
            
        }

        private void ResetPrimaryPerson(double originalleft, double originaltop)
        {

            int seconds = 2;
            Point target = new Point(450, 385);
                      //var tt = primaryPerson.RenderTransform as TranslateTransform;
            //if (tt == null)
            //{
            //    tt = new TranslateTransform();
            //    primaryPerson.RenderTransform = tt;
            //}

            //var sb = new Storyboard();
            //sb.Duration = TimeSpan.FromSeconds(seconds);

            //var animX = new DoubleAnimation
            //{
            //    To = target.X,
            //    Duration = sb.Duration,
            //    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            //};
            //Storyboard.SetTarget(animX, primaryPerson);
            //Storyboard.SetTargetProperty(animX, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));

            //var animY = new DoubleAnimation
            //{
            //    To = target.Y,
            //    Duration = sb.Duration,
            //    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            //};
            //Storyboard.SetTarget(animY, primaryPerson);
            //Storyboard.SetTargetProperty(animY, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

            //sb.Children.Add(animX);
            //sb.Children.Add(animY);
            //sb.Completed += (s, e) =>
            //{
            //    Canvas.SetLeft(primaryPerson, target.X);
            //    Canvas.SetTop(primaryPerson, target.Y);

            //    Canvas.SetZIndex(primaryPerson, 4001);

            //    tt.X = 0;
            //    tt.Y = 0;
            //};
            //sb.Begin();


            Canvas.SetLeft(primaryPerson, target.X);
            Canvas.SetTop(primaryPerson, target.Y);

            Canvas.SetZIndex(primaryPerson, 4001);
        }

        internal void Startup(int streetwidth)
        {
            try
            {
                cLogger.Log("MainWindowController Startup");
                HouseService houseService = new HouseService();

                _streetwidth = streetwidth;
                CreateLots();
                CityScapeGlobal.Houses = houseService.GetHousesAsync().Result;
                CityScapeGlobal.Banks = ImageFileHelper.GetBanks();
               
                CityScapeGlobal.Businesses.AddRange(ImageFileHelper.GetLocations());
                LoadHouses();
                LoadBanks();
                LoadStores();
                LoadLocations();
                AddPoliceStation();
                loadPoliceCars();
                
                primaryPerson = new PersonControl(GlobalAllApps.CurrentPerson, _dragManager, _view.MainLayout);
                primaryPerson.MovePersonStop += PrimaryPerson_MovePersonStop;
                primaryPerson.MovePersonStart += PrimaryPerson_MovePersonStart;
                _view.MainLayout.Children.Add(primaryPerson);
                CityScapeGlobal.ShowPrimaryPerson();
                Canvas.SetLeft(primaryPerson, 120);
                Canvas.SetTop(primaryPerson, 120);
                Canvas.SetZIndex(primaryPerson, 4001);
                primaryPerson.PersonMouseDown += Person_PersonMouseDown; ;
                primaryPerson.PersonMouseUp += Person_PersonMouseUp;
                //AddToLandscapeItems();
                LoadLandscapeObjects();
                GetLandscapeObjectsGroupIds();
                _view.BaloonHelpUC.Visibility = Visibility.Hidden;
                double mainwidth = _view.Width;
                double mainheight = _view.Height;
                Canvas.SetTop(_view.BadGuySpeedStackPanel, mainheight - _view.BadGuySpeedStackPanel.Height);
                Canvas.SetLeft(_view.BadGuySpeedStackPanel, mainwidth - _view.BadGuySpeedStackPanel.Width - 20);
                CityScapeGlobal.mainWindowViewModel.BadGuyTravelSpeed = GlobalServices.GetSettingByName("BadGuyTravelSpeed").IntSetting;
                CityScapeGlobal.mainWindowViewModel.PolicecarToravelSpeed = GlobalServices.GetSettingByName("PoliceCarTravelSpeed").IntSetting;
                _view.BaloonHelpUC.MoveBaloon += BaloonHelpUC_MoveBaloon;
                _view.BaloonHelpUC.Visibility = Visibility.Hidden;
                _view.BadGuyTravelSpeedSlider.Value = CityScapeGlobal.mainWindowViewModel.BadGuyTravelSpeed;
                _updatePathsTimerTimer = new DispatcherTimer();
                _updatePathsTimerTimer.Tick += new EventHandler(__updatePathsTimerTimer_Tick);
                _updatePathsTimerTimer.Interval = new TimeSpan(0, 0, 30);
                _startupTimer = new DispatcherTimer();
                _startupTimer.Tick += new EventHandler(StartupTimer_Tick);
                _startupTimer.Interval = new TimeSpan(0, 0, 1);

                interestAddTimer = new System.Timers.Timer();
                interestAddTimer.Interval = 60000 * 10; // 10 minutes
                interestAddTimer.Elapsed += InterestAddTimer_Elapsed;
                interestAddTimer.Start();

                _startupTimer.Start();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void StartupTimer_Tick(object? sender, EventArgs e)
        {
            _startupTimer?.Stop();
            ResetPrimaryPerson(120, 120);
        }

        private void PrimaryPerson_MovePersonStart(object? sender, PrimaryPrsonDragArgg e)
        {
            cLogger.Log("Event Hit");
            MovePerson = true;
        }

        private void PrimaryPerson_MovePersonStop(object? sender, PrimaryPrsonDragArgg e)
        {
            cLogger.Log("Event Hit");
            MovePerson = false;
        }

     
        


            //foreach (var store in GlobalStuff.Stores)
            //{
            //    var foundShelf = from sh in GlobalStuff.ShelfViewModels
            //                     where sh.ShopItemId == store.Key
            //                         select sh;
            //    foreach (var shelf in foundShelf)
            //    {
            //        var storeitems = from si in GlobalStuff.ShopItems
            //                         where si.ShopItemId == shelf.ShopItemId
            //                         select si;

            //        if (storeitems.Any())
            //        {
            //            ShopItemViewModel newShopItem = new ShopItemViewModel();
            //            newShopItem = storeitems.FirstOrDefault();
            //            ShopItemControl shopitem = new ShopItemControl();
            //            shopitem.Width = newShopItem.Width;
            //            shopitem.Height = newShopItem.Height;
            //            shopitem.Name = newShopItem.Name;
            //            shopitem.Width = newShopItem.Width;
            //            shopitem.Height = newShopItem.Height;
            //            //hopitem.Model = newShopItem;
            //            shopitem.ShopItemMouseDown += storeHardwareInteriorUC.ShopItemMouseDown;
            //            shopitem.ShopItemMouseUp += storeHardwareInteriorUC.ShopItemMouseUp; ;
            //            if (newShopItem.RotationDegree > 0)
            //            {
            //                RotateTransform rotateTransform = new RotateTransform(newShopItem.RotationDegree);
            //                shopitem.RenderTransform = rotateTransform;

            //            }
            //            shopitem.Visibility = Visibility.Visible;
            //            shelfUC.AddSHopItemToShelfLocation(shopitem);

            //            //shopitem.ItemCost = item.Price.ToString("C");
            //            string filepath = Imagehelper.GetImagePath(newShopItem.ImageName);
            //            shopitem.MainImage.Source = UIUtility.GetImageControl(filepath, 100, 100, 0).Source;
            //        }

            //    }
            //}

        
       
       

        private void Shopitem_StopDrag(object? sender, CatoriApp.Core.Objects.Arguments.ShopItemControlDrag e)
        {
            throw new NotImplementedException();
        }

        private void LoadStores()
        {
            StoreHardwareControl ucStore1 = new StoreHardwareControl();
            ucStore1.Width = CityScapeGlobal.buildingsize;
            ucStore1.Height = CityScapeGlobal.buildingsize;
            Canvas.SetZIndex(ucStore1, 4);
            var found = from lot in _lots
                        where lot.LotOccupied == false
                        && lot.Street == StreetsEnum.MikAve
                        select lot;
            LotControl lotControl = found.First();
            if (found.Any())
            {
                try
                {
                    lotControl.AddBuilding(ucStore1, false);
                    _dragManager.RegisterDropTarget(lotControl  );
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
            //Canvas.SetLeft(storeHardwareInteriorUC, 0);
            //Canvas.SetTop(storeHardwareInteriorUC, 0);
            //storeHardwareInteriorUC.Visibility = Visibility.Hidden;
            //_view.MainLayout.Children.Add(storeHardwareInteriorUC);
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

        private void AddlocationIntrnalView(double leftLocation,double topLocation)
        {
            double width = 300;
            double height = 200;
            try
            {
                foreach (var child in CityScapeGlobal.locationInteriorControls)
                {
                    child.Width = width;
                    child.Height = height;
                    child.Visibility = Visibility.Collapsed;
                    _view.MainLayout.Children.Add(child);
                    Canvas.SetLeft(child, leftLocation - width);
                    Canvas.SetTop(child, topLocation + 100);
                    Canvas.SetZIndex(child, 2000);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void BaloonHelpUC_MoveBaloon(object? sender, CatoriApp.Core.Objects.Arguments.BaloonLocationInfoArgument e)
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
            CityScapeGlobal.PoliceCars = (List<PoliceCarEntity>) policeCarRepo.GetPoliceCars();
            LotControl lotControl = CityScapeGlobal.PoliceStationLocation;
            int policecarincrementer = 0;
            int carNumber = 1;
            foreach (var car in CityScapeGlobal.PoliceCars)
            {

                if (car.CarType.ToLower() == "normal")
                {
                    AddPoliceCar(car, policecarincrementer, lotControl, carNumber);
                }
                else 
                {
                    AddCarPoliceOffRoad(car,policecarincrementer, lotControl);
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
      
        
        #region Add methods

        internal void AddCarPoliceOffRoad(PoliceCarEntity car,int policecarincrementer, LotControl lotControl)
        {
            CarPoliceOffRoadControl carControl = new CarPoliceOffRoadControl();
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
                CarPoliceControl carControl = new CarPoliceControl(policeCar.ImageName, 60, 25, carNumber);
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
                ps.Width = CityScapeGlobal.buildingsize - 10;
                ps.Height = CityScapeGlobal.buildingsize - 10;
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
                        lotControl.AddBuilding(ps,false);
                        GlobalGeo.PoliceStationLocationEntity = policeLot;
                        CityScapeGlobal.PoliceStationLocation = lotControl;
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
                cLogger.Log($" bottom lot loc " + "x " + leftlot + " y " + toplot + " lotlocationconter " + lotControl); // HouseControl: /CatoriApp; 100 100

                lotloccounter += GlobalGeo.LotSize;
            }
            lotloccounter = (int)GlobalGeo.YouStreetloc.LocationStartXY.y;
            //layout NS lots
            for (int i = 0; i < nslotcount; i++)
            {
                LotControl lotControl = GetLotControl();
                lotControl.Street = StreetsEnum.YodelLane;
                Canvas.SetTop(lotControl, lotloccounter);
                Canvas.SetLeft(lotControl, 0);
                _view.MainLayout.Children.Add(lotControl);

                double left = GlobalGeo.TeaStreetLoc.LocationStartXY.x + 50;
               
                LotControl lotControlMiddle = GetLotControl();
                lotControlMiddle.Street = StreetsEnum.MooDr;
                Canvas.SetTop(lotControlMiddle, lotloccounter);
                Canvas.SetLeft(lotControlMiddle, left);

                LotControl lotControlRight = GetLotControl();
                lotControlRight.Street = StreetsEnum.Teastreet;
                 _lots.Add(lotControlRight);
                Canvas.SetTop(lotControlRight, lotloccounter);
                Canvas.SetLeft(lotControlRight, left);
                _view.MainLayout.Children.Add(lotControlRight);
                //double left = Canvas.GetLeft(lotControlRight);
                lotloccounter += GlobalGeo.LotSize;
            }
        }

        public void ResizeLots(Size previousSize, Size newSize)
        {
            var lotsTea = from l in _lots
                       where l.Street == StreetsEnum.Teastreet 
                       select l;
            foreach (var lot in lotsTea)
            {
                double left = GlobalGeo.TeaStreetLoc.LocationStartXY.x + 50;

                Canvas.SetLeft(lot, left);

            }

            var lotsMiks = from l in _lots
                          where l.Street == StreetsEnum.MikAve
                          select l;
            foreach (var lot in lotsMiks)
            {
                double top = GlobalGeo.MikAveLoc.LocationStartXY.y + 50;
                Canvas.SetTop(lot, top);

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
            cLogger.Log($"houses : {CityScapeGlobal.Houses.Count}"); // HouseControl: /CatoriApp; 100 100

            try
            {
                bool primaryPersonHouse = false;
                foreach (var house in CityScapeGlobal.Houses)
                {
                    try
                    {
                        HouseControl houseControl = new HouseControl(house);
                        if (house.Name.Trim().ToLower() == CityScapeGlobal.CurrentHouseName.Trim().ToLower()
                            && GlobalAllApps.CurrentPerson.Name != null && GlobalAllApps.CurrentPerson.Name != "")
                        {
                            houseControl.AddPersonModel(GlobalAllApps.CurrentPerson);
                            primaryPersonHouse = true;  
                        }
                        houseControl.Width = CityScapeGlobal.buildingsize;
                        houseControl.Height = CityScapeGlobal.buildingsize;
                        Canvas.SetZIndex(houseControl, 100);
                        Canvas.SetLeft(houseControl, left);
                        Canvas.SetTop(houseControl, top);
                        var found2 = from lot in _lots
                                    where lot.LotOccupied == false
                                    && lot.Street == StreetsEnum.YouStreet
                                    select lot;
                        if (found2.Any())
                        {
                            LotControl lotControl = found2.First();
                            lotControl.AddBuilding(houseControl, primaryPersonHouse);
                            _dragManager.RegisterDropTarget(lotControl);
                        }
                        left += 90;
                        primaryPersonHouse = false;
                    }
                    catch (Exception exi)
                    {

                        throw;
                    }
                    cLogger.Log($"HouseControl: {house.HouseImageFileName}"); // HouseControl: /CatoriApp; 100 100
                }
                RealEstateControl realEstateControl = new RealEstateControl();
                realEstateControl.Width = CityScapeGlobal.buildingsize;
                realEstateControl.Height = CityScapeGlobal.buildingsize;
                Canvas.SetZIndex(realEstateControl, 100);
                Canvas.SetLeft(realEstateControl, left);
                Canvas.SetTop(realEstateControl, top);
                var foundReal = from lot in _lots
                            where lot.LotOccupied == false
                            && lot.Street == StreetsEnum.YouStreet
                            select lot;
                if (foundReal.Any())
                {
                    foundReal.First().AddBuilding(realEstateControl, false);
                }


                PostOfficeControl postOffice = new PostOfficeControl();
                postOffice.Width = CityScapeGlobal.buildingsize;
                postOffice.Height = CityScapeGlobal.buildingsize;
                _dragManager.RegisterDropTarget(  postOffice);
                Canvas.SetZIndex(postOffice, 100);
                Canvas.SetLeft(postOffice, left);
                Canvas.SetTop(postOffice, top);
                var found = from lot in _lots
                            where lot.LotOccupied == false
                            && lot.Street == StreetsEnum.YouStreet
                            select lot;
                if (found.Any())
                {
                    found.First().AddBuilding(postOffice, primaryPersonHouse);
                }
                
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void ResetCharacters()
        {
            var foundlot = from lot in _lots
                          where lot.LotOccupied == true
                          && lot.IsPrimaryPersonHouse == true
                          select lot;   
           if (foundlot.Any())
            {
                PersonViewModel model = GlobalAllApps.CurrentPerson;
                LotControl lotControl = foundlot.First();
                lotControl.Building.Visibility = Visibility.Visible;
                lotControl.Building.AddPersonModel(model);
            }
           var badguys =UIUtility.FindChildrenOfType<BadPersonControl>(_view.MainLayout);
            foreach (var badguy in badguys)
            {
                badguy.ResetAnimation();
            }
        }
        private void LoadBanks()
        {
            try
            {
                int top = 100;
                int left = 100;
                double Width = CityScapeGlobal.buildingsize; //  double Width
                double Height = CityScapeGlobal.buildingsize; //  double Width
                var banks = from b in CityScapeGlobal.Banks
                            select b;
                cLogger.Log($"houses : {CityScapeGlobal.Houses.Count}"); // HouseControl: /CatoriApp; 100 100
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
                        bankUC.Width = CityScapeGlobal.buildingsize;
                        bankUC.Height = CityScapeGlobal.buildingsize;
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
                            lotControl.AddBuilding(bankUC, false);
                            CityScapeGlobal.FinancialLotCobtrols.Add(found.First());
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
        
        private void LoadLocations()
        {
            int top = 100;
            int left = 100;
            double Width = CityScapeGlobal.buildingsize - 20; //  double Width
            double Height = CityScapeGlobal.buildingsize - 20; //  double Width
            var business = from b in CityScapeGlobal.Businesses
                           where b.BusinessType == BusinessTypeEnum.Location
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
            Canvas.SetLeft(_view.LocationHelp, x - 75);
            Canvas.SetTop(_view.LocationHelp, y + 50 );
            Canvas.SetZIndex(_view.LocationHelp, 100);
            int I = 0;
            double leftFistLocation = Canvas.GetLeft( firstlot);
            double topFistLocation = Canvas.GetTop(firstlot);
            int locationCount = 1;
            try
            {
                foreach (var item in business)
                {
                    FactoryControl factoryControl = new FactoryControl(locationCount);
                    

                    string controlname = "FactoryControl_" + System.IO.Path.GetFileNameWithoutExtension(item.Name);
                    factoryControl.Name = controlname;
                    factoryControl.BusinessImage.Source = UIUtility.GetImageControl(item.ImageName, Width, Height, 0).Source; ;
                    factoryControl.Width = CityScapeGlobal.buildingsize;
                    factoryControl.Height = CityScapeGlobal.buildingsize;
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
                        LotControl thislot = found.First();
                        thislot.AddBuilding(factoryControl, false);
                        _dragManager.RegisterDropTarget(thislot);

                        Debug.WriteLine($"Registered {factoryControl.Name}: Left={Canvas.GetLeft(thislot)}, Top={Canvas.GetTop(thislot)}");
                    }
                    string leftpos = Canvas.GetLeft(factoryControl).ToString();
                    // cLogger.Log($"BusinessControl: {businessControl.BusinessImage.Source}  {left} {top}");
                    //_view.MainLayout.Children.Add(businessControl);
                    left += 90;
                    locationCount++;
                    cLogger.Log($"BusinessControl: {item.ImageName}"); // HouseControl: /CatoriApp; 100 100
                }
             AddlocationIntrnalView(leftFistLocation,topFistLocation);
           }
            catch (Exception ex)
            {

                throw;
            }
            locationCount++;
        }

        #endregion

        #region Landscape loading
        public void GetLandscapeObjectsGroupIds()
        {
            double mainwidth = _view.Width;
            double mainheight = _view.Height;

            LandscapeObjectRepository repository = new LandscapeObjectRepository();
            CityScapeGlobal.landscapeObjectGroupIds = repository.GetLandscapeObjectsGroupIds();
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
            CityScapeGlobal.LandscapeUCs = new List<LandscapeObjectControl>();
            LandscapeObjectService landscapeservice = new LandscapeObjectService();
            CityScapeGlobal.LandscapeObjects = await landscapeservice.GetLandscapeObjectsAsync(GlobalServices.LandscapeObjecGroupid);
            LandscapeObjectViewModel featureModel = new LandscapeObjectViewModel();
            foreach (var landscapeObject in CityScapeGlobal.LandscapeObjects)
            {
                if (landscapeObject.FeatureNote == null || landscapeObject.FeatureNote == "")
                {
                    LandscapeObjectControl thisUC = new LandscapeObjectControl();
                    double x, y;
                    thisUC = GetLandscapeObject(landscapeObject,landscapeObject.Name);
                    x = landscapeObject.xActual;
                    y = landscapeObject.yActual;
                    _dragManager.RegisterDropTarget( thisUC);

                    CityScapeGlobal.LandscapeUCs.Add(thisUC);
                    Canvas.SetZIndex(thisUC, 1101);
                    _view.MainLayout.Children.Add(thisUC);
                    Canvas.SetLeft(thisUC, x);
                    Canvas.SetTop(thisUC, y);
                    thisUC.SetCenter(x, y);
                    if (landscapeObject.NextFromHomeObject)
                        CityScapeGlobal.NextFromHomeObject = landscapeObject;
                    if (landscapeObject.HomeObject)
                        CityScapeGlobal.HomeLandscapeObject = landscapeObject;
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
            thisUC.Location = new LocationXYEntity() { x = CityScapeGlobal.Tentx, y = CityScapeGlobal.Tenty };
            thisUC.Height = landscapeObject.Height;
            thisUC.Width = landscapeObject.Width;
            //thisUC.Height = 60;
            //thisUC.Width = 40;
            thisUC.AddImage(landscapeObject.ImageName, (int)thisUC.Width, (int)thisUC.Height);
            thisUC.OnDragDropChange += ThisUC_OnDragDropChange;

            return thisUC;
        }
        private void Person_PersonMouseUp(object? sender, PrimaryPrsonDragArgg e)
        {
        }

        private void Person_PersonMouseDown(object? sender, CatoriApp.Game.Objects.Arguments.PrimaryPrsonDragArgg e)
        {
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

                CityScapeGlobal.LandscapeObjectApproachNextControls = new List<LandscapeObjectControl>()
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
                Canvas.SetLeft(thisUCN, CityScapeGlobal.ApproachPointN.x);
                Canvas.SetTop(thisUCN, CityScapeGlobal.ApproachPointN.y - offset);

                Canvas.SetZIndex(thisUCE, 1101);
                Canvas.SetLeft(thisUCE, CityScapeGlobal.ApproachPointE.x);
                Canvas.SetTop(thisUCE, CityScapeGlobal.ApproachPointE.y);
                
                Canvas.SetZIndex(thisUCS, 1101);
                Canvas.SetLeft(thisUCS, CityScapeGlobal.ApproachPointS.x);
                Canvas.SetTop(thisUCS, CityScapeGlobal.ApproachPointS.y+ offset);

                Canvas.SetZIndex(thisUCW, 1101);
                Canvas.SetLeft(thisUCW, CityScapeGlobal.ApproachPointW.x- offset);
                Canvas.SetTop(thisUCW, CityScapeGlobal.ApproachPointW.y);
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
           var nextuc = CityScapeGlobal.GetNextFromHomeObject(); ;
            double nextx = Canvas.GetLeft(nextuc);
            double nexty = Canvas.GetTop(nextuc);
            double width = nextuc.Width;
            double height = nextuc.Height;
            double centerx = nextx + (width / 2);
            double centery = nexty + (height / 2);
            double distancedivsor = 1.4;
            CityScapeGlobal.ApproachPointN = new LocationXYEntity()
            {
                x = centerx ,
                y = centery - (height / distancedivsor)
            };
            CityScapeGlobal.ApproachPointE = new LocationXYEntity()
            {
                x = centerx + (width + (width / distancedivsor)),
                y = centery + (height / distancedivsor)
            };
            CityScapeGlobal.ApproachPointS = new LocationXYEntity()
            {
                x = centerx ,
                y = centery + (height / 2 + (height / distancedivsor))
            };
            CityScapeGlobal.ApproachPointW = new LocationXYEntity()
            {
                x = centerx - (width / distancedivsor),
                y = centery 
            };
        }

        private void ThisUC_OnDragDropChange(object? sender, CatoriApp.Core.Objects.Arguments.DragDropChangeArgs e)
        {
           _view.ispagedirty = true;
        }

        internal void RunPoliceCars()
        {
            //get financials and set below
            var banks = CityScapeGlobal.Banks;
            BankViewModel bank = banks.First();
            CarPoliceControl foundPolice = UIHelper.FindChild<CarPoliceControl>(_view,null);
            RobberyMessage robberyMessage = new RobberyMessage("Robber1",bank );
            if ( foundPolice != null)
            {
                foundPolice.StartanimationToBank(robberyMessage);
            }
        }

        internal void StopPersonWalkingAnimation()
        {
            primaryPerson.StopAnimation();
        }

        #endregion



    }

}


