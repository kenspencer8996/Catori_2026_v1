using CatoriCity2025WPF.Objects;
using CityAppServices.Objects.Entities;
using System.Windows.Controls;

namespace CatoriCity2025WPF.ViewModels
{
    public class HouseViewModel : ViewmodelBase
    {
        private bool _isVisible = true;
        private string _personcurrentImage;
        private string _name = "";
        private string _frontImage = "";
        private string _livingRoomImage = "";
        private string _garageImage = "";
        private string _ownerName = "";
        private decimal _price = 0;
        private double _garageButtonLocX = 0;
        private double _garageButtonLocY = 0;
        private double _garageProductsLocX = 0;
        private double _garageProductsLocY = 0;

        public int HouseId { get; set; }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string OwnerName
        {
            get { return _ownerName; }
            set
            {
                if (_ownerName != value)
                {
                    _ownerName = value;
                    OnPropertyChanged(nameof(OwnerName));
                }
            }
        }

        public decimal Price
        {
            get { return _price; }
            set
            {
                if (_price != value)
                {
                    _price = value;
                    OnPropertyChanged(nameof(Price));
                }
            }
        }

        public string PriceFormatted
        {
            get { return _price.ToString("C"); }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged(nameof(IsVisible));
                }
            }
        }

        public double GarageButtonLocX
        {
            get { return _garageButtonLocX; }
            set
            {
                if (_garageButtonLocX != value)
                {
                    _garageButtonLocX = value;
                    OnPropertyChanged(nameof(GarageButtonLocX));
                }
            }
        }

        public double GarageButtonLocY
        {
            get { return _garageButtonLocY; }
            set
            {
                if (_garageButtonLocY != value)
                {
                    _garageButtonLocY = value;
                    OnPropertyChanged(nameof(GarageButtonLocY));
                }
            }
        }

        public double GarageProductsLocX
        {
            get { return _garageProductsLocX; }
            set
            {
                if (_garageProductsLocX != value)
                {
                    _garageProductsLocX = value;
                    OnPropertyChanged(nameof(GarageProductsLocX));
                }
            }
        }

        public double GarageProductsLocY
        {
            get { return _garageProductsLocY; }
            set
            {
                if (_garageProductsLocY != value)
                {
                    _garageProductsLocY = value;
                    OnPropertyChanged(nameof(GarageProductsLocY));
                }
            }
        }

        public string NameAsControlName
        {
            get { return _name.Replace(" ", ""); }
        }

        public string HouseImageFileName
        {
            get
            {
                string filename = _frontImage;
                if (string.IsNullOrEmpty(filename))
                    return filename;

                if (filename.StartsWith(GlobalStuff.ImageFolder))
                    filename = _frontImage;
                else
                    filename = System.IO.Path.Combine(GlobalStuff.ImageFolder, "Houses", _frontImage);
                return filename;
            }
            set
            {
                if (_frontImage != value)
                {
                    _frontImage = value;
                    OnPropertyChanged(nameof(HouseImageFileName));
                }
            }
        }

        public string ImageLivingRoomFileName
        {
            get
            {
                string filename = _livingRoomImage;
                if (string.IsNullOrEmpty(filename))
                    return filename;

                if (filename.StartsWith(GlobalStuff.ImageFolder))
                    filename = _livingRoomImage;
                else
                    filename = System.IO.Path.Combine(GlobalStuff.ImageFolder, "Houses", _livingRoomImage);
                return filename;
            }
            set
            {
                if (_livingRoomImage != value)
                {
                    _livingRoomImage = value;
                    OnPropertyChanged(nameof(ImageLivingRoomFileName));
                }
            }
        }

        public string ImageGarageFileName
        {
            get
            {
                string filename = _garageImage;
                if (string.IsNullOrEmpty(filename))
                    return filename;

                if (filename.StartsWith(GlobalStuff.ImageFolder))
                    filename = _garageImage;
                else
                    filename = System.IO.Path.Combine(GlobalStuff.ImageFolder, "Houses", _garageImage);
                return filename;
            }
            set
            {
                if (_garageImage != value)
                {
                    _garageImage = value;
                    OnPropertyChanged(nameof(ImageGarageFileName));
                }
            }
        }

        // Legacy property for compatibility
        public string ImageKitchenFileName
        {
            get { return ""; }
            set { }
        }

        public string PersonCurrentImagePath
        {
            get
            {
                string path = Imagehelper.GetImagePath(_personcurrentImage);
                return path;
            }
            set
            {
                if (_personcurrentImage != value)
                {
                    _personcurrentImage = value;
                    OnPropertyChanged(nameof(PersonCurrentImagePath));
                }
            }
        }

        public Image CurrentImage
        {
            get { return UIUtility.GetImageControl(_personcurrentImage, 50, 50, 0); }
        }

        public HouseEntity GetEntity()
        {
            HouseEntity entity = new HouseEntity();
            entity.HouseId = HouseId;
            entity.Name = Name;
            entity.Price = Price;
            entity.FrontImage = _frontImage;
            entity.LivingRoomImage = _livingRoomImage;
            entity.GarageImage = _garageImage;
            entity.OwnerName = OwnerName;
            entity.GarageButtonLocX = GarageButtonLocX;
            entity.GarageButtonLocY = GarageButtonLocY;
            entity.GarageProductsLocX = GarageProductsLocX;
            entity.GarageProductsLocY = GarageProductsLocY;
            return entity;
        }

        public void ToModel(HouseEntity entity)
        {
            if (entity == null) return;

            HouseId = entity.HouseId;
            Name = entity.Name ?? "";
            Price = entity.Price;
            _frontImage = entity.FrontImage ?? "";
            _livingRoomImage = entity.LivingRoomImage ?? "";
            _garageImage = entity.GarageImage ?? "";
            OwnerName = entity.OwnerName ?? "";
            GarageButtonLocX = entity.GarageButtonLocX;
            GarageButtonLocY = entity.GarageButtonLocY;
            GarageProductsLocX = entity.GarageProductsLocX;
            GarageProductsLocY = entity.GarageProductsLocY;

            // Support legacy property access
            if (!string.IsNullOrEmpty(entity.ImageFileName))
                _frontImage = entity.ImageFileName;
            if (!string.IsNullOrEmpty(entity.ImageLivingRoomFileName))
                _livingRoomImage = entity.ImageLivingRoomFileName;
            if (!string.IsNullOrEmpty(entity.ImageGarageFileName))
                _garageImage = entity.ImageGarageFileName;
        }
    }
}