using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Room3D;

public sealed class Room3DEditor
{
    private RoomSceneDefinition definition = new();
    private readonly Dictionary<Guid, RoomChildInstance> instances = new();
    private readonly Dictionary<Model3D, RoomFloorHotspot> hotspotModels = new();
    private readonly Model3DGroup tools = new();
    private readonly RoomLinePlayback playback = new();
    public Model3DGroup Root { get; } = new();
    public Model3DGroup Preview { get; } = new();
    public ObservableCollection<RoomChildModel> Models { get; } = new();
    public ObservableCollection<RoomNamedLine> Lines { get; } = new();
    public ObservableCollection<RoomFloorHotspot> Hotspots { get; } = new();
    public RoomChildModel? SelectedModel { get; private set; }
    public event Action? Changed;
    public event Action? SelectionChanged;
    public event Action<long>? SceneRequested;
    public string LoadWarnings { get; private set; } = "";
    public Rect3D SelectedBounds => SelectedModel is not null && instances.TryGetValue(SelectedModel.Id, out var instance) ? instance.Model.Bounds : Rect3D.Empty;

    public Room3DEditor()
    {
        Root.Children.Add(tools);
    }

    public void Load(string json)
    {
        var data = RoomSceneCodec.Read(json);
        playback.Stop();
        definition = data;
        Models.Clear();
        Lines.Clear();
        Hotspots.Clear();
        instances.Clear();
        Root.Children.Clear();
        Root.Children.Add(tools);
        LoadWarnings = "";
        foreach (var model in data.Models)
        {
            Models.Add(model);
            try
            {
                var instance = new RoomChildInstance(model);
                instances.Add(model.Id, instance);
                Root.Children.Add(instance.Model);
            }
            catch (Exception exception) when (exception is System.IO.IOException or ArgumentException or InvalidOperationException)
            {
                LoadWarnings += model.Name + ": " + exception.Message + " ";
            }
        }
        foreach (var line in data.Lines)
        {
            Lines.Add(line);
        }
        foreach (var hotspot in data.Hotspots)
        {
            Hotspots.Add(hotspot);
        }
        RebuildTools();
        Select(null);
    }

    public string Save()
    {
        return RoomSceneCodec.Write(definition);
    }

    public void SetDuration(RoomNamedLine line, double duration)
    {
        if (!Lines.Contains(line) || !double.IsFinite(duration) || duration < 0.05 || duration > 3600)
        {
            throw new ArgumentException("Choose a line and a duration of 0.05â€“3600 seconds.");
        }
        if (line.Duration == duration)
        {
            return;
        }
        playback.Stop();
        line.Duration = duration;
        Changed?.Invoke();
    }

    public RoomChildModel AddModel(string path, bool yUp)
    {
        string baseName = System.IO.Path.GetFileNameWithoutExtension(path);
        string name = baseName;
        int suffix = 2;
        while (Models.Any(item => item.Name == name))
        {
            name = baseName + " " + suffix++;
        }
        var data = new RoomChildModel { Name = name, ModelPath = path, SourceYUp = yUp };
        var instance = new RoomChildInstance(data);
        instances.Add(data.Id, instance);
        Root.Children.Add(instance.Model);
        definition.Models.Add(data);
        Models.Add(data);
        Select(data);
        Changed?.Invoke();
        return data;
    }

    public RoomChildModel AddVisual(string typeName)
    {
        var data = new RoomChildModel { Name = typeName.Split('.').Last(), VisualType = typeName };
        var instance = new RoomChildInstance(data);
        instances.Add(data.Id, instance);
        Root.Children.Add(instance.Model);
        definition.Models.Add(data);
        Models.Add(data);
        Select(data);
        Changed?.Invoke();
        return data;
    }

