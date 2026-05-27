using CatoriApp.Core.Objects.DragDrop;
namespace CatoriApp.Core.Objects.Shared
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


