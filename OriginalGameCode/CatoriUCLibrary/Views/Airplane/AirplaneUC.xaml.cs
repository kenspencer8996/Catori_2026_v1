using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using CatoriUCLibrary.Views.Person;
using CatoriApp.Core.Objects.DragDrop;
using System.Windows.Input;
using CatoriUCLibrary.Views.VisualParts;

namespace CatoriUCLibrary.Views.Airplane;

public sealed class AirplaneSettings
{
    public VisualPartConfiguration Body { get; init; } = Part("Body", 20, 45, 280, 90, 0);
    public VisualPartConfiguration? LeftWing { get; init; }
    public VisualPartConfiguration? RightWing { get; init; }
    public VisualPartConfiguration? LeftPropeller { get; init; }
    public VisualPartConfiguration? RightPropeller { get; init; }
    // Retained for projects created before separate left/right propellers were supported.
    public VisualPartConfiguration? Propeller { get; init; }
    public VisualPartConfiguration? Door { get; init; }
    public VisualPartConfiguration? LandingGear { get; init; }
    public IReadOnlyList<VisualPartConfiguration> LandingGearParts { get; init; } = [];
    public double DoorOpenAngle { get; init; } = -15;
    public double LandingGearRetractedAngle { get; init; } = 90;
    private static VisualPartConfiguration Part(string name, double x, double y, double width, double height, int z)
    {
        return new() { Name = name, X = x, Y = y, Width = width, Height = height, PivotX = .5, PivotY = .5, ZOrder = z };
    }
}

public partial class AirplaneUC : UserControl,IDraggable,ICanvasDragAnchor,IDropTarget
{
    private AirplaneSettings _settings = new();
    private readonly List<Image> _additionalLandingGearImages = [];
    private CancellationTokenSource? _leftEngineCancellation;
    private CancellationTokenSource? _rightEngineCancellation;
    private bool _isDragging;
    private bool _boardingInProgress;
    private Point _dragOffset;
    public AirplaneUC() { InitializeComponent(); ApplySettings(_settings); }
    public VisualPartAnimatorUC Animator => PartAnimator;
    public event EventHandler? DragCompleted;
    public event EventHandler? PassengerDropCompleted;

    public bool IsDragEnabled { get; set; } = true;
    public UIElement Visual => this;
    public Point OriginalPosition => new(Normalize(Canvas.GetLeft(this)),Normalize(Canvas.GetTop(this)));
    public double DragAnchorX => ActualWidth>0?ActualWidth/2:Width/2;
    public double DragAnchorY => ActualHeight>0?ActualHeight/2:Height/2;
    public PersonUC Pilot => PilotView;
    public PersonUC Passenger => PassengerView;
    public bool HasPilot { get; private set; }
    public bool HasPassenger { get; private set; }
    public bool IsFacingRight
    {
        get => (bool)GetValue(IsFacingRightProperty);
        set => SetValue(IsFacingRightProperty,value);
    }
    public static readonly DependencyProperty IsFacingRightProperty=DependencyProperty.Register(
        nameof(IsFacingRight),typeof(bool),typeof(AirplaneUC),
        new FrameworkPropertyMetadata(false,static (control,_)=>((AirplaneUC)control).ApplyHorizontalFacing()));
    public event EventHandler? TaxiStarted;
    public event EventHandler? LeftTarmac;
    public void OnDragMouseup()
    {
        DragCompleted?.Invoke(this, EventArgs.Empty);
    }

    public void FaceLeft()
    {
        IsFacingRight = false;
    }

    public void FaceRight()
    {
        IsFacingRight = true;
    }

    public void FlipHorizontal()
    {
        IsFacingRight = !IsFacingRight;
    }

    public bool CanDrop(IDraggable element)
    {
        return element is PersonUC person &&
        ((person.Role == PersonRole.Pilot && !HasPilot)
            || (person.Role == PersonRole.Passenger && !HasPassenger));
    }

