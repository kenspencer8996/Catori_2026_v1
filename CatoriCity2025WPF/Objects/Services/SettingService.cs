using CityAppServices.Objects.database;
using CityAppServices.Objects.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace CatoriCity2025WPF.Objects.Services
{
    public class SettingService
    {
        SettingsRepository repository = new SettingsRepository();
        public ObservableCollection<SettingEntity> GetSetting()
        {
            ObservableCollection<SettingEntity> Setting = new ObservableCollection<SettingEntity>();
            try
            {
                Setting = repository.GetSettings();
            }
            catch (Exception ex)
            {
                throw;
            }
            return Setting;
        }
        public SettingEntity GetSettingbyName(string name)
        {
            SettingEntity Settingout = new SettingEntity();
            try
            {
                Settingout = repository.GetSettingbyName(name);
            }
            catch (Exception ex)
            {
                throw;
            }
            return Settingout;
        }

        public void UpsertSetting(SettingEntity Setting)
        {
            try
            {
                repository.UpsertSetting(Setting);
            }
            catch (Exception ex)
            {

                throw;
            }


        }
    }
}

