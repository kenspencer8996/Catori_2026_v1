using System.Windows;
using System.Windows.Controls;

namespace CatoriUCLibrary.Views.Room3D;

public static class RoomChildDropPlacement
{
    public static bool CanDrop(Room3DEditor editor, Viewport3D viewport, IDataObject data, Point point, DragDropEffects allowed)
    {
        return allowed.HasFlag(DragDropEffects.Move)
            && data.GetDataPresent(RoomChildDragData.Format)
            && data.GetData(RoomChildDragData.Format) is RoomChildDragData child
            && ReferenceEquals(child.Owner, editor)
            && editor.HasLoadedChild(child.ChildId)
            && Room3DFloorProjection.FloorPoint(viewport, point).HasValue;
    }

    public static bool Drop(Room3DEditor editor, Viewport3D viewport, IDataObject data, Point point, DragDropEffects allowed)
    {
        if (!CanDrop(editor, viewport, data, point, allowed))
        {
            return false;
        }
        var child = (RoomChildDragData)data.GetData(RoomChildDragData.Format);
        var floor = Room3DFloorProjection.FloorPoint(viewport, point)!.Value;
        editor.Select(editor.Models.Single(item => item.Id == child.ChildId));
        editor.MoveSelected(floor);
        return true;
    }
}
