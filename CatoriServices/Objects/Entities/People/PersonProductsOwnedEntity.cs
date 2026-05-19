namespace CatoriServices.Objects.Entities.People
{
    public class PersonProductsOwnedEntity
    {
        private int _personProductsOwnedId = 0;
        private int _personId = 0;
        private int _shopItemId = 0;
        private int _quantity = 0;
        private string _name = string.Empty;
        private string _imageName = string.Empty;

        public PersonProductsOwnedEntity()
        {
        }

        public int PersonProductsOwnedId
        {
            get { return _personProductsOwnedId; }
            set { _personProductsOwnedId = value; }
        }

        public int PersonId
        {
            get { return _personId; }
            set { _personId = value; }
        }

        public int ShopItemId
        {
            get { return _shopItemId; }
            set { _shopItemId = value; }
        }

        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string ImageName
        {
            get { return _imageName; }
            set { _imageName = value; }
        }

        public void Add(int personId, int shopItemId, int quantity, string name, string imagename)
        {
            _personId = personId;
            _shopItemId = shopItemId;
            _quantity = quantity;
            _name = name;
            _imageName = imagename;
        }
    }
}