    public void ConfigureSelectedRoom(RoomConstructionSettings settings)
    {
        var data = SelectedModel ?? throw new InvalidOperationException("Select a Room3D object first.");
        if (data.VisualType != RoomVisualCatalog.RoomType)
        {
            throw new InvalidOperationException("Select a Room3D object first.");
        }
        var previous = data.Room;
        data.Room = settings;
        RoomChildInstance replacement;
        try
        {
            replacement = new RoomChildInstance(data);
        }
        catch
        {
            data.Room = previous;
            throw;
        }
        playback.Stop();
        Root.Children.Remove(instances[data.Id].Model);
        instances[data.Id] = replacement;
        Root.Children.Add(replacement.Model);
        SelectionChanged?.Invoke();
        Changed?.Invoke();
    }

    public void LoadShelfItem(string path, bool sourceYUp, int level)
    {
        var data = SelectedModel ?? throw new InvalidOperationException("Select a shelf first.");
        if (!RoomVisualCatalog.Types.Any(type => type.FullName == data.VisualType && (type.FullName == RoomVisualCatalog.ShelfType || type.FullName == RoomVisualCatalog.SimpleShelfType)))
        {
            throw new InvalidOperationException("Select a shelf first.");
        }
        var entry = new RoomShelfContent { ModelPath = path, SourceYUp = sourceYUp, Level = level };
        data.ShelfContents.Add(entry);
        RoomChildInstance replacement;
        try
        {
            replacement = new RoomChildInstance(data);
        }
        catch
        {
            data.ShelfContents.Remove(entry);
            throw;
        }
        playback.Stop();
        Root.Children.Remove(instances[data.Id].Model);
        instances[data.Id] = replacement;
        Root.Children.Add(replacement.Model);
        SelectionChanged?.Invoke();
        Changed?.Invoke();
    }

    public void RotateSelected(double degrees)
    {
        if (SelectedModel is null || !double.IsFinite(degrees) || !instances.TryGetValue(SelectedModel.Id, out var instance))
        {
            return;
        }
        playback.Stop();
        SelectedModel.Rotation = ((degrees % 360) + 360) % 360;
        instance.Orientation.Angle = SelectedModel.Rotation;
        SelectionChanged?.Invoke();
        Changed?.Invoke();
    }

    public void ResizeSelected(double scale)
    {
        if (SelectedModel is null || !double.IsFinite(scale) || !instances.TryGetValue(SelectedModel.Id, out var instance))
        {
            return;
        }
        playback.Stop();
        SelectedModel.Scale = Math.Clamp(scale, 0.02, 100);
        instance.Size.ScaleX = SelectedModel.Scale;
        instance.Size.ScaleY = SelectedModel.Scale;
        instance.Size.ScaleZ = SelectedModel.Scale;
        SelectionChanged?.Invoke();
        Changed?.Invoke();
    }

    public void Select(RoomChildModel? model)
    {
        SelectedModel = model;
        SelectionChanged?.Invoke();
    }

    public void MoveSelected(Point3D point)
    {
        if (SelectedModel is null || !instances.TryGetValue(SelectedModel.Id, out var instance))
        {
            return;
        }
        playback.Stop();
        SelectedModel.X = point.X;
        SelectedModel.Y = point.Y;
        instance.Position.OffsetX = point.X;
        instance.Position.OffsetY = point.Y;
        Changed?.Invoke();
    }

    public bool HasLoadedChild(Guid id)
    {
        return instances.ContainsKey(id);
    }

    public Point3D SelectedPosition => SelectedModel is not null && instances.TryGetValue(SelectedModel.Id, out var instance) ? new Point3D(instance.Position.OffsetX, instance.Position.OffsetY, instance.Position.OffsetZ) : new Point3D();

    public RoomNamedLine AddLine(string name, Point3D start, Point3D end, double duration)
    {
        ValidateName(name);
        if ((end - start).Length < 0.01 || !double.IsFinite(duration) || duration < 0.05 || duration > 3600)
        {
            throw new ArgumentException("Draw a nonzero line and enter a duration of 0.05â€“3600 seconds.");
        }
        var line = new RoomNamedLine { Name = name, StartX = start.X, StartY = start.Y, EndX = end.X, EndY = end.Y, Duration = duration };
        definition.Lines.Add(line);
        Lines.Add(line);
        RebuildTools();
        Changed?.Invoke();
        return line;
    }

