using CatoriApp.Views.Controls;

namespace CatoriApp.Objects
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

