using CatoriCity2025WPF.Objects;
using CatoriCity2025WPF.ViewModels;
using CatoriCity2025WPF.Views.Controls;
using CatoriServices.Objects;
using CatoriServices.Objects.Entities;
using CityAppServices;
using CityAppServices.Objects;
using CityAppServices.Objects.Entities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CatoriCity2025WPF.Controllers
{
    public class BadPersonController
    {
        internal LandScapeWithBank_ReturnEntity ReturnStackEntity;
        internal Stack<PositionNavigationMasterModel> _pathGeometryToBank = new Stack<PositionNavigationMasterModel>();
        internal Stack<PositionNavigationMasterModel> _pathGeometryHome = new Stack<PositionNavigationMasterModel>();
        private Stack<LandscapeObjectViewModel> LandscapeObjectsForPathTo_Bank { get; set; }
        private Stack<LandscapeObjectViewModel> LandscapeObjectsForPathHome { get; set; }
        private GeometryHelper geometryHelper = new GeometryHelper();
        public int ApproachPoint;
        private PathFigure startpathFigure;
        public Int32 approachx = 0;
        public Int32 approachy = 0;
        List<string> LogImages = new List<string>();
        private ApproqchPoitsEnum FromStartApproachPoint = ApproqchPoitsEnum.None;
        private BadPersonControl _view;
        public static MainWindowViewModel mainWindowViewModel;
        public double BankLocationX { get; set; }
        public double BankLocationY { get; set; }
        public BadPersonController(BadPersonControl view)
        {
            _view = view;

            if (GlobalStuff.ShowAllBordersIfAvailable)
            {
                _view.MainBorder.BorderThickness = new Thickness(2);
            }
            else
            {
                _view.MainBorder.BorderThickness = new Thickness(0);
            }
         }


        public void ControlLoaded()
        {
            ApproachPoint = 1;

            switch (ApproachPoint)
            {
                case 1:
                    approachx = Convert.ToInt32(GlobalStuff.ApproachPointN.x);
                    approachy = Convert.ToInt32(GlobalStuff.ApproachPointN.y);
                    break;
                case 2:
                    approachx = Convert.ToInt32(GlobalStuff.ApproachPointE.x);
                    approachy = Convert.ToInt32(GlobalStuff.ApproachPointE.y);
                    break;
                case 3:
                    approachx = Convert.ToInt32(GlobalStuff.ApproachPointS.x);
                    approachy = Convert.ToInt32(GlobalStuff.ApproachPointS.y);
                    break;
                case 4:
                    approachx = Convert.ToInt32(GlobalStuff.ApproachPointW.x);
                    approachy = Convert.ToInt32(GlobalStuff.ApproachPointW.y);
                    break;
                default:
                    break;
            }
            ReturnStackEntity = geometryHelper.GetShuffledDeckOfLandscapeObjects();
            LandscapeObjectsForPathTo_Bank = geometryHelper.SetupGeometryForPath(ReturnStackEntity.LandscapeObjectsStack, approachx, approachy);
            
            ReturnStackEntity = geometryHelper.GetShuffledDeckOfLandscapeObjects();
            LandscapeObjectsForPathHome = geometryHelper.SetupGeometryForPath(ReturnStackEntity.LandscapeObjectsStack, approachx, approachy);
            ReturnStackEntity.BankVM = GlobalStuff.GetBankViewModelByName(ReturnStackEntity.financial.Name);
            string info = "BadPersonControl constructor total stackedViewModels " + ReturnStackEntity.LandscapeObjectsStack.Count();
            ReturnStackEntity.BankVM.X= BankLocationX;
            ReturnStackEntity.BankVM.Y= BankLocationY;


            cLogger.Log(info);

        }
        internal void CreateGeometry()
        {
            CreatePathToBank();
            CreatePathHome();
            //DrawpathToNextfromHome(approachx, approachy);
           // DrawPathToBank(approachx, approachy);
           // pathGeometry.Freeze();

        }
        private int GetUnsedApproachPoint()
        {
             int approachPoint = geometryHelper.GetRandomIntAtOrBelowMax(4);
            var found = from a in GlobalStuff.ApproachesUsed
                        where a == approachPoint
                        select a;
            if (found.Any())
            {
                var allPoints = Enumerable.Range(1, 4);
                var unused = allPoints.Except(GlobalStuff.ApproachesUsed).ToList();

                if (unused.Count == 0)
                {
                    approachPoint = 1; // fallback if all are used
                }
                else
                {
                    // Optionally, pick a random unused point
                    Random rnd = new Random();
                    approachPoint = unused[rnd.Next(unused.Count)];
                }
            }
            GlobalStuff.ApproachesUsed.Add(approachPoint);

            return approachPoint;
        }
       
        internal void MovePersonOutToApproachStartAndSetupAnimation()
        {
            Storyboard storyboard = new Storyboard();
            storyboard.Completed += MovePersonOutToApproachAndSetupAnimation_Completed;
            SettingEntity badGuyTravelSpeedSetting = GlobalServices.GetSetting("badguytravelspeed");
            GlobalStuff.mainWindowViewModel.BadGuyTravelSpeed = badGuyTravelSpeedSetting.IntSetting;
            TimeSpan duration = TimeSpan.FromSeconds(4); //
            double xstart = Canvas.GetLeft(_view);
            double xend = xstart + 75;
            _view.StartImageAnimation(xend,xend);
            DoubleAnimation moveStartX = new DoubleAnimation
            {
                From = xstart,
                To = xend,
                Duration = new Duration(TimeSpan.FromSeconds(2))
            };
            Storyboard.SetTarget(moveStartX, _view);
            Storyboard.SetTargetProperty(moveStartX, new PropertyPath("(Canvas.Left)"));

            DoubleAnimation fadeInAnimation = new DoubleAnimation()
            { From = 0.0, To = 1.0, Duration = new Duration(duration) };

            Storyboard.SetTargetName(fadeInAnimation, _view.Name);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath("Opacity"));
            storyboard.Children.Add(moveStartX);
            storyboard.Children.Add(fadeInAnimation);
            storyboard.Begin(_view);
        }

        private void MovePersonOutToApproachAndSetupAnimation_Completed(object? sender, EventArgs e)
        {
            MoveFromStartToApproach();
            //_view._moveFromHomeToApproachTimer.Start();
        }

        private void MoveFromStartToApproach()
        {
            long moveoutduration = 12;
            double xstart = Canvas.GetLeft(_view);
            double ystart = Canvas.GetTop(_view);
            _view.Opacity = 1;
            double yend = ystart + 50;
            double xend = xstart + 100;
            _view.StartImageAnimation(xstart,yend);
            DoubleAnimation moveStartX = new DoubleAnimation
            {
                From = xstart,
                To = xend,
                Duration = new Duration(TimeSpan.FromSeconds(moveoutduration))
            };

            // Create the animation for Canvas.Top
            DoubleAnimation moveStartY = new DoubleAnimation
            {
                From = ystart,
                To = yend,
                Duration = new Duration(TimeSpan.FromSeconds(moveoutduration))
            };
            int approachPoint = 0;
            approachPoint = GetUnsedApproachPoint();
            switch (approachPoint)
            {
                case 1:
                    FromStartApproachPoint = ApproqchPoitsEnum.N;
                    break;
                case 2:
                    FromStartApproachPoint = ApproqchPoitsEnum.E;
                    break;
                case 3:
                    FromStartApproachPoint = ApproqchPoitsEnum.S;
                    break;
                case 4:
                    FromStartApproachPoint = ApproqchPoitsEnum.W;
                    break;
                default:
                    break;
            }
            // Apply animations to the control
            Storyboard storyboard = new Storyboard();
            storyboard.Completed += MoveFromStartStoryboard_Completed;
            storyboard.Children.Add(moveStartX);
            storyboard.Children.Add(moveStartY);

            Storyboard.SetTarget(moveStartX, _view);
            Storyboard.SetTargetProperty(moveStartX, new PropertyPath("(Canvas.Left)"));

            Storyboard.SetTarget(moveStartY, _view);
            Storyboard.SetTargetProperty(moveStartY, new PropertyPath("(Canvas.Top)"));

            cLogger.Log(" approachPoint " + approachPoint);
            switch (approachPoint)
            {
                case 1:
                    LocationXYEntity nextLocationN = GlobalStuff.ApproachPointN;
                    DoubleAnimation moveNextX = new DoubleAnimation
                    {
                        From = xend,
                        To = nextLocationN.x,
                        Duration = new Duration(TimeSpan.FromSeconds(moveoutduration))
                    };

                    // Create the animation for Canvas.Top
                    DoubleAnimation moveNextY = new DoubleAnimation
                    {
                        From = yend,
                        To = nextLocationN.y -50,
                        Duration = new Duration(TimeSpan.FromSeconds(moveoutduration))
                    };
                    Storyboard.SetTarget(moveNextX, _view);
                    Storyboard.SetTargetProperty(moveNextX, new PropertyPath("(Canvas.Left)"));

                    Storyboard.SetTarget(moveNextY, _view);
                    Storyboard.SetTargetProperty(moveNextY, new PropertyPath("(Canvas.Top)"));
                    storyboard.Children.Add(moveNextX);
                    storyboard.Children.Add(moveNextY);
                    break;
                case 2:
                    LocationXYEntity nextLocationE = GlobalStuff.ApproachPointE;
                    DoubleAnimation moveNextEX = new DoubleAnimation
                    {
                        From = xend,
                        To = nextLocationE.x - 5,
                        Duration = new Duration(TimeSpan.FromSeconds(moveoutduration))
                    };

                    // Create the animation for Canvas.Top
                    DoubleAnimation moveNextEY = new DoubleAnimation
                    {
                        From = yend,
                        To = nextLocationE.y -30,
                        Duration = new Duration(TimeSpan.FromSeconds(moveoutduration))
                    };
                    Storyboard.SetTarget(moveNextEX, _view);
                    Storyboard.SetTargetProperty(moveNextEX, new PropertyPath("(Canvas.Left)"));

                    Storyboard.SetTarget(moveNextEY, _view);
                    Storyboard.SetTargetProperty(moveNextEY, new PropertyPath("(Canvas.Top)"));
                    storyboard.Children.Add(moveNextEX);
                    storyboard.Children.Add(moveNextEY);
                    break;
                case 3:
                    LocationXYEntity nextLocationS = GlobalStuff.ApproachPointS;
                    DoubleAnimation moveNextSX = new DoubleAnimation
                    {
                        From = xend,
                        To = nextLocationS.x,
                        Duration = new Duration(TimeSpan.FromSeconds(moveoutduration))
                    };

                    // Create the animation for Canvas.Top
                    DoubleAnimation moveNextSY = new DoubleAnimation
                    {
                        From = yend,
                        To = nextLocationS.y - 10,
                        Duration = new Duration(TimeSpan.FromSeconds(moveoutduration))
                    };
                    storyboard.Children.Add(moveNextSX);
                    storyboard.Children.Add(moveNextSY);

                    Storyboard.SetTarget(moveNextSX, _view);
                    Storyboard.SetTargetProperty(moveNextSX, new PropertyPath("(Canvas.Left)"));

                    Storyboard.SetTarget(moveNextSY, _view);
                    Storyboard.SetTargetProperty(moveNextSY, new PropertyPath("(Canvas.Top)")); break;
                case 4:
                    LocationXYEntity nextLocationWest = GlobalStuff.ApproachPointW;
                    double firstx = xend - 150;
                    DoubleAnimation moveNextWX = new DoubleAnimation
                    {
                        From = firstx,
                        To = nextLocationWest.x,
                        Duration = new Duration(TimeSpan.FromSeconds(moveoutduration))
                    };

                    // Create the animation for Canvas.Top
                    DoubleAnimation moveNextWY = new DoubleAnimation
                    {
                        From = yend,
                        To = nextLocationWest.y,
                        Duration = new Duration(TimeSpan.FromSeconds(moveoutduration))
                    };
                    storyboard.Children.Add(moveNextWX);
                    storyboard.Children.Add(moveNextWY);
                    Storyboard.SetTarget(moveNextWX, _view);
                    Storyboard.SetTargetProperty(moveNextWX, new PropertyPath("(Canvas.Left)"));

                    Storyboard.SetTarget(moveNextWY, _view);
                    Storyboard.SetTargetProperty(moveNextWY, new PropertyPath("(Canvas.Top)"));
                    DoubleAnimation moveSecondWX = new DoubleAnimation
                    {
                        From = xend,
                        To = nextLocationWest.x,
                        Duration = new Duration(TimeSpan.FromSeconds(moveoutduration))
                    };

                    // Create the animation for Canvas.Top
                    DoubleAnimation moveSecondWY = new DoubleAnimation
                    {
                        From = yend,
                        To = nextLocationWest.y,
                        Duration = new Duration(TimeSpan.FromSeconds(moveoutduration))
                    };
                    storyboard.Children.Add(moveSecondWX);
                    storyboard.Children.Add(moveSecondWY);
                    Storyboard.SetTarget(moveSecondWX, _view);
                    Storyboard.SetTargetProperty(moveSecondWX, new PropertyPath("(Canvas.Left)"));

                    Storyboard.SetTarget(moveSecondWY, _view);
                    Storyboard.SetTargetProperty(moveSecondWY, new PropertyPath("(Canvas.Top)"));
                    break;
                default:
                    break;
            }

            // Start the animation
            // Loaded += (s, e) => storyboard.Begin();
            storyboard.Begin();
        }

        internal void MoveFromStartStoryboard_Completed(object? sender, EventArgs e)
        {
            _view.StopImageAnimation();

            SitCharacterOnLog_ApproachItem();
            _view.StartImageAnimation(Canvas.GetLeft(_view), Canvas.GetTop(_view));
            _view._moveOutToBankTimer.Start();
        }
        internal void SitCharacterOnLog_ApproachItem()
        {
            string NameSelector = "";
            switch (FromStartApproachPoint)
            {
                case ApproqchPoitsEnum.N:
                    NameSelector = "NorthApproach";
                    if (_view.Person.SittingFacingFrontImage != "NA")
                        _view.MainImage.Source = UIUtility.GetImageControl(_view.Person.SittingFacingFrontImage, 10, 5, 0).Source; ;
                    break;
                case ApproqchPoitsEnum.E:
                    NameSelector = "EastApproach";
                    if (_view.Person.SittingFacingLeftImage != "NA")
                        _view.MainImage.Source = UIUtility.GetImageControl(_view.Person.SittingFacingLeftImage, 10, 5, 0).Source; ;
                    break;
                case ApproqchPoitsEnum.S:
                    NameSelector = "SouthApproach";
                    if (_view.Person.SittingFacingFrontImage != "NA")
                        _view.MainImage.Source = UIUtility.GetImageControl(_view.Person.SittingFacingRearImage, 10, 5, 0).Source; ;
                    break;
                case ApproqchPoitsEnum.W:
                    NameSelector = "WestApproach";
                    if (_view.Person.SittingFacingRightImage != "NA")
                        _view.MainImage.Source = UIUtility.GetImageControl(_view.Person.SittingFacingLeftImage, 10, 5, 0).Source; ;
                     break;
                default:
                    break;
            }
            //var found = from a in GlobalStuff.LandscapeObjectApproachNextControls
            //            where a.Name == NameSelector
            //            select a;
            //if (found.Any())
            //    found.First().Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }


        internal void CreatePathToBank()
        {
            //foreach (var item in GlobalStuff.LandscapeObjects)
            Random random = new Random();
            double xvar = 0;
            double yvar = 0;
            PositionNavigationMasterModel positionNavigationMaster = null;

            cLogger.Log("---------------------------------CreatePathToBank-for item xActual ");
            double lastendx = 0;
            double lastendy = 0;
            LandscapeObjectViewModel startmodel = null;
            var models = from m in GlobalStuff.LandscapeObjects
                         where m.HomeObject == true
                         select m;
            if (models.Count() > 0)
                startmodel = models.First();

            PathPositionModel startPosition = new PathPositionModel
            {
                LandscapeName = startmodel.Name,
                startx = approachx,
                starty = approachy,
                centerx = startmodel.xCenter,
                centery = startmodel.yCenter,
                endx = startmodel.xCenter,
                endy = startmodel.yCenter,
                GeometryType = "Line"
            };
            foreach (var itemLandscape in LandscapeObjectsForPathTo_Bank)
            {
                cLogger.Log("CreatePathToBank-for item xActual " + itemLandscape.Name + " X " + itemLandscape.xActual + " Y " + itemLandscape.yActual);
                positionNavigationMaster = new PositionNavigationMasterModel
                {
                    PositionName = itemLandscape.Name
                };

                lastendx = startPosition.endx;
                lastendy = startPosition.endy;
                positionNavigationMaster.PathPositions.Add(startPosition);
                foreach (var position in itemLandscape.PathPositions)
                {
                    position.LandscapeName = itemLandscape.Name;
                    //List<PointEntity> points = new List<PointEntity>(); 
                    double xdist = position.endx - position.startx;
                    double ydist = position.endy - position.starty;
                    double secondaryndaryoffPathDist = geometryHelper.GetRandomIntAtOrBelowMax(8);
                    int segments = 0;
                    int segmentlength = 0;
                    bool mainx = false;
                    if (xdist > ydist)
                    {
                        mainx = true;
                        //segments = (int)(xdist / geometryHelper.GetRandomIntAtOrBelowMax(4));
                    }
                    else
                    {
                        mainx = false;
                        //segments = (int)(ydist / geometryHelper.GetRandomIntAtOrBelowMax(4));
                    }
                    segmentlength = (int)(xdist / segments);

                    if (mainx)
                    {
                        int randomNumber = random.Next(3, 15);
                        if (yvar > 0)
                            yvar = -randomNumber;
                        else
                            yvar = randomNumber;
                    }
                    lastendx = position.endx;
                    lastendy = position.endy;
                    // GlobalStuff.PathThickness
                    positionNavigationMaster.PathPositions.Add(position);
                    
                    cLogger.Log("   --  " + position.GeometryType + " startx " + position.startx + " Y " + position.startx + " endx " + position.endx + " endy " + position.endy);
                }
            }
            PathPositionModel bankPosition = new PathPositionModel
            {
                LandscapeName = ReturnStackEntity.financial.Name,
                startx = lastendx,
                starty = lastendy,
                centerx = BankLocationX + (100 / 2),
                centery = BankLocationY + (100 / 2),
                endx = BankLocationX,
                endy = BankLocationY,
                GeometryType = "Line"
            };
            cLogger.Log("Bank position  x st " + bankPosition.startx + " Y " + bankPosition.starty + " endx " + bankPosition.endx + " endy " + bankPosition.endy);
            positionNavigationMaster.PathPositions.Add(bankPosition);
            _pathGeometryToBank.Push(positionNavigationMaster);
            cLogger.Log("---------------------------------*End CreatePathToBank-for item xActual ");
        }
        private void CreatePathHome()
        {
            //foreach (var item in GlobalStuff.LandscapeObjects)
            Random random = new Random();
            double xvar = 0;
            double yvar = 0;

            cLogger.Log("---------------------------------CreatePathHome-for item xActual ");
            foreach (var item in LandscapeObjectsForPathTo_Bank)
            {
                cLogger.Log("CreatePathHome-for item xActual " + item.Name + " X " + item.xActual + " Y " + item.yActual);
                PositionNavigationMasterModel positionNavigationMaster = new PositionNavigationMasterModel
                {
                    PositionName = item.Name

                };
                if (item.PathPositions.Count > 0)
                {
                    foreach (var position in item.PathPositions)
                    {
                        //List<PointEntity> points = new List<PointEntity>(); 
                        double xdist = position.endx - position.startx;
                        double ydist = position.endy - position.starty;
                        double secondaryndaryoffPathDist = geometryHelper.GetRandomIntAtOrBelowMax(8);
                        int segments = 0;
                        int segmentlength = 0;

                        double xdistpositiveNumber = Math.Abs(xdist);

                        segmentlength = (int)(xdistpositiveNumber / segments);

                        int randomNumber = random.Next(3, 15);
                        if (yvar > 0)
                            yvar = -randomNumber;
                        else
                            yvar = randomNumber;

                        // GlobalStuff.PathThickness

                        positionNavigationMaster.PathPositions.Add(position);
                        cLogger.Log("   --  " + position.GeometryType + "x st " + position.startx + " Y " + position.startx + " endx " + position.endx + " endy " + position.endy);
                    }
                }
                _pathGeometryHome.Push(positionNavigationMaster);
             cLogger.Log("---------------------------------*End CreatePathHome-for item xActual ");
            }
        }

    

    }
}
