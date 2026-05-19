using CatoriApp.Views.Controls;
using CatoriApp.ViewModels;

namespace CatoriApp.Objects.Arguments
{
    public class GaragePageViewModel : ViewmodelBase
    {
        public HouseControl House { get; set; }
        public string GarageImage
        {
            get
            {
                return House.ImageGarageFileName;
            }
        }
    }
}

