using CatoriApp.Core.Objects;
using CatoriApp.Game.Views.Controls;
namespace CatoriApp.Game.ViewModels.House
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



