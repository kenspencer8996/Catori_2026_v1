namespace CityAppServices.Objects.Entities
{
    public class ResidenceEntity
    {
        private string _Name;
        private bool _IsHome;
        private string _ImageName;
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
        public bool IsHome
        {
            get
            {
                return _IsHome;
            }
            set
            {
                _IsHome = value;
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
    }
}