    public async void OnDrop(IDraggable element)
    {
        try
        {
            if (element is not PersonUC person || !CanDrop(person)) return;
            _boardingInProgress = true;
            try
            {
                await SetDoorOpenAsync(true, TimeSpan.FromMilliseconds(250));
                if (person.Role == PersonRole.Pilot)
                    await SetPilotAsync(person);
                else
                {
                    await SetPassengerAsync(person);
                    StartTaxiAsync();
                }
                person.Visibility = Visibility.Collapsed;
                await SetDoorOpenAsync(false, TimeSpan.FromMilliseconds(450));
            }
            finally
            {
                _boardingInProgress = false;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("airplane drop exception " + ex.Message);
            
        }
    }

    public Point GetSnapPoint(IDraggable dragged)
    {
        if(Parent is not Canvas canvas || dragged.Visual is not FrameworkElement person)
            return OriginalPosition;
        Point airplaneTopLeft=TranslatePoint(new Point(),canvas);
        return new Point(
            airplaneTopLeft.X+(ActualWidth-person.ActualWidth)/2,
            airplaneTopLeft.Y+(ActualHeight-person.ActualHeight)/2);
    }

    public void HighlightOn()
    {
        _ = SetDoorOpenAsync(true, TimeSpan.FromMilliseconds(250));
    }

    public void HighlightOff()
    {
        if(!_boardingInProgress&&!HasPassenger)
            _=SetDoorOpenAsync(false,TimeSpan.FromMilliseconds(250));
    }

    private void ApplyHorizontalFacing()
    {
        AirplaneRoot.RenderTransformOrigin=new Point(.5,.5);
        AirplaneRoot.RenderTransform=new ScaleTransform(IsFacingRight?-1:1,1);
    }

    private void AirplaneMouseDown(object sender,MouseButtonEventArgs e)
    {
        if(!IsDragEnabled||Parent is not Canvas canvas)return;
        Point pointer=e.GetPosition(canvas);
        _dragOffset=new Point(pointer.X-Normalize(Canvas.GetLeft(this)),pointer.Y-Normalize(Canvas.GetTop(this)));
        _isDragging=true;CaptureMouse();e.Handled=true;
    }
    private void AirplaneMouseMove(object sender,MouseEventArgs e)
    {
        if(!_isDragging||e.LeftButton!=MouseButtonState.Pressed||Parent is not Canvas canvas)return;
        Point pointer=e.GetPosition(canvas);Canvas.SetLeft(this,Math.Max(0,pointer.X-_dragOffset.X));Canvas.SetTop(this,Math.Max(0,pointer.Y-_dragOffset.Y));e.Handled=true;
    }
    private void AirplaneMouseUp(object sender,MouseButtonEventArgs e)
    {
        if(!_isDragging)return;
        _isDragging=false;ReleaseMouseCapture();DragCompleted?.Invoke(this,EventArgs.Empty);e.Handled=true;
    }

    public void ApplySettings(AirplaneSettings settings)
    {
        _leftEngineCancellation?.Cancel();
        _rightEngineCancellation?.Cancel();
        _leftEngineCancellation=null;
        _rightEngineCancellation=null;
        _settings = NormalizeSettings(settings ?? throw new ArgumentNullException(nameof(settings)));
        settings=_settings;
        PartAnimator.ClearParts();
        foreach (Image image in _additionalLandingGearImages)
            AirplaneRoot.Children.Remove(image);
        _additionalLandingGearImages.Clear();
        Configure(BodyImage, settings.Body);
        ConfigureOptional(LeftWingImage, settings.LeftWing);
        ConfigureOptional(RightWingImage, settings.RightWing);
        ConfigureOptional(LeftPropellerImage, settings.LeftPropeller);
        ConfigureOptional(RightPropellerImage, settings.RightPropeller);
        ConfigureOptional(PropellerImage, settings.LeftPropeller is null && settings.RightPropeller is null
            ? settings.Propeller : null);
        ConfigureOptional(DoorImage, settings.Door);
        IReadOnlyList<VisualPartConfiguration> landingGear = GetLandingGearParts(settings);
        ConfigureOptional(LandingGearImage, landingGear.FirstOrDefault());
        LandingGearImage.Opacity=1;
        foreach (VisualPartConfiguration part in landingGear.Skip(1))
        {
            var image = new Image { Stretch = System.Windows.Media.Stretch.Fill };
            AirplaneRoot.Children.Add(image);
            _additionalLandingGearImages.Add(image);
            Configure(image, part);
            image.Opacity=1;
        }
        PilotWindshield.Opacity=0;
        PassengerView.Opacity=0;
        HasPilot=false;
        HasPassenger=false;
        if(settings.Door is not null)
            _=SetDoorOpenAsync(false,TimeSpan.Zero);
    }

    public Task FlyToAsync(double x, double y, TimeSpan duration, CancellationToken token = default)
    {
        return PartAnimator.AnimateToPoseAsync(new VisualPartPose(new Dictionary<string, double>(),
            new Dictionary<string, Point> { { _settings.Body.Name, new Point(x, y) } }), duration, token);
    }

    public Task SpinPropellersAsync(double revolutions,TimeSpan duration,CancellationToken token=default)
    {
        var angles = new Dictionary<string,double>(StringComparer.OrdinalIgnoreCase);
        AddAngle(angles, _settings.LeftPropeller, revolutions * 360);
        AddAngle(angles, _settings.RightPropeller, revolutions * 360);
        if (angles.Count == 0)
            AddAngle(angles, _settings.Propeller, revolutions * 360);
        return angles.Count == 0 ? Task.CompletedTask : PartAnimator.AnimateToPoseAsync(
            new VisualPartPose(angles), duration, token);
    }

    public Task SpinPropellerAsync(double revolutions, TimeSpan duration, CancellationToken token = default)
    {
        return SpinPropellersAsync(revolutions, duration, token);
    }

    public Task SpinLeftPropellerAsync(double revolutions, TimeSpan duration, CancellationToken token = default)
    {
        return SpinOneAsync(_settings.LeftPropeller ?? _settings.Propeller, revolutions, duration, token);
    }

    public Task SpinRightPropellerAsync(double revolutions, TimeSpan duration, CancellationToken token = default)
    {
        return SpinOneAsync(_settings.RightPropeller ?? _settings.Propeller, revolutions, duration, token);
    }

    public Task SetPilotAsync(PersonUC person,CancellationToken token=default)
    {
        ArgumentNullException.ThrowIfNull(person);
        PilotView.ApplySettings(person.AvatarSettings);
        PilotView.CurrentActivity=PersonActivity.Idle;
        HasPilot=true;
        PilotWindshield.Opacity=1;
        StartEngine(_settings.RightPropeller??_settings.Propeller,RightSmoke,ref _rightEngineCancellation);
        return Task.CompletedTask;
    }

    public async Task SetPassengerAsync(PersonUC person,CancellationToken token=default)
    {
        ArgumentNullException.ThrowIfNull(person);
        PassengerView.ApplySettings(person.AvatarSettings);
        PassengerView.CurrentActivity=PersonActivity.Idle;
        PassengerView.Opacity=1;
        HasPassenger=true;
        await SetDoorOpenAsync(false,TimeSpan.FromMilliseconds(450),token);
        StartEngine(_settings.LeftPropeller??_settings.Propeller,LeftSmoke,ref _leftEngineCancellation);
    }

    public async Task StartEnginesAsync(CancellationToken token=default)
    {
        StartEngine(_settings.LeftPropeller??_settings.Propeller,LeftSmoke,ref _leftEngineCancellation);
        StartEngine(_settings.RightPropeller??_settings.Propeller,RightSmoke,ref _rightEngineCancellation);
        await Task.CompletedTask;
    }

    public async Task StartTaxiAsync(double distance=180,TimeSpan? duration=null,CancellationToken token=default)
    {
        if (PassengerDropCompleted != null)
            PassengerDropCompleted.Invoke(this, EventArgs.Empty);
        //TaxiStarted?.Invoke(this,EventArgs.Empty);
        //await StartEnginesAsync(token);
        //var move=new DoubleAnimation
        //{
        //    By=distance,Duration=new Duration(duration??TimeSpan.FromSeconds(3)),
        //    FillBehavior=FillBehavior.HoldEnd
        //};
        //var completed=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        //move.Completed+=(_,_)=>completed.TrySetResult();
        //BeginAnimation(System.Windows.Controls.Canvas.LeftProperty,move,HandoffBehavior.SnapshotAndReplace);
        //using(token.Register(()=>completed.TrySetCanceled(token)))await completed.Task;
        //LeftTarmac?.Invoke(this,EventArgs.Empty);
    }

    public bool IsDoorOpen { get; private set; }

    public async Task SetDoorOpenAsync(bool open,TimeSpan duration,CancellationToken token=default)
    {
        if (_settings.Door is null)
        {
            IsDoorOpen = false;
            return;
        }
        // A side-view aircraft door cannot be represented by rotating its bitmap in the
        // image plane: that swings the entire door outside the fuselage. Retract it in
        // place so the doorway already cut out of the body becomes visible.
        PartAnimator.SetPartAngle(_settings.Door.Name, _settings.Door.InitialAngle);
        await FadeGearImageAsync(DoorImage, open ? 0d : 1d, duration, token);
        IsDoorOpen = open;
    }

    public Task SetLandingGearAsync(bool lowered,TimeSpan duration,CancellationToken token=default)
    {
        var images=new List<Image>();
        if(_settings.LandingGear is not null||_settings.LandingGearParts.Count>0)
            images.Add(LandingGearImage);
        images.AddRange(_additionalLandingGearImages);
        return Task.WhenAll(images.Select(image=>FadeGearImageAsync(
            image,lowered?1d:0d,duration,token)));
    }

    private static async Task FadeGearImageAsync(
        Image image,double opacity,TimeSpan duration,CancellationToken token)
    {
        image.Visibility=Visibility.Visible;
        if(duration<=TimeSpan.Zero)
        {
            image.BeginAnimation(OpacityProperty,null);
            image.Opacity=opacity;
            return;
        }
        var completion=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var animation=new DoubleAnimation(image.Opacity,opacity,new Duration(duration))
        {FillBehavior=FillBehavior.HoldEnd};
        animation.Completed+=(_,_)=>completion.TrySetResult();
        image.BeginAnimation(OpacityProperty,animation,HandoffBehavior.SnapshotAndReplace);
        using(token.Register(()=>completion.TrySetCanceled(token)))await completion.Task;
    }

    public void Reset()
    {
        PartAnimator.ResetParts();
        LandingGearImage.BeginAnimation(OpacityProperty,null);
        LandingGearImage.Opacity=1;
        foreach(Image image in _additionalLandingGearImages)
        {
            image.BeginAnimation(OpacityProperty,null);
            image.Opacity=1;
        }
    }
    private void Configure(Image image,VisualPartConfiguration part) { image.Visibility=Visibility.Visible; PartAnimator.ConfigurePart(image,part); }
    private void ConfigureOptional(Image image,VisualPartConfiguration? part)
    { image.Visibility=part is null?Visibility.Collapsed:Visibility.Visible; if(part is not null)PartAnimator.ConfigurePart(image,part); }
    private static void AddAngle(IDictionary<string,double> angles,VisualPartConfiguration? part,double angle)
    { if(part is not null)angles[part.Name]=angle; }
    private Task SpinOneAsync(VisualPartConfiguration? part, double revolutions, TimeSpan duration, CancellationToken token)
    {
        return part is null ? Task.CompletedTask : PartAnimator.AnimateToPoseAsync(
                new VisualPartPose(new Dictionary<string, double> { { part.Name, revolutions * 360 } }), duration, token);
    }

    private void StartEngine(VisualPartConfiguration? part,System.Windows.Shapes.Ellipse smoke,ref CancellationTokenSource? cancellation)
    {
        if(part is null||cancellation is not null)return;
        cancellation=new CancellationTokenSource();
        SetEngineSmoke(smoke,.24,true);
        _=SpinEngineContinuouslyAsync(part,cancellation.Token);
    }
    private async Task SpinEngineContinuouslyAsync(VisualPartConfiguration part,CancellationToken token)
    {
        try
        {
            while(!token.IsCancellationRequested)
            {
                double target=PartAnimator.GetPartAngle(part.Name)+360;
                await PartAnimator.AnimateToPoseAsync(
                    new VisualPartPose(new Dictionary<string,double>{{part.Name,target}}),
                    TimeSpan.FromMilliseconds(650),token,useEasing:false);
            }
        }
        catch(OperationCanceledException) when(token.IsCancellationRequested){ }
    }
    private static void SetEngineSmoke(System.Windows.Shapes.Ellipse smoke,double opacity,bool flowing)
    {
        smoke.Opacity=opacity;
        var transform=smoke.RenderTransform as TranslateTransform??new TranslateTransform();
        smoke.RenderTransform=transform;
        transform.BeginAnimation(TranslateTransform.XProperty,new DoubleAnimation
        {
            From=0,To=flowing?70:12,Duration=TimeSpan.FromSeconds(flowing?1.1:1.8),
            AutoReverse=!flowing,RepeatBehavior=RepeatBehavior.Forever
        });
    }
    private static IReadOnlyList<VisualPartConfiguration> GetLandingGearParts(AirplaneSettings settings)
    {
        var parts = new List<VisualPartConfiguration>();
        if (settings.LandingGear is not null)
            parts.Add(settings.LandingGear);
        foreach (VisualPartConfiguration part in settings.LandingGearParts)
            if (!parts.Any(existing => string.Equals(existing.Name, part.Name, StringComparison.OrdinalIgnoreCase)))
                parts.Add(part);
        return parts;
    }
    private static AirplaneSettings NormalizeSettings(AirplaneSettings settings)
    {
        var parts=new List<VisualPartConfiguration?>
        {settings.Body,settings.LeftWing,settings.RightWing,settings.LeftPropeller,settings.RightPropeller,settings.Propeller,settings.Door,settings.LandingGear};
        parts.AddRange(settings.LandingGearParts);
        var visible=parts.OfType<VisualPartConfiguration>().Where(part=>part.Width>0&&part.Height>0).ToList();
        if(visible.Count==0)return settings;
        double minX=visible.Min(part=>part.X),minY=visible.Min(part=>part.Y);
        double maxX=visible.Max(part=>part.X+part.Width),maxY=visible.Max(part=>part.Y+part.Height);
        double scale=Math.Min(310/Math.Max(1,maxX-minX),170/Math.Max(1,maxY-minY));
        double offsetX=(320-(maxX-minX)*scale)/2,offsetY=(180-(maxY-minY)*scale)/2;
        VisualPartConfiguration? Part(VisualPartConfiguration? part)
        {
            return part is null ? null : new VisualPartConfiguration
            {
                Name = part.Name,
                ImagePath = part.ImagePath,
                X = offsetX + (part.X - minX) * scale,
                Y = offsetY + (part.Y - minY) * scale,
                Width = part.Width * scale,
                Height = part.Height * scale,
                PivotX = part.PivotX,
                PivotY = part.PivotY,
                InitialAngle = part.InitialAngle,
                ZOrder = part.ZOrder,
                ParentPartName = part.ParentPartName
            };
        }

        return new AirplaneSettings
        {
            Body=Part(settings.Body)!,LeftWing=Part(settings.LeftWing),RightWing=Part(settings.RightWing),
            LeftPropeller=Part(settings.LeftPropeller),RightPropeller=Part(settings.RightPropeller),Propeller=Part(settings.Propeller),
            Door=Part(settings.Door),LandingGear=Part(settings.LandingGear),
            LandingGearParts=settings.LandingGearParts.Select(part=>Part(part)!).ToList(),
            DoorOpenAngle=settings.DoorOpenAngle,LandingGearRetractedAngle=settings.LandingGearRetractedAngle
        };
    }
    private static double Normalize(double value)
    {
        return double.IsNaN(value) ? 0 : value;
    }
}
