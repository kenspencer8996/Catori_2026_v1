using CatoriApp.Objects;
using CatoriApp.Views.Controls;
namespace CatoriApp.ViewModels.House
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



