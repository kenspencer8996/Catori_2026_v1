namespace CatoriCity2025WPF.Objects
{
    public class GlobalCode
    {
        public static DragManager GetDragmanager(Canvas canvas)
        {
            //var physicsA = new PhysicsDragController(canvas, p =>
            //{
            //    Canvas.SetLeft(primaryPerson, p.X);
            //    Canvas.SetTop(primaryPerson, p.Y);
            //});

            var dragA = new DragManager( canvas);
         
            return dragA;
        }
    }
}
