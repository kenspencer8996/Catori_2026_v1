using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Threading;
using CatoriApp.Game.Objects.Services.Locations;
using CatoriApp.Game.ViewModels.Locations;
using CatoriUCLibrary.Views.RobotArm;
using CatoriApp.Core.Objects.Production;
using CatoriUCLibrary.Views.Airplane;
using CatoriUCLibrary.Views.Person;
using CatoriUCLibrary.Views.VisualParts;
using CatoriUCLibrary.Views.Tickets;
using CatoriUCLibrary.Views.PersonItems;
using CatoriApp.Game.Objects.Travel;
using CommunityToolkit.Mvvm.Messaging;
using CatoriServices.Objects.Services.Finance;
using System.Text.Json;
namespace CatoriApp.Game.Views.Controls.Locations.Factory
{
    /// <summary>
    /// Interaction logic for Location_UC.xaml
    /// </summary>
    public partial class Location_UC : UserControl,IDropTarget
    {
        Location_UController _controller;
        public string ImagesFolderLeft = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "Locations", "RobotArms", "Left");
        public string restImageLeft = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "Locations", "RobotArms", "00RobotArmStart.png");
        DragManager _dragManager;
        readonly DispatcherTimer _animation_timer;
        private readonly LocationLayoutItemService _layoutItemService = new();
        private readonly List<RoboticArmUC> _loadedRobotArms = new();
        public IReadOnlyList<RoboticArmUC> LoadedRobotArms=>_loadedRobotArms;
        private readonly int _locationId;

        public Location_UC(int locationId)
        {
            _locationId = locationId;
            InitializeComponent();
            _dragManager = GlobalCode.GetDragmanager(MainCanvas);
            _animation_timer = new DispatcherTimer(DispatcherPriority.Normal);

            // Debug.WriteLine("RobotControl is now the source: " + e.Source);
            _controller = new Location_UController(this, locationId);
            // RobotPanel.RunRequested += RobotPanel_RunRequested;
            //RobotPanel.DesignModeRequested += RobotPanel_EditModeRequested; ;
            //RobotPanel.DesignModeEndRequested += RobotPanel_EditModeEndRequested; ;
            _dragManager.RegisterDropTarget(this);
            //Canvas.SetLeft(lightPanel, 1476);
            //Canvas.SetTop(lightPanel, 815);
            RobotArmNew.SetupRobot(CatoriUCLibrary.RobotColorEnum.Blue);
            RobotArmNew.Visibility = Visibility.Collapsed;

            //lightPanel.PanelTriggered += LightPanel_PanelTriggered;
            //lightPanel.StartFlicker();


            List<string> conveyors = new List<string?>();
            conveyors.Add("A");
            conveyors.Add("B");
            conveyors.Add("C");
            List<string> outPutItems = new List<string>();
            outPutItems.Add("Paint Booth");
            outPutItems.Add("No Paint");
            //RobotPanel.LoadData("Yellow Builder Robot", conveyors, conveyors, outPutItems);
            // _controller.RobotBuilder.MouseUpAfterRobotMove += RobotBuilder_MoveRobotComplete;
            _animation_timer.Interval = TimeSpan.FromMilliseconds(1000);
            _animation_timer.Tick += Animation_timer_Tick;
            _animation_timer.Start();

            WeakReferenceMessenger.Default.Register<RobotPartTransferMessage>(this,
                static (recipient,message)=>((Location_UC)recipient).HandleFactoryPartTransfer(message));

           // PersonActive.PointArmAt()
        }
        private Window? _hostWindow;

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            ////GlobalGame.MainWindow.AddHandler(Keyboard.PreviewKeyDownEvent,
            ////    new KeyEventHandler(UC_KeyDown), handledEventsToo: true);
            _hostWindow = Window.GetWindow(this);

            if (_hostWindow != null)
            {
                _hostWindow.PreviewKeyDown += UC_KeyDown;
            }
            await _controller.LoadedAsync();
            await LoadLayoutRobotsAsync();
            await LoadLayoutVisualComponentsAsync();
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Debug.WriteLine($"Canvas size: {MainCanvas.ActualWidth}, {MainCanvas.ActualHeight}");
                Debug.WriteLine($"Image size: {LocationImage.ActualWidth}, {LocationImage.ActualHeight}");

                System.Diagnostics.Debug.WriteLine(
                    $"Location_UC arranged size: {ActualWidth}x{ActualHeight}; canvas: {MainCanvas.ActualWidth}x{MainCanvas.ActualHeight}");
            }), DispatcherPriority.Loaded);
        }

        private async Task LoadLayoutVisualComponentsAsync()
        {
            var layoutItems = await _layoutItemService.GetByLocationIdAsync(_locationId, includePoints: false);
            foreach (LocationLayoutItemViewModel item in layoutItems)
            {
                if (string.IsNullOrWhiteSpace(item.MetadataJson))
                    continue;
                try
                {
                    using JsonDocument document = JsonDocument.Parse(item.MetadataJson);
                    JsonElement metadata = document.RootElement;
                    string? kind = GetMetadataString(metadata, "ComponentKind");
                    bool isAirplane = string.Equals(kind, "Airplane", StringComparison.OrdinalIgnoreCase);
                    bool isTicketKiosk = string.Equals(kind, "TicketKiosk", StringComparison.OrdinalIgnoreCase);
                    if (!isAirplane && !isTicketKiosk)
                        continue;

                    bool alreadyLoaded = MainCanvas.Children.OfType<FrameworkElement>().Any(control =>
                        (control.Tag is long id && id == item.LocationLayoutItemId)
                        || ((isAirplane && control is AirplaneUC)
                            || (isTicketKiosk && control is TicketKioskUC))
                        && Math.Abs(Normalize(Canvas.GetLeft(control)) - item.X) < .5
                        && Math.Abs(Normalize(Canvas.GetTop(control)) - item.Y) < .5);
                    if (alreadyLoaded)
                        continue;

                    FrameworkElement? visual = null;
                    if (isAirplane)
                    {
                        string? definitionPath = GetMetadataString(metadata, "DefinitionPath");
                        if (!string.IsNullOrWhiteSpace(definitionPath)
                            && VisualObjectControlFactory.Create(kind, definitionPath) is AirplaneUC airplane)
                        {
                            airplane.IsDragEnabled = false;
                            airplane.IsFacingRight = metadata.TryGetProperty("IsFacingRight", out JsonElement facing)
                                && facing.ValueKind is JsonValueKind.True or JsonValueKind.False
                                && facing.GetBoolean();
                            visual = airplane;
                        }
                    }
                    else
                    {
                        var kiosk = new TicketKioskUC
                        {
                            Destination = GetMetadataString(metadata, "TicketDestination") ?? "Beach",
                            Cost = GetMetadataDecimal(metadata, "TicketCost"),
                            IsDragEnabled = false
                        };
                        kiosk.BuyTicketRequested += TicketKiosk_BuyTicketRequested;
                        visual = kiosk;
                    }

                    if (visual == null)
                        continue;
                    visual.Tag = item.LocationLayoutItemId;
                    visual.ToolTip = item.ItemName;
                    visual.Width = item.Width > 0 ? item.Width : visual.Width;
                    visual.Height = item.Height > 0 ? item.Height : visual.Height;
                    visual.RenderTransformOrigin = new Point(.5, .5);
                    visual.RenderTransform = new RotateTransform(item.RotationDegrees);
                    Canvas.SetLeft(visual, item.X);
                    Canvas.SetTop(visual, item.Y);
                    Panel.SetZIndex(visual, Math.Max(2200, item.ZIndex));
                    MainCanvas.Children.Add(visual);
                }
                catch (Exception ex)
                {
                    CatoriShared.Diagnostics.GameSafetyLog.Error("Layout visual loading",
                        $"Location {_locationId} could not load visual item '{item.ItemName}'.", ex);
                }
            }
        }

        private static string? GetMetadataString(JsonElement metadata, string propertyName) =>
            metadata.TryGetProperty(propertyName, out JsonElement value) && value.ValueKind == JsonValueKind.String
                ? value.GetString()
                : null;

        private static decimal GetMetadataDecimal(JsonElement metadata, string propertyName)
        {
            return metadata.TryGetProperty(propertyName, out JsonElement value)
                && value.TryGetDecimal(out decimal result)
                    ? result
                    : 0m;
        }

        private void TicketKiosk_BuyTicketRequested(object? sender, TicketPurchaseRequestedEventArgs e)
        {
            if (sender is not TicketKioskUC kiosk)
                return;

            PersonUC? person = FindNearestPerson(kiosk, 250);
            if (person == null)
            {
                kiosk.SetPurchaseStatus("Move a traveler closer", true);
                return;
            }

            try
            {
                var ticket = new Ticket_UC
                {
                    Destination = e.Destination,
                    Cost = e.Cost,
                    PurchasedDateTime = e.PurchasedDateTime
                };
                if (e.Cost > 0 && GlobalGame.CurrentPerson != null)
                {
                    var destination = new TravelDestination(
                        e.Destination.ToLowerInvariant().Replace(" ", "-"),
                        e.Destination,
                        e.Destination,
                        e.Cost,
                        string.Empty);
                    TravelTicket purchasedTicket = TravelService.Current.BuyTicket(
                        GlobalGame.CurrentPerson.PersonId,
                        destination);
                    GlobalGame.CurrentPerson.Funds = FinanceService.FromCents(purchasedTicket.WalletBalanceCents);
                }
                person.ReceiveTicket(ticket);
                kiosk.SetPurchaseStatus("Ticket purchased");
                foreach (AirplaneUC airplane in MainCanvas.Children.OfType<AirplaneUC>())
                {
                    _ = airplane.SetDoorOpenAsync(true, TimeSpan.FromMilliseconds(450));
                }
            }
            catch (Exception exception)
            {
                kiosk.SetPurchaseStatus(exception.Message, true);
                CatoriShared.Diagnostics.GameSafetyLog.Error(
                    "Ticket kiosk",
                    $"Could not purchase a ticket to '{e.Destination}'.",
                    exception);
            }
        }

        private PersonUC? FindNearestPerson(FrameworkElement source, double maximumDistance)
        {
            Point sourceCenter = new(
                Normalize(Canvas.GetLeft(source)) + source.ActualWidth / 2,
                Normalize(Canvas.GetTop(source)) + source.ActualHeight / 2);
            return MainCanvas.Children
                .OfType<PersonUC>()
                .Select(person => new
                {
                    Person = person,
                    Distance = (new Point(
                        Normalize(Canvas.GetLeft(person)) + person.ActualWidth / 2,
                        Normalize(Canvas.GetTop(person)) + person.ActualHeight / 2) - sourceCenter).Length
                })
                .Where(candidate => candidate.Distance <= maximumDistance)
                .OrderBy(candidate => candidate.Distance)
                .Select(candidate => candidate.Person)
                .FirstOrDefault();
        }

        private static double Normalize(double value) => double.IsNaN(value) ? 0 : value;

        private async Task LoadLayoutRobotsAsync()
        {
            foreach (var robot in _loadedRobotArms.Skip(1).ToList())
                MainCanvas.Children.Remove(robot);
            _loadedRobotArms.Clear();

            var layoutItems=await _layoutItemService.GetByLocationIdAsync(_locationId,includePoints:false);
            int minimumRobotZ=layoutItems.Where(item=>item.ItemType==LocationLayoutItemType.Conveyor)
                .Select(item=>item.ZIndex).DefaultIfEmpty(0).Max()+1;
            var robotItems = layoutItems
                .Where(item => string.Equals(item.MajorItemType, "Robot", StringComparison.OrdinalIgnoreCase))
                .ToList();

            for (var index = 0; index < robotItems.Count; index++)
            {
                var item = robotItems[index];
                var robot = index == 0 ? RobotArmNew : new RoboticArmUC();
                ConfigureLayoutRobot(robot,item,minimumRobotZ);
                if (index > 0)
                    MainCanvas.Children.Add(robot);
                _loadedRobotArms.Add(robot);
            }
        }

        private static void ConfigureLayoutRobot(RoboticArmUC robot,LocationLayoutItemViewModel layoutItem,int minimumRobotZ)
        {
            robot.Tag = layoutItem;
            robot.AnimationTargetName=layoutItem.ItemName;
            robot.IsDragEnabled = false;
            // Studio leaves these at zero when the robot uses its natural
            // RoboticArmUC design size. Match that 800x600 canvas here so the
            // saved Canvas.Left/Top coordinates describe the same visual.
            robot.Width = layoutItem.Width > 0 ? layoutItem.Width : 800;
            robot.Height = layoutItem.Height > 0 ? layoutItem.Height : 600;
            robot.SetBinding(RoboticArmUC.ItemDataJsonProperty, new Binding(nameof(LocationLayoutItemViewModel.ItemDataJson))
            {
                Source = layoutItem,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
            robot.SetupRobot(CatoriUCLibrary.RobotColorEnum.Blue);
            Canvas.SetLeft(robot, layoutItem.X);
            Canvas.SetTop(robot, layoutItem.Y);
            Canvas.SetZIndex(robot,Math.Max(minimumRobotZ,layoutItem.ZIndex==0?1221:layoutItem.ZIndex));
            robot.Visibility = Visibility.Visible;
        }
        private void UC_Unloaded(object sender, RoutedEventArgs e)
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
            GlobalGame.MainWindow?.RemoveHandler(Keyboard.PreviewKeyDownEvent,
                   new KeyEventHandler(UC_KeyDown));
        }

        private async void HandleFactoryPartTransfer(RobotPartTransferMessage message)
        {
            if(message.Stage!=RobotPartTransferStage.Dropped||message.Part==null||GlobalGame.CurrentPerson==null)
                return;
            try
            {
                var result=await Task.Run(()=>new FinanceService().CreditFactoryDrop(
                    GlobalGame.CurrentPerson.PersonId,_locationId,message.PartName));
                GlobalGame.CurrentPerson.Funds=FinanceService.FromCents(result.WalletBalanceCents);
            }
            catch(Exception exception)
            {
                cLogger.Log($"Factory earning failed for location {_locationId}: {exception}");
            }
        }

        private void UserControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _controller.HandleMouseMove(e);
        }
        private void UserControl_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _controller.HandleMouseLeftButtonDown(e);
        }
        private void UC_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _controller.HandleMouseLeftButtonUp(e);
        }
   

      

        private void LightPanel_PanelTriggered(object? sender, EventArgs e)
        {
            //ShowrobotControlPanel();
        }

        private void Animation_timer_Tick(object? sender, EventArgs e)
        {
            _animation_timer.Stop();
            //run robot animation step here
            _animation_timer.Start();
        }

       
      
        private void MenuItem_Start_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Cross;
            _controller.StartDrawing();
        }

        private void MenuItem_Complete_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Arrow;
            _controller.StopDrawing();
        }

       

        public void OnDropped()
        {
        }

        public bool CanDrop(IDraggable element)
        {
            return true;
        }

        public async void OnDrop(IDraggable element)
        {
            if (element is not PersonUC person)
                return;
            Point center = person.TranslatePoint(new Point(person.ActualWidth / 2, person.ActualHeight / 2), MainCanvas);
            AirplaneUC? airplane = MainCanvas.Children.OfType<AirplaneUC>().Reverse().FirstOrDefault(candidate =>
            {
                Point topLeft = candidate.TranslatePoint(new Point(), MainCanvas);
                return new Rect(topLeft, new Size(candidate.ActualWidth, candidate.ActualHeight)).Contains(center);
            });
            if (airplane == null)
                return;
            if (person.Role == PersonRole.Pilot)
                await airplane.SetPilotAsync(person);
            else if (person.Role == PersonRole.Passenger)
                await airplane.SetPassengerAsync(person);
            else
                return;
            person.Visibility = Visibility.Collapsed;
        }

        public void HighlightOn()
        {

        }

        public void HighlightOff()
        {

        }

        public Point GetSnapPoint(IDraggable dragged)
        {
            return new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
        }

        public UIElement Visual
        {
            get
            {
                return this;
            }
        }

        public Point OriginalPosition
        {
            get
            {
                double left = Canvas.GetLeft(this);
                double top = Canvas.GetTop(this);
                return new Point(left, top);
            }
        }
        public void DanceRobot()
        {
            Random rnd = new Random();
            double j1 = rnd.Next(30, 130) * -1;
            double j2 = rnd.Next(70, 120);
            double j3 = rnd.Next(2, 110);
            double j4 = rnd.Next(1, 80);
            int i = rnd.Next(1, 2000);
            if (is_odd(i))
            {
                j4 = j4 * -1;
                j2 = j2 * -1;
            }
            CatoriUCLibrary.Views.RobotArm.RobotPose targetPose =
                new CatoriUCLibrary.Views.RobotArm.RobotPose(j1, j2, j3, j4);

            (_loadedRobotArms.FirstOrDefault() ?? RobotArmNew).MoveToPoseAsync(targetPose);

        }
        bool is_odd(int n)
        {
            return n % 2 != 0;
        }
        private void UC_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.None:
                    break;
                case Key.A:
                    break;
                case Key.B:
                    break;
                case Key.C:
                    break;
                case Key.D:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        DanceRobot();
                    }
                    break;
                case Key.E:
                    break;
                case Key.F:
                    break;
                case Key.G:
                    break;
                case Key.H:
                    break;
                case Key.I:
                    break;
                case Key.J:
                    break;
                case Key.K:
                    break;
                case Key.L:
                    break;
                case Key.M:
                    break;
                case Key.N:
                    break;
                case Key.O:
                    break;
                case Key.P:
                    break;
                case Key.Q:
                    break;
                case Key.R:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        _controller.ProducePart();
                    }
                    break;
                case Key.S:
                    break;
                case Key.T:
                    break;
                case Key.U:
                    break;
                case Key.V:
                    break;
                case Key.W:
                    break;
                case Key.X:
                    break;
                case Key.Y:
                    break;
                case Key.Z:
                    break;
                default:
                    break;
            }
          }

        private void Zone1_MouseEnter(object sender, MouseEventArgs e)
        {

        }

     }
}







