using CatoriCity2025WPF.Objects.Arguments;
using CatoriCity2025WPF.ViewModels;
using CatoriCity2025WPF.Views.Controls;
using CatoriServices.Objects.Entities;
using CityAppServices.Objects.Entities;
using System.Windows.Threading;

namespace CatoriCity2025WPF.Objects
{
    public class PersonViewModel : PersonViewModelBase
    {
        public event EventHandler<BadPersonTimerFiredEventArg> ShowPersonTimerFired;

        decimal _currentPay;
        DispatcherTimer _showTimer;
        HouseControl _host;
        PersonControl _PersonControl;
        BadPersonControl _badPersonControl;
        private Int32 _personId;
        public CampfireSpotLocationViewmodel CampfireSpotLocation { get; set; }
        public List<int> CampFirePath { get; set; }
        private string _staticImageFilePath;
        public double lastlocationX { get; set; }
        public double lastlocationY { get; set; }
        private string _personcurrentImage;
        public string StaticImageFilePath
        {
            get
            {
                if (_staticImageFilePath == null || _staticImageFilePath == "")
                {
                    _staticImageFilePath = System.IO.Path.Combine(ImagesFolder, FileNameOptional);
                }
                return _staticImageFilePath;
            }
            set
            {
                _staticImageFilePath = value;
            }

        }
        public PersonControl PersonHost
        {
            get { return _PersonControl; }
            set
            {
                if (_PersonControl != value)
                {
                    _PersonControl = value;
                }
            }
        }
       
        internal BadPersonControl BadPersonHost
        {
            get { return _badPersonControl; }
            set
            {
                if (_badPersonControl != value)
                {
                    _badPersonControl = value;
                }
            }
        }
       
        public Int32 PersonId
        {
            get { return _personId; }
            set
            {
                if (_personId != value)
                {
                    _personId = value;
                }
            }
        }
       
        public bool IsUser
        {
            get
            {
                return _IsUser;
            }
            set
            {
                _IsUser = value;
                OnPropertyChanged("IsUser");
            }
        }

       
        public decimal Funds
        {
            get
            {
                return _Funds;
            }
            set
            {
                _Funds = value;
                OnPropertyChanged("Funds");
            }
        }
       
        private bool _IsUser = false;
        private decimal _Funds = 0;
        public Type HostType { get; set; }
        private PersonEnum _PersonRole = PersonEnum.Individual;
        private int _TimerSeconds = 49;
        private int _currentImageKey = 0;
        private bool _active = false;
        private string _name;
        private string _imagesFolder;
        private string _fileNameOptional;

        public string FileNameOptional
        {
            get
            {
                return _fileNameOptional;
            }
            set
            {
                if (_fileNameOptional != value)
                {
                    _fileNameOptional = value;
                }
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                }
            }
        }
       
        public bool IsActive
        {
            get
            {
                return _active;
            }
            set
            {
                _active = value;
            }
        }
       
        public PersonEnum PersonRole
        {
            get
            {
                return _PersonRole;
            }
            set
            {
                _PersonRole = value;
            }
        }
        public string ImagesFolder
        {
            get
            {
                return _imagesFolder;
            }
            set
            {
                _imagesFolder = value;
            }
        }
        
        public PersonEnum PersonType { get; set; }
       
        public bool Girl { get; set; }
       
        public PersonViewModel BadPerson { get; internal set; }

        public void ToModel(PersonEntity person)
        {
            Name = person.Name;
            PersonId = person.PersonId ;
            //CurrentPay = person.currentPay;
            _PersonRole = person.PersonRole;
            Funds = person.Funds;
            IsActive = person.Active;
            IsUser = person.IsUser;
            
            ImagesFolder = person.ImagesFolder;
            FileNameOptional = person.FileNameOptional;
        }
        public PersonEntity GetEntity()
        {
            PersonEntity person = new PersonEntity();
            person.Name = Name;
            person.PersonId =PersonId;
            //CurrentPay = person.currentPay;
            person.PersonRole = _PersonRole;
            person.Active = IsActive;
            person.IsUser = IsUser;
            person.ImagesFolder = ImagesFolder;
            person.FileNameOptional = FileNameOptional;
            person.Funds = Funds;
            return person;
        }
        public List<PersonImageViewModel> Images { get; set; } = new List<PersonImageViewModel>();