    public RoomFloorHotspot AddHotspot(string name, Point3D start, Point3D end, long targetSceneId)
    {
        ValidateName(name);
        if (Math.Abs(start.X - end.X) < 0.05 || Math.Abs(start.Y - end.Y) < 0.05 || targetSceneId <= 0)
        {
            throw new ArgumentException("Draw a floor rectangle and choose its target scene first.");
        }
        var hotspot = new RoomFloorHotspot { Name = name, X = Math.Min(start.X, end.X), Y = Math.Min(start.Y, end.Y), Width = Math.Abs(start.X - end.X), Depth = Math.Abs(start.Y - end.Y), TargetSceneId = targetSceneId };
        definition.Hotspots.Add(hotspot);
        Hotspots.Add(hotspot);
        RebuildTools();
        Changed?.Invoke();
        return hotspot;
    }

    public void Assign(RoomChildModel model, RoomNamedLine line)
    {
        if (!Models.Contains(model) || !Lines.Contains(line))
        {
            throw new ArgumentException("Choose a child model and a line in this scene.");
        }
        model.LineId = line.Id;
        Changed?.Invoke();
    }

    public void Run(RoomChildModel model)
    {
        var line = Lines.SingleOrDefault(item => item.Id == model.LineId) ?? throw new InvalidOperationException("Assign the child model to a line first.");
        if (!instances.TryGetValue(model.Id, out var instance))
        {
            throw new InvalidOperationException("The selected model could not be loaded.");
        }
        playback.Run(instance, line);
    }

    public void Stop()
    {
        playback.Stop();
    }

    public void RemoveSelected()
    {
        if (SelectedModel is null)
        {
            return;
        }
        playback.Stop();
        if (instances.Remove(SelectedModel.Id, out var instance))
        {
            Root.Children.Remove(instance.Model);
        }
        definition.Models.Remove(SelectedModel);
        Models.Remove(SelectedModel);
        Select(null);
        Changed?.Invoke();
    }

    public RoomChildModel? FindModel(Model3D hit)
    {
        return instances.Values.FirstOrDefault(instance => Contains(instance.Model, hit))?.Data;
    }

    public RoomFloorHotspot? FindHotspot(Model3D hit)
    {
        return hotspotModels.GetValueOrDefault(hit);
    }

    public void ActivateHotspot(RoomFloorHotspot hotspot)
    {
        if (Hotspots.Contains(hotspot))
        {
            SceneRequested?.Invoke(hotspot.TargetSceneId);
        }
    }

    internal void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || Lines.Any(line => line.Name == name) || Hotspots.Any(hotspot => hotspot.Name == name))
        {
            throw new ArgumentException("Enter a unique, nonempty name for the line or hotspot.");
        }
    }

    private void RebuildTools()
    {
        tools.Children.Clear();
        hotspotModels.Clear();
        foreach (var line in Lines)
        {
            tools.Children.Add(Room3DGeometry.Line(new Point3D(line.StartX, line.StartY, 0), new Point3D(line.EndX, line.EndY, 0)));
        }
        foreach (var hotspot in Hotspots)
        {
            var model = Room3DGeometry.Hotspot(new Point3D(hotspot.X, hotspot.Y, 0), new Point3D(hotspot.X + hotspot.Width, hotspot.Y + hotspot.Depth, 0));
            tools.Children.Add(model);
            hotspotModels.Add(model, hotspot);
        }
    }

    private static bool Contains(Model3D parent, Model3D hit)
    {
        return ReferenceEquals(parent, hit) || parent is Model3DGroup group && group.Children.Any(child => Contains(child, hit));
    }
}
