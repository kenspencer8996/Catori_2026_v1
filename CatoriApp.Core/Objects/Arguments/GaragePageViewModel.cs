using CatoriApp.Game.Views.Controls;
namespace CatoriApp.Core.Objects.Arguments
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



