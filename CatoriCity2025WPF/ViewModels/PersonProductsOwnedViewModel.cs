namespace CatoriCity2025WPF.ViewModels
{
    public class PersonProductsOwnedViewModel:ViewmodelBase
    {
        private int _personProductsOwnedId;
        private int _personId;
        private int _shopItemId;
        private int _quantity;
        private string _name = string.Empty;
        private string _imageName = string.Empty;

        public PersonProductsOwnedViewModel()
        {
        }

        public PersonProductsOwnedViewModel(PersonProductsOwnedEntity entity)
        {
            ToModel(entity);
        }

        public int PersonProductsOwnedId
        {
            get => _personProductsOwnedId;
            set
            {
                if (_personProductsOwnedId != value)
                {
                    _personProductsOwnedId = value;
                    OnPropertyChanged(nameof(PersonProductsOwnedId));
                }
            }
        }

        public int PersonId
        {
            get => _personId;
            set
            {
                if (_personId != value)
                {
                    _personId = value;
                    OnPropertyChanged(nameof(PersonId));
                }
            }
        }

        public int ShopItemId
        {
            get => _shopItemId;
            set
            {
                if (_shopItemId != value)
                {
                    _shopItemId = value;
                    OnPropertyChanged(nameof(ShopItemId));
                }
            }
        }

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                }
            }
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
        public PersonProductsOwnedEntity GetEntity()
        {
            return new PersonProductsOwnedEntity
            {
                PersonProductsOwnedId = this.PersonProductsOwnedId,
                PersonId = this.PersonId,
                ShopItemId = this.ShopItemId,
                Quantity = this.Quantity,
                ImageName = this.ImageName,
                Name = this.Name
            };
        }

        public void ToModel(PersonProductsOwnedEntity entity)
        {
            if (entity == null) return;

            PersonProductsOwnedId = entity.PersonProductsOwnedId;
            PersonId = entity.PersonId;
            ShopItemId = entity.ShopItemId;
            Quantity = entity.Quantity;
            Name = entity.Name;
            ImageName = entity.ImageName;
        }
    }
}
