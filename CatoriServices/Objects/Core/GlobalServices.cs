using CatoriServices.Objects.database;
using CatoriServices.Objects.Entities;
using System.Collections.ObjectModel;
namespace CatoriServices.Objects.Core
{
    public class GlobalServices
    {
        public static ObservableCollection<SettingEntity> Settings { get; set; } = new ObservableCollection<SettingEntity>();
        public static string Database = "C:\\Development\\Gaming\\Databases\\CatoriDatabase2026\\cityapps026.db";
        public static Int32 LandscapeObjecGroupid = 1;

        public static void LoadSettings()
        {
            try
            {
                            SettingsRepo= new SettingsRepository();
                            Settings = SettingsRepo.GetSettings();
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
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
            try
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
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }
        public static void CheckOrCreateDB()
        {
            try
            {
                            DatabaseHelper databaseHelper;
                
                            databaseHelper = new DatabaseHelper();
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }
        public static SettingEntity GetSettingByName(string name)
        {
            try
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
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }
        public static SettingEntity GetSetting(string settingName)
        {
            try
            {
                            SettingEntity result = new SettingEntity();
                            var settingfound = from s in Settings where s.Name == settingName select s;
                            if (settingfound != null && settingfound.Any())
                            {
                                result = settingfound.FirstOrDefault();
                            }
                            return result;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }
        public static void UpdateSetting(string settingName, string stringSetting, int intSetting)
        {
            try
            {
                            SettingEntity result = new SettingEntity();
                            var settingfound = from s in Settings where s.Name == settingName select s;
                            if (settingfound != null && settingfound.Any())
                            {
                                result = settingfound.FirstOrDefault();
                                result.IntSetting = intSetting;
                                result.StringSetting = stringSetting;
                            }
                            else
                            {
                               InsertSetting( settingName, stringSetting, intSetting);
                            }
                            LoadSettings();
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public static void InsertSetting(string name, string stringSetting, int intSetting)
        {
            try
            {
                            SettingEntity setting = new SettingEntity(name, stringSetting, intSetting);
                            SettingsRepo.UpsertSetting(setting);
                            LoadSettings();
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }
        public static void InsertBusiness(string name, decimal payrate,
            string imagenme, BusinessTypeEnum businessType)
        {
            try
            {
                            BusinessEntity bus1 = new BusinessEntity();
                            bus1.Add(name, payrate, imagenme, businessType);
                            BusinessRepo.UpsertBusiness(bus1);
                
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }
        public static async Task<PersonEntity> InsertPersonAsync(string name,
            bool isUser, PersonEnum personRole, string imagesFolder)
        {
            try
            {
                            PersonEntity personEntity = new PersonEntity();
                            personEntity.Name = name;
                            //personEntity.IsUser = false;
                            personEntity.PersonRole = personRole;
                            personEntity.ImagesFolder = imagesFolder;
                            PersonEntity newpersonEntity = await PersonRepo.UpsertPerson(personEntity);
                
                            return newpersonEntity;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }
        public static void InsertPoliceCar(int id,string carName,
            string CarType, string currentImage)
        {
            try
            {
                            PoliceCarEntity policdCar = new PoliceCarEntity();
                            policdCar.CarName = carName;
                            policdCar.PoliceCarId = id;
                            policdCar.CarType = currentImage;
                            policdCar.ImageName = currentImage;
                            PoliceRepo.Upsert(policdCar);
                
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }
        public static void InsertPersonImage(string Name, PersonImageTypeEnum personImageType, PersonImageStatusEnum PersonImageStatus,
         string FilePath, string ImageType,
         int fkpersonId)
        {
            try
            {
                            PersonImageEntity image1 = new PersonImageEntity();
                            image1.Add(Name, personImageType, PersonImageStatus,
                            FilePath, ImageType, fkpersonId);
                            PersonImageRepo.UpsertPersonImage(image1);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
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
                cLogger.Log(ex.ToString());

                throw;
            }
        }
    }

}


