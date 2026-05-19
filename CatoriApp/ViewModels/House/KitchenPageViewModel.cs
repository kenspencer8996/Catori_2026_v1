using CatoriApp.Views.Controls;
namespace CatoriApp.ViewModels.House
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



