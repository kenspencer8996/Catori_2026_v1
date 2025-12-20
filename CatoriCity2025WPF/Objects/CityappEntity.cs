using CityAppServices.Objects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatoriCity2025WPF.Objects
{
    public class CityappEntity
    {
        public int CityAppNumber { get; set; }
        public List<BusinessEntity> Businesses { get; set; } = new List<BusinessEntity>();

        //public List<BadPersonEntity> BadPersons { get; set; } = new List<BadPersonEntity>();
        public List<PersonEntity> Persons { get; set; } = new List<PersonEntity>();
        public List<PersonImageEntity> PersonImages { get; set; } = new List<PersonImageEntity>();
        //  public List<ResidenceEntity> Residences { get; set; } = new List<ResidenceEntity>();
        //public List<VehicleEntity> Vehicles { get; set; } = new List<VehicleEntity>();
        public List<HouseEntity> Houses { get; set; } = new List<HouseEntity>();
    }
}
