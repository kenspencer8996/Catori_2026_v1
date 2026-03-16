using CatoriCity2025WPF.Objects.DragDrop;

namespace CatoriCity2025WPF.Objects
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
