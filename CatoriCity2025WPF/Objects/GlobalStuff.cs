using CatoriCity2025WPF.Objects.Arguments;
using CatoriCity2025WPF.Objects.Services;
using CatoriCity2025WPF.Viewmodels;
using CatoriCity2025WPF.ViewModels;
using CatoriCity2025WPF.Views.Controls;
using CatoriServices.Objects;
using CatoriServices.Objects.Entities;
using CityAppServices.Objects;
using CityAppServices.Objects.Entities;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace CatoriCity2025WPF.Objects
{
    internal class GlobalStuff
    {
      
        public static MainWindow MainView { get; set; }   
        public static double MainViewWidth = 0;
        public static double MainViewHeight = 0;
        public static int buildingsize = 90;
        public static int BuildingLocationBuffer = 2;
        public static bool ShowAllBordersIfAvailable = false;
        public static bool ShowPaths = false;
        public static Int32 PathSegmentsToRun = 1;
        public static Int32 PathSegmentsInnerToCreate = 1;
        public static int HouseMagFactor = 5;
        public static List<Int32> landscapeObjectGroupIds;
        public static ObservableCollection<LandscapeObjectViewModel> LandscapeObjects { get; set; }
        public static List<LandscapeObjectControl> LandscapeUCs;
        public static string Burglaralarmfile = "";
       
        public static int BadGuyWith { get; set; } = 30;
        public static int BadGuyHeight { get; set; } = 70;
        // public static GeometryHelper GlobalGeometryHelper;
        public static List<int> ApproachesUsed = new List<int>();

        public static BusinessViewModel GetBusinessViewModelByName(string name)
        {
            BusinessViewModel result = new BusinessViewModel();
            var found = from f in Businesses
                        where f.Name == name
                        select f;
            if (found != null && found.Any())
                result = found.FirstOrDefault();
            return result;
        }
        public static BankViewModel GetBankViewModelByName(string name)
        {
            BankViewModel result = new BankViewModel();
            var found = from f in Banks
                        where f.Name == name
                        select f;
            if (found != null && found.Any())
                result = found.FirstOrDefault();
            return result;
        }
        public static List<BusinessViewModel> GetBusinessViewModels()
        {
            var business = from b in GlobalStuff.Businesses
                           where b.BusinessType == BusinessTypeEnum.Financial
                           select b;
            List<BusinessViewModel> results = business.ToList();
            return results;
        }
      
        //___________________________________________________
        //Street Intersections
        public static LocationXYEntity IntersectuonYouYodel { get; set; } = new LocationXYEntity();
        public static LocationXYEntity IntersectuonYouTea { get; set; } = new LocationXYEntity();
        public static LocationXYEntity IntersectuonYouMoo { get; set; } = new LocationXYEntity();
        public static LocationXYEntity IntersectuonMikYodel { get; set; } = new LocationXYEntity();
        public static LocationXYEntity IntersectuonMikMoo { get; set; } = new LocationXYEntity();
        public static LocationXYEntity IntersectuonMikTea { get; set; } = new LocationXYEntity();
        //___________________________________________________
        public static double StreetWidth = 50;
        public static List<BankViewModel> Banks { get; set; } = new List<BankViewModel>();
        public static List<BusinessViewModel> Businesses { get; set; } = new List<BusinessViewModel>();
        public static List<DepositViewModel> Deposits { get; set; } = new List<DepositViewModel>();
        public static List<LandscapeObjectControl> LandscapeObjectGroupIds { get; set; } = new List<LandscapeObjectControl>();
        public static List<LandscapeObjectControl> LandscapeObjectApproachNextControls { get; set; } = new List<LandscapeObjectControl>();
        public static BusinessService businessService { get; set; } = new BusinessService();
        public static ImageService imageService { get; set; } = new ImageService();
        public static PersonImageService personimageService { get; set; } = new PersonImageService();
        public static PersonService personService { get; set; } = new PersonService();
        public static SettingService settingService { get; set; } = new SettingService();
        
        public static List<ObjectLocationPathEntity> PathsList = new List<ObjectLocationPathEntity>();
        public static List<ObjectLocationPathEntity> EscapePathsList = new List<ObjectLocationPathEntity>();
        internal static List<ImageTypeEntity> HouseImageList { get; set; } = new List<ImageTypeEntity>();
        internal static List<ImageTypeEntity> VehicleSalesImageList { get; set; } = new List<ImageTypeEntity>();
        internal static List<ImageTypeEntity> RetailImageList { get; set; } = new List<ImageTypeEntity>();
        internal static List<ImageTypeEntity> FinancialImageList { get; set; } = new List<ImageTypeEntity>();
        internal static List<ImageTypeEntity> RoomImageList { get; set; } = new List<ImageTypeEntity>();
        internal static List<ImageTypeEntity> BadguyImageList { get; set; } = new List<ImageTypeEntity>();
        internal static List<ImageTypeEntity> PersonImageList { get; set; } = new List<ImageTypeEntity>();
        internal static List<ImageTypeEntity> VechileImageList { get; set; } = new List<ImageTypeEntity>();
        internal static List<ImageTypeEntity> CarlotImageList { get; set; } = new List<ImageTypeEntity>();
        internal static List<ImageTypeEntity> ClassroomImageList { get; set; } = new List<ImageTypeEntity>();
        internal static List<ImageTypeEntity> FactoryImageList { get; set; } = new List<ImageTypeEntity>();
        public static List<int> TimingsRandom { get; set; }
        internal static List<ImageTypeEntity> GarageImageList { get; set; } = new List<ImageTypeEntity>();
        private static Brush PathStrokBrush1 = Brushes.Black;
        private static Brush PathStrokBrush2 = Brushes.Red;
        private static int PathStrokeColorCounter = 0;
        public void SendShowBaloonMessage(string title, string caption, BaloonLocationEnum location)
        {
            BaloonMesssage baloonmsg = new BaloonMesssage
            {
                Caption = caption,
                Title = title,
                Location = location
            };
            WeakReferenceMessenger.Default.Send(baloonmsg); 
        }
        internal static void WriteDebugOutput(string message, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            if (message.EndsWith(Environment.NewLine) == false)
                message += Environment.NewLine;
            cLogger.Log(memberName + ":" + message);
        }
        public static MainWindowViewModel mainWindowViewModel { get; set; }
        public static Brush PathStrokBrush
        { 
            get
            {
                bool evenno = PathStrokeColorCounter % 2 != 0;
                PathStrokeColorCounter++;
                if (evenno)
                    return PathStrokBrush1;
                else
                    return PathStrokBrush2;
            }
        } 
        public static LocationXYEntity GetLocationXY(double x, double y)
        {
            LocationXYEntity loc = new LocationXYEntity();
            loc.x = x;
            loc.y = y;
            return loc;
        }
        //public static Image GetImage(string imageName, int height,int width,int zindex)
        //{
        //    Image img = new Image();
        //    img.Source = Imagehelper.GetImageSource(imageName);
        //    img.HeightRequest = height;
        //    img.WidthRequest = width;
        //    img.ZIndex = zindex;
        //    return img;
        //}
        public static List<PersonViewModel> BadPersons
        {
            get
            {
                var badguys = from p in AllPersons
                              where
                              p.PersonRole == PersonEnum.BadPerson
                              select p;
                if (badguys.Any())
                {
                    return badguys.ToList();
                }
                else
                    return new List<PersonViewModel>();
            }
        }
        public static List<PersonViewModel> AllPersons { get; set; } = new List<PersonViewModel>();
        public static List<PersonImageViewModel> PersonImages { get; set; } = new List<PersonImageViewModel>();
        public static List<ImageViewModel> Images { get; set; } = new List<ImageViewModel>();
        //  public List<ResidenceEntity> Residences { get; set; } = new List<ResidenceEntity>();
        //public List<VehicleEntity> Vehicles { get; set; } = new List<VehicleEntity>();
        public static List<HouseViewModel> Houses { get; set; } = new List<HouseViewModel>();
        public static string _imageFolder = "C:\\Develpoment\\Gaming\\Catori2026\\Catori_2026_v1\\Images";
        
        public static string ImageFolder
        {
            set
            {
                _imageFolder = value;
            }
            get
            {
                return _imageFolder;
            }
        }
        public static int cityforest1x = 0;
        public static int cityforest1y = 0;
        public static int citybush1x = 0;
        public static int citybush1y = 0;
        public static int citybush2x = 0;
        public static int citybush2y = 0;
        public static int Tentx = 550;
        public static int Tenty = 230;
        public static void SetVariables()
        {
            Tentx = 550;
            Tenty = 230;
            citybarn1Width = 75;
            citybarn1Height = 75;
            cityshed1Width = 95;
            cityshed1Height = 50;
            cityforestWidth = 305;
            cityforestHeight = 155;
            citybush1Width = 60;
            citybush1Height = 50;
            citybush2Width = 50;
            citybush2Height = 50;
        }
        public static FactoryInteriorUC factoryInteriorControl = new FactoryInteriorUC();
        public static void SetFactoryWorking(string personImagePath)
        {
            factoryInteriorControl.Visibility = System.Windows.Visibility.Visible;
            factoryInteriorControl.StartWorking(personImagePath);
        }
        public static void SetFactoryNotWorking()
        {
            factoryInteriorControl.Visibility = System.Windows.Visibility.Hidden;
        }
        public static int BadGuyStartY
        {
            get
            {
                return Tenty + 45;
            }
        }
        public static int BadGuyStartX
        {
            get
            {
                return Tentx + 10;
            }
        }
        public static int citybarn1Width;
        public static int citybarn1Height;
        public static int cityshed1Width;
        public static int cityshed1Height;
        public static int cityforestWidth;
        public static int cityforestHeight;
        public static int citybush1Width;
        public static int citybush1Height;
        public static int citybush2Width;
        public static int citybush2Height;
        public static List<BankControl> BusinessControlList { get; set; } = new List<BankControl>();

        internal static MapPositionEntity Mappositions { get; set; } = new MapPositionEntity();

        public static BankControl GetFinancialBusinessControlForRobber(string RobberName)
        {
            BankControl BusinessControl = null;
            var financebus = from f in BusinessControlList
                             where
                       f.businessName.Contains("Bank") | f.businessName.Contains("Fin.") &&
                       f.RobberName != RobberName
                             select f;
            if (financebus.Any())
                BusinessControl = financebus.First();
            return BusinessControl;
        }
        public static BankControl GetFinancialBusinessControlByModel(LandscapeObjectViewModel model)
        {
            
            BankControl BusinessControl =null;
            var financebus = from f in BusinessControlList
                             where
                       f.businessName == model.Name
                             select f;
            if (financebus.Any())
                BusinessControl = financebus.First();
            return BusinessControl;
        }
        public static BankControl GetFinancialBusinessControlByRobber(string RobberName)
        {
            BankControl BusinessControl = null;
            var financebus = from f in BusinessControlList
                             where
                       f.RobberName == RobberName
                             select f;
            if (financebus.Any())
                BusinessControl = financebus.First();
            return BusinessControl;
        }
        public static void Addbusinessntity(
          string name, int payrate, BusinessTypeEnum businessType)
        {
            Random rand = new Random();
            BusinessViewModel busiess = new BusinessViewModel();
            ImageTypeEntity imageType = new ImageTypeEntity();
            int index;
            switch (businessType)
            {
                case BusinessTypeEnum.Government:
                    //int index = rand.Next(bus.Count);
                    //questions[index];
                    break;
                case BusinessTypeEnum.Retail:
                    index = rand.Next(RetailImageList.Count);
                    if (index > -1 && RetailImageList.Any())
                        imageType = RetailImageList[index];
                    break;
                case BusinessTypeEnum.Financial:
                    index = rand.Next(FinancialImageList.Count);
                    if (index > -1 && FinancialImageList.Any())
                        imageType = FinancialImageList[index];
                    break;
                case BusinessTypeEnum.Airport:
                    //index = rand.Next(RetailImages.Count);
                    //imageType = RetailImages[index];
                    break;
                case BusinessTypeEnum.Medical:
                    //index = rand.Next(RetailImages.Count);
                    //imageType = RetailImages[index];
                    break;
                case BusinessTypeEnum.Factory:
                    index = rand.Next(FactoryImageList.Count);
                    if (index > -1 && FactoryImageList.Any())
                        imageType = FactoryImageList[index];
                    break;
                case BusinessTypeEnum.Carlot:
                    index = rand.Next(CarlotImageList.Count);
                    if (index > -1 && CarlotImageList.Any())
                        imageType = CarlotImageList[index];
                    break;
                default:
                    break;
            }
            busiess.Add(name, payrate, imageType.Name, businessType);
            busiess.SetImage(imageType.Name);
            Businesses.Add(busiess);
        }
        public static void AddPersonViewModel(string name, bool girl, PersonImageTypeEnum personImageType)
        {
            Random rand = new Random();
            PersonViewModel person = new PersonViewModel();
            PersonImageEntity personimage = new PersonImageEntity();
            int index;
            List<PersonImageEntity> personimages = new List<PersonImageEntity>();
            person.Add(name, girl, PersonEnum.Individual);
            person.Images = GetImagesForPerson(person.ImagesFolder);
            person.SetupImageLists();
            GlobalStuff.AllPersons.Add(person);
        }

        public static List<PersonImageViewModel> GetImagesForPerson(string imagesFolder)
        {
            string[] files = System.IO.Directory.GetFiles(imagesFolder);
            List<PersonImageViewModel> images = new List<PersonImageViewModel>();
            foreach (string file in files)
            {
                PersonImageViewModel imgvm = new PersonImageViewModel();
                imgvm.Name = System.IO.Path.GetFileNameWithoutExtension(file);
                imgvm.FilePath = file;
                images.Add(imgvm);
            }
            return images;
        }

        public static void AddPersonViewModel(string name)
        {
            Random rand = new Random();
            PersonViewModel person = new PersonViewModel();
            // BadPersonImageEntity personimage = new BadPersonImageEntity();
            int index;
            ImageTypeEntity image1 = new ImageTypeEntity();
            var foundbadguys = from b in PersonImageList
                               where b.Imagetype == ImageContentEnum.badguy
                               select b;
            index = rand.Next(foundbadguys.Count());
            image1 = foundbadguys.ElementAt(index);
            //var foundbadguys = from b in cityapp.BadPersons where b.StartsWith("badguy") select b;
            //index = rand.Next(foundbadguys.Count());
            //image1 = foundbadguys.ElementAt(index);

        }


       
        public static double CheckvalidDouble(double value)
        {
            double x = 0;
            if (Double.IsNaN(value))
            {
                value = 0; ;
            }
            x = value;
            return x;
        }
        public double PathThickness { get; set; } = 1; 
        public static LotControl PoliceStationLocation { get; internal set; }
        public static LandscapeObjectViewModel NextFromHomeObject { get; internal set; }
        public static LandscapeObjectViewModel HomeLandscapeObject { get; internal set; }
        public static LocationXYEntity ApproachPointN { get; internal set; }
        public static LocationXYEntity ApproachPointE { get; internal set; }
        public static LocationXYEntity ApproachPointS { get; internal set; }
        public static LocationXYEntity ApproachPointW { get; internal set; }
        public static List<PoliceCarEntity> PoliceCars { get; internal set; }
        public static double Screenwidth { get; internal set; }
        public static double Screenheight { get; internal set; }
        public static int CurrentUserPersonId { get; internal set; }
        public static string CurrentHouseName { get; internal set; }

        public static List<LotControl> FinancialLotCobtrols = new List<LotControl>();
        public static LandscapeObjectControl GetNextFromHomeObject()
        {
            LandscapeObjectControl result = new LandscapeObjectControl();
            var found = from f in LandscapeUCs
                        where f.Name == NextFromHomeObject.Name
                        select f;   
            if (found != null && found.Any())
                result = found.FirstOrDefault();
            return result;
        }
        public static LandscapeObjectControl GextHomeObject()
        {
            LandscapeObjectControl result = new LandscapeObjectControl();
            var found = from f in LandscapeUCs
                        where f.Name == HomeLandscapeObject.Name
                        select f;
            if (found != null && found.Any())
                result = found.FirstOrDefault();
            return result;
        }


    }
}
