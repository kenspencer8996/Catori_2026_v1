using CatoriApp.Core.Objects.DragDrop;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CatoriUCLibrary.Views.VisualParts;
using CatoriInterfaces;
using CatoriUCLibrary.Views.PersonItems;
using CatoriUCLibrary.Views.Tickets;
using CommunityToolkit.Mvvm.Messaging;

namespace CatoriUCLibrary.Views.Person;

public partial class PersonUC : UserControl, IDraggable, ICanvasDragAnchor, IDragStartFilter
{
    private PersonAvatarSettings _settings = PersonAvatarSettings.CreateDefault();
    private CancellationTokenSource? _activityCancellation;
    private bool _isArmDragging;
    private bool _isArmEditingEnabled = true;
    private bool _isPartDragging;
    private bool _isMouseDragging;
    private PersonActivity _activityBeforeMouseDrag;
    private double _lastDragX;
    private double _partDragOffset;
    private PersonPartType _selectedPart = PersonPartType.Body;
    private readonly List<ITicket> _tickets = [];
    private readonly HashSet<string> _receivedTicketKeys = [];
    private readonly List<IClothes> _clothes = [];

    public PersonUC()
    {
        InitializeComponent();
        Loaded += (_, _) =>
        {
            ApplySettings(_settings);
            RestorePurchasedTickets();
        };
        WeakReferenceMessenger.Default.Register<TicketPurchasedMessage>(this, static (recipient, message) =>
        {
            PersonUC person = (PersonUC)recipient;
            person.ReceivePurchasedTicket(message);
        });
    }

    public event EventHandler? DragCompleted;
    public event EventHandler? ArmMoved;
    public event EventHandler<PersonArmInteractionEventArgs>? ArmInteractionCompleted;
    public event EventHandler<PersonActivityCompletedEventArgs>? ActivityCompleted;
    public event EventHandler<PersonPartSelectedEventArgs>? PartSelected;
    public event EventHandler<PersonPartSelectedEventArgs>? PartMoved;
    public event EventHandler<PersonItemDroppedEventArgs>? ItemDropped;
    public event EventHandler<PersonItemDroppedEventArgs>? TicketReceived;
    public event EventHandler<PersonItemDroppedEventArgs>? ClothesEquipped;
    public event EventHandler<PersonItemDroppedEventArgs>? FoodReceived;

    public IReadOnlyList<ITicket> Tickets => _tickets;

    public IReadOnlyList<IClothes> Clothes => _clothes;

    public void ReceiveTicket(ITicket ticket)
    {
        ArgumentNullException.ThrowIfNull(ticket);
        _tickets.Add(ticket);
        if (ticket is UIElement ticketVisual)
        {
            HeldTicketContentControl.Content = ticketVisual;
            HeldTicketViewbox.Visibility = Visibility.Visible;
        }
        RaiseDroppedEvents(PersonDroppedItemType.Ticket, ticket, TicketReceived);
    }

    private void RestorePurchasedTickets()
    {
        foreach (TicketPurchasedMessage ticket in PurchasedTicketRegistry.GetAll())
        {
            ReceivePurchasedTicket(ticket);
        }
    }

