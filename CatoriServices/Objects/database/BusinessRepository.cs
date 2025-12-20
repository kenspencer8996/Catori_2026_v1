using CatoriServices;
using CityAppServices.Objects.Entities;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace CityAppServices.Objects.database
{
    public class BusinessRepository
    {
        AdoNetHelper adoNetHelper = new AdoNetHelper();
        SqlHelper sqlhelper = new SqlHelper();

        public async Task<List<BusinessEntity>> GetBusinesssAsync()
        {
            List<BusinessEntity> Businesss = new List<BusinessEntity>();
            try
            {
                string sql = "SELECT EmployeePayHour,[Name],[BusinessType],[ImageName] FROM [Business]";
                IDataReader reader = sqlhelper.GetReader(sql);
                Int32 EmployeePayHourIndex = 0;
                Int32 NameIndex = 1;
                Int32 BusinessTypeIndex = 2;
                Int32 ImageNameIndex = 3;
                while (reader.Read())
                {
                    BusinessEntity bus = new BusinessEntity();
                    try
                    {
                        bus.Name = reader.GetString(NameIndex);

                        if (!reader.IsDBNull(EmployeePayHourIndex))
                            bus.EmployeePayHour = reader.GetInt32(EmployeePayHourIndex);
                        string busType = reader.GetString(BusinessTypeIndex);
                        bus.BusinessType = Convertors.GetBusinessTypeEnum(busType);
                        bus.ImageName = reader.GetString(ImageNameIndex);
                        Businesss.Add(bus);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return Businesss;
        }
        public async Task<BusinessEntity> GetBusinessbyNameAsync(string name)
        {
            List<BusinessEntity> Businesss = new List<BusinessEntity>();
            try
            {
                var sql = "SELECT EmployeePayHour,[Name],[BusinessType],[ImageName] FROM Business ";
                sql += "where name  = " + "'" + name + "' ";
                Int32 EmployeePayHourIndex = 0;
                Int32 NameIndex = 1;
                Int32 BusinessTypeIndex = 2;
                Int32 ImageNameIndex = 3;

                string connectionString = "Data Source=" + GlobalServices.Database + " ;";
                IDataReader reader = sqlhelper.GetReader(sql);

                while (reader.Read())
                {
                    BusinessEntity bus = new BusinessEntity();
                    try
                    {
                        bus.Name = reader.GetString(NameIndex);

                        if (!reader.IsDBNull(EmployeePayHourIndex))
                            bus.EmployeePayHour = reader.GetInt32(EmployeePayHourIndex);
                        string busType = reader.GetString(BusinessTypeIndex) ;
                        bus.BusinessType = Convertors.GetBusinessTypeEnum(busType);
                        bus.ImageName = reader.GetString(ImageNameIndex);
                        Businesss.Add(bus);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
                     
            }
            catch (Exception ex)
            {

                throw;
            }
            BusinessEntity Businessout = new BusinessEntity();
            if (Businesss.Any())
                Businessout = Businesss.FirstOrDefault();
            return Businessout;
        }

        public async void UpsertBusiness(BusinessEntity Business)
        {
            bool result = false;
            BusinessEntity Businessfound = await GetBusinessbyNameAsync(Business.Name);
            var connection = adoNetHelper.GetConnection();
            if (Businessfound != null && Businessfound.Name != "")
            {
                try
                {
                    string sqlraw = "Update Business ";
                    sqlraw += "Set Name = " + "'" + Business.Name + "', ";
                    sqlraw += "EmployeePayHour = " + Business.EmployeePayHour + ", ";
                    sqlraw += "BusinessType = '" + Business.BusinessType + "', ";
                    sqlraw += "ImageName = " + "'" + Business.ImageName + "'";
                    sqlraw += "where BusinessID = " + Business.BusinesskeyImageNameWOExtension;

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
                    string sqlraw = "INSERT INTO Business (Name, EmployeePayHour, BusinessType, ImageName)";
                    sqlraw += "VALUES('" + Business.Name + "'," + Business.EmployeePayHour + ",";
                    sqlraw += "'" + Business.BusinessType + "','" + Business.ImageName + "'" + ")"; 
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
        }
    }
}
