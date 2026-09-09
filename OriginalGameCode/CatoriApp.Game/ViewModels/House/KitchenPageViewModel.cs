using CatoriApp.Game.Views.Controls;
namespace CatoriApp.Game.ViewModels.House
{
    public class KitchenPageViewModel: ViewmodelBase
    {
        public HouseControl House { get; set; }
        public string KitchenImage
        {
            get
            {
                return House.ImageKitchenFileName;
            }
        }
    }
}



