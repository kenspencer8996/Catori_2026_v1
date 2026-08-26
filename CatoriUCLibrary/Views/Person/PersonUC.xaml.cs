using CatoriApp.Core.Objects.DragDrop;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CatoriUCLibrary.Views.Person;

public enum PersonPartType
{
    Body, Head, LeftUpperArm, LeftForearm, RightUpperArm, RightForearm,
    LeftThigh, LeftLowerLeg, RightThigh, RightLowerLeg
}

public sealed class PersonPartSelectedEventArgs(PersonPartType part, double angle) : EventArgs
{
    public PersonPartType Part { get; } = part;
    public double Angle { get; } = angle;
}

public partial class PersonUC : UserControl, IDraggable, ICanvasDragAnchor, IDragStartFilter
{
    private PersonAvatarSettings _settings = PersonAvatarSettings.CreateDefault();
    private CancellationTokenSource? _activityCancellation;
    private bool _isArmDragging;
    private bool _isArmEditingEnabled = true;
    private bool _isPartDragging;
    private double _partDragOffset;
    private PersonPartType _selectedPart = PersonPartType.Body;

    public PersonUC()
    {
        InitializeComponent();
        Loaded += (_, _) => ApplySettings(_settings);
    }

    public event EventHandler? DragCompleted;
    public event EventHandler? ArmMoved;
    public event EventHandler<PersonArmInteractionEventArgs>? ArmInteractionCompleted;
    public event EventHandler<PersonActivityCompletedEventArgs>? ActivityCompleted;
    public event EventHandler<PersonPartSelectedEventArgs>? PartSelected;
    public event EventHandler<PersonPartSelectedEventArgs>? PartMoved;

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
                static (d, e) => ((PersonUC)d).ApplyRightShoulderAngle((double)e.NewValue)));

    public bool IsDragEnabled { get; set; } = true;
    public bool IsPartEditingEnabled { get; set; }
    public PersonPartType SelectedPart => _selectedPart;
    public double SelectedPartAngle => GetPartAngle(_selectedPart);
    public double LeftElbowAngle
    {
        get => GetJointAngle(LeftForearmImage);
        set => SetLowerJointAngle(LeftForearmImage, value);
    }
    public double RightElbowAngle
    {
        get => GetJointAngle(RightForearmImage);
        set => SetLowerJointAngle(RightForearmImage, value);
    }
    public double LeftKneeAngle
    {
        get => GetJointAngle(LeftLowerLegImage);
        set => SetLowerJointAngle(LeftLowerLegImage, value);
    }
    public double RightKneeAngle
    {
        get => GetJointAngle(RightLowerLegImage);
        set => SetLowerJointAngle(RightLowerLegImage, value);
    }
    public double LeftShoulderAngle
    {
        get => GetJointAngle(LeftArmRoot) - _settings.LeftArm.InitialAngle;
        set => SetJointAngle(LeftArmRoot, _settings.LeftArm.InitialAngle + value);
    }
    public double LeftHipAngle
    {
        get => GetJointAngle(LeftLegRoot) - _settings.LeftLeg.InitialAngle;
        set => SetJointAngle(LeftLegRoot, _settings.LeftLeg.InitialAngle + value);
    }
    public double RightHipAngle
    {
        get => GetJointAngle(RightLegRoot) - _settings.RightLeg.InitialAngle;
        set => SetJointAngle(RightLegRoot, _settings.RightLeg.InitialAngle + value);
    }
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
            double shoulderAngle=(_settings.RightArm.InitialAngle+ArmAngle)*Math.PI/180d;
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
        ApplyRightShoulderAngle(ArmAngle);
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
        if (!interactiveUpper)
        {
            root.RenderTransform = new System.Windows.Media.RotateTransform(
                upperPart.InitialAngle,
                upperPart.PivotX * upper.Width,
                upperPart.PivotY * upper.Height
            );
        }
        else if (root.RenderTransform is System.Windows.Media.RotateTransform armRotate)
        {
            armRotate.CenterX = upperPart.PivotX * upper.Width;
            armRotate.CenterY = upperPart.PivotY * upper.Height;
        }
        lower.Source=LoadImage(lowerPart.ImagePath);lower.Width=lowerPart.Width;lower.Height=lowerPart.Height;
        Point upperPivot = new(upperPart.PivotX * upper.Width, upperPart.PivotY * upper.Height);
        Point lowerPosition;
        if (upperPart.HasEndJoint)
        {
            Point elbow = new(upperPart.EndJointX * upper.Width, upperPart.EndJointY * upper.Height);
            lowerPosition = new Point(
                elbow.X - lowerPart.PivotX * lower.Width,
                elbow.Y - lowerPart.PivotY * lower.Height
            );
        }
        else
        {
            lowerPosition = OffsetPosition(lowerPart, lower.Width, lower.Height, upperPivot);
        }
        Canvas.SetLeft(lower,lowerPosition.X);Canvas.SetTop(lower,lowerPosition.Y);Panel.SetZIndex(lower,lowerPart.ZIndex);
        lower.RenderTransformOrigin = new Point(lowerPart.PivotX, lowerPart.PivotY);
        lower.RenderTransform = new System.Windows.Media.RotateTransform(
            Math.Max(0, lowerPart.InitialAngle)
        );
    }

    public void SetJointAngles(double leftElbow,double rightElbow,double leftKnee,double rightKnee)
    {
        LeftElbowAngle = leftElbow;
        RightElbowAngle = rightElbow;
        LeftKneeAngle = leftKnee;
        RightKneeAngle = rightKnee;
    }

    private static double GetJointAngle(FrameworkElement element)=>element.RenderTransform is System.Windows.Media.RotateTransform rotate?rotate.Angle:0;
    private static void SetJointAngle(FrameworkElement element,double angle)
    {
        if(element.RenderTransform is System.Windows.Media.RotateTransform rotate)rotate.Angle=angle;
        else element.RenderTransform=new System.Windows.Media.RotateTransform(angle);
    }

    private static void SetLowerJointAngle(FrameworkElement element, double angle)
    {
        SetJointAngle(element, Math.Max(0, angle));
    }

    private void ApplyRightShoulderAngle(double animationAngle)
    {
        ArmRotate.Angle = _settings.RightArm.InitialAngle + animationAngle;
    }

    public void SelectPart(PersonPartType part)
    {
        _selectedPart = part;
        PartSelected?.Invoke(this, new PersonPartSelectedEventArgs(part, GetPartAngle(part)));
    }

    private void PartMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (!IsPartEditingEnabled || sender is not Image { Tag: string tag }
            || !Enum.TryParse(tag, out PersonPartType part))
            return;

        SelectPart(part);
        Point pivot = GetPartPivot(part);
        double mouseAngle = AngleFromPivot(pivot, e.GetPosition(PersonRoot));
        _partDragOffset = GetPartWorldAngle(part) - mouseAngle;
        _isPartDragging = true;
        PersonRoot.CaptureMouse();
        e.Handled = true;
    }

    private void PartEditorMouseMove(object sender, MouseEventArgs e)
    {
        if (!_isPartDragging || e.LeftButton != MouseButtonState.Pressed)
            return;

        double worldAngle = AngleFromPivot(GetPartPivot(_selectedPart), e.GetPosition(PersonRoot))
            + _partDragOffset;
        SetPartAngle(_selectedPart, NormalizePartAngle(worldAngle - GetParentWorldAngle(_selectedPart)));
        PartMoved?.Invoke(this, new PersonPartSelectedEventArgs(_selectedPart, GetPartAngle(_selectedPart)));
        e.Handled = true;
    }

    private void PartEditorMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (!_isPartDragging)
            return;
        _isPartDragging = false;
        PersonRoot.ReleaseMouseCapture();
        PartMoved?.Invoke(this, new PersonPartSelectedEventArgs(_selectedPart, GetPartAngle(_selectedPart)));
        e.Handled = true;
    }

    private FrameworkElement GetPartElement(PersonPartType part) => part switch
    {
        PersonPartType.Body => BodyImage,
        PersonPartType.Head => HeadImage,
        PersonPartType.LeftUpperArm => LeftArmRoot,
        PersonPartType.LeftForearm => LeftForearmImage,
        PersonPartType.RightUpperArm => RightArmRoot,
        PersonPartType.RightForearm => RightForearmImage,
        PersonPartType.LeftThigh => LeftLegRoot,
        PersonPartType.LeftLowerLeg => LeftLowerLegImage,
        PersonPartType.RightThigh => RightLegRoot,
        _ => RightLowerLegImage,
    };

    private AvatarPartSettings GetPartSettings(PersonPartType part) => part switch
    {
        PersonPartType.Body => _settings.Body,
        PersonPartType.Head => _settings.Head,
        PersonPartType.LeftUpperArm => _settings.LeftArm,
        PersonPartType.LeftForearm => _settings.LeftForearm,
        PersonPartType.RightUpperArm => _settings.RightArm,
        PersonPartType.RightForearm => _settings.RightForearm,
        PersonPartType.LeftThigh => _settings.LeftLeg,
        PersonPartType.LeftLowerLeg => _settings.LeftLowerLeg,
        PersonPartType.RightThigh => _settings.RightLeg,
        _ => _settings.RightLowerLeg,
    };

    private double GetPartAngle(PersonPartType part) => GetJointAngle(GetPartElement(part));

    private void SetPartAngle(PersonPartType part, double angle)
    {
        if (part == PersonPartType.RightUpperArm)
            ArmAngle = angle - _settings.RightArm.InitialAngle;
        else if (part is PersonPartType.LeftForearm
            or PersonPartType.RightForearm
            or PersonPartType.LeftLowerLeg
            or PersonPartType.RightLowerLeg)
            SetLowerJointAngle(GetPartElement(part), angle);
        else
            SetJointAngle(GetPartElement(part), angle);
    }

    private double GetPartWorldAngle(PersonPartType part) =>
        GetParentWorldAngle(part) + GetPartAngle(part);

    private double GetParentWorldAngle(PersonPartType part) => part switch
    {
        PersonPartType.LeftForearm => GetPartAngle(PersonPartType.LeftUpperArm),
        PersonPartType.RightForearm => GetPartAngle(PersonPartType.RightUpperArm),
        PersonPartType.LeftLowerLeg => GetPartAngle(PersonPartType.LeftThigh),
        PersonPartType.RightLowerLeg => GetPartAngle(PersonPartType.RightThigh),
        _ => 0,
    };

    private Point GetPartPivot(PersonPartType part)
    {
        FrameworkElement element = GetPartElement(part);
        AvatarPartSettings settings = GetPartSettings(part);
        Point local = new(settings.PivotX * element.ActualWidth, settings.PivotY * element.ActualHeight);
        return element.TranslatePoint(local, PersonRoot);
    }

    private static double AngleFromPivot(Point pivot, Point point) =>
        Math.Atan2(point.Y - pivot.Y, point.X - pivot.X) * 180 / Math.PI;

    private static double NormalizePartAngle(double angle)
    {
        angle %= 360;
        if (angle > 180) angle -= 360;
        if (angle < -180) angle += 360;
        return angle;
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
        bool usesComposedParts = !string.IsNullOrWhiteSpace(_settings.Body.ImagePath);
        if (usesComposedParts && activity == PersonActivity.Walk)
        {
            try
            {
                await PlayComposedWalkAsync(token);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
            }
            return;
        }
        if (frames.Count == 0 || usesComposedParts)
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

    private async Task PlayComposedWalkAsync(CancellationToken cancellationToken)
    {
        double phase = -1;
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            phase = -phase;
            await MoveWalkPoseToAsync(
                phase,
                TimeSpan.FromMilliseconds(220),
                cancellationToken
            );
            await Task.Delay(60, cancellationToken);
        }
    }

    public async Task MoveWalkPoseToAsync(
        double targetPhase,
        TimeSpan duration,
        CancellationToken cancellationToken = default
    )
    {
        targetPhase = Math.Clamp(targetPhase, -1, 1);
        double startLeftHip = LeftHipAngle;
        double startRightHip = RightHipAngle;
        double startLeftShoulder = LeftShoulderAngle;
        double startRightShoulder = ArmAngle;
        double startLeftElbow = LeftElbowAngle;
        double startRightElbow = RightElbowAngle;
        double startLeftKnee = LeftKneeAngle;
        double startRightKnee = RightKneeAngle;
        double targetLeftHip = 24 * targetPhase;
        double targetRightHip = -24 * targetPhase;
        double targetLeftShoulder = 18 * targetPhase;
        double targetRightShoulder = -18 * targetPhase;
        double targetLeftElbow = targetPhase > 0 ? 5 : 18;
        double targetRightElbow = targetPhase > 0 ? 18 : 5;
        double targetLeftKnee = targetPhase > 0 ? 28 : 5;
        double targetRightKnee = targetPhase > 0 ? 5 : 28;
        Stopwatch stopwatch = Stopwatch.StartNew();
        while (stopwatch.Elapsed < duration)
        {
            cancellationToken.ThrowIfCancellationRequested();
            double amount = duration <= TimeSpan.Zero
                ? 1
                : Math.Clamp(
                    stopwatch.Elapsed.TotalMilliseconds / duration.TotalMilliseconds,
                    0,
                    1
                );
            amount = amount < .5
                ? 2 * amount * amount
                : 1 - Math.Pow(-2 * amount + 2, 2) / 2;
            LeftHipAngle = Interpolate(startLeftHip, targetLeftHip, amount);
            RightHipAngle = Interpolate(startRightHip, targetRightHip, amount);
            LeftShoulderAngle = Interpolate(startLeftShoulder, targetLeftShoulder, amount);
            ArmAngle = Interpolate(startRightShoulder, targetRightShoulder, amount);
            LeftElbowAngle = Interpolate(startLeftElbow, targetLeftElbow, amount);
            RightElbowAngle = Interpolate(startRightElbow, targetRightElbow, amount);
            LeftKneeAngle = Interpolate(startLeftKnee, targetLeftKnee, amount);
            RightKneeAngle = Interpolate(startRightKnee, targetRightKnee, amount);
            await Task.Delay(16, cancellationToken);
        }
        LeftHipAngle = targetLeftHip;
        RightHipAngle = targetRightHip;
        LeftShoulderAngle = targetLeftShoulder;
        ArmAngle = targetRightShoulder;
        LeftElbowAngle = targetLeftElbow;
        RightElbowAngle = targetRightElbow;
        LeftKneeAngle = targetLeftKnee;
        RightKneeAngle = targetRightKnee;
        ArmMoved?.Invoke(this, EventArgs.Empty);
    }

    private static double Interpolate(double start, double end, double amount)
    {
        return start + (end - start) * amount;
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

    public async Task MoveShouldersToAsync(
        double leftTargetAngle,
        double rightTargetAngle,
        TimeSpan duration,
        CancellationToken cancellationToken = default
    )
    {
        double leftStartAngle = LeftShoulderAngle;
        double rightStartAngle = ArmAngle;
        if (duration <= TimeSpan.Zero)
        {
            LeftShoulderAngle = leftTargetAngle;
            ArmAngle = rightTargetAngle;
            ArmMoved?.Invoke(this, EventArgs.Empty);
            return;
        }
        Stopwatch stopwatch = Stopwatch.StartNew();
        while (stopwatch.Elapsed < duration)
        {
            cancellationToken.ThrowIfCancellationRequested();
            double amount = Math.Clamp(
                stopwatch.Elapsed.TotalMilliseconds / duration.TotalMilliseconds,
                0,
                1
            );
            amount = amount < .5
                ? 2 * amount * amount
                : 1 - Math.Pow(-2 * amount + 2, 2) / 2;
            LeftShoulderAngle = leftStartAngle
                + (leftTargetAngle - leftStartAngle) * amount;
            ArmAngle = rightStartAngle
                + (rightTargetAngle - rightStartAngle) * amount;
            await Task.Delay(16, cancellationToken);
        }
        LeftShoulderAngle = leftTargetAngle;
        ArmAngle = rightTargetAngle;
        ArmMoved?.Invoke(this, EventArgs.Empty);
    }

    public void PointArmAt(Point pointInPersonCoordinates)
    {
        ArmAngle = Math.Atan2(pointInPersonCoordinates.Y - _settings.ShoulderY,
            pointInPersonCoordinates.X - _settings.ShoulderX) * 180d / Math.PI
            - _settings.RightArm.InitialAngle;
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
        if (
            setFirstFrame
            && animation.FrameImagePaths.Count > 0
            && string.IsNullOrWhiteSpace(_settings.Body.ImagePath)
        )
        {
            BodyImage.Source = LoadImage(animation.FrameImagePaths[0]);
        }
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
