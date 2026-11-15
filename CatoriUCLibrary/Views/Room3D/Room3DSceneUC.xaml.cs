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
        Interaction.Status += message => ModelStatusText.Text = message;
        Editor.SceneRequested += id => SceneTransitionRequested?.Invoke(id);
        Editor.Models.CollectionChanged += (_, _) => UpdateDemonstration();
        Editor.Lines.CollectionChanged += (_, _) => UpdateDemonstration();
        Editor.Hotspots.CollectionChanged += (_, _) => UpdateDemonstration();
        Editor.SelectionChanged += () => ChildSelectionBounds.BoundingBox = Editor.SelectedBounds;
        Editor.Root.Changed += (_, _) => ChildSelectionBounds.BoundingBox = Editor.SelectedBounds;
        Loaded += (_, _) =>
        {
            ReloadModel();
            ReloadDefinition();
        };
        IsVisibleChanged += (_, _) =>
        {
            if (!IsVisible)
            {
                Interaction.Cancel();
                Editor.Stop();
            }
        };
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
        Interaction.Cancel();
        Editor.Load(SceneDefinitionJson);
        if (Editor.LoadWarnings.Length > 0)
        {
            ModelStatusText.Text = Editor.LoadWarnings;
        }
    }

    private void UpdateDemonstration()
    {
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
        return IsEditing && RoomChildDropPlacement.CanDrop(Editor, RoomViewport.Viewport, data, point, allowed);
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
        e.Effects = CanDropChild(e.Data, e.GetPosition(RoomViewport.Viewport), e.AllowedEffects) ? DragDropEffects.Move : DragDropEffects.None;
        e.Handled = true;
    }

    private void Room_Drop(object sender, DragEventArgs e)
    {
        e.Effects = DropChild(e.Data, e.GetPosition(RoomViewport.Viewport), e.AllowedEffects) ? DragDropEffects.Move : DragDropEffects.None;
        e.Handled = true;
    }

    private void Room_MouseMove(object sender, MouseEventArgs e)
    {
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
    }

    private void Room_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape && Interaction.IsDrawing)
        {
            Interaction.Cancel();
            RoomViewport.ReleaseMouseCapture();
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
            ModelStatusText.Text = System.IO.Path.GetFileName(ModelPath) + (loader.MaterialWarning is null ? "" : " · " + loader.MaterialWarning);
        }
        catch (Exception exception) when (exception is System.IO.IOException or ArgumentException or InvalidOperationException or FormatException or NotSupportedException)
        {
            ModelStatusText.Text = "Unable to load OBJ; showing sample room. " + exception.Message;
        }
    }

    private void SetSampleVisible(bool visible)
    {
        double scale = visible ? 1 : 0;
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
