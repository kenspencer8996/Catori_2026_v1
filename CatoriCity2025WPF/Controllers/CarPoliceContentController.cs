using CatoriCity2025WPF.Objects;
using CatoriCity2025WPF.Objects.Arguments;
using CatoriCity2025WPF.ViewModels;
using CatoriCity2025WPF.Views.Controls;
using CatoriServices.Objects;
using CityAppServices;
using CityAppServices.Objects.Entities;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace CatoriCity2025WPF.Controllers
{
    internal class CarPoliceContentController
    {
        private CarPoliiceControl _view;
        //BankControl _business;
        public PoliceCarMoveFiredEventArg arg;
        string _robberName;
        private List<StreetNavigationEntity> PathToRobery = new List<StreetNavigationEntity>();
        private LotEntity _robberyLot;
        //bool runOne = false;
        //bool firstrun = false;
        double _currentX = 0;
        double _currentY = 0;
        internal BankViewModel bank { get; set; } = new BankViewModel();
        private double _movepolicecary;
        private double _movepolicecarx;
        private double _startingX;
        private double _startingY;
        private RobberyMessage _robberyMessage;
        private PositionsEWNSEnum _currentDirectionTea;
        private PositionsEWNSEnum _currentDirectionYodel;
        private PositionsEWNSEnum _currentDirectionParking;
        private PositionsEWNSEnum _currentDirectionYou;
        private PositionsEWNSEnum _currentDirectionMik;
        private DispatcherTimer _moveCarBackToStationTimer;
        private bool MoveToPoliceStation_start_Execute = false;
        public bool HandledWeakReferenceRobbery { get; set; } = false;
        private void SetCurrentXY(double x, double y)
        {
            // var currentLoc = AbsoluteLayout.GetLayoutBounds(_view);
            _currentX = x;
            _currentY = y;
        }
        internal CarPoliceContentController(CarPoliiceControl view, double startingX, double startingY)
        {
            
            _view = view;
            _startingX = startingX;
            _startingY = startingY;
            SetCurrentXY(startingX, startingY);
            GlobalStuff.WriteDebugOutput("CarPoliceContentController startx " + startingX + "  starty " + startingY);
            //_controller = new CarPoliceContentViewController(this);
            //RobberyMessageDetailViewModel RobberyMessageDetail;
            _moveCarBackToStationTimer = new DispatcherTimer();
            _moveCarBackToStationTimer.Tick += new EventHandler(_moveCarBackToStationTimer_Tick);

            RobberyMessage RobberyMessage;
            WeakReferenceMessenger.Default.Register<RobberyMessage>(this, (r, m) =>
            {
                RobberyMessage = m;
                //GlobalStuff.WriteDebutOutput("Playing sound");
                //mediaElement.Play();
                bank = RobberyMessage.Bank;
                //SendMessageOnRobbery();
                if (_view.RobberName == RobberyMessage.RobberName &&
                   HandledWeakReferenceRobbery == false)
                {
                    GlobalStuff.WriteDebugOutput("WeakReferenceMessenger pol car robber " + _view.RobberName);
                    arg = new PoliceCarMoveFiredEventArg(RobberyMessage.RobberName);
                    PoliceCarStartMove(RobberyMessage.RobberName);
                    HandledWeakReferenceRobbery = true;
                }
            });
        }

        private void _moveCarBackToStationTimer_Tick(object? sender, EventArgs e)
        {
            _currentDirectionParking = PositionsEWNSEnum.South;
            _view.ChangeDirection(_currentDirectionParking);

            (sender as DispatcherTimer).Stop();
            if (MoveToPoliceStation_start_Execute)
                MoveToPoliceStation_start();   
            MoveToPoliceStation_start_Execute = false;
        }

        private void MoveToPoliceStation_start()
        {
            try
            {
                SettingEntity policeCarspeedSetting = GlobalServices.GettingSetting("policecarspeed");
                GlobalStuff.mainWindowViewModel.PolicecarToravelSpeed = policeCarspeedSetting.IntSetting;
                _currentDirectionParking = PositionsEWNSEnum.North;
                _view.ChangeDirection(_currentDirectionParking);
                double start = Canvas.GetTop(_view);
                Random rnd = new Random();
                double endPoint = 0;
                // Apply animations to the control
                Storyboard currentSB = new Storyboard();
                currentSB.Completed += MoveToPoliceStation_start_Completed_Next;

                // Apply animations to the control
                //endPoint = GlobalStuff.IntersectuonMikYodel.y + _view.Width/2;
                endPoint = GlobalStuff.IntersectuonMikYodel.y;

                cLogger.Log("MoveToPoliceStation_start: y " + GlobalStuff.IntersectuonMikMoo.y + " end " + endPoint);
                cLogger.Log("MoveToPoliceStation_start: " + _currentDirectionParking + " end " + endPoint);
                // Create the animation for Canvas.Top
                DoubleAnimation moveStreet = new DoubleAnimation
                {
                    From = start,
                    To = endPoint,
                    Duration = new Duration(TimeSpan.FromSeconds(GlobalStuff.mainWindowViewModel.PolicecarToravelSpeed))
                };
                currentSB.Children.Add(moveStreet);

                Storyboard.SetTarget(moveStreet, _view);
                Storyboard.SetTargetProperty(moveStreet, new PropertyPath("(Canvas.Top)"));
                currentSB.Begin(_view);
            }
            catch (Exception ex)
            {
                GlobalStuff.WriteDebugOutput(" exception " + ex.Message);
            }
        }

        private void MoveToPoliceStation_start_Completed_Next(object? sender, EventArgs e)
        {
             _currentDirectionParking = PositionsEWNSEnum.East;
           _view.ChangeDirection(_currentDirectionParking);
            MoveEastDownMikSt();
        }

        private void MoveEastDownMikSt()
        {
            try
            {
                double start = Canvas.GetLeft(_view);
                Random rnd = new Random();

                double endPoint = 0;
                // Apply animations to the control
                Storyboard currentSB = new Storyboard();
                endPoint = GlobalStuff.IntersectuonMikTea.x + GlobalStuff.StreetWidth / 2;
                currentSB.Completed += MoveEastDownMikSt_Completed_Next_Mik;

                cLogger.Log("MoveEastDownMikSt: y " + GlobalStuff.IntersectuonMikMoo.y + " end " + endPoint);
                cLogger.Log("MoveEastDownMikSt: " + _currentDirectionParking + " end " + endPoint);
                // Create the animation for Canvas.Left
                DoubleAnimation moveStreet = new DoubleAnimation
                {
                    From = start,
                    To = endPoint,
                    Duration = new Duration(TimeSpan.FromSeconds(GlobalStuff.mainWindowViewModel.PolicecarToravelSpeed))
                };
                currentSB.Children.Add(moveStreet);

                Storyboard.SetTarget(moveStreet, _view);
                Storyboard.SetTargetProperty(moveStreet, new PropertyPath("(Canvas.Left)"));
                currentSB.Begin(_view);

            }
            catch (Exception ex)
            {
                GlobalStuff.WriteDebugOutput(" exception " + ex.Message);
            }
        }

        private void MoveEastDownMikSt_Completed_Next_Mik(object? sender, EventArgs e)
        {
            _currentDirectionParking = PositionsEWNSEnum.North;
            _view.ChangeDirection(_currentDirectionParking);
            MoveUpToPoliceStation();
        }

        private void MoveUpToPoliceStation()
        {
            try
            {
                double endPoint = GlobalGeo.PoliceStationLocationEntity.y;
                double start = Canvas.GetTop(_view);
                Random rnd = new Random();

                // Apply animations to the control
                Storyboard currentSB = new Storyboard();
                currentSB.Completed += MoveUpToPoliceStation_Completed_Next_Tea;

                cLogger.Log("MoveUpToPoliceStation: y " + GlobalStuff.IntersectuonMikMoo.y + " end " + endPoint);
                cLogger.Log("MoveUpToPoliceStation: " + _currentDirectionParking + " end " + endPoint);
                // Create the animation for Canvas.Left
                DoubleAnimation moveStreet = new DoubleAnimation
                {
                    From = start,
                    To = endPoint,
                    Duration = new Duration(TimeSpan.FromSeconds(GlobalStuff.mainWindowViewModel.PolicecarToravelSpeed))
                };
                currentSB.Children.Add(moveStreet);

                Storyboard.SetTarget(moveStreet, _view);
                Storyboard.SetTargetProperty(moveStreet, new PropertyPath("(Canvas.Top)"));
                currentSB.Begin(_view);
            }
            catch (Exception ex)
            {

                GlobalStuff.WriteDebugOutput(" exception " + ex.Message);
            }
        }

        private void MoveUpToPoliceStation_Completed_Next_Tea(object? sender, EventArgs e)
        {
            _currentDirectionParking = PositionsEWNSEnum.East;
            _view.ChangeDirection(_currentDirectionParking);
            MoveToPoliceStation();
        }

        private void MoveToPoliceStation()
        {
            try
            {
                double endPoint = GlobalGeo.PoliceStationLocationEntity.x + 50;
                double start = Canvas.GetLeft(_view);
                Random rnd = new Random();

                // Apply animations to the control
                Storyboard currentSB = new Storyboard();
                currentSB.Completed += MoveToPoliceStation_Completed_Next_PoliceStation;

                cLogger.Log("MoveToPoliceStation: y " + GlobalStuff.IntersectuonMikMoo.y + " end " + endPoint);
                cLogger.Log("MoveToPoliceStation: " + _currentDirectionParking + " end " + endPoint);
                // Create the animation for Canvas.Left
                DoubleAnimation moveStreet = new DoubleAnimation
                {
                    From = start,
                    To = endPoint,
                    Duration = new Duration(TimeSpan.FromSeconds(GlobalStuff.mainWindowViewModel.PolicecarToravelSpeed))
                };
                currentSB.Children.Add(moveStreet);

                Storyboard.SetTarget(moveStreet, _view);
                Storyboard.SetTargetProperty(moveStreet, new PropertyPath("(Canvas.Left)"));
                currentSB.Begin(_view);

            }
            catch (Exception ex)
            {

                GlobalStuff.WriteDebugOutput(" exception " + ex.Message);
            }
        }

        private void MoveToPoliceStation_Completed_Next_PoliceStation(object? sender, EventArgs e)
        {
        }

        internal async void PoliceCarStartMove(string robberName)
        {
            try
            {
                _robberName = robberName;

                BankControl bc =  GlobalStuff.GetFinancialBusinessControlForRobber(_view.RobberName);

                _robberyLot = bc.Lot;
                _currentX = _startingX;
                //todo:setup switch for steet
                GlobalStuff.WriteDebugOutput("PoliceCarStartMove robber " + _robberName);
                GlobalStuff.WriteDebugOutput("PoliceCarStartMove " + "You next" + " x" + GlobalGeo.YouStreetloc.LocationStartXY.x + " y" + GlobalGeo.YouStreetloc.LocationStartXY.y);

                GetStreetPathForPoliceCar();

                double y = GlobalGeo.YouStreetloc.LocationStartXY.y;
                //MoveCarYOnly(_startingY, y, "Police Station", 1);
               // await _view.FadeTo(1, 500);
            }
            catch (Exception ex)
            {

                GlobalStuff.WriteDebugOutput(" exception " + ex.Message);
            }
        }

        private void GetStreetPathForPoliceCar()
        {
            try
            {
                switch (_robberyLot.StreetName)
                {
                    case StreetsEnum.YodelLane:
                        StreetNavigationEntity street = new StreetNavigationEntity();
                        double startx = 0;
                        double starty = 0;
                        double finalx = 0;
                        double finaly = 0;
                        startx = GlobalGeo.PoliceStationLocationEntity.x;
                        starty = GlobalGeo.YouStreetloc.LocationStartXY.y;
                        finalx = GlobalGeo.YodelLaneStreetLoc.LocationStartXY.x + 40;
                        finaly = GlobalGeo.YouStreetloc.LocationStartXY.y;
                        street.AddTravelInfo(StreetsEnum.YouStreet, finalx, finaly, StreetTraverseEnum.Traverse,
                             PositionsEWNSEnum.West, startx, starty, 1);
                        PathToRobery.Add(street);

                        street = new StreetNavigationEntity();
                        var carylocinlot = _robberyLot.y + 40;
                        street.AddTravelInfo(StreetsEnum.YodelLane, finalx, carylocinlot,
                            StreetTraverseEnum.StopAtLot,
                             PositionsEWNSEnum.South,
                             GlobalGeo.YodelLaneStreetLoc.LocationStartXY.x, GlobalGeo.YouStreetloc.LocationStartXY.y, 2);
                        //street.AddTravelInfo(StreetsEnum.YodelLane, finalx, carylocinlot,
                        //    StreetTraverseEnum.StopAtLot,
                        //     PositionsEWNSEnum.South,
                        //     finalx, GlobalStuff.YouStStreetloc.LocationXY.y, 2);
                        street.LotToTravelTo = _robberyLot;
                        PathToRobery.Add(street);
                        break;
                    case StreetsEnum.MikAve:
                        StreetNavigationEntity streetMik = new StreetNavigationEntity();
                        streetMik.AddTravelInfo(StreetsEnum.YouStreet, GlobalGeo.YodelLaneStreetLoc.LocationStartXY.x,
                            GlobalGeo.YouStreetloc.LocationStartXY.y, StreetTraverseEnum.Traverse,
                             PositionsEWNSEnum.West, GlobalGeo.PoliceStationLocationEntity.x,
                             GlobalGeo.YouStreetloc.LocationStartXY.y, 1);
                        PathToRobery.Add(streetMik);
                        streetMik = new StreetNavigationEntity();
                        streetMik.AddTravelInfo(StreetsEnum.YodelLane, GlobalGeo.YodelLaneStreetLoc.LocationStartXY.x,
                            GlobalGeo.MikAveLoc.LocationStartXY.y,
                            StreetTraverseEnum.Traverse, PositionsEWNSEnum.South,
                            GlobalGeo.YodelLaneStreetLoc.LocationStartXY.x, GlobalGeo.YouStreetloc.LocationStartXY.y, 2);
                        PathToRobery.Add(streetMik);
                        streetMik = new StreetNavigationEntity();
                        streetMik.AddTravelInfo(StreetsEnum.MikAve,
                            _robberyLot.x, _robberyLot.y,
                            StreetTraverseEnum.StopAtLot,
                             PositionsEWNSEnum.East,
                             GlobalGeo.YodelLaneStreetLoc.LocationStartXY.x,
                             GlobalGeo.MikAveLoc.LocationStartXY.y, 3);
                        streetMik.LotToTravelTo = _robberyLot;
                        PathToRobery.Add(streetMik);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {

                GlobalStuff.WriteDebugOutput(" exception " + ex.Message);
            }

        }

        private string StreetName { get; set; }
        
        private string _currentStreetName = "";
      
       
        internal void StartAnimation(RobberyMessage m)
        {
            try
            {
                _robberyMessage = m;
                bank = m.Bank;
                Storyboard storyboard = new Storyboard();
                TimeSpan duration = TimeSpan.FromSeconds(GlobalStuff.mainWindowViewModel.PolicecarToravelSpeed); //

                DoubleAnimation fadeInAnimation = new DoubleAnimation()
                { From = 0.0, To = 1.0, Duration = new Duration(duration) };

                Storyboard.SetTarget(fadeInAnimation, _view);
                //'Cannot resolve all property references in the property path '(BadPersonControl.BadPersonLocation)'. Verify that applicable objects support the properties.'
                Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath("Opacity", 1));
                storyboard.Children.Add(fadeInAnimation);
                storyboard.Begin(_view);

                double xstart = Canvas.GetLeft(_view);

                double xend = GlobalStuff.IntersectuonYouTea.x + _view.Width / 2;
                DoubleAnimation moveStartX = new DoubleAnimation
                {
                    From = xstart,
                    To = xend,
                    Duration = new Duration(TimeSpan.FromSeconds(GlobalStuff.mainWindowViewModel.PolicecarToravelSpeed))
                };

                _currentDirectionYou = PositionsEWNSEnum.West;
                _currentDirectionYodel = PositionsEWNSEnum.South;
                _currentDirectionParking = PositionsEWNSEnum.South;

                // Apply animations to the control
                Storyboard storyboard2 = new Storyboard();
                storyboard2.Completed += Storyboard2_Completed; ;
                storyboard2.Children.Add(moveStartX);

                Storyboard.SetTarget(moveStartX, _view);
                Storyboard.SetTargetProperty(moveStartX, new PropertyPath("(Canvas.Left)"));
                storyboard2.Begin(_view);
            }
            catch (Exception ex)
            {

                GlobalStuff.WriteDebugOutput(" exception " + ex.Message);
            }
        }

        private void Storyboard2_Completed(object? sender, EventArgs e)
        {

            int updown = 0; //1 is up, 2 is down
            Random rnd = new Random();
            updown = rnd.Next(1, 2);
            switch (updown)
            {
                case 1:
                    _currentDirectionTea = PositionsEWNSEnum.North;
                   break;  
                case 2:
                    _currentDirectionTea = PositionsEWNSEnum.South;
                    break;
                default:
                    break;
            }
            _view.ChangeDirection(_currentDirectionTea);
            MoveToBankOnTeaSt();
       }
        private void MoveToBankOnTeaSt()
        {
            try
            {
                double ystart;
                ystart = Canvas.GetTop(_view);
                double yend = 0;
                Storyboard storyboard2 = new Storyboard();
                switch (_currentDirectionTea)
                {
                    case PositionsEWNSEnum.North:
                        yend = GlobalStuff.IntersectuonYouTea.y - _view.Width / 2;
                        _currentDirectionYou = PositionsEWNSEnum.West;
                        storyboard2.Completed += MoveToBankOnTeaSt_Completed_Next_You; ;
                        break;
                    case PositionsEWNSEnum.South:
                        yend = GlobalStuff.IntersectuonMikTea.y - _view.Width / 2;
                        _currentDirectionMik = PositionsEWNSEnum.West;
                        storyboard2.Completed += MoveToBankOnTeaSt_Completed_Next_Mik;
                        break;
                    default:
                        break;
                }

                // Create the animation for Canvas.Top
                DoubleAnimation moveStartY = new DoubleAnimation
                {
                    From = ystart,
                    To = yend,
                    Duration = new Duration(TimeSpan.FromSeconds(GlobalStuff.mainWindowViewModel.PolicecarToravelSpeed))
                };
                // Apply animations to the control
                storyboard2.Children.Add(moveStartY);
                Storyboard.SetTarget(moveStartY, _view);
                Storyboard.SetTargetProperty(moveStartY, new PropertyPath("(Canvas.Top)"));
                storyboard2.Begin(_view);
            }
            catch (Exception ex)
            {

                GlobalStuff.WriteDebugOutput(" exception " + ex.Message);
            }
        }
        #region Completed Events
        private void MoveToBankOnTeaSt_Completed_Next_Mik(object? sender, EventArgs e)
        {
        }

        private void MoveToBankOnTeaSt_Completed_Next_You(object? sender, EventArgs e)
        {
            _view.ChangeDirection(_currentDirectionYou);
            MoveOnYouSt();
        }

        private void MoveOnYouSt_East_Completed_Next_Tea(object? sender, EventArgs e)
        {
            MoveOnTeaSt();

        }
        private void MoveOnYouSt_East_Completed_Next_Moo(object? sender, EventArgs e)
        {
            _currentDirectionParking = PositionsEWNSEnum.South;
            MoveOnTeaSt();
        }
        private void MoveOnYouSt_West_Completed_Next_Yodel(object? sender, EventArgs e)
        {
            MoveOnYodelDr();
        }

        private void MoveOnYouSt_West_Completed_Next_Moo(object? sender, EventArgs e)
        {
            _view.ChangeDirection(_currentDirectionParking);
            MoveOnMoo();
        }
        #endregion
        private void MoveOnYouSt()
        {
            try
            {
                double xstart = Canvas.GetLeft(_view);
                Random rnd = new Random();
                int straight = 0; //1 is left, 2 is straight
                straight = rnd.Next(1, 2);

                //double yend = 0;
                double xend = 0;
                // Apply animations to the control
                Storyboard sbYouSt = new Storyboard();
                switch (_currentDirectionYou)
                {
                    case PositionsEWNSEnum.West:
                        xend = GlobalStuff.IntersectuonYouMoo.x;
                        sbYouSt.Completed += MoveOnYouSt_West_Completed_Next_Moo;
                        //if (straight == 1)
                        //{
                        //    xend = GlobalStuff.IntersectuonYouMoo.x;
                        //    sbYouSt.Completed += MoveOnYouSt_West_Completed_Next_Moo;
                        //}
                        //else if (straight == 2)
                        //{
                        //    xend = GlobalStuff.IntersectuonYouYodel.x + (_view.Width / 2);
                        //    sbYouSt.Completed += MoveOnYouSt_West_Completed_Next_Yodel;
                        //}
                        break;
                    case PositionsEWNSEnum.East:
                        xend = GlobalStuff.IntersectuonYouMoo.x + (_view.Width / 2);
                        sbYouSt.Completed += MoveOnYouSt_East_Completed_Next_Moo;
                        //if (straight == 1)
                        //{
                        //    xend = GlobalStuff.IntersectuonYouMoo.x + (_view.Width / 2);
                        //    sbYouSt.Completed += MoveOnYouSt_East_Completed_Next_Moo;
                        //}
                        //else if (straight == 2)
                        //{
                        //    xend = GlobalStuff.IntersectuonYouTea.x + (_view.Width / 2);
                        //    sbYouSt.Completed += MoveOnYouSt_East_Completed_Next_Tea;
                        //}
                        break;
                    default:
                        break;
                }
                xend = GlobalStuff.IntersectuonYouMoo.x;
                sbYouSt.Completed += MoveDownMooDr; ;
                //if (straight == 1)
                //{
                //    xend = GlobalStuff.IntersectuonYouMoo.x;
                //    sbYouSt.Completed += MoveDownMooDr; ;
                //}
                //else if (straight == 2)
                //{
                //    xend = GlobalStuff.IntersectuonYouYodel.x + (_view.Width / 2);
                //    sbYouSt.Completed += MoveDownYodelSt; ;
                //}

                // Create the animation for Canvas.Top
                DoubleAnimation moveStreet = new DoubleAnimation
                {
                    From = xstart,
                    To = xend,
                    Duration = new Duration(TimeSpan.FromSeconds(GlobalStuff.mainWindowViewModel.PolicecarToravelSpeed))
                };
                sbYouSt.Children.Add(moveStreet);

                Storyboard.SetTarget(moveStreet, _view);
                Storyboard.SetTargetProperty(moveStreet, new PropertyPath("(Canvas.Left)"));
                sbYouSt.Begin(_view);
            }
            catch (Exception ex)
            {

                GlobalStuff.WriteDebugOutput(" exception " + ex.Message);
            }
        }
        private void MoveOnTeaSt()
        {
            GlobalStuff.WriteDebugOutput(" MoveOnTeaSt");
        }

        private void MoveOnYodelDr()
        {
            GlobalStuff.WriteDebugOutput(" MoveOnYodelDr");
        }
        private void MoveOnMoo()
        {
            try
            {
                double start = Canvas.GetTop(_view);
                Random rnd = new Random();

                double endPoint = 0;
                // Apply animations to the control
                Storyboard currentSB = new Storyboard();
                switch (_currentDirectionParking)
                {
                    case PositionsEWNSEnum.South:
                        endPoint = GlobalStuff.IntersectuonMikMoo.y - GlobalStuff.StreetWidth / 2;
                        currentSB.Completed += MoveOnMoo_South_Completed_Next_Mik;
                        break;
                    case PositionsEWNSEnum.North:
                        endPoint = GlobalStuff.IntersectuonYouMoo.x + (_view.Width);
                        currentSB.Completed += MoveOnMoo_North_Completed_Next_You;
                        break;
                    default:
                        break;
                }

                cLogger.Log("MoveOnMoo: y " + GlobalStuff.IntersectuonMikMoo.y + " end " + endPoint);
                cLogger.Log("MoveOnMoo: " + _currentDirectionParking + " end " + endPoint);
                // Create the animation for Canvas.Top
                DoubleAnimation moveStreet = new DoubleAnimation
                {
                    From = start,
                    To = endPoint,
                    Duration = new Duration(TimeSpan.FromSeconds(GlobalStuff.mainWindowViewModel.PolicecarToravelSpeed))
                };
                currentSB.Children.Add(moveStreet);

                Storyboard.SetTarget(moveStreet, _view);
                Storyboard.SetTargetProperty(moveStreet, new PropertyPath("(Canvas.Top)"));
                currentSB.Begin(_view);
            }
            catch (Exception ex)
            {

                GlobalStuff.WriteDebugOutput(" exception " + ex.Message);
            }
        }

        private void MoveOnMoo_North_Completed_Next_You(object? sender, EventArgs e)
        {
            GlobalStuff.WriteDebugOutput(" MoveOnMoo_North_Completed_Next_You ");
        }

        private void MoveOnMoo_South_Completed_Next_Mik(object? sender, EventArgs e)
        {
            _currentDirectionParking = PositionsEWNSEnum.West;
            _view.ChangeDirection(_currentDirectionParking);
            MoveToBankOnMik();
        }

        private void MoveToBankOnMik()
        {
            try
            {
                double start = Canvas.GetTop(_view);
                Random rnd = new Random();

                double endPoint = 0;
                // Apply animations to the control
                Storyboard currentSB = new Storyboard();
                endPoint = bank.X + 10;
                currentSB.Completed += MoveToBankOn_West_Completed_Next_Mik;

                cLogger.Log("MoveToBankOnMik: y " + GlobalStuff.IntersectuonMikMoo.y + " end " + endPoint);
                cLogger.Log("MoveToBankOnMik: " + _currentDirectionParking + " end " + endPoint);
                // Create the animation for Canvas.Top
                DoubleAnimation moveStreet = new DoubleAnimation
                {
                    From = start,
                    To = endPoint,
                    Duration = new Duration(TimeSpan.FromSeconds(GlobalStuff.mainWindowViewModel.PolicecarToravelSpeed))
                };
                currentSB.Children.Add(moveStreet);

                Storyboard.SetTarget(moveStreet, _view);
                Storyboard.SetTargetProperty(moveStreet, new PropertyPath("(Canvas.Left)"));
                currentSB.Begin(_view);
            }
            catch (Exception ex)
            {

                GlobalStuff.WriteDebugOutput(" exception " + ex.Message);
            }
        }

        private void MoveToBankOn_West_Completed_Next_Mik(object? sender, EventArgs e)
        {
            _currentDirectionParking = PositionsEWNSEnum.South;
            _view.ChangeDirection(_currentDirectionParking);
            MoveToBankParking();
        }

        private void MoveToBankParking()
        {
            try
            {
                double start = Canvas.GetTop(_view);
                Random rnd = new Random();
                var financialLot = GlobalStuff.FinancialLotCobtrols.FirstOrDefault();
                double loty = Canvas.GetTop(financialLot);
                double endPoint = loty;
                // Apply animations to the control
                Storyboard currentSB = new Storyboard();
                currentSB.Completed += MoveToBankParking_South_Completed_Next;
                cLogger.Log("MoveToBankParking: y " + GlobalStuff.IntersectuonMikMoo.y + " end " + endPoint);
                cLogger.Log("MoveToBankParking: " + _currentDirectionParking);
                // Create the animation for Canvas.Top
                DoubleAnimation moveStreet = new DoubleAnimation
                {
                    From = start,
                    To = endPoint,
                    Duration = new Duration(TimeSpan.FromSeconds(GlobalStuff.mainWindowViewModel.PolicecarToravelSpeed))
                };
                currentSB.Children.Add(moveStreet);

                Storyboard.SetTarget(moveStreet, _view);
                Storyboard.SetTargetProperty(moveStreet, new PropertyPath("(Canvas.Top)"));
                currentSB.Begin(_view);
            }
            catch (Exception ex)
            {

                GlobalStuff.WriteDebugOutput(" exception " + ex.Message);
            }
        }

        private void MoveToBankParking_South_Completed_Next(object? sender, EventArgs e)
        {
            _currentDirectionParking = PositionsEWNSEnum.North;
            _view.ChangeDirection(_currentDirectionParking);
            Random rnd = new Random();
            int interval = rnd.Next(2, 10);
            _moveCarBackToStationTimer.Interval = new TimeSpan(0, 0, interval);
            _moveCarBackToStationTimer.Start();
            MoveToPoliceStation_start_Execute = true;
        }

        private void MoveDownYouSt(object? sender, EventArgs e)
        {
            try
            {
                double xstart = Canvas.GetLeft(_view);
                Random rnd = new Random();
                int straight = 0; //1 is left, 2 is straight
                straight = rnd.Next(1, 2);
                _view.ChangeDirection(PositionsEWNSEnum.West);

                //double yend = 0;
                double xend = 0;
                // Apply animations to the control
                Storyboard storyboard2 = new Storyboard();
                if (straight == 1)
                {
                    xend = GlobalStuff.IntersectuonYouMoo.x;
                    storyboard2.Completed += MoveDownMooDr; ;
                }
                else if (straight == 2)
                {
                    xend = GlobalStuff.IntersectuonYouYodel.x + (_view.Width / 2);
                    storyboard2.Completed += MoveDownYodelSt; ;
                }

                // Create the animation for Canvas.Top
                DoubleAnimation moveStreet = new DoubleAnimation
                {
                    From = xstart,
                    To = xend,
                    Duration = new Duration(TimeSpan.FromSeconds(GlobalStuff.mainWindowViewModel.PolicecarToravelSpeed))
                };
                storyboard2.Children.Add(moveStreet);

                Storyboard.SetTarget(moveStreet, _view);
                Storyboard.SetTargetProperty(moveStreet, new PropertyPath("(Canvas.Left)"));
                storyboard2.Begin(_view);
            }
            catch (Exception ex)
            {

                GlobalStuff.WriteDebugOutput(" exception " + ex.Message);
            }
        }

        private void MoveDownYodelSt(object? sender, EventArgs e)
        {
           // throw new NotImplementedException();
        }

        private void MoveDownMooDr(object? sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

      
       
    
    }
}
