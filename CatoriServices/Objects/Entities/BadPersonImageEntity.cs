namespace CityAppServices.Objects.Entities
{
    public class BadPersonImageEntity
    {
        public BadPersonImageEntity() 
        { 
        }
        private BadPersonTypeEnum _personImageType;
        private string _Name = "";
        private string _FilePath = "";
        private string _ImageType = ""; 
        public BadPersonTypeEnum PersonImageType
        {
            get 
            {
                return _personImageType;
            } 
            set 
            {
                _personImageType = value;
            }
        }
        public string  Name
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
       public string FilePath
        {
            get
            {
                return _FilePath;
            }
            set
            {
                _FilePath = value;
            }
        }
        public string ImageType
        {
            get
            {
                return _ImageType;
            }
            set
            {
                _ImageType = value;
            }
        }
    }
}
