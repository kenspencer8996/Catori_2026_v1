using CatoriServices;
using CityAppServices.Objects.Entities;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Linq.Expressions;
using static System.Net.Mime.MediaTypeNames;

namespace CityAppServices.Objects.database
{
    public class PersonImageRepository
    {
        AdoNetHelper adoNetHelper = new AdoNetHelper();

        public async Task<List<PersonImageEntity>> GetPersonImagesAsync()
        {
            List<PersonImageEntity> PersonImages = new List<PersonImageEntity>();
            try
            {
                string connectionString = "Data Source=" + GlobalServices.Database + " ;";

                using (SqliteConnection connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    string selectQuery = "SELECT [FKImageID],[Name],[PersonImageType],[FilePath],[ImageType],[FKPersonID],[PersonImageStatus] FROM PersonImage";
                    using (SqliteCommand command = new SqliteCommand(selectQuery, connection))
                    {
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PersonImageEntity personimage = new PersonImageEntity();
                                try
                                {
                                    personimage.Name = reader["Name"].ToString();
                                    personimage.FilePath = reader["FilePath"].ToString();
                                    var fkpersonid = reader.GetInt32("FKPersonID");
                                    personimage.FKPersonId = Convert.ToInt32(fkpersonid);

                                    if (!reader.IsDBNull(6))
                                        personimage.PersonImageStatus = Convertors.GetPersonImageStatusEnumFromInt (reader.GetInt32("PersonImageStatus"));
                                    else
                                        personimage.PersonImageStatus = Convertors.GetPersonImageStatusEnumFromInt(99);
                                    personimage.PersonImageType = Convertors.GetPersonImageTypeEnum(reader["PersonImageType"].ToString());
                                    if (!reader.IsDBNull(4))
                                        personimage.ImageType = Convert.ToString(reader["ImageType"]);
                                    PersonImages.Add(personimage);

                                }
                                catch (Exception ex)
                                {
                                    throw;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
                return PersonImages;
        }
        public async Task<PersonImageEntity> GetPersonImagebyNameAsync(string name)
        {
            List<PersonImageEntity> PersonImages = new List<PersonImageEntity>();
            PersonImageEntity results = new PersonImageEntity();
            try
            {
                var connection = adoNetHelper.GetConnection();
                var sql = "SELECT Name,FilePath,PersonImageType,ImageType,FKPersonId FROM PersonImage where name  = " + "'" + name + "' ";
                //var results = await connection.QueryAsync<PersonImageEntity>(sql);
                var con = adoNetHelper.GetConnection();
                var cmd = con.CreateCommand();
                cmd.CommandText = sql;
                try
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Name = reader.GetString(0);
                            results.FilePath = reader.GetString(1);
                            PersonImageTypeEnum imagetype = PersonImageTypeEnum.Normal;
                            switch (reader.GetString(2))
                            {
                                case "Normal":
                                    imagetype = PersonImageTypeEnum.Normal;
                                    break;
                                case "Working":
                                    imagetype = PersonImageTypeEnum.Working;
                                    break;
                                case "BadGuy":
                                    imagetype = PersonImageTypeEnum.BadGuy;
                                    break;
                                default:
                                    break;
                            }
                            results.PersonImageType = imagetype;
                            results.ImageType = reader.GetString(3);
                            results.FKPersonId = reader.GetInt32(4);

                            Console.WriteLine($"Hello, {name}!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    cmd.Dispose();
                    cmd = null;
                    con.Close();
                }
                //if (results != null && results.Any())
                //{
                //    PersonImages = results.ToList();
                //}
            }
            catch (Exception ex)
            {
                throw;
            }
            //PersonImageEntity PersonImageout = new PersonImageEntity();
            //if (PersonImages.Any())
            //    PersonImageout = PersonImages.FirstOrDefault();
            //return PersonImageout;
            return results;
        }
   
        public async void UpsertPersonImage(PersonImageEntity PersonImage)
        {
            bool result = false;
            PersonImageEntity PersonImagefound = await GetPersonImagebyNameAsync(PersonImage.Name);
            var connection = adoNetHelper.GetConnection();
            if (PersonImagefound != null && PersonImagefound.Name != null && PersonImagefound.Name != "")
            {
                try
                {   
                    string sqlraw = "Update PersonImage ";
                    sqlraw += "Set Name = " + "'" + PersonImage.Name + "', ";
                    sqlraw += "FilePath = '" + PersonImage.FilePath + "', ";
                    sqlraw += "PersonImageType = '" + PersonImage.PersonImageType + "', ";
                    sqlraw += "PersonImageStatus = '" + PersonImage.PersonImageStatus + "', ";                   
                    sqlraw += "ImageType = " + "'" + PersonImage.ImageType + "', ";
                    sqlraw += "FKPersonId = " + "" + PersonImage.FKPersonId + " ";
                    sqlraw += "where PersonImageID = " + PersonImage.PersonImageID;

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
                    string sqlraw = "INSERT INTO PersonImage ( Name,FilePath,PersonImageType,ImageType,FKPersonId,PersonImageStatus) ";
                    sqlraw += "VALUES('" + PersonImage.Name + "','" + PersonImage.FilePath + "',";
                    sqlraw+= "'" + PersonImage.PersonImageType + "','" + PersonImage.ImageType + "'," + PersonImage.FKPersonId + ",'" + PersonImage.PersonImageStatus +  "')"; 
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
