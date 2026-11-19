using System;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows;
using CatoriUCLibrary.Views.Room3D;
using System.Windows.Input;

namespace CatoriUCLibrary.Views;

public partial class Room3DSceneUC : UserControl
{
    private bool configuring;
    public Room3DEditor Editor { get; } = new();
    public Room3DInteraction Interaction { get; }
    public RoomInteriorNavigation Navigation { get; }
    public bool IsEditing { get; set; }
    public event Action<long>? SceneTransitionRequested;
    public static readonly DependencyProperty SceneDefinitionJsonProperty = DependencyProperty.Register(nameof(SceneDefinitionJson), typeof(string), typeof(Room3DSceneUC), new PropertyMetadata("", DefinitionChanged));

    public string SceneDefinitionJson
    {
        get => (string)GetValue(SceneDefinitionJsonProperty);
        set => SetValue(SceneDefinitionJsonProperty, value);
    }
    public static readonly DependencyProperty ModelPathProperty = DependencyProperty.Register(nameof(ModelPath), typeof(string), typeof(Room3DSceneUC), new PropertyMetadata("", ModelConfigurationChanged));
    public static readonly DependencyProperty SourceYUpProperty = DependencyProperty.Register(nameof(SourceYUp), typeof(bool), typeof(Room3DSceneUC), new PropertyMetadata(true, ModelConfigurationChanged));

    public string ModelPath
    {
        get => (string)GetValue(ModelPathProperty);
        set => SetValue(ModelPathProperty, value);
    }

    public bool SourceYUp
    {
        get => (bool)GetValue(SourceYUpProperty);
        set => SetValue(SourceYUpProperty, value);
    }

    public Model3DGroup? SceneModel => ImportedEnvironment.Content as Model3DGroup;
    public string ModelStatus => ModelStatusText.Text;

    public Room3DSceneUC()
    {
        InitializeComponent();
        SceneChildrenVisual.Content = Editor.Root;
        SceneDrawingPreviewVisual.Content = Editor.Preview;
        Interaction = new Room3DInteraction(RoomViewport, Editor);
        Navigation = new RoomInteriorNavigation(RoomViewport);
        Unloaded += (_, _) => ExitRoom();
        Interaction.Status += message => ModelStatusText.Text = message;
        Editor.SceneRequested += id => SceneTransitionRequested?.Invoke(id);
        Editor.Models.CollectionChanged += (_, _) => UpdateDemonstration();
        Editor.Lines.CollectionChanged += (_, _) => UpdateDemonstration();
        Editor.Hotspots.CollectionChanged += (_, _) => UpdateDemonstration();
        Interaction.IsRotationHandle = model => ReferenceEquals(ChildRotationArc.Content, model) || ReferenceEquals(ChildRotationArrow.Content, model);
        Interaction.IsResizeHandle = model => Handles.Any(handle => ReferenceEquals(handle.Content, model));
        Editor.SelectionChanged += UpdateSelection;
        Editor.Root.Changed += (_, _) => UpdateSelection();
        Loaded += (_, _) =>
        {
            ReloadModel();
            ReloadDefinition();
        };
        IsVisibleChanged += (_, _) =>
        {
            if (!IsVisible)
            {
                ExitRoom();
                Interaction.Cancel();
                Editor.Stop();
            }
        };
    }

    private HelixToolkit.Wpf.SphereVisual3D[] Handles => new[] { ResizeHandle0, ResizeHandle1, ResizeHandle2, ResizeHandle3, ResizeHandle4, ResizeHandle5, ResizeHandle6, ResizeHandle7 };

    private void UpdateSelection()
    {
        var bounds = IsEditing && !Navigation.IsInside ? Editor.SelectedBounds : Rect3D.Empty;
        ChildSelectionBounds.BoundingBox = bounds;
        UpdateRotationHandle(bounds);
        for (int i = 0; i < Handles.Length; i++)
        {
            Handles[i].Radius = bounds.IsEmpty ? 0 : Math.Max(0.06, Math.Max(bounds.SizeX, Math.Max(bounds.SizeY, bounds.SizeZ)) * 0.045);
            if (!bounds.IsEmpty)
            {
                Handles[i].Center = new Point3D(bounds.X + ((i & 1) == 0 ? 0 : bounds.SizeX), bounds.Y + ((i & 2) == 0 ? 0 : bounds.SizeY), bounds.Z + ((i & 4) == 0 ? 0 : bounds.SizeZ));
            }
        }
    }

