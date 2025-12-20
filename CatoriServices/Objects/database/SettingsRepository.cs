using CityAppServices.Objects.Entities;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;

namespace CityAppServices.Objects.database
{
    public class SettingsRepository
    {
        AdoNetHelper adoNetHelper = new AdoNetHelper();
        public ObservableCollection<SettingEntity> GetSettings()
        {
            List<SettingEntity> Setting = new List<SettingEntity>();
            ObservableCollection<SettingEntity> SettingObservable = new ObservableCollection<SettingEntity>();
            try
            {
                var connection = adoNetHelper.GetConnection();

                var sql = "SELECT * FROM Settings";
                var results = connection.Query<SettingEntity>(sql);
                if (results != null && results.Any())
                {
                    Setting = results.ToList();
                    foreach (var item in Setting)
                    {
                        SettingObservable.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return SettingObservable;
        }
        public  SettingEntity GetSettingbyName(string name)
        {
            List<SettingEntity> Setting = new List<SettingEntity>();
            try
            {
                var connection = adoNetHelper.GetConnection();

                var sql = "SELECT * FROM Settings where name  = " + "'" + name + "' ";
                var results = connection.Query<SettingEntity>(sql);
                if (results != null && results.Any())
                {
                    Setting = results.ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            SettingEntity Settingout = new SettingEntity();
            if (Setting.Any())
                Settingout = Setting.FirstOrDefault();
            return Settingout;
        }
   
        public  void UpsertSetting(SettingEntity Setting)
        {
            bool result = false;
            SettingEntity Settingfound =  GetSettingbyName(Setting.Name);
            var connection = adoNetHelper.GetConnection();

            if (Settingfound != null && Settingfound.Name != "")
            {
                try
                {   
                    string sqlraw = "Update Settings ";
                    sqlraw += "Set Name = " + "'" + Setting.Name + "', ";
                    sqlraw += "StringSetting = '" + Setting.StringSetting + "', ";
                    sqlraw += "IntSetting = " + Setting.IntSetting + " where SettingID = " + Setting.SettingID;
                    
                    var command = new SqliteCommand();
                    command.Connection = connection;
                    command.CommandText = sqlraw;
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    throw;
                }
            } 
            else
            {
                try
                {                  
                    string sqlraw = "INSERT INTO Settings ( Name,StringSetting,IntSetting) ";
                    sqlraw += "VALUES('" + Setting.Name + "','" + Setting.StringSetting + "', ";
                    sqlraw+= "" + Setting.IntSetting + ")"; 
                    var command = new SqliteCommand();
                    command.Connection = adoNetHelper.GetConnection(); 
                    command.CommandText = sqlraw;
                    command.ExecuteNonQuery();
     
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }
    }
}
