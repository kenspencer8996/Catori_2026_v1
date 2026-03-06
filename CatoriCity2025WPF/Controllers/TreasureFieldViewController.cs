using CatoriCity2025WPF.Objects.Arguments;
using CatoriCity2025WPF.Objects.DragDrop;
using CatoriCity2025WPF.Views;
using CatoriCity2025WPF.Views.Controls.Digging;
using CatoriCity2025WPF.Views.Controls.Treasure;

namespace CatoriCity2025WPF.Controllers
{
    public class TreasureFieldViewController
    {
        TreasureFieldView _view;
        double viewMainwidth;
        double viewMainheight;
        double startOfTopLayout = 600;
        DragManager _dragManager;
        PersonControl person;

        public TreasureFieldViewController(TreasureFieldView view)
        {
            _view = view;
            _dragManager = GlobalCode.GetDragmanager(_view.MainLayoutField);

            person = new PersonControl(GlobalStuff.CurrentPerson, _dragManager, _view.MainLayoutField);
        }

        public async Task LoadLandscapeAsync(int landscapegroup)
        {
            viewMainwidth = _view.Width;
            viewMainheight = _view.Height;
            GlobalStuff.LandscapeUCs = new List<LandscapeObjectControl>();
            LandscapeObjectService landscapeservice = new LandscapeObjectService();
            GlobalStuff.LandscapeObjects = await landscapeservice.GetLandscapeObjectsAsync(landscapegroup);
            LandscapeObjectViewModel featureModel = new LandscapeObjectViewModel();
            foreach (var landscapeObject in GlobalStuff.LandscapeObjects)
            {
                if (landscapeObject.FeatureNote == null || landscapeObject.FeatureNote == "")
                {
                    LandscapeObjectControl thisUC = new LandscapeObjectControl();
                    double x, y;
                    thisUC = GetLandscapeObject(landscapeObject, landscapeObject.Name);
                    x = landscapeObject.xActual;
                    y = landscapeObject.yActual;

                    GlobalStuff.LandscapeUCs.Add(thisUC);
                    Canvas.SetZIndex(thisUC, 1101);
                    _view.MainLayoutField.Children.Add(thisUC);
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
            //AddTreasureSpot();
            AddPersonControls();
        }

        private void AddPersonControls()
        {
            DigInHoleControl digInHoleControl = new DigInHoleControl(_dragManager, _view.MainLayoutField    ); 
            _view.MainLayoutField.Children.Add(digInHoleControl);
            double left = GetRandomInRangeDouble(1, viewMainwidth - digInHoleControl.Width);
            double top = GetRandomInRangeDouble(startOfTopLayout, (viewMainheight - (digInHoleControl.Height-50)));
            Canvas.SetLeft(digInHoleControl, left);
            Canvas.SetTop(digInHoleControl, top);
            Canvas.SetZIndex(digInHoleControl, 1100);
            _dragManager.RegisterDropTarget(digInHoleControl);

            _view.MainLayoutField.Children.Add(person);
            left = GetRandomInRangeDouble(1, viewMainwidth - person.Width);
            top = GetRandomInRangeDouble(startOfTopLayout, viewMainheight - digInHoleControl.Height);
            Canvas.SetLeft(person, left);
            Canvas.SetTop(person, top);
            person.ShowPerson();

        }
     
        private void AddTreasureSpot()
        {
            TreasureSpotControl treasureSpotControl = new TreasureSpotControl();
            _view.MainLayoutField.Children.Add(treasureSpotControl);
             Canvas.SetZIndex(treasureSpotControl, 1100);
            double left = GetRandomInRangeDouble(1, viewMainwidth - treasureSpotControl.Width);
            double top = GetRandomInRangeDouble(startOfTopLayout, viewMainheight - treasureSpotControl.Height);
            Canvas.SetLeft(treasureSpotControl, left);
            Canvas.SetTop(treasureSpotControl, top);
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