    private void UpdateRotationHandle(Rect3D bounds)
    {
        var path = new System.Windows.Media.Media3D.Point3DCollection();
        if (bounds.IsEmpty)
        {
            ChildRotationArc.Path = path;
            ChildRotationArrow.Visible = false;
            return;
        }
        double size = Math.Max(bounds.SizeX, Math.Max(bounds.SizeY, bounds.SizeZ));
        double radius = Math.Max(0.25, size * 0.18);
        double z = bounds.Z + bounds.SizeZ + Math.Max(0.2, size * 0.14);
        double x = bounds.X + bounds.SizeX / 2;
        double y = bounds.Y + bounds.SizeY / 2;
        for (int index = 0; index <= 32; index++)
        {
            double angle = (-50 + index * 260.0 / 32) * Math.PI / 180;
            path.Add(new Point3D(x + radius * Math.Cos(angle), y + radius * Math.Sin(angle), z));
        }
        ChildRotationArc.Diameter = Math.Max(0.045, radius * 0.16);
        ChildRotationArc.Path = path;
        ChildRotationArrow.Diameter = Math.Max(0.065, radius * 0.22);
        ChildRotationArrow.Point1 = path[path.Count - 4];
        ChildRotationArrow.Point2 = path[path.Count - 1];
        ChildRotationArrow.Visible = true;
    }

    private void Room_DragLeave(object sender, DragEventArgs e)
    {
        ChildDragIndicator.Visibility = Visibility.Collapsed;
    }

