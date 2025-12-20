using CatoriCity2025WPF.ViewModels;
using CatoriCity2025WPF.Views.Controls;

namespace CatoriCity2025WPF.Viewmodels
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
