using CatoriApp.MachineLayoutDesigner.ViewModels.Robots;
using CatoriUCLibrary.Views.Airplane;
using CatoriUCLibrary.Views.VisualParts;
using CommunityToolkit.Mvvm.Messaging;
using System.Security.Policy;
using System.Text.Json;

namespace CatoriApp.Game.Controllers.LocationSubControllers
{
    public class LocationSubControllerBase : CatoriApp.Game.Controllers.ControllerAnimateLayoutsBase
    {
        public Location_UC _view;
        public LocationService locationService = new LocationService();
        public List<RobotPoseViewModel> robotposes { get; private set; }
        public MachineLayoutDesignerViewModel machineLayoutDesignermodel;
        public RobotPoseService _poseservice = new RobotPoseService();
        public readonly MachineLayoutDesignerService _robotdesignservice = new MachineLayoutDesignerService();
        private readonly LocationService _locationService = new LocationService();
        public LocationViewModel _locationViewModel;
        public readonly List<System.Windows.Shapes.Path> PathsForMovement = new();
        string _backgroundImagePath;
        public LocationSubControllerBase(long locationId, Location_UC view)
            : base(locationId)
        {
            this._view = view;

            _locationService = new LocationService();
            _locationViewModel = _locationService.GetByLocationId(_locationId);
            _view.LocationImage.Source = UIUtility.GetImageControl(System.IO.Path.Combine(GlobalAllApps.ImageFolder,
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
            _view.LocationImage.Source = UIUtility.GetImageControl(_backgroundImagePath, 1920, 1080, 0).Source;

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

        protected void SetupStaticAirplanes()
        {
            foreach (LocationLayoutItemViewModel item in LoadLayoutItemsNow())
            {
                if (string.IsNullOrWhiteSpace(item.MetadataJson))
                    continue;

                try
                {
                    using JsonDocument document = JsonDocument.Parse(item.MetadataJson);
                    JsonElement metadata = document.RootElement;
                    if (!TryGetString(metadata, "ComponentKind", out string? componentKind)
                        || !string.Equals(componentKind, "Airplane", StringComparison.OrdinalIgnoreCase)
                        || !TryGetString(metadata, "DefinitionPath", out string? definitionPath)
                        || string.IsNullOrWhiteSpace(definitionPath))
                        continue;

                    if (_view.MainCanvas.Children.OfType<FrameworkElement>()
                        .Any(child => child.Tag is long id && id == item.LocationLayoutItemId))
                        continue;

                    if (VisualObjectControlFactory.Create(componentKind, definitionPath) is not AirplaneUC airplane)
                        continue;

                    airplane.Tag = item.LocationLayoutItemId;
                    airplane.ToolTip = item.ItemName;
                    airplane.Width = item.Width > 0 ? item.Width : 320;
                    airplane.Height = item.Height > 0 ? item.Height : 180;
                    airplane.IsDragEnabled = false;
                    airplane.IsFacingRight = metadata.TryGetProperty("IsFacingRight", out JsonElement facing)
                        && facing.ValueKind is JsonValueKind.True or JsonValueKind.False
                        && facing.GetBoolean();
                    airplane.RenderTransformOrigin = new Point(.5, .5);
                    airplane.RenderTransform = new RotateTransform(item.RotationDegrees);
                    Canvas.SetLeft(airplane, item.X);
                    Canvas.SetTop(airplane, item.Y);
                    Panel.SetZIndex(airplane, Math.Max(2200, item.ZIndex));
                    _view.MainCanvas.Children.Add(airplane);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Static airplane '{item.ItemName}' could not be loaded: {ex}");
                }
            }
        }

        protected void InitializeLocationFeature(string featureName, Action setup)
        {
            try
            {
                setup();
            }
            catch (Exception ex)
            {
                CatoriShared.Diagnostics.GameSafetyLog.Error("Location initialization",
                    $"Location {_locationId} could not initialize {featureName}. Other scene content will continue loading.", ex);
            }
        }

        private static bool TryGetString(JsonElement metadata, string propertyName, out string? value)
        {
            value = null;
            if (!metadata.TryGetProperty(propertyName, out JsonElement property)
                || property.ValueKind != JsonValueKind.String)
                return false;
            value = property.GetString();
            return true;
        }
      
    }
}
