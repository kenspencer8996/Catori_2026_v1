namespace CatoriUCLibrary.Views.Room3D;

public sealed record RoomChildDragData(Room3DEditor Owner, Guid ChildId)
{
    public const string Format = "Catori.Room3D.ChildInstance";
}
