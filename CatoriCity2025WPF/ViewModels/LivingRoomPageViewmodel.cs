using CatoriApp.Objects;
using CatoriApp.ViewModels;
using CatoriApp.Views.Controls;

namespace CatoriApp.Viewmodels
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

