using CatoriCity2025WPF.Objects;
using CityAppServices.Objects.Entities;
using System.Windows.Controls;

namespace CatoriCity2025WPF.ViewModels
{
    public class HouseViewModel : ViewmodelBase
    {
        private bool _isVisible = true;
       
      
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                _isVisible = value;
                OnPropertyChanged("IsVisible");
            }
        }
        private string _personcurrentImage;

        private string _Name = "";
        private bool _IsUserHouse;
        private string _imageFileName = "";
        private string _imageLivingRoomFileName = "";
        private string _imageKitchenFileName = "";
        private string _imageGarageFileName = "";
        private string _ownerName = "";

        public int HouseID { get; set; }
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
        public string OwnerName

        {
            get
            {
                return _ownerName;
            }
            set
            {
                _ownerName = value;
            }
        }
        public bool IsUserHouse
        {
            get
            {
                return _IsUserHouse;
            }
            set
            {
                _IsUserHouse = value;
            }
        }


        public string NameAsControlName
        {
            get
            {
                return _Name.Replace(" ", "");
            }

        }
        public string HouseImageFileName
        {
            get
            {
                string filename = _imageFileName;
                if (filename.StartsWith(GlobalStuff.ImageFolder))
                    filename = _imageFileName;
                else
                    filename = System.IO.Path.Combine(GlobalStuff.ImageFolder, _imageFileName);
                return filename;
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
                string filename = _imageLivingRoomFileName;
                if (filename.StartsWith(GlobalStuff.ImageFolder))
                    filename = _imageLivingRoomFileName;
                else
                    filename = System.IO.Path.Combine(GlobalStuff.ImageFolder, _imageLivingRoomFileName);
                return filename;
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
                string filename = _imageKitchenFileName;
                if (filename.StartsWith(GlobalStuff.ImageFolder))
                    filename = _imageKitchenFileName;
                else
                    filename = System.IO.Path.Combine(GlobalStuff.ImageFolder, _imageKitchenFileName);
                return filename;
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
                string filename = _imageGarageFileName;
                if (filename.StartsWith(GlobalStuff.ImageFolder))
                    filename = _imageGarageFileName;
                else
                    filename = System.IO.Path.Combine(GlobalStuff.ImageFolder, _imageGarageFileName);
                return filename;
            }
            set
            {
                _imageGarageFileName = value;
            }
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
                _personcurrentImage = value;
                OnPropertyChanged("PersonCurrentImagePath");

            }
        }
        public Image CurrentImage
        {
            get
            {
                return UIUtility.GetImageControl(_personcurrentImage, 50, 50, 0);
            }
        }
        public HouseEntity GetEntity()
        {
            HouseEntity entity = new HouseEntity();
            entity.HouseID = HouseID;
            entity.ImageKitchenFileName = ImageKitchenFileName;
            entity.ImageGarageFileName = ImageGarageFileName;
            entity.ImageLivingRoomFileName = ImageLivingRoomFileName;
            return entity;
        }
        public void ToModel(HouseEntity entity)
        {
            HouseID = entity.HouseID;
            Name = System.IO.Path.GetFileName( entity.ImageFileName);
            HouseImageFileName = entity.ImageFileName;
            
            ImageKitchenFileName = entity.ImageKitchenFileName;
            ImageGarageFileName = entity.ImageGarageFileName;
            ImageLivingRoomFileName = entity.ImageLivingRoomFileName;
        }
       
    }
}
