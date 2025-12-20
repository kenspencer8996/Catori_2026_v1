using System.Windows.Shapes;

namespace CatoriCity2025WPF.Objects
{
    public class GeometryObjectForAnimationsModel
    {
        public Stack<Shape> Shapes { get; set; }
   
        public GeometryObjectForAnimationsModel()
        {
            Shapes = new Stack<Shape>();
        }
        public void AddShape(Shape shape)
        {
            Shapes.Push(shape);
        }
    }
}
