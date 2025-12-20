using CatoriServices;
using CityAppServices.Objects.database;
using CityAppServices.Objects.Entities;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Xml.Linq;

namespace CityAppServices.Objects.database
{
    public class ImageRepository
    {
        AdoNetHelper adoNetHelper = new AdoNetHelper();
        SqlHelper sqlhelper = new SqlHelper();
        public async Task<List<ImageDetailEntity>> GetImagesAsync()
        {
            List<ImageDetailEntity> Images = new List<ImageDetailEntity>();
            try
            {
                string connectionString = "Data Source=" + GlobalServices.Database + " ;";

                using (SqliteConnection connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    string selectQuery = "SELECT [SequenceNumber],[Name],[Filename],[FilePath],[NamePart],[NumberPart],[TrailingPart],[ImageRole] FROM [Image]";
                    using (SqliteCommand command = new SqliteCommand(selectQuery, connection))
                    {
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ImageDetailEntity image = new ImageDetailEntity();
                                try
                                {
                                    image.Name = reader["Name"].ToString();
                                    image.FilePath = reader["FilePath"].ToString();
                                    image.NamePart = reader["NamePart"].ToString();
                                    image.NumberPart = 0;
                                    image.TrailingPart = reader["TrailingPart"].ToString();
                                    image.ImageRole = Convertors.GedImageEnumFromString(reader["ImageRole"].ToString());
                                    string seq = reader["SequenceNumber"].ToString();
                                    if (!reader.IsDBNull(0))
                                        image.SequenceNumber = Convert.ToInt32(reader["SequenceNumber"]);
                                    Images.Add(image);

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
            return Images;
        }
        public async Task<ImageDetailEntity> GetImageByNameAsync(string name)
        {
            List<ImageDetailEntity> Images = new List<ImageDetailEntity>();
            ImageDetailEntity image = new ImageDetailEntity();
            try
            {
                var connection = adoNetHelper.GetConnection();
                var sql = "SELECT * FROM Image where name = '" + name + "'";
                var results = await connection.QueryAsync<ImageDetailEntity>(sql);
                if (results != null && results.Any())
                {
                    image = results.First();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return image;
        }
        public async void UpsertImage(ImageDetailEntity Image)
        {
            bool result = false;
            ImageDetailEntity Imagefound = await GetImageByNameAsync(Image.Name);
            var connection = adoNetHelper.GetConnection();
            if (Imagefound != null && Imagefound.Name != null && Imagefound.Name != "")
            {
                try
                {
                    string sqlraw = "Update Image ";
                    sqlraw += "Set Name = " + "'" + Image.Name + "', ";
                   // sqlraw += "ImageFilename = '" + Image.FilePath + "', ";
                    sqlraw += "FilePath = '" + Image.FilePath + "', ";
                    sqlraw += "NamePart = '" + Image.NamePart + "', ";                   
                    sqlraw += "NumberPart = " + "'" + Image.NumberPart + "', ";
                    sqlraw += "TrailingPart = " + "'" + Image.TrailingPart + "', ";
                    sqlraw += "SequenceNumber = " + "" + Image.SequenceNumber + " ";
                    sqlraw += "where ImageId = " + Image.ImageId;

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
                    string sqlraw = "INSERT INTO Image ( Name,FilePath,NamePart,NumberPart,SequenceNumber,TrailingPart,ImageRole) ";
                    sqlraw += "VALUES('" + Image.Name + "','" + Image.FilePath + "',";
                    sqlraw+= "'" + Image.NamePart + "','" + Image.NumberPart + "'," + Image.SequenceNumber + ",'" + Image.TrailingPart + "','" + Image.ImageRole +  "')"; 
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
