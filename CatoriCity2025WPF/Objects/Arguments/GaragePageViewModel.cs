using CatoriCity2025WPF.Views.Controls;
using CatoriCity2025WPF.ViewModels;

namespace CatoriCity2025WPF.Objects.Arguments
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
