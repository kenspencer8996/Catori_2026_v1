using CatoriApp.Core.Objects.DragDrop;
using CatoriUCLibrary.Views.Person;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CatoriUCLibrary.Views.FactoryControls;

public partial class FactoryControlPanelUC : UserControl, IDraggable
{
    private static readonly Brush ReadyBrush = new SolidColorBrush(Color.FromRgb(91, 32, 32));
    private static readonly Brush ActiveBrush = new SolidColorBrush(Color.FromRgb(45, 220, 83));

    public FactoryControlPanelUC()
    {
        InitializeComponent();
        ApplyMode(Mode);
    }

    public event EventHandler? DragCompleted;
    public event EventHandler<FactoryControlPanelActivatedEventArgs>? Activated;
    public event EventHandler? ActivationRejected;

    public FactoryControlPanelMode Mode
    {
        get => (FactoryControlPanelMode)GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    public static readonly DependencyProperty ModeProperty =
        DependencyProperty.Register(nameof(Mode), typeof(FactoryControlPanelMode),
            typeof(FactoryControlPanelUC), new PropertyMetadata(FactoryControlPanelMode.Lever,
                static (d, e) => ((FactoryControlPanelUC)d).ApplyMode((FactoryControlPanelMode)e.NewValue)));

    public string AnimationTargetName
    {
        get => (string)GetValue(AnimationTargetNameProperty);
        set => SetValue(AnimationTargetNameProperty, value);
    }

    public static readonly DependencyProperty AnimationTargetNameProperty =
        DependencyProperty.Register(nameof(AnimationTargetName), typeof(string),
            typeof(FactoryControlPanelUC), new PropertyMetadata(string.Empty));

    public PersonUC? TargetPerson
    {
        get => (PersonUC?)GetValue(TargetPersonProperty);
        set => SetValue(TargetPersonProperty, value);
    }

    public static readonly DependencyProperty TargetPersonProperty =
        DependencyProperty.Register(nameof(TargetPerson), typeof(PersonUC),
            typeof(FactoryControlPanelUC), new PropertyMetadata(null, OnTargetPersonChanged));

    public double ActivationRadius
    {
        get => (double)GetValue(ActivationRadiusProperty);
        set => SetValue(ActivationRadiusProperty, value);
    }

    public static readonly DependencyProperty ActivationRadiusProperty =
        DependencyProperty.Register(nameof(ActivationRadius), typeof(double),
            typeof(FactoryControlPanelUC), new PropertyMetadata(50d));

    public bool IsActivated
    {
        get => (bool)GetValue(IsActivatedProperty);
        private set => SetValue(IsActivatedPropertyKey, value);
    }

    private static readonly DependencyPropertyKey IsActivatedPropertyKey =
        DependencyProperty.RegisterReadOnly(nameof(IsActivated), typeof(bool),
            typeof(FactoryControlPanelUC), new PropertyMetadata(false));

    public static readonly DependencyProperty IsActivatedProperty = IsActivatedPropertyKey.DependencyProperty;

    public bool IsDragEnabled { get; set; } = true;
    public UIElement Visual => this;
    public Point OriginalPosition => new(Normalize(Canvas.GetLeft(this)), Normalize(Canvas.GetTop(this)));

    public void Activate(PersonUC? person = null)
    {
        if (IsActivated)
            return;

        IsActivated = true;
        StatusLight.Fill = ActiveBrush;
        ButtonVisual.Fill = ActiveBrush;
        LeverRotate.Angle = 25;

        string panelName = string.IsNullOrWhiteSpace(Name) ? nameof(FactoryControlPanelUC) : Name;
        FactoryControlPanelActivatedEventArgs args = new(AnimationTargetName, person);
        Activated?.Invoke(this, args);
        WeakReferenceMessenger.Default.Send(new FactoryControlPanelActivatedMessage(
            panelName, AnimationTargetName, person));
    }

    public void Reset()
    {
        IsActivated = false;
        StatusLight.Fill = ReadyBrush;
        ButtonVisual.Fill = new SolidColorBrush(Color.FromRgb(198, 40, 40));
        LeverRotate.Angle = -25;
    }

    public bool IsHandWithinReach(PersonUC person, Point handPoint)
    {
        Point handInPanel = person.TranslatePoint(handPoint, this);
        Point target = new(Width / 2d, 82d);
        Vector distance = handInPanel - target;
        return distance.Length <= ActivationRadius;
    }

    public void OnDragMouseup() => DragCompleted?.Invoke(this, EventArgs.Empty);

    private static void OnTargetPersonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        FactoryControlPanelUC panel = (FactoryControlPanelUC)d;
        if (e.OldValue is PersonUC oldPerson)
            oldPerson.ArmInteractionCompleted -= panel.TargetPerson_ArmInteractionCompleted;
        if (e.NewValue is PersonUC newPerson)
            newPerson.ArmInteractionCompleted += panel.TargetPerson_ArmInteractionCompleted;
    }

    private void TargetPerson_ArmInteractionCompleted(object? sender, PersonArmInteractionEventArgs e)
    {
        if (sender is not PersonUC person)
            return;

        if (IsHandWithinReach(person, e.HandPoint))
            Activate(person);
        else
            ActivationRejected?.Invoke(this, EventArgs.Empty);
    }

    private void ActivationVisual_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (IsDragEnabled)
            return;

        Activate(TargetPerson);
        e.Handled = true;
    }

    private void ApplyMode(FactoryControlPanelMode mode)
    {
        if (!IsInitialized)
            return;
        LeverVisual.Visibility = mode == FactoryControlPanelMode.Lever ? Visibility.Visible : Visibility.Collapsed;
        ButtonVisual.Visibility = mode == FactoryControlPanelMode.Button ? Visibility.Visible : Visibility.Collapsed;
    }

    private static double Normalize(double value) => double.IsNaN(value) ? 0 : value;
}
