using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace CatoriUCLibrary.Views.Room3D;

public sealed class Room3DInteraction
{
    private readonly HelixViewport3D viewport;
    private readonly Room3DEditor editor;
    private string? drawingName;
    private Point3D? first;
    private bool hotspot;
    private double duration;
    private long target;
    private Vector3D? dragOffset;
    public bool IsDrawing => drawingName is not null;
    public event Action<string>? Status;

    public Room3DInteraction(HelixViewport3D viewport, Room3DEditor editor)
    {
        this.viewport = viewport;
        this.editor = editor;
    }

    public void Begin(string name, bool isHotspot, double seconds, long targetSceneId)
    {
        Cancel();
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Enter a name before drawing.");
        }
        editor.ValidateName(name);
        drawingName = name;
        hotspot = isHotspot;
        duration = seconds;
        target = targetSceneId;
        viewport.Focus();
        viewport.Cursor = Cursors.Cross;
        Status?.Invoke("Click the floor for the first point, then the second point. Escape cancels.");
    }

    public void Cancel()
    {
        drawingName = null;
        first = null;
        dragOffset = null;
        editor.Preview.Children.Clear();
        viewport.Cursor = null;
    }

    public void ClickFloor(Point3D point)
    {
        if (drawingName is null)
        {
            return;
        }
        if (first is null)
        {
            first = point;
            return;
        }
        if (hotspot)
        {
            editor.AddHotspot(drawingName, first.Value, point, target);
        }
        else
        {
            editor.AddLine(drawingName, first.Value, point, duration);
        }
        string name = drawingName;
        Cancel();
        Status?.Invoke("Created " + name + ".");
    }

    public bool MouseDown(Point point, bool editing, int clickCount)
    {
        var floor = Room3DFloorProjection.FloorPoint(viewport.Viewport, point);
        if (drawingName is not null)
        {
            if (floor.HasValue)
            {
                ClickFloor(floor.Value);
            }
            return true;
        }
        foreach (var hit in Hits(point))
        {
            var model = editor.FindModel(hit.ModelHit);
            if (model is not null && editing)
            {
                editor.Select(model);
                editor.Stop();
                if (floor.HasValue)
                {
                    dragOffset = editor.SelectedPosition - floor.Value;
                }
                return true;
            }
            var region = editor.FindHotspot(hit.ModelHit);
            if (region is not null)
            {
                Status?.Invoke(region.Name + " → scene " + region.TargetSceneId + ". Double-click to test navigation.");
                if (!editing || clickCount == 2)
                {
                    editor.ActivateHotspot(region);
                }
                return true;
            }
        }
        return false;
    }

    public void MouseMove(Point point, bool pressed)
    {
        var floor = Room3DFloorProjection.FloorPoint(viewport.Viewport, point);
        if (!floor.HasValue)
        {
            return;
        }
        if (drawingName is not null && first.HasValue)
        {
            editor.Preview.Children.Clear();
            editor.Preview.Children.Add(hotspot ? Room3DGeometry.Hotspot(first.Value, floor.Value, true) : Room3DGeometry.Line(first.Value, floor.Value, true));
        }
        else if (pressed && dragOffset.HasValue)
        {
            editor.MoveSelected(floor.Value + dragOffset.Value);
        }
    }

    public void EndDrag()
    {
        dragOffset = null;
    }

    private IEnumerable<RayMeshGeometry3DHitTestResult> Hits(Point point)
    {
        var hits = new List<RayMeshGeometry3DHitTestResult>();
        VisualTreeHelper.HitTest(viewport.Viewport, null, result =>
        {
            if (result is RayMeshGeometry3DHitTestResult hit)
            {
                hits.Add(hit);
            }
            return HitTestResultBehavior.Continue;
        }, new PointHitTestParameters(point));
        return hits.OrderBy(hit => hit.DistanceToRayOrigin);
    }
}
