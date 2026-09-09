using CatoriServices.Objects.Entities;
using System.Xml.Linq;
namespace CatoriApp.Game.ViewModels.Police
{
    public class CarPoliceViewModel : BaseViewModel
    {
        public int Id { get; set; }
        public string CarName { get; set; }
        public string ImageName { get; set; }
        public string CarType { get; set; }

        public PoliceCarEntity GetEntity()
        {
            PoliceCarEntity entity = new PoliceCarEntity();
            entity.PoliceCarId = Id;
            entity.CarName = CarName;
            entity.ImageName = ImageName;
            entity.CarType = CarType;
         
            return entity;
        }
        public void ToModel(PoliceCarEntity entity)
        {
            Id = entity.PoliceCarId;
            CarName = entity.CarName;
            ImageName = entity.ImageName;
            CarType = entity.CarType;
            
        }
    }
}



