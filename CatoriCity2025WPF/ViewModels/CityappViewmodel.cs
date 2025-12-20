using CatoriCity2025WPF.Objects;
using CatoriCity2025WPF.ViewModels;
using CityAppServices.Objects.Entities;

namespace CatoriCity2025WPF.Viewmodels
{
    public class CityappViewmodel
    {
        public int CityAppNumber { get; set; }
        public List<BusinessViewModel> Businesses { get; set; } = new List<BusinessViewModel>();

        public List<PersonViewModel> BadPersons 
        { 
            get
            {
                var badguys = from p in Persons where
                              p.PersonRole == PersonEnum.BadPerson select p;
                if (badguys.Any())
                {
                    return badguys.ToList();
                }
                else
                    return new List<PersonViewModel>();
            }
        }
        public List<PersonViewModel> Persons { get; set; } = new List<PersonViewModel>();
        public List<PersonImageViewModel> PersonImages { get; set; } = new List<PersonImageViewModel>();
        public List<ImageViewModel> Images { get; set; } = new List<ImageViewModel>();
        //  public List<ResidenceEntity> Residences { get; set; } = new List<ResidenceEntity>();
        //public List<VehicleEntity> Vehicles { get; set; } = new List<VehicleEntity>();
        public List<HouseViewModel> Houses { get; set; } = new List<HouseViewModel>();
        public List<SettingEntity> Settings { get; internal set; }
    }
}