    private void ReceivePurchasedTicket(TicketPurchasedMessage message)
    {
        if (!string.Equals(Name, message.PassengerName, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(ToolTip as string, message.PassengerName, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }
        System.Diagnostics.Debug.WriteLine($"Received purchased ticket: {message.Destination}, {message.Cost}, {message.PurchasedDateTime}");
        System.Diagnostics.Debug.WriteLine($"Name {Name} Current role: {Role}");
        bool isPurchasingPassenger = false;
        if (Role == PersonRole.Pilot || Name.StartsWith("Pilot") == true)
        {
            return;
        }
        else
        {
            isPurchasingPassenger = true;
        }
        if (isPurchasingPassenger)
        {
            Role = PersonRole.Passenger;
        }
        string ticketKey = $"{message.Destination}\u001f{message.Cost}\u001f{message.PurchasedDateTime.Ticks}";
        if (!_receivedTicketKeys.Add(ticketKey))
        {
            return;
        }
        ReceiveTicket(new Ticket_UC
        {
            Destination = message.Destination,
            Cost = message.Cost,
            PurchasedDateTime = message.PurchasedDateTime
        });
    }

    public VisualPartAnimatorUC Animator => PartAnimator;

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

    public PersonRole Role
    {
        get => (PersonRole)GetValue(RoleProperty);
        set => SetValue(RoleProperty, value);
    }

    public static readonly DependencyProperty RoleProperty =
        DependencyProperty.Register(nameof(Role), typeof(PersonRole), typeof(PersonUC),
            new FrameworkPropertyMetadata(PersonRole.Avatar));

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
        get => GetPartAngle(PersonPartType.LeftForearm);
        set => SetPartAngle(PersonPartType.LeftForearm, Math.Max(0, value));
    }
    public double RightElbowAngle
    {
        get => GetPartAngle(PersonPartType.RightForearm);
        set => SetPartAngle(PersonPartType.RightForearm, Math.Max(0, value));
    }
    public double LeftKneeAngle
    {
        get => GetPartAngle(PersonPartType.LeftLowerLeg);
        set => SetPartAngle(PersonPartType.LeftLowerLeg, Math.Max(0, value));
    }
    public double RightKneeAngle
    {
        get => GetPartAngle(PersonPartType.RightLowerLeg);
        set => SetPartAngle(PersonPartType.RightLowerLeg, Math.Max(0, value));
    }
    public double LeftShoulderAngle
    {
        get => GetPartAngle(PersonPartType.LeftUpperArm) - _settings.LeftArm.InitialAngle;
        set => SetPartAngle(PersonPartType.LeftUpperArm, _settings.LeftArm.InitialAngle + value);
    }
    public double LeftHipAngle
    {
        get => GetPartAngle(PersonPartType.LeftThigh) - _settings.LeftLeg.InitialAngle;
        set => SetPartAngle(PersonPartType.LeftThigh, _settings.LeftLeg.InitialAngle + value);
    }
    public double RightHipAngle
    {
        get => GetPartAngle(PersonPartType.RightThigh) - _settings.RightLeg.InitialAngle;
        set => SetPartAngle(PersonPartType.RightThigh, _settings.RightLeg.InitialAngle + value);
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
    public double DragAnchorX => PositiveRenderedSize(ActualWidth,Width,_settings.DesignWidth) / 2;
    public double DragAnchorY => PositiveRenderedSize(ActualHeight,Height,_settings.DesignHeight);

    public Point OriginalPosition => new(
        NormalizeCanvasCoordinate(Canvas.GetLeft(this)),
        NormalizeCanvasCoordinate(Canvas.GetTop(this)));

    private static double PositiveRenderedSize(double actual, double explicitSize, double fallback)
    {
        return actual > 0 ? actual : explicitSize > 0 && !double.IsNaN(explicitSize) ? explicitSize : fallback;
    }

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
        Role = Enum.TryParse(settings.PersonRole, ignoreCase: true, out PersonRole role)
            ? role
            : PersonRole.Avatar;
        PartAnimator.ClearParts();
        PersonRoot.Width = settings.DesignWidth;
        PersonRoot.Height = settings.DesignHeight;
        Point avatarCenter = new(settings.DesignWidth / 2, settings.DesignHeight / 2);
        ConfigurePart(PersonPartType.Body, BodyImage, settings.Body, avatarCenter, settings.DesignWidth, settings.DesignHeight);
        ConfigurePart(PersonPartType.Head, HeadImage, settings.Head, avatarCenter);
        ConfigureLimb(PersonPartType.LeftUpperArm, PersonPartType.LeftForearm, LeftArmRoot, LeftArmImage, LeftForearmImage, settings.LeftArm, settings.LeftForearm, avatarCenter);
        ConfigureLimb(PersonPartType.LeftThigh, PersonPartType.LeftLowerLeg, LeftLegRoot, LeftLegImage, LeftLowerLegImage, settings.LeftLeg, settings.LeftLowerLeg, avatarCenter);
        ConfigureLimb(PersonPartType.RightThigh, PersonPartType.RightLowerLeg, RightLegRoot, RightLegImage, RightLowerLegImage, settings.RightLeg, settings.RightLowerLeg, avatarCenter);
        AvatarPartSettings right=settings.RightArm;
        if (string.IsNullOrWhiteSpace(right.ImagePath))
        {
            right.ImagePath = settings.ArmImagePath;
        }
        ConfigureLimb(PersonPartType.RightUpperArm, PersonPartType.RightForearm, RightArmRoot, ArmImage, RightForearmImage, right, settings.RightForearm, avatarCenter, settings.ArmWidth, settings.ArmHeight);
        ApplyRightShoulderAngle(ArmAngle);
        settings.ShoulderX=right.X;settings.ShoulderY=right.Y;
        settings.ArmWidth=right.Width;settings.ArmHeight=right.Height;
        ApplyActivity(CurrentActivity, setFirstFrame: true);
    }

    private void ConfigureLimb(
        PersonPartType upperPartType,
        PersonPartType lowerPartType,
        Canvas root,
        Image upper,
        Image lower,
        AvatarPartSettings upperPart,
        AvatarPartSettings lowerPart,
        Point avatarCenter,
        double? fallbackWidth = null,
        double? fallbackHeight = null
    )
    {
        double upperWidth = upperPart.Width > 0 ? upperPart.Width : fallbackWidth ?? 100;
        double upperHeight = upperPart.Height > 0 ? upperPart.Height : fallbackHeight ?? 100;
        Point rootPosition = OffsetPosition(upperPart, upperWidth, upperHeight, avatarCenter);
        PartAnimator.ConfigurePart(
            upper,
            CreateConfiguration(upperPartType.ToString(), upperPart, rootPosition, upperWidth, upperHeight),
            root
        );
        Canvas.SetLeft(upper, 0);
        Canvas.SetTop(upper, 0);
        Point upperPivot = new(upperPart.PivotX * upperWidth, upperPart.PivotY * upperHeight);
        Point lowerPosition;
        if (upperPart.HasEndJoint)
        {
            Point elbow = new(upperPart.EndJointX * upperWidth, upperPart.EndJointY * upperHeight);
            lowerPosition = new Point(
                elbow.X - lowerPart.PivotX * lowerPart.Width,
                elbow.Y - lowerPart.PivotY * lowerPart.Height
            );
        }
        else
        {
            lowerPosition = OffsetPosition(lowerPart, lowerPart.Width, lowerPart.Height, upperPivot);
        }
        PartAnimator.ConfigurePart(
            lower,
            CreateConfiguration(lowerPartType.ToString(), lowerPart, lowerPosition, lowerPart.Width, lowerPart.Height, upperPartType.ToString())
        );
    }

    public void SetJointAngles(double leftElbow,double rightElbow,double leftKnee,double rightKnee)
    {
        LeftElbowAngle = leftElbow;
        RightElbowAngle = rightElbow;
        LeftKneeAngle = leftKnee;
        RightKneeAngle = rightKnee;
    }

    private void ApplyRightShoulderAngle(double animationAngle)
    {
        if (PartAnimator.ContainsPart(PersonPartType.RightUpperArm.ToString()))
        {
            PartAnimator.SetPartAngle(
                PersonPartType.RightUpperArm.ToString(),
                _settings.RightArm.InitialAngle + animationAngle
            );
        }
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

    private FrameworkElement GetPartElement(PersonPartType part)
    {
        return part switch
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
    }

    private AvatarPartSettings GetPartSettings(PersonPartType part)
    {
        return part switch
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
    }

    private double GetPartAngle(PersonPartType part)
    {
        return PartAnimator.ContainsPart(part.ToString())
            ? PartAnimator.GetPartAngle(part.ToString())
            : 0;
    }

    private void SetPartAngle(PersonPartType part, double angle)
    {
        if (part == PersonPartType.RightUpperArm)
            ArmAngle = angle - _settings.RightArm.InitialAngle;
        else if (part is PersonPartType.LeftForearm
            or PersonPartType.RightForearm
            or PersonPartType.LeftLowerLeg
            or PersonPartType.RightLowerLeg)
            PartAnimator.SetPartAngle(part.ToString(), Math.Max(0, angle));
        else
            PartAnimator.SetPartAngle(part.ToString(), angle);
    }

    private double GetPartWorldAngle(PersonPartType part)
    {
        return PartAnimator.GetPartWorldAngle(part.ToString());
    }

    private double GetParentWorldAngle(PersonPartType part)
    {
        return GetPartWorldAngle(part) - GetPartAngle(part);
    }

    private Point GetPartPivot(PersonPartType part)
    {
        FrameworkElement element = GetPartElement(part);
        AvatarPartSettings settings = GetPartSettings(part);
        Point local = new(settings.PivotX * element.ActualWidth, settings.PivotY * element.ActualHeight);
        return element.TranslatePoint(local, PersonRoot);
    }

    private static double AngleFromPivot(Point pivot, Point point)
    {
        return Math.Atan2(point.Y - pivot.Y, point.X - pivot.X) * 180 / Math.PI;
    }

    private static double NormalizePartAngle(double angle)
    {
        angle %= 360;
        if (angle > 180) angle -= 360;
        if (angle < -180) angle += 360;
        return angle;
    }

    private void ConfigurePart(
        PersonPartType partType,
        Image image,
        AvatarPartSettings part,
        Point avatarCenter,
        double? fallbackWidth = null,
        double? fallbackHeight = null
    )
    {
        double width = part.Width > 0 ? part.Width : fallbackWidth ?? 100;
        double height = part.Height > 0 ? part.Height : fallbackHeight ?? 100;
        Point position = OffsetPosition(part, width, height, avatarCenter);
        PartAnimator.ConfigurePart(
            image,
            CreateConfiguration(partType.ToString(), part, position, width, height)
        );
    }

    private static VisualPartConfiguration CreateConfiguration(
        string name,
        AvatarPartSettings part,
        Point position,
        double width,
        double height,
        string? parentPartName = null
    )
    {
        return new VisualPartConfiguration
        {
            Name = name,
            ImagePath = part.ImagePath,
            X = position.X,
            Y = position.Y,
            Width = width,
            Height = height,
            PivotX = part.PivotX,
            PivotY = part.PivotY,
            InitialAngle = part.InitialAngle,
            ZOrder = part.ZIndex,
            ParentPartName = parentPartName,
        };
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
        double targetLeftHip = 24 * targetPhase;
        double targetRightHip = -24 * targetPhase;
        double targetLeftShoulder = 18 * targetPhase;
        double targetRightShoulder = -18 * targetPhase;
        double targetLeftElbow = targetPhase > 0 ? 5 : 18;
        double targetRightElbow = targetPhase > 0 ? 18 : 5;
        double targetLeftKnee = targetPhase > 0 ? 28 : 5;
        double targetRightKnee = targetPhase > 0 ? 5 : 28;
        VisualPartPose pose = new(new Dictionary<string, double>
        {
            [PersonPartType.LeftThigh.ToString()] = _settings.LeftLeg.InitialAngle + targetLeftHip,
            [PersonPartType.RightThigh.ToString()] = _settings.RightLeg.InitialAngle + targetRightHip,
            [PersonPartType.LeftUpperArm.ToString()] = _settings.LeftArm.InitialAngle + targetLeftShoulder,
            [PersonPartType.RightUpperArm.ToString()] = _settings.RightArm.InitialAngle + targetRightShoulder,
            [PersonPartType.LeftForearm.ToString()] = targetLeftElbow,
            [PersonPartType.RightForearm.ToString()] = targetRightElbow,
            [PersonPartType.LeftLowerLeg.ToString()] = targetLeftKnee,
            [PersonPartType.RightLowerLeg.ToString()] = targetRightKnee,
        });
        await PartAnimator.AnimateToPoseAsync(pose, duration, cancellationToken);
        SetCurrentValue(ArmAngleProperty, targetRightShoulder);
        ArmMoved?.Invoke(this, EventArgs.Empty);
    }

    public async Task MoveArmToAsync(double targetAngle, TimeSpan duration,
        CancellationToken cancellationToken = default)
    {
        VisualPartPose pose = new(new Dictionary<string, double>
        {
            [PersonPartType.RightUpperArm.ToString()] = _settings.RightArm.InitialAngle + targetAngle,
        });
        await PartAnimator.AnimateToPoseAsync(pose, duration, cancellationToken);
        SetCurrentValue(ArmAngleProperty, targetAngle);
        ArmMoved?.Invoke(this, EventArgs.Empty);
    }

    public async Task MoveShouldersToAsync(
        double leftTargetAngle,
        double rightTargetAngle,
        TimeSpan duration,
        CancellationToken cancellationToken = default
    )
    {
        VisualPartPose pose = new(new Dictionary<string, double>
        {
            [PersonPartType.LeftUpperArm.ToString()] = _settings.LeftArm.InitialAngle + leftTargetAngle,
            [PersonPartType.RightUpperArm.ToString()] = _settings.RightArm.InitialAngle + rightTargetAngle,
        });
        await PartAnimator.AnimateToPoseAsync(pose, duration, cancellationToken);
        SetCurrentValue(ArmAngleProperty, rightTargetAngle);
        ArmMoved?.Invoke(this, EventArgs.Empty);
    }

    public void PointArmAt(Point pointInPersonCoordinates)
    {
        ArmAngle = Math.Atan2(pointInPersonCoordinates.Y - _settings.ShoulderY,
            pointInPersonCoordinates.X - _settings.ShoulderX) * 180d / Math.PI
            - _settings.RightArm.InitialAngle;
        ArmMoved?.Invoke(this, EventArgs.Empty);
    }

    public void OnDragStarted()
    {
        if (_isMouseDragging)
            return;
        _isMouseDragging = true;
        _activityBeforeMouseDrag = CurrentActivity;
        _lastDragX = NormalizeCanvasCoordinate(Canvas.GetLeft(this));
        LayoutUpdated += PersonLayoutUpdatedDuringDrag;
        _ = PlayActivityAsync(PersonActivity.Walk);
    }

    public void OnDragMouseup()
    {
        if (_isMouseDragging)
        {
            _isMouseDragging = false;
            LayoutUpdated -= PersonLayoutUpdatedDuringDrag;
            _activityCancellation?.Cancel();
            CurrentActivity = _activityBeforeMouseDrag;
        }
        DragCompleted?.Invoke(this, EventArgs.Empty);
    }

    private void PersonLayoutUpdatedDuringDrag(object? sender, EventArgs e)
    {
        double currentX = NormalizeCanvasCoordinate(Canvas.GetLeft(this));
        double deltaX = currentX - _lastDragX;
        if (Math.Abs(deltaX) < 0.01)
            return;

        FaceDirection(deltaX > 0);
        _lastDragX = currentX;
    }

    private void FaceDirection(bool faceRight)
    {
        PersonRoot.RenderTransformOrigin = new Point(.5, .5);
        if (PersonRoot.RenderTransform is ScaleTransform scale)
        {
            scale.ScaleX = faceRight ? Math.Abs(scale.ScaleX) : -Math.Abs(scale.ScaleX);
            return;
        }

        PersonRoot.RenderTransform = new ScaleTransform(faceRight ? 1 : -1, 1);
    }

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

    private static void OnCurrentActivityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((PersonUC)d).ApplyActivity((PersonActivity)e.NewValue, setFirstFrame: true);
    }

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

    private static double NormalizeCanvasCoordinate(double value)
    {
        return double.IsNaN(value) ? 0 : value;
    }

    private void Person_DragEnter(object sender, DragEventArgs e)
    {
        SetDropEffect(e);
    }

    private void Person_DragOver(object sender, DragEventArgs e)
    {
        SetDropEffect(e);
    }

    private void Person_Drop(object sender, DragEventArgs e)
    {
        object? item = GetSupportedDroppedItem(e.Data);
        switch (item)
        {
            case ITicket ticket:
                ReceiveTicket(ticket);
                break;
            case IClothes clothes:
                _clothes.Add(clothes);
                RaiseDroppedEvents(PersonDroppedItemType.Clothes, clothes, ClothesEquipped);
                break;
            case IFood food:
                RaiseDroppedEvents(PersonDroppedItemType.Food, food, FoodReceived);
                break;
            default:
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
        }
        e.Effects = DragDropEffects.Copy;
        e.Handled = true;
    }

    private void RaiseDroppedEvents(
        PersonDroppedItemType itemType,
        object item,
        EventHandler<PersonItemDroppedEventArgs>? typedHandler)
    {
        var eventArgs = new PersonItemDroppedEventArgs(itemType, item);
        typedHandler?.Invoke(this, eventArgs);
        ItemDropped?.Invoke(this, eventArgs);
    }

    private static void SetDropEffect(DragEventArgs e)
    {
        e.Effects = GetSupportedDroppedItem(e.Data) == null
            ? DragDropEffects.None
            : DragDropEffects.Copy;
        e.Handled = true;
    }

    private static object? GetSupportedDroppedItem(IDataObject data)
    {
        foreach (string format in data.GetFormats())
        {
            object? item = data.GetData(format);
            if (item is ITicket or IClothes or IFood)
            {
                return item;
            }
        }
        return null;
    }
}