        public void Add(string name,bool girl, PersonEnum Individual,string imagesFolder)
        {
            PersonType = PersonEnum.Individual;
            _name = name;
            this.Girl = girl;
            _imagesFolder = imagesFolder;
        }
        public List<string> WalkingRightImages = new List<string>();
        public List<string> WalkingLeftImages = new List<string>();
        public List<string> WalkingRightBagImages = new List<string>();
        public List<string> WalkingLeftBagImages = new List<string>();
        public List<string> DiggingImages = new List<string>();
        public List<string> SittingImages = new List<string>();
        public List<string> JumpingImages = new List<string>();
        public List<string> LayingDownImages = new List<string>();
        public string SittingFacingLeftImage = "NA";
        public string SittingFacingRightImage = "NA";
        public string SittingFacingRearImage = "NA";
        public string SittingFacingFrontImage = "NA";

        public void SetupImageLists()
        {
            WalkingRightImages = GetImageList(BadPersonImageTypeEnum.WalkingRight);
            WalkingLeftImages = GetImageList(BadPersonImageTypeEnum.WalkingLeft);
            WalkingRightBagImages = GetImageList(BadPersonImageTypeEnum.WalkingRightBag);
            WalkingLeftBagImages = GetImageList(BadPersonImageTypeEnum.WalkingLeftBag);
            DiggingImages = GetImageList(BadPersonImageTypeEnum.Digging);
            SittingImages = GetImageList(BadPersonImageTypeEnum.Sitting);
            JumpingImages = GetImageList(BadPersonImageTypeEnum.Jumping);
            LayingDownImages = GetImageList(BadPersonImageTypeEnum.LayingDown);
            try
            {
                string ApproachPath = System.IO.Path.Combine(ImagesFolder, "Approach");
                string[] sittingImage = System.IO.Directory.GetFiles(ApproachPath);
                var sittingLeft = sittingImage.Where(i => i.Contains("Sitting_L")).FirstOrDefault();
                var sittingRight = sittingImage.Where(i => i.Contains("Sitting_Right")).FirstOrDefault();
                var sittingRear = sittingImage.Where(i => i.Contains("Sitting_Rear")).FirstOrDefault();
                var sittingFront = sittingImage.Where(i => i.Contains("Sitting_Front")).FirstOrDefault();
                if (sittingLeft != null) SittingFacingLeftImage = sittingLeft;
                if (sittingRight != null) SittingFacingRightImage = sittingRight;
                if (sittingRear != null) SittingFacingRearImage = sittingRear;
                if (sittingFront != null) SittingFacingFrontImage = sittingFront;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<string> GetImageList(BadPersonImageTypeEnum badPersonImageType)
        {
           List<string> imagelist = new List<string>();
           IEnumerable<PersonImageViewModel> images = new List<PersonImageViewModel>(); 
            switch (badPersonImageType)
            {
                case BadPersonImageTypeEnum.WalkingRight:
                    images = Images.Where(i => i.Name.StartsWith("BadGirl_WalkR"));
                    break;
                case BadPersonImageTypeEnum.WalkingLeft:
                    images = Images.Where(i => i.Name.StartsWith("BadGirl_WalkL"));
                    break;
                case BadPersonImageTypeEnum.WalkingRightBag:
                    images = Images.Where(i => i.Name.StartsWith("BadGirl_Bag_WalkR"));
                    break;
                case BadPersonImageTypeEnum.WalkingLeftBag:
                    images = Images.Where(i => i.Name.StartsWith("BadGirl_Bag_WalkL"));
                    break;
                case BadPersonImageTypeEnum.Sitting:
                    images = Images.Where(i => i.Name.StartsWith("BadGirl_Sitting"));
                    break;
                case BadPersonImageTypeEnum.LayingDown:
                    images = Images.Where(i => i.Name.StartsWith("BadGirl_LayingDown"));
                    break;
                case BadPersonImageTypeEnum.Jumping:
                    images = Images.Where(i => i.Name.StartsWith("BadGirl_Jumping"));
                    break;
                case BadPersonImageTypeEnum.Digging:
                    images = Images.Where(i => i.Name.StartsWith("BadGirl_Digging"));
                    break;
                default:
                    break;
            }
            foreach (var item in images)
            {
                imagelist.Add(item.FilePath);
            }
            return imagelist;
        }
        //internal void StartMainAnimation()
        //{
        //    _badPersonControl.StartMainAnimation();
        //}

    }
}
