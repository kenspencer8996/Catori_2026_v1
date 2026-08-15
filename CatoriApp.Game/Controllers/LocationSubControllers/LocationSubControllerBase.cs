using CatoriApp.MachineLayoutDesigner.ViewModels.Robots;
using CommunityToolkit.Mvvm.Messaging;
using System.Security.Policy;

namespace CatoriApp.Game.Controllers.LocationSubControllers
{
    public class LocationSubControllerBase : CatoriApp.Game.Controllers.ControllerAnimateLayoutsBase
    {
        public FactoryInterior_UC _view;
        public LocationService locationService = new LocationService();
        public List<RobotPoseViewModel> robotposes { get; private set; }
        public MachineLayoutDesignerViewModel machineLayoutDesignermodel;
        public RobotPoseService _poseservice = new RobotPoseService();
        public readonly MachineLayoutDesignerService _robotdesignservice = new MachineLayoutDesignerService();
        private readonly LocationService _locationService = new LocationService();
        public LocationViewModel _locationViewModel;
        public readonly List<System.Windows.Shapes.Path> PathsForMovement = new();
        string _backgroundImagePath;
        public LocationSubControllerBase(long locationId, FactoryInterior_UC view)
            : base(locationId)
        {
            this._view = view;

            _locationService = new LocationService();
            _locationViewModel = _locationService.GetByLocationId(_locationId);
            _view.FactoryInteriorImage.Source = UIUtility.GetImageControl(System.IO.Path.Combine(GlobalAllApps.ImageFolder,
                "LocationInteriors", "Factory", _locationViewModel.BackgroundImagePath), 1920, 1080, 0).Source;

            //_zones = new List<Polygon>();
        }

        public async Task InitializeAsync()
        {
            robotposes = _locationViewModel.LocationId > 0
              ? await _poseservice.GetByLocationIdAsync(_locationViewModel.LocationId)
              : new List<RobotPoseViewModel>();

            await SetupInteriorAsync(_locationViewModel);
            await LoadLayoutItemsAsync();
        }
        public async Task SetupInteriorAsync(LocationViewModel vm)
        {
            _backgroundImagePath = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "LocationInteriors", "Factory", vm.BackgroundImagePath);
            _view.FactoryInteriorImage.Source = UIUtility.GetImageControl(_backgroundImagePath, 1920, 1080, 0).Source;

            //_view.RobotPanel.LocationBackgroundImagePath = _backgroundImagePath;
        }
        private TranslateTransform GetTransform()
        {

            TranslateTransform transform = new TranslateTransform();
            return transform;

        }
        private void SwitchPathColor()
        {
            //foreach (var path in _designerPaths)
            //{

            //    if (_editMode)
            //    {
            //        path.Stroke = Brushes.LimeGreen;
            //        path.StrokeThickness = 4;
            //        path.Opacity = 1.0;
            //    }
            //    else
            //    {
            //        path.Stroke = Brushes.LimeGreen;
            //        path.StrokeThickness = 2;
            //        path.Opacity = 0.15;
            //    }
            //}
        }
        public async Task LoadLayoutItems()
        {
            PathPlayer pathPlayer = new PathPlayer(_view);
            var layoutItems = await LoadLayoutItemsAsync();
            foreach (var item in layoutItems)
            {
                var layoutItem = item.ToEntity();
                pathPlayer.LoadExistingPath(layoutItem);
                PathsForMovement.AddRange(pathPlayer.PathsForMovement);
            }
        }
      
    }
}
