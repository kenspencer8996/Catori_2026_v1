using CatoriCity2025WPF.Objects.Arguments;
using CatoriCity2025WPF.Objects.DragDrop;
using CatoriCity2025WPF.Views;
using CatoriCity2025WPF.Views.Controls.Treasure;

namespace CatoriCity2025WPF.Controllers
{
    public class TreasureFieldViewController
    {
        TreasureFieldView _view;
        double viewMainwidth;
        double viewMainheight;
        double topfieldboundryleft = 680;
        double topfieldboundryright = 720;
        DragManager _dragManager;
        PersonControl person;

        public TreasureFieldViewController(TreasureFieldView view)
        {
            _view = view;
            _dragManager = GlobalCode.GetDragmanager(_view.MainLayoutField);

            person = new PersonControl(GlobalAllApps.CurrentPerson, _dragManager, _view.MainLayoutField);
        }

        public async Task LoadLandscapeAsync(int landscapegroup)
        {
            viewMainwidth = _view.Width;
            viewMainheight = _view.Height;
            CityScapeGlobal.LandscapeUCs = new List<LandscapeObjectControl>();
            LandscapeObjectService landscapeservice = new LandscapeObjectService();
            CityScapeGlobal.LandscapeObjects = await landscapeservice.GetLandscapeObjectsAsync(landscapegroup);
            LandscapeObjectViewModel featureModel = new LandscapeObjectViewModel();
            foreach (var landscapeObject in CityScapeGlobal.LandscapeObjects)
            {
                if (landscapeObject.FeatureNote == null || landscapeObject.FeatureNote == "")
                {
                    LandscapeObjectControl thisUC = new LandscapeObjectControl();
                    double x, y;
                    thisUC = GetLandscapeObject(landscapeObject, landscapeObject.Name);
                    x = landscapeObject.xActual;
                    y = landscapeObject.yActual;

                    CityScapeGlobal.LandscapeUCs.Add(thisUC);
                    Canvas.SetZIndex(thisUC, 1101);
                    _view.MainLayoutField.Children.Add(thisUC);
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
            //AddTreasureSpot();
            AddPersonAndTreasureControls();
        }

        private void AddPersonAndTreasureControls()
        {
            Random rnd = new Random();
            int randomNumber = rnd.Next(1, 10); // Generates a random number between 1 and 3 (inclusive)
            TreasureSpotControl treasurespotControl = null;
            double left;
            double top;
            cLogger.Log($"workbench width {_view.workbench.Width} _view.workbench height {_view.workbench.Height}");
            _dragManager.RegisterDropTarget(_view.workbench);
            
            for (int i = 1; i <= randomNumber; i++)
            {
                treasurespotControl = new TreasureSpotControl();
                _view.MainLayoutField.Children.Add(treasurespotControl);
                left = GetRandomInRangeDouble(1, viewMainwidth - treasurespotControl.Width);
                double topCalculationStart;
                if (left < (viewMainwidth /2))
                    topCalculationStart = topfieldboundryleft;
                else
                    topCalculationStart = topfieldboundryright;
                cLogger.Log("Treasure spot " + i + " top calculation start: " + topCalculationStart);
                top = GetRandomInRangeDouble(topfieldboundryleft, (viewMainheight - (treasurespotControl.Height - 50)));
                Point point = GetValidPointForTreasureSpot(left, top, _view.workbench);
                cLogger.Log("Treasure spot " + i + " x: " + point.X + " point: " + point.Y);
                left = point.X;
                top = point.Y;
                Canvas.SetLeft(treasurespotControl, left);
                Canvas.SetTop(treasurespotControl, top);
                Canvas.SetZIndex(treasurespotControl, 1100);
                _dragManager.RegisterDropTarget(treasurespotControl); 
            }
            
            _view.MainLayoutField.Children.Add(person);
            Canvas.SetZIndex(person, 1102);
            left = GetRandomInRangeDouble(1, viewMainwidth - person.Width);
            top = GetRandomInRangeDouble(topfieldboundryright, viewMainheight - treasurespotControl.Height);
            double personmaxtop = viewMainheight - person.Height;
            if (top > personmaxtop)
                    top = personmaxtop; 
            Canvas.SetLeft(person, left);
            Canvas.SetTop(person, top);
            Canvas.SetZIndex(person, 1102);
            person.ShowPerson();

        }
     private Point GetValidPointForTreasureSpot(double treasurespotleft, 
         double treasurespottop,TreasureWorkBench workbench)
        {
            cLogger.Log("GetValidPointForTreasureSpot - treasurespotleft: " + treasurespotleft + " treasurespottop: " + treasurespottop);
            double workbenchoffset = 75;
            Point resultPoint = new Point();
            double workbenchleft = Canvas.GetLeft(_view.workbench);
            double workbenchtop = Canvas.GetTop(_view.workbench);
            double workbenchright = workbenchleft + _view.workbench.TableHeight;
            double workbenchbottom = workbenchtop + _view.workbench.TableWidth;

            double treasurespotmaxtopdist = workbenchtop - treasurespottop;
            double treasurespotmaxleftdist = workbenchleft - treasurespotleft;
            if (treasurespotmaxtopdist > 0 && treasurespotmaxtopdist < workbenchoffset)
                treasurespottop = workbenchtop - workbenchoffset;
            if (treasurespotmaxleftdist > 0 && treasurespotmaxleftdist < workbenchoffset)
                treasurespotleft = workbenchleft - workbenchoffset;
            if (treasurespotmaxtopdist > 0 && treasurespotmaxtopdist < workbenchtop)
                treasurespottop = workbenchtop - workbenchoffset;
            if (treasurespotmaxleftdist < workbenchoffset)
                treasurespotleft = workbenchleft - workbenchoffset;

            resultPoint.X = treasurespotleft;
            resultPoint.Y = treasurespottop;

            cLogger.Log("GetValidPointForTreasureSpot - result point x: " + resultPoint.X + " result point y: " + resultPoint.Y);
            return resultPoint;
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

        private void ThisUC_OnDragDropChange(object? sender, DragDropChangeArgs e)
        {
        }

        public int RandomInRangeInt(int min, int max)
        {
            Random rnd = new Random();

            // Generate a random integer in the range [min, max)
            int randomNumber = rnd.Next(min, max);


            return randomNumber;
        }
        // Method to generate a random double in a given range [min, max)
        public static double GetRandomInRangeDouble(double minValue, double maxValue)
        {
            if (minValue >= maxValue)
                throw new ArgumentException("minValue must be less than maxValue.");

            Random random = new Random(); // For better randomness, reuse this in real apps
            return random.NextDouble() * (maxValue - minValue) + minValue;
        }
    }
}
