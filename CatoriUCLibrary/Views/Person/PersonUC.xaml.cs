using CatoriApp.Core.Objects.DragDrop;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CatoriUCLibrary.Views.Person;

public partial class PersonUC : UserControl, IDraggable, ICanvasDragAnchor, IDragStartFilter
{
    private PersonAvatarSettings _settings = PersonAvatarSettings.CreateDefault();
    private CancellationTokenSource? _activityCancellation;
    private bool _isArmDragging;
    private bool _isArmEditingEnabled = true;

    public PersonUC()
    {
        InitializeComponent();
        Loaded += (_, _) => ApplySettings(_settings);
    }

    public event EventHandler? DragCompleted;
    public event EventHandler? ArmMoved;
    public event EventHandler<PersonArmInteractionEventArgs>? ArmInteractionCompleted;
    public event EventHandler<PersonActivityCompletedEventArgs>? ActivityCompleted;

    public PersonAvatarSettings AvatarSettings
    {
        get => _settings;
        set => ApplySettings(value ?? throw new ArgumentNullException(nameof(value)));
    }

    public string? AvatarSettingsJson
    {
        get => (string?)GetValue(AvatarSettingsJsonProperty);
        set => SetValue(AvatarSettingsJsonProperty, value);
    }

    public static readonly DependencyProperty AvatarSettingsJsonProperty =
        DependencyProperty.Register(nameof(AvatarSettingsJson), typeof(string), typeof(PersonUC),
            new FrameworkPropertyMetadata(null, OnAvatarSettingsJsonChanged));

    public PersonActivity CurrentActivity
    {
        get => (PersonActivity)GetValue(CurrentActivityProperty);
        set => SetValue(CurrentActivityProperty, value);
    }

    public static readonly DependencyProperty CurrentActivityProperty =
        DependencyProperty.Register(nameof(CurrentActivity), typeof(PersonActivity), typeof(PersonUC),
            new FrameworkPropertyMetadata(PersonActivity.Idle, OnCurrentActivityChanged));

    public double ArmAngle
    {
        get => (double)GetValue(ArmAngleProperty);
        set => SetValue(ArmAngleProperty, value);
    }

