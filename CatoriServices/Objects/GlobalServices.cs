using CatoriServices.Objects.database;
using CatoriServices.Objects.Entities;
using CityAppServices.Objects.database;
using CityAppServices.Objects.Entities;
using System.Collections.ObjectModel;

namespace CityAppServices
{
    public class GlobalServices
    {
        public static ObservableCollection<SettingEntity> Settings { get; set; } = new ObservableCollection<SettingEntity>();
        public static string Database = "C:\\Development\\Gaming\\Databases\\CatoriDatabase2026\\cityapps026.db";
        public static Int32 LandscapeObjecGroupid = 1;

        public static void LoadSettings()
        {
            SettingsRepo= new SettingsRepository();
            Settings = SettingsRepo.GetSettings();
        }
        private static PersonImageRepository PersonImageRepo;
        private static PersonRepository PersonRepo;
        private static BusinessRepository BusinessRepo;
        private static ImageRepository ImageRepo;
        private static SettingsRepository SettingsRepo;
        private static PoliceCarRepository PoliceRepo;
        public static string ImageFolder { get; set; }


        public static void CreateInstances()
        {
            AdoNetHelper adoNetHelper = new AdoNetHelper();
            PersonRepo = new PersonRepository();
            ImageRepo = new ImageRepository();
            BusinessRepo = new BusinessRepository();
            SettingsRepo = new SettingsRepository();
            PersonImageRepo = new PersonImageRepository();
            PoliceRepo = new PoliceCarRepository();
            CheckOrCreateDB();
        }
        public static void CheckOrCreateDB()
        {
            DatabaseHelper databaseHelper;

            databaseHelper = new DatabaseHelper();
        }
        public static SettingEntity GetSettingByName(string name)
        {
            SettingEntity setting = new SettingEntity();
            var found = from s in Settings where s.Name == name select s;
            if (found != null && found.Any())
            {
                setting = found.First();
            }
            if (setting.IntSetting == 0)
                setting.IntSetting = -1;

            return setting;
        }
        public static SettingEntity GettingSetting(string settingName)
        {
            SettingEntity result = new SettingEntity();
            var settingfound = from s in Settings where s.Name == settingName select s;
            if (settingfound != null && settingfound.Any())
            {
                result = settingfound.FirstOrDefault();
            }
            return result;
        }
       
            
        public static void InsertSetting(string name, string stringSetting, int intSetting)
        {
            SettingEntity setting = new SettingEntity(name, stringSetting, intSetting);
            SettingsRepo.UpsertSetting(setting);
            LoadSettings();
        }
        public static void InsertBusiness(string name, decimal payrate,
            string imagenme, BusinessTypeEnum businessType)
        {
            BusinessEntity bus1 = new BusinessEntity();
            bus1.Add(name, payrate, imagenme, businessType);
            BusinessRepo.UpsertBusiness(bus1);

        }
        public static async Task<PersonEntity> InsertPersonAsync(string name,
            bool isUser, PersonEnum personRole, string imagesFolder)
        {
            PersonEntity personEntity = new PersonEntity();
            personEntity.Name = name;
            //personEntity.IsUser = false;
            personEntity.PersonRole = personRole;
            personEntity.ImagesFolder = imagesFolder;
            PersonEntity newpersonEntity = await PersonRepo.UpsertPerson(personEntity);

            return newpersonEntity;
        }
        public static void InsertPoliceCar(int id,string carName,
            string CarType, string currentImage)
        {
            PoliceCarEntity policdCar = new PoliceCarEntity();
            policdCar.CarName = carName;
            policdCar.PoliceCarId = id;
            policdCar.CarType = currentImage;
            policdCar.ImageName = currentImage;
            PoliceRepo.Upsert(policdCar);

        }
        public static void InsertPersonImage(string Name, PersonImageTypeEnum personImageType, PersonImageStatusEnum PersonImageStatus,
         string FilePath, string ImageType,
         int fkpersonId)
        {
            PersonImageEntity image1 = new PersonImageEntity();
            image1.Add(Name, personImageType, PersonImageStatus,
            FilePath, ImageType, fkpersonId);
            PersonImageRepo.UpsertPersonImage(image1);
        }
    
        
        public static void InsertImage(string Name, int imageId, string NamePart,
        int NumberPart, int SequenceNumber, string TrailingPart,ImageEnum ImageRole)
        {
            try
            {
                string path = System.IO.Path.Combine(GlobalServices.ImageFolder, NamePart, TrailingPart);
                ImageDetailEntity image1 = new ImageDetailEntity();
                image1.FilePath = path;
                image1.ImageId = imageId;
                image1.Name = Name;
                image1.NamePart = NamePart;
                image1.NumberPart = NumberPart;
                image1.SequenceNumber = SequenceNumber;
                image1.TrailingPart = TrailingPart;
                image1.ImageRole = ImageRole;
                ImageRepo.UpsertImage(image1);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }

}
