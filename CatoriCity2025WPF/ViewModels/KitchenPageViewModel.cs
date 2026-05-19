using CatoriApp.ViewModels;
using CatoriApp.Views.Controls;

namespace CatoriApp.Viewmodels
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

