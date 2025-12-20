using CatoriCity2025WPF.Views.Controls;

namespace CatoriCity2025WPF.Objects
{
    public class careventArg
    {
        public careventArg(CarUserControl Car)
        { 
            this.Car = Car;
        }
        public CarUserControl Car { get; set; }
    }
}
