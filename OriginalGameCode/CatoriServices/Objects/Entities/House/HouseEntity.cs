namespace CatoriServices.Objects.Entities.House
{
    public class HouseEntity
    {
        public HouseEntity()
        {
            try
            {
                        
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }
        public HouseEntity(string name, string frontImage, string livingRoomImage,
            string garageImage, string ownerName)
        {
            try
            {
                            _name = name;
                            _frontImage = frontImage?.ToLower() ?? "";
                            _livingRoomImage = livingRoomImage?.ToLower() ?? "";
                            _garageImage = garageImage?.ToLower() ?? "";
                            _ownerName = ownerName ?? "";
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private string _name = "";
        private string _frontImage = "";
        private string _livingRoomImage = "";
        private string _garageImage = "";
        private string _ownerName = "";
        private double _garageButtonLocX = 0;
        private double _garageButtonLocY = 0;
        private double _garageProductsLocX = 0;
        private double _garageProductsLocY = 0;
        private decimal _price = 0;
        private int _forSale = 0;

        public int HouseId { get; set; }
        public int ForSale
        {
            get { return _forSale; }
            set { _forSale = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string FrontImage
        {
            get { return _frontImage; }
            set { _frontImage = value; }
        }
        public decimal Price
        {
            get { return _price; }
            set { _price = value; }
        }
        public string LivingRoomImage
        {
            get { return _livingRoomImage; }
            set { _livingRoomImage = value; }
        }

        public string GarageImage
        {
            get { return _garageImage; }
            set { _garageImage = value; }
        }

        public string OwnerName
        {
            get { return _ownerName; }
            set { _ownerName = value; }
        }

        public double GarageButtonLocX
        {
            get { return _garageButtonLocX; }
            set { _garageButtonLocX = value; }
        }

        public double GarageButtonLocY
        {
            get { return _garageButtonLocY; }
            set { _garageButtonLocY = value; }
        }

        public double GarageProductsLocX
        {
            get { return _garageProductsLocX; }
            set { _garageProductsLocX = value; }
        }

        public double GarageProductsLocY
        {
            get { return _garageProductsLocY; }
            set { _garageProductsLocY = value; }
        }

        // Legacy property for compatibility
        public string ImageFileName
        {
            get { return _frontImage; }
            set { _frontImage = value; }
        }

        // Legacy property for compatibility
        public string ImageLivingRoomFileName
        {
            get { return _livingRoomImage; }
            set { _livingRoomImage = value; }
        }

        // Legacy property for compatibility
        public string ImageGarageFileName
        {
            get { return _garageImage; }
            set { _garageImage = value; }
        }

        public void Add(string name, string frontImage, string livingRoomImage,
            string garageImage, string ownerName)
        {
            try
            {
                            _name = name;
                            _frontImage = frontImage;
                            _livingRoomImage = livingRoomImage;
                            _garageImage = garageImage;
                            _ownerName = ownerName;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }
    }
}