    public static readonly DependencyProperty ArmAngleProperty =
        DependencyProperty.Register(nameof(ArmAngle), typeof(double), typeof(PersonUC),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                static (d, e) => ((PersonUC)d).ArmRotate.Angle = (double)e.NewValue));

    public bool IsDragEnabled { get; set; } = true;
    public double LeftElbowAngle { get=>GetJointAngle(LeftForearmImage);set=>SetJointAngle(LeftForearmImage,value); }
    public double RightElbowAngle { get=>GetJointAngle(RightForearmImage);set=>SetJointAngle(RightForearmImage,value); }
    public double LeftKneeAngle { get=>GetJointAngle(LeftLowerLegImage);set=>SetJointAngle(LeftLowerLegImage,value); }
    public double RightKneeAngle { get=>GetJointAngle(RightLowerLegImage);set=>SetJointAngle(RightLowerLegImage,value); }
    public bool IsArmEditingEnabled
    {
        get => _isArmEditingEnabled;
        set
        {
            _isArmEditingEnabled = value;
            ApplyActivity(CurrentActivity, setFirstFrame: false);
        }
    }
    public UIElement Visual => this;
    public double DragAnchorX => _settings.DesignWidth / 2;
    public double DragAnchorY => _settings.DesignHeight;

    public Point OriginalPosition => new(
        NormalizeCanvasCoordinate(Canvas.GetLeft(this)),
        NormalizeCanvasCoordinate(Canvas.GetTop(this)));

    public Point HandPoint
    {
        get
        {
            double shoulderAngle=ArmAngle*Math.PI/180d;
            double elbowX=_settings.ShoulderX+Math.Cos(shoulderAngle)*_settings.RightForearm.X;
            double elbowY=_settings.ShoulderY+Math.Sin(shoulderAngle)*_settings.RightForearm.X;
            double forearmAngle=shoulderAngle+_settings.RightForearm.InitialAngle*Math.PI/180d;
            return new(elbowX+Math.Cos(forearmAngle)*_settings.RightForearm.Width,elbowY+Math.Sin(forearmAngle)*_settings.RightForearm.Width);
        }
    }

    public void ApplySettings(PersonAvatarSettings settings)
    {
        _settings = settings;
        PersonRoot.Width = settings.DesignWidth;
        PersonRoot.Height = settings.DesignHeight;
        Point avatarCenter = new(settings.DesignWidth / 2, settings.DesignHeight / 2);
        ApplyPart(BodyImage, settings.Body, avatarCenter, settings.DesignWidth, settings.DesignHeight);
        ApplyPart(HeadImage, settings.Head, avatarCenter);
        ApplyLimb(LeftArmRoot,LeftArmImage,LeftForearmImage,settings.LeftArm,settings.LeftForearm,avatarCenter);
        ApplyLimb(LeftLegRoot,LeftLegImage,LeftLowerLegImage,settings.LeftLeg,settings.LeftLowerLeg,avatarCenter);
        ApplyLimb(RightLegRoot,RightLegImage,RightLowerLegImage,settings.RightLeg,settings.RightLowerLeg,avatarCenter);
        AvatarPartSettings right=settings.RightArm;
        if(string.IsNullOrWhiteSpace(right.ImagePath))right.ImagePath=settings.ArmImagePath;
        ApplyLimb(RightArmRoot,ArmImage,RightForearmImage,right,settings.RightForearm,avatarCenter,settings.ArmWidth,settings.ArmHeight,true);
        settings.ShoulderX=right.X;settings.ShoulderY=right.Y;
        settings.ArmWidth=right.Width;settings.ArmHeight=right.Height;
        ApplyActivity(CurrentActivity, setFirstFrame: true);
    }

    private static void ApplyLimb(Canvas root,Image upper,Image lower,AvatarPartSettings upperPart,AvatarPartSettings lowerPart,Point avatarCenter,double? fallbackWidth=null,double? fallbackHeight=null,bool interactiveUpper=false)
    {
        upper.Source=LoadImage(upperPart.ImagePath);upper.Width=upperPart.Width>0?upperPart.Width:fallbackWidth??100;upper.Height=upperPart.Height>0?upperPart.Height:fallbackHeight??100;
        Point rootPosition = OffsetPosition(upperPart, upper.Width, upper.Height, avatarCenter);
        Canvas.SetLeft(root,rootPosition.X);Canvas.SetTop(root,rootPosition.Y);Panel.SetZIndex(root,upperPart.ZIndex);
        Canvas.SetLeft(upper,0);Canvas.SetTop(upper,0);upper.RenderTransformOrigin=new Point(upperPart.PivotX,upperPart.PivotY);
        if(!interactiveUpper)root.RenderTransform=new System.Windows.Media.RotateTransform(upperPart.InitialAngle,upperPart.PivotX*upper.Width,upperPart.PivotY*upper.Height);
        lower.Source=LoadImage(lowerPart.ImagePath);lower.Width=lowerPart.Width;lower.Height=lowerPart.Height;
        Point upperPivot = new(upperPart.PivotX * upper.Width, upperPart.PivotY * upper.Height);
        Point lowerPosition = OffsetPosition(lowerPart, lower.Width, lower.Height, upperPivot);
        Canvas.SetLeft(lower,lowerPosition.X);Canvas.SetTop(lower,lowerPosition.Y);Panel.SetZIndex(lower,lowerPart.ZIndex);
        lower.RenderTransformOrigin=new Point(lowerPart.PivotX,lowerPart.PivotY);lower.RenderTransform=new System.Windows.Media.RotateTransform(lowerPart.InitialAngle);
    }

    public void SetJointAngles(double leftElbow,double rightElbow,double leftKnee,double rightKnee)
    {
        LeftElbowAngle=leftElbow;RightElbowAngle=rightElbow;LeftKneeAngle=leftKnee;RightKneeAngle=rightKnee;
    }

    private static double GetJointAngle(Image image)=>image.RenderTransform is System.Windows.Media.RotateTransform rotate?rotate.Angle:0;
    private static void SetJointAngle(Image image,double angle)
    {
        if(image.RenderTransform is System.Windows.Media.RotateTransform rotate)rotate.Angle=angle;
        else image.RenderTransform=new System.Windows.Media.RotateTransform(angle);
    }

    private static void ApplyPart(Image image,AvatarPartSettings part,Point avatarCenter,double? fallbackWidth=null,double? fallbackHeight=null)
    {
        image.Source=LoadImage(part.ImagePath);
        image.Width=part.Width>0?part.Width:fallbackWidth??100;
        image.Height=part.Height>0?part.Height:fallbackHeight??100;
        Point position = OffsetPosition(part, image.Width, image.Height, avatarCenter);
        Canvas.SetLeft(image,position.X);Canvas.SetTop(image,position.Y);
        Panel.SetZIndex(image,part.ZIndex);
        image.RenderTransformOrigin=new Point(part.PivotX,part.PivotY);
        if(image.Name!="ArmImage")image.RenderTransform=new System.Windows.Media.RotateTransform(part.InitialAngle);
    }

    private static Point OffsetPosition(AvatarPartSettings part, double width, double height, Point origin)
    {
        if (Math.Abs(part.Offset) < 0.0001)
        {
            return new Point(part.X, part.Y);
        }
        double anchorX = part.X + part.PivotX * width;
        double anchorY = part.Y + part.PivotY * height;
        double directionX = anchorX - origin.X;
        double directionY = anchorY - origin.Y;
        double distance = Math.Sqrt(directionX * directionX + directionY * directionY);
        if (distance < 0.0001)
        {
            return new Point(part.X, part.Y);
        }
        return new Point(
            part.X + directionX / distance * part.Offset,
            part.Y + directionY / distance * part.Offset
        );
    }

    public async Task PlayActivityAsync(PersonActivity activity, CancellationToken cancellationToken = default)
    {
        _activityCancellation?.Cancel();
        _activityCancellation?.Dispose();
        _activityCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        CancellationToken token = _activityCancellation.Token;

        CurrentActivity = activity;
        if (!_settings.Activities.TryGetValue(activity, out PersonActivitySettings? animation))
            return;

        IReadOnlyList<string> frames = animation.FrameImagePaths;
        if (frames.Count == 0)
        {
            ActivityCompleted?.Invoke(this, new PersonActivityCompletedEventArgs(activity));
            return;
        }

        try
        {
            do
            {
                foreach (string frame in frames)
                {
                    token.ThrowIfCancellationRequested();
                    BodyImage.Source = LoadImage(frame);
                    await Task.Delay(Math.Max(16, animation.FrameDurationMilliseconds), token);
                }
            } while (animation.Loop && !token.IsCancellationRequested);

            ActivityCompleted?.Invoke(this, new PersonActivityCompletedEventArgs(activity));
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
        }
    }

    public async Task MoveArmToAsync(double targetAngle, TimeSpan duration,
        CancellationToken cancellationToken = default)
    {
        double start = ArmAngle;
        if (duration <= TimeSpan.Zero)
        {
            ArmAngle = targetAngle;
            ArmMoved?.Invoke(this, EventArgs.Empty);
            return;
        }

        Stopwatch stopwatch = Stopwatch.StartNew();
        while (stopwatch.Elapsed < duration)
        {
            cancellationToken.ThrowIfCancellationRequested();
            double amount = Math.Clamp(stopwatch.Elapsed.TotalMilliseconds / duration.TotalMilliseconds, 0, 1);
            amount = amount < .5 ? 2 * amount * amount : 1 - Math.Pow(-2 * amount + 2, 2) / 2;
            ArmAngle = start + (targetAngle - start) * amount;
            await Task.Delay(16, cancellationToken);
        }

        ArmAngle = targetAngle;
        ArmMoved?.Invoke(this, EventArgs.Empty);
    }

    public void PointArmAt(Point pointInPersonCoordinates)
    {
        ArmAngle = Math.Atan2(pointInPersonCoordinates.Y - _settings.ShoulderY,
            pointInPersonCoordinates.X - _settings.ShoulderX) * 180d / Math.PI;
        ArmMoved?.Invoke(this, EventArgs.Empty);
    }

    public void OnDragMouseup() => DragCompleted?.Invoke(this, EventArgs.Empty);

    public bool CanStartDrag(DependencyObject? originalSource)
    {
        if (!IsArmEditingEnabled)
            return true;

        DependencyObject? current = originalSource;
        while (current != null && !ReferenceEquals(current, this))
        {
            if (ReferenceEquals(current, ArmImage))
                return false;
            current = current is System.Windows.Media.Visual or System.Windows.Media.Media3D.Visual3D
                ? System.Windows.Media.VisualTreeHelper.GetParent(current)
                : LogicalTreeHelper.GetParent(current);
        }
        return true;
    }

    private static void OnAvatarSettingsJsonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not PersonUC person || e.NewValue is not string json || string.IsNullOrWhiteSpace(json))
            return;

        try
        {
            person.ApplySettings(PersonAvatarSettings.FromJson(json));
        }
        catch (Exception ex) when (ex is System.Text.Json.JsonException or ArgumentException)
        {
            Debug.WriteLine($"Person avatar settings are invalid: {ex.Message}");
        }
    }

    private static void OnCurrentActivityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((PersonUC)d).ApplyActivity((PersonActivity)e.NewValue, setFirstFrame: true);

    private void ApplyActivity(PersonActivity activity, bool setFirstFrame)
    {
        if (!_settings.Activities.TryGetValue(activity, out PersonActivitySettings? animation))
            return;

        // Editing the arm requires a visible hit target even when a complete
        // idle/walk frame normally supplies its own baked-in arms.
        ArmImage.Visibility = animation.ArmVisible || IsArmEditingEnabled
            ? Visibility.Visible
            : Visibility.Collapsed;
        ArmAngle = animation.ArmAngle;
        if (setFirstFrame && animation.FrameImagePaths.Count > 0)
            BodyImage.Source = LoadImage(animation.FrameImagePaths[0]);
    }

    private void ArmImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!IsArmEditingEnabled)
            return;

        _isArmDragging = true;
        ArmImage.CaptureMouse();
        PointArmAt(e.GetPosition(PersonRoot));
        e.Handled = true;
    }

    private void ArmImage_MouseMove(object sender, MouseEventArgs e)
    {
        if (!_isArmDragging || e.LeftButton != MouseButtonState.Pressed)
            return;

        PointArmAt(e.GetPosition(PersonRoot));
        e.Handled = true;
    }

    private void ArmImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (!_isArmDragging)
            return;

        _isArmDragging = false;
        ArmImage.ReleaseMouseCapture();
        ArmMoved?.Invoke(this, EventArgs.Empty);
        ArmInteractionCompleted?.Invoke(this, new PersonArmInteractionEventArgs(HandPoint));
        e.Handled = true;
    }

    private static BitmapImage? LoadImage(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return null;

        try
        {
            Uri uri;
            if (Path.IsPathRooted(path) || path.StartsWith("pack:", StringComparison.OrdinalIgnoreCase))
                uri = new Uri(path, UriKind.Absolute);
            else if (path.Contains('/') || path.Contains('\\'))
                uri = new Uri(path, UriKind.RelativeOrAbsolute);
            else
                uri = new Uri($"pack://application:,,,/CatoriUCLibrary;component/Images/{path}");

            BitmapImage image = new();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = uri;
            image.EndInit();
            image.Freeze();
            return image;
        }
        catch (Exception ex) when (ex is IOException or UriFormatException or NotSupportedException)
        {
            Debug.WriteLine($"Could not load person image '{path}': {ex.Message}");
            return null;
        }
    }

    private static double NormalizeCanvasCoordinate(double value) => double.IsNaN(value) ? 0 : value;
}
