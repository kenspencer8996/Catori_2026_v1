using CatoriCity2025WPF.Objects;
using CatoriCity2025WPF.ViewModels;
using CatoriCity2025WPF.Views.Controls;

namespace CatoriCity2025WPF.Viewmodels
{
    public partial class LivingRoomPageViewmodel: ViewmodelBase
    {
       public HouseControl House { get; set; }
        public double widthofparent;
        public double heightofparent;
       
        public string LivingRoomImage
        {
            get
            {
                return House.ImageLivingFileName;
            }
        }
        public LivingRoomPageViewmodel()
        {

        }
         
    }
}
