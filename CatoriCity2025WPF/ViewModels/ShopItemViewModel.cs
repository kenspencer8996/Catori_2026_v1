namespace CatoriCity2025WPF.ViewModels
{
    public class ShopItemViewModel: ViewmodelBase
    {
        private int _shopItemId;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private decimal _price;
        private string _imageName = string.Empty;
        private string _category = string.Empty;
        private string _filePath = string.Empty;
        private double _height;
        private double _wwidth;
        private double _rotationDegree;
        private double _x;
        private double _y;

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

        public string Category
        {
            get => _category;
            set
            {
                if (_category != value)
                {
                    _category = value;
                    OnPropertyChanged(nameof(Category));
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
            get => _wwidth;
            set
            {
                if (_wwidth != value)
                {
                    _wwidth = value;
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

        public double X
        {
            get => _x;
            set
            {
                if (_x != value)
                {
                    _x = value;
                    OnPropertyChanged(nameof(X));
                }
            }
        }

        public double Y
        {
            get => _y;
            set
            {
                if (_y != value)
                {
                    _y = value;
                    OnPropertyChanged(nameof(Y));
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
                Category = this.Category ?? string.Empty,
                FilePath = this.FilePath ?? string.Empty,
                Height = this.Height,
                Width = this.Width,
                RotationDegree = this.RotationDegree,
                X = this.X,
                Y = this.Y
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
            Category = entity.Category ?? string.Empty;
            FilePath = entity.FilePath ?? string.Empty;
            Height = entity.Height;
            Width = entity.Width;
            RotationDegree = entity.RotationDegree;
            X = entity.X;
            Y = entity.Y;
        }
    }
}
