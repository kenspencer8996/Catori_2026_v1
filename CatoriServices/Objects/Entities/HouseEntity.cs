namespace CityAppServices.Objects.Entities
{
    public class HouseEntity
    {
        public HouseEntity()
        {

        }
        public HouseEntity(string imageFileName,
            string imageLivingFileName, string imageKitchenFileName,
            string imageGarageFileName)
        {
            _imageFileName = imageFileName.ToLower();
            ImageLivingRoomFileName = imageLivingFileName.ToLower();
            ImageKitchenFileName = imageKitchenFileName.ToLower();
            ImageGarageFileName = imageGarageFileName.ToLower();
        }
        private string _imageFileName = "";
        private string _imageLivingRoomFileName = "";
        private string _imageKitchenFileName = "";
        private string _imageGarageFileName = "";

        public int HouseID { get; set; }
       
        public string ImageFileName
        {
            get
            {
                return _imageFileName;
            }
            set
            {
                _imageFileName = value;
            }
        }
        public string ImageLivingRoomFileName
        {
            get
            {
                return _imageLivingRoomFileName;
            }
            set
            {
                _imageLivingRoomFileName = value;
            }
        }


        public string ImageKitchenFileName
        {
            get
            {
                return _imageKitchenFileName;
            }
            set
            {
                _imageKitchenFileName = value;
            }
        }
        public string ImageGarageFileName
        {
            get
            {
                return _imageGarageFileName;
            }
            set
            {
                _imageGarageFileName = value;
            }
        }
    }
}
