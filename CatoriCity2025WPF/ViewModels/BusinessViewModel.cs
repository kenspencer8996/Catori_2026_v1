using CatoriCity2025WPF.ViewModels;
using CatoriCity2025WPF.Views.Controls;
using CityAppServices.Objects.Entities;

namespace CatoriCity2025WPF.Viewmodels
{
    public class BusinessViewModel: ViewmodelBase
    {
        public PersonControl BadPerson { get; set; }
        private string _Name = "";
        private decimal _EmployeePayHour = 0;
        private string _ImageName = "";
        private BusinessTypeEnum _BusinessType;
        public string Bankkey { get; set; }
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }
        public decimal EmployeePayHour
        {
            get
            {
                return _EmployeePayHour;
            }
            set
            {
                _EmployeePayHour = value;
            }
        }
        public string ImageName
        {
            get
            {
                return _ImageName;
            }
            set
            {
                _ImageName = value;
            }
        }
        public string ImageNameWOExtension
        {
            get
            {
                return System.IO.Path.GetFileNameWithoutExtension( _ImageName);
            }
        }
        public double X { get; set; }
        public double Y { get; set; }
        public BusinessTypeEnum BusinessType
        {
            get
            {
                return _BusinessType;
            }
            set
            {
                _BusinessType = value;
            }
        }
        public void Add(string name, decimal payrate,string imageName, BusinessTypeEnum businessType)
        {
            _Name += name;
            _EmployeePayHour = payrate;
            _BusinessType = businessType;
            _ImageName = imageName;
        }
        public void SetImage(string imageName)
        {
            _ImageName = imageName;
        }
        public BusinessEntity GetEntity()
        {
            BusinessEntity entity = new BusinessEntity();
            entity.BusinesskeyImageNameWOExtension = ImageNameWOExtension;
            entity.BusinessType = _BusinessType;
            entity.EmployeePayHour = _EmployeePayHour;
            entity.Name = _Name;
            return entity;
        }
        public void ToModel(BusinessEntity entity)
        {
             _BusinessType = entity.BusinessType;
            _EmployeePayHour = entity.EmployeePayHour ;
            _Name = entity.Name ;
            _ImageName = entity.ImageName;
            
        }
      
    }
}
