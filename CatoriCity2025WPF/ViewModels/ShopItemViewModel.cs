namespace CatoriApp.ViewModels
{
    public class ShopItemViewModel: ViewmodelBase
    {
        private int _shopItemId;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private decimal _price;
        private string _imageName = string.Empty;
        private string _storeType = string.Empty;
        private string _filePath = string.Empty;
        private double _height;
        private double _width;
        private double _rotationDegree;

        public ShopItemViewModel()
        {
        }

        public ShopItemViewModel(ShopItemEntity entity)
        {
            ToModel(entity);
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

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public decimal Price
        {
            get => _price;
            set
            {
                if (_price != value)
                {
                    _price = value;
                    OnPropertyChanged(nameof(Price));
                }
            }
        }

        public string ImageName
        {
            get => _imageName;
            set
            {
                if (_imageName != value)
                {
                    _imageName = value;
                    OnPropertyChanged(nameof(ImageName));
                }
            }
        }

        public string StoreType
        {
            get => _storeType;
            set
            {
                if (_storeType != value)
                {
                    _storeType = value;
                    OnPropertyChanged(nameof(StoreType));
                }
            }
        }

        public string FilePath
        {
            get => _filePath;
            set
            {
                if (_filePath != value)
                {
                    _filePath = value;
                    OnPropertyChanged(nameof(FilePath));
                }
            }
        }
        public string FilePathAbsolute
        {
            get => System.IO.Path.Combine(GlobalAllApps.ImageFolder, _filePath);
        }
        public double Height
        {
            get => _height;
            set
            {
                if (_height != value)
                {
                    _height = value;
                    OnPropertyChanged(nameof(Height));
                }
            }
        }

        public double Width
        {
            get => _width;
            set
            {
                if (_width != value)
                {
                    _width = value;
                    OnPropertyChanged(nameof(Width));
                }
            }
        }
        
        public double RotationDegree
        {
            get => _rotationDegree;
            set
            {
                if (_rotationDegree != value)
                {
                    _rotationDegree = value;
                    OnPropertyChanged(nameof(RotationDegree));
                }
            }
        }

  
        public ShopItemEntity GetEntity()
        {
            return new ShopItemEntity
            {
                ShopItemId = this.ShopItemId,
                Name = this.Name ?? string.Empty,
                Description = this.Description ?? string.Empty,
                Price = this.Price,
                ImageName = this.ImageName ?? string.Empty,
                StoreType = this.StoreType ?? string.Empty,
                FilePath = this.FilePath ?? string.Empty,
                Height = this.Height,
                Width = this.Width,
                RotationDegree = this.RotationDegree,
            };
        }

        public void ToModel(ShopItemEntity entity)
        {
            if (entity == null) return;

            ShopItemId = entity.ShopItemId;
            Name = entity.Name ?? string.Empty;
            Description = entity.Description ?? string.Empty;
            Price = entity.Price;
            ImageName = entity.ImageName ?? string.Empty;
            StoreType = entity.StoreType ?? string.Empty;
            FilePath = entity.FilePath ?? string.Empty;
            Height = entity.Height;
            Width = entity.Width;
            RotationDegree = entity.RotationDegree;
        }
    }
}

