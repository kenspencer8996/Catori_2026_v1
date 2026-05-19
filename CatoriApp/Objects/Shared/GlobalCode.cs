using CatoriApp.Objects.DragDrop;
namespace CatoriApp.Objects.Shared
{
    public class GlobalCode
    {
        public static DragManager GetDragmanager(Canvas canvas)
        {
             var manager = new DragManager( canvas);
         
            return manager;
        }
    }
}


