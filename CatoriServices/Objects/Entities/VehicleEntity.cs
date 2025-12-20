namespace CityAppServices.Objects.Entities
{
    public class VehicleEntity 
    {
        private string _Name;
        private bool _IsSelected;
        private bool _IsUser;
        private decimal _Cost;
        private string _ImageName;
        private VechicleTypeEnum _VechicleType;

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

        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                _IsSelected = value;
            }
        }
        public bool IsUser
        {
            get
            {
                return _IsUser;
            }
            set
            {
                _IsUser = value;
            }
        }
        public decimal Cost
        {
            get
            {
                return _Cost;
            }
            set
            {
                _Cost = value;
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
        public VechicleTypeEnum VechicleType
        {
            get
            {
                return _VechicleType;
            }
            set
            {
                _VechicleType = value;
            }
        }

    }
}