    private static void DefinitionChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
        var scene = (Room3DSceneUC)target;
        if (scene.IsLoaded)
        {
            scene.ReloadDefinition();
        }
    }

    public void ReloadDefinition()
    {
        ExitRoom();
        Interaction.Cancel();
        Editor.Load(SceneDefinitionJson);
        if (Editor.LoadWarnings.Length > 0)
        {
            ModelStatusText.Text = Editor.LoadWarnings;
        }
    }

    private void UpdateDemonstration()
    {
        SetSampleVisible(ImportedEnvironment.Content is null);
        double scale = Editor.Models.Count + Editor.Lines.Count + Editor.Hotspots.Count == 0 ? 1 : 0;
        SampleDemonstration.Transform = new ScaleTransform3D(scale, scale, scale);
    }

    private void Room_MouseDown(object sender, MouseButtonEventArgs e)
    {
        try
        {
            e.Handled = Interaction.MouseDown(e.GetPosition(RoomViewport.Viewport), IsEditing, e.ClickCount);
            if (e.Handled && IsEditing)
            {
                RoomViewport.CaptureMouse();
            }
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelStatusText.Text = exception.Message;
            e.Handled = true;
        }
    }

    public bool CanDropChild(IDataObject data, Point point, DragDropEffects allowed = DragDropEffects.Move)
    {
        return IsEditing && !Navigation.IsInside && RoomChildDropPlacement.CanDrop(Editor, RoomViewport.Viewport, data, point, allowed);
    }

    public bool DropChild(IDataObject data, Point point, DragDropEffects allowed = DragDropEffects.Move)
    {
        if (!CanDropChild(data, point, allowed))
        {
            return false;
        }
        Interaction.Cancel();
        bool placed = RoomChildDropPlacement.Drop(Editor, RoomViewport.Viewport, data, point, allowed);
        if (placed)
        {
            ModelStatusText.Text = "Placed " + Editor.SelectedModel?.Name + ". Left-drag the model to move it; Save retains its position.";
        }
        return placed;
    }

    private void Room_DragOver(object sender, DragEventArgs e)
    {
        e.Effects = CanDropChild(e.Data, e.GetPosition(RoomViewport.Viewport), e.AllowedEffects) ? (e.Data.GetDataPresent("Catori.RoomVisualType") ? DragDropEffects.Copy : DragDropEffects.Move) : DragDropEffects.None;
        var pointer = e.GetPosition(this);
        ChildDragIndicator.Margin = new Thickness(Math.Max(0, Math.Min(ActualWidth - 350, pointer.X + 20)), Math.Max(0, Math.Min(ActualHeight - 65, pointer.Y + 20)), 0, 0);
        string name = e.Data.GetData("Catori.RoomVisualType") is string visualType ? visualType.Split('.').Last() : Editor.SelectedModel?.Name ?? "object";
        ChildDragLabel.Text = e.Effects == DragDropEffects.None ? "Move onto the room floor to place" : "Release to place " + name;
        ChildDragIndicator.Visibility = Visibility.Visible;
        e.Handled = true;
    }

    private void Room_Drop(object sender, DragEventArgs e)
    {
        ChildDragIndicator.Visibility = Visibility.Collapsed;
        e.Effects = DropChild(e.Data, e.GetPosition(RoomViewport.Viewport), e.AllowedEffects) ? (e.Data.GetDataPresent("Catori.RoomVisualType") ? DragDropEffects.Copy : DragDropEffects.Move) : DragDropEffects.None;
        e.Handled = true;
    }

    private void Room_MouseMove(object sender, MouseEventArgs e)
    {
        if (Navigation.IsInside)
        {
            Navigation.Look(e.GetPosition(RoomViewport));
            e.Handled = true;
            return;
        }
        Interaction.MouseMove(e.GetPosition(RoomViewport.Viewport), IsEditing && e.LeftButton == MouseButtonState.Pressed);
    }

    private void Room_MouseUp(object sender, MouseButtonEventArgs e)
    {
        Interaction.EndDrag();
        RoomViewport.ReleaseMouseCapture();
    }

    private void Room_LostCapture(object sender, MouseEventArgs e)
    {
        Interaction.EndDrag();
        Navigation.EndLook();
    }

    private void Room_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (Navigation.IsInside)
        {
            if (e.Key == Key.Escape)
            {
                ExitRoom();
            }
            else
            {
                Navigation.SetKey(e.Key, true);
            }
            e.Handled = true;
            return;
        }
        if (e.Key == Key.Escape && Interaction.IsDrawing)
        {
            Interaction.Cancel();
            RoomViewport.ReleaseMouseCapture();
            e.Handled = true;
        }
    }

    public void EnterRoom()
    {
        if (!IsEditing || Navigation.IsInside)
        {
            return;
        }
        var room = Editor.SelectedModel ?? throw new InvalidOperationException("Select a Room3D object first.");
        Interaction.Cancel();
        Editor.Stop();
        RoomViewport.ReleaseMouseCapture();
        Navigation.Enter(room);
        UpdateSelection();
        ModelStatusText.Text = "Inside room: WASD to move, right-drag to look. Exit Room or Escape returns to Design Mode.";
    }

    public void ExitRoom()
    {
        if (!Navigation.IsInside)
        {
            return;
        }
        Navigation.Exit();
        UpdateSelection();
        ModelStatusText.Text = "Design Mode: left-drag objects, blue handles resize, orange handle rotates. Right-drag orbits.";
    }

    private void Room_PreviewKeyUp(object sender, KeyEventArgs e)
    {
        if (Navigation.IsInside)
        {
            Navigation.SetKey(e.Key, false);
            e.Handled = true;
        }
    }

    private void Room_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (Navigation.IsInside)
        {
            Navigation.ClearInput();
        }
    }

    private void Interior_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (!Navigation.IsInside)
        {
            return;
        }
        RoomViewport.Focus();
        if (e.ChangedButton == MouseButton.Right)
        {
            Navigation.BeginLook(e.GetPosition(RoomViewport));
            RoomViewport.CaptureMouse();
        }
        e.Handled = true;
    }

    private void Interior_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (Navigation.IsInside)
        {
            Navigation.EndLook();
            RoomViewport.ReleaseMouseCapture();
            e.Handled = true;
        }
    }

    private void Interior_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (Navigation.IsInside)
        {
            e.Handled = true;
        }
    }

    public void ConfigureModel(string path, bool sourceYUp)
    {
        configuring = true;
        try
        {
            SetCurrentValue(ModelPathProperty, path);
            SetCurrentValue(SourceYUpProperty, sourceYUp);
        }
        finally
        {
            configuring = false;
        }
        if (IsLoaded)
        {
            ReloadModel();
        }
    }

    private static void ModelConfigurationChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
        var scene = (Room3DSceneUC)target;
        if (scene.IsLoaded && !scene.configuring)
        {
            scene.ReloadModel();
        }
    }

    private void ReloadModel()
    {
        ImportedEnvironment.Content = null;
        SetSampleVisible(true);
        ModelStatusText.Text = "Sample room (no OBJ selected)";
        if (string.IsNullOrWhiteSpace(ModelPath))
        {
            return;
        }
        try
        {
            var loader = new Room3DModelLoader();
            ImportedEnvironment.Content = loader.Load(ModelPath, SourceYUp, 8);
            SetSampleVisible(false);
            ModelStatusText.Text = System.IO.Path.GetFileName(ModelPath) + (loader.MaterialWarning is null ? "" : " Â· " + loader.MaterialWarning);
        }
        catch (Exception exception) when (exception is System.IO.IOException or ArgumentException or InvalidOperationException or FormatException or NotSupportedException)
        {
            ModelStatusText.Text = "Unable to load OBJ; showing sample room. " + exception.Message;
        }
    }

    private void SetSampleVisible(bool visible)
    {
        double scale = visible && !Editor.Models.Any(model => model.VisualType == RoomVisualCatalog.RoomType) ? 1 : 0;
        SampleEnvironment.Transform = new ScaleTransform3D(scale, scale, scale);
        SampleFurniture.Transform = new ScaleTransform3D(scale, scale, scale);
    }

    public Point3D ActorPosition => new(CharacterPosition.OffsetX, CharacterPosition.OffsetY, CharacterPosition.OffsetZ);

    public void SetActorPosition(Point3D position)
    {
        if (!double.IsFinite(position.X) || !double.IsFinite(position.Y) || !double.IsFinite(position.Z))
        {
            throw new ArgumentOutOfRangeException(nameof(position), "Position must contain finite coordinates.");
        }
        CharacterPosition.OffsetX = position.X;
        CharacterPosition.OffsetY = position.Y;
        CharacterPosition.OffsetZ = position.Z;
    }

    public void ResetScene()
    {
        SetActorPosition(new Point3D(-2.5, 0, 0));
    }
}
