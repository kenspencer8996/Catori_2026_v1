using Microsoft.Data.Sqlite;
using System.Data;
namespace CatoriServices.Objects.database.House
{
    public class HouseRepository
    {
        AdoNetHelper adoNetHelper = new AdoNetHelper();
        SqlHelper sqlhelper = new SqlHelper();

            public async Task<List<HouseEntity>> GetHousesAsync()
            {
                List<HouseEntity> houses = new List<HouseEntity>();
                try
                {
                    string sql = @"SELECT HouseId, Name, Price, ForSale, FrontImage, LivingRoomImage, GarageImage, OwnerName, 
                    GarageButtonLocX, GarageButtonLocY, GarageProductsLocX, GarageProductsLocY 
                    FROM House";
                    IDataReader reader = sqlhelper.GetReader(sql);

                    int idxId = reader.GetOrdinal("HouseId");
                    int idxName = reader.GetOrdinal("Name");
                    int idxPrice = reader.GetOrdinal("Price");
                    int idxForSale = reader.GetOrdinal("ForSale");
                    int idxFrontImage = reader.GetOrdinal("FrontImage");
                    int idxLivingRoom = reader.GetOrdinal("LivingRoomImage");
                    int idxGarage = reader.GetOrdinal("GarageImage");
                    int idxOwnerName = reader.GetOrdinal("OwnerName");
                    int idxGarageButtonX = reader.GetOrdinal("GarageButtonLocX");
                    int idxGarageButtonY = reader.GetOrdinal("GarageButtonLocY");
                    int idxGarageProductsX = reader.GetOrdinal("GarageProductsLocX");
                    int idxGarageProductsY = reader.GetOrdinal("GarageProductsLocY");

                    while (reader.Read())
                    {
                        var entity = new HouseEntity();

                        if (!reader.IsDBNull(idxId)) entity.HouseId = reader.GetInt32(idxId);
                        if (!reader.IsDBNull(idxName)) entity.Name = reader.GetString(idxName);
                        if (!reader.IsDBNull(idxPrice)) entity.Price = reader.GetDecimal(idxPrice);
                        if (!reader.IsDBNull(idxForSale)) entity.ForSale = reader.GetInt32(idxForSale);
                        if (!reader.IsDBNull(idxFrontImage)) entity.FrontImage = reader.GetString(idxFrontImage);
                        if (!reader.IsDBNull(idxLivingRoom)) entity.LivingRoomImage = reader.GetString(idxLivingRoom);
                        if (!reader.IsDBNull(idxGarage)) entity.GarageImage = reader.GetString(idxGarage);
                        if (!reader.IsDBNull(idxOwnerName)) entity.OwnerName = reader.GetString(idxOwnerName);
                        if (!reader.IsDBNull(idxGarageButtonX)) entity.GarageButtonLocX = reader.GetDouble(idxGarageButtonX);
                        if (!reader.IsDBNull(idxGarageButtonY)) entity.GarageButtonLocY = reader.GetDouble(idxGarageButtonY);
                        if (!reader.IsDBNull(idxGarageProductsX)) entity.GarageProductsLocX = reader.GetDouble(idxGarageProductsX);
                        if (!reader.IsDBNull(idxGarageProductsY)) entity.GarageProductsLocY = reader.GetDouble(idxGarageProductsY);

                        houses.Add(entity);
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    throw;
                }

                return houses;
            }

            public async Task<HouseEntity> GetHouseByIdAsync(int id)
            {
                HouseEntity house = new HouseEntity();
                try
                {
                    string sql = $@"SELECT HouseId, Name, Price, ForSale, FrontImage, LivingRoomImage, GarageImage, OwnerName, 
                    GarageButtonLocX, GarageButtonLocY, GarageProductsLocX, GarageProductsLocY 
                    FROM House WHERE HouseId = {id}";
                    IDataReader reader = sqlhelper.GetReader(sql);

                    int idxId = reader.GetOrdinal("HouseId");
                    int idxName = reader.GetOrdinal("Name");
                    int idxPrice = reader.GetOrdinal("Price");
                    int idxForSale = reader.GetOrdinal("ForSale");
                    int idxFrontImage = reader.GetOrdinal("FrontImage");
                    int idxLivingRoom = reader.GetOrdinal("LivingRoomImage");
                    int idxGarage = reader.GetOrdinal("GarageImage");
                    int idxOwnerName = reader.GetOrdinal("OwnerName");
                    int idxGarageButtonX = reader.GetOrdinal("GarageButtonLocX");
                    int idxGarageButtonY = reader.GetOrdinal("GarageButtonLocY");
                    int idxGarageProductsX = reader.GetOrdinal("GarageProductsLocX");
                    int idxGarageProductsY = reader.GetOrdinal("GarageProductsLocY");

                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(idxId)) house.HouseId = reader.GetInt32(idxId);
                        if (!reader.IsDBNull(idxName)) house.Name = reader.GetString(idxName);
                        if (!reader.IsDBNull(idxPrice)) house.Price = reader.GetDecimal(idxPrice);
                        if (!reader.IsDBNull(idxForSale)) house.ForSale = reader.GetInt32(idxForSale);
                        if (!reader.IsDBNull(idxFrontImage)) house.FrontImage = reader.GetString(idxFrontImage);
                        if (!reader.IsDBNull(idxLivingRoom)) house.LivingRoomImage = reader.GetString(idxLivingRoom);
                        if (!reader.IsDBNull(idxGarage)) house.GarageImage = reader.GetString(idxGarage);
                        if (!reader.IsDBNull(idxOwnerName)) house.OwnerName = reader.GetString(idxOwnerName);
                        if (!reader.IsDBNull(idxGarageButtonX)) house.GarageButtonLocX = reader.GetDouble(idxGarageButtonX);
                        if (!reader.IsDBNull(idxGarageButtonY)) house.GarageButtonLocY = reader.GetDouble(idxGarageButtonY);
                        if (!reader.IsDBNull(idxGarageProductsX)) house.GarageProductsLocX = reader.GetDouble(idxGarageProductsX);
                        if (!reader.IsDBNull(idxGarageProductsY)) house.GarageProductsLocY = reader.GetDouble(idxGarageProductsY);
                    }

                    reader.Close();
                }
                catch (Exception)
                {
                    throw;
                }

                return house;
            }

            public async Task<HouseEntity> GetHouseByNameAsync(string name)
            {
                HouseEntity house = new HouseEntity();
                try
                {
                    string sql = $@"SELECT HouseId, Name, Price, ForSale, FrontImage, LivingRoomImage, GarageImage, OwnerName, 
                    GarageButtonLocX, GarageButtonLocY, GarageProductsLocX, GarageProductsLocY 
                    FROM House WHERE Name = '{name}'";
                    IDataReader reader = sqlhelper.GetReader(sql);

                    int idxId = reader.GetOrdinal("HouseId");
                    int idxName = reader.GetOrdinal("Name");
                    int idxPrice = reader.GetOrdinal("Price");
                    int idxForSale = reader.GetOrdinal("ForSale");
                    int idxFrontImage = reader.GetOrdinal("FrontImage");
                    int idxLivingRoom = reader.GetOrdinal("LivingRoomImage");
                    int idxGarage = reader.GetOrdinal("GarageImage");
                    int idxOwnerName = reader.GetOrdinal("OwnerName");
                    int idxGarageButtonX = reader.GetOrdinal("GarageButtonLocX");
                    int idxGarageButtonY = reader.GetOrdinal("GarageButtonLocY");
                    int idxGarageProductsX = reader.GetOrdinal("GarageProductsLocX");
                    int idxGarageProductsY = reader.GetOrdinal("GarageProductsLocY");

                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(idxId)) house.HouseId = reader.GetInt32(idxId);
                        if (!reader.IsDBNull(idxName)) house.Name = reader.GetString(idxName);
                        if (!reader.IsDBNull(idxPrice)) house.Price = reader.GetDecimal(idxPrice);
                        if (!reader.IsDBNull(idxForSale)) house.ForSale = reader.GetInt32(idxForSale);
                        if (!reader.IsDBNull(idxFrontImage)) house.FrontImage = reader.GetString(idxFrontImage);
                        if (!reader.IsDBNull(idxLivingRoom)) house.LivingRoomImage = reader.GetString(idxLivingRoom);
                        if (!reader.IsDBNull(idxGarage)) house.GarageImage = reader.GetString(idxGarage);
                        if (!reader.IsDBNull(idxOwnerName)) house.OwnerName = reader.GetString(idxOwnerName);
                        if (!reader.IsDBNull(idxGarageButtonX)) house.GarageButtonLocX = reader.GetDouble(idxGarageButtonX);
                        if (!reader.IsDBNull(idxGarageButtonY)) house.GarageButtonLocY = reader.GetDouble(idxGarageButtonY);
                        if (!reader.IsDBNull(idxGarageProductsX)) house.GarageProductsLocX = reader.GetDouble(idxGarageProductsX);
                        if (!reader.IsDBNull(idxGarageProductsY)) house.GarageProductsLocY = reader.GetDouble(idxGarageProductsY);
                    }

                    reader.Close();
                }
                catch (Exception)
                {
                    throw;
                }

                return house;
            }

            public async Task<HouseEntity> GetHouseByOwnerNameAsync(string ownerName)
            {
                HouseEntity house = new HouseEntity();
                try
                {
                    string sql = $@"SELECT HouseId, Name, Price, ForSale, FrontImage, LivingRoomImage, GarageImage, OwnerName, 
                    GarageButtonLocX, GarageButtonLocY, GarageProductsLocX, GarageProductsLocY 
                    FROM House WHERE OwnerName = '{ownerName}'";
                    IDataReader reader = sqlhelper.GetReader(sql);

                    int idxId = reader.GetOrdinal("HouseId");
                    int idxName = reader.GetOrdinal("Name");
                    int idxPrice = reader.GetOrdinal("Price");
                    int idxForSale = reader.GetOrdinal("ForSale");
                    int idxFrontImage = reader.GetOrdinal("FrontImage");
                    int idxLivingRoom = reader.GetOrdinal("LivingRoomImage");
                    int idxGarage = reader.GetOrdinal("GarageImage");
                    int idxOwnerName = reader.GetOrdinal("OwnerName");
                    int idxGarageButtonX = reader.GetOrdinal("GarageButtonLocX");
                    int idxGarageButtonY = reader.GetOrdinal("GarageButtonLocY");
                    int idxGarageProductsX = reader.GetOrdinal("GarageProductsLocX");
                    int idxGarageProductsY = reader.GetOrdinal("GarageProductsLocY");

                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(idxId)) house.HouseId = reader.GetInt32(idxId);
                        if (!reader.IsDBNull(idxName)) house.Name = reader.GetString(idxName);
                        if (!reader.IsDBNull(idxPrice)) house.Price = reader.GetDecimal(idxPrice);
                        if (!reader.IsDBNull(idxForSale)) house.ForSale = reader.GetInt32(idxForSale);
                        if (!reader.IsDBNull(idxFrontImage)) house.FrontImage = reader.GetString(idxFrontImage);
                        if (!reader.IsDBNull(idxLivingRoom)) house.LivingRoomImage = reader.GetString(idxLivingRoom);
                        if (!reader.IsDBNull(idxGarage)) house.GarageImage = reader.GetString(idxGarage);
                        if (!reader.IsDBNull(idxOwnerName)) house.OwnerName = reader.GetString(idxOwnerName);
                        if (!reader.IsDBNull(idxGarageButtonX)) house.GarageButtonLocX = reader.GetDouble(idxGarageButtonX);
                        if (!reader.IsDBNull(idxGarageButtonY)) house.GarageButtonLocY = reader.GetDouble(idxGarageButtonY);
                        if (!reader.IsDBNull(idxGarageProductsX)) house.GarageProductsLocX = reader.GetDouble(idxGarageProductsX);
                        if (!reader.IsDBNull(idxGarageProductsY)) house.GarageProductsLocY = reader.GetDouble(idxGarageProductsY);
                    }

                    reader.Close();
                }
                catch (Exception)
                {
                    throw;
                }

                return house;
            }

            public async Task<List<HouseEntity>> GetHousesForSaleAsync()
            {
                List<HouseEntity> houses = new List<HouseEntity>();
                try
                {
                    string sql = @"SELECT HouseId, Name, Price, ForSale, FrontImage, LivingRoomImage, GarageImage, OwnerName, 
                    GarageButtonLocX, GarageButtonLocY, GarageProductsLocX, GarageProductsLocY 
                    FROM House WHERE ForSale = 1";
                    IDataReader reader = sqlhelper.GetReader(sql);

                    int idxId = reader.GetOrdinal("HouseId");
                    int idxName = reader.GetOrdinal("Name");
                    int idxPrice = reader.GetOrdinal("Price");
                    int idxForSale = reader.GetOrdinal("ForSale");
                    int idxFrontImage = reader.GetOrdinal("FrontImage");
                    int idxLivingRoom = reader.GetOrdinal("LivingRoomImage");
                    int idxGarage = reader.GetOrdinal("GarageImage");
                    int idxOwnerName = reader.GetOrdinal("OwnerName");
                    int idxGarageButtonX = reader.GetOrdinal("GarageButtonLocX");
                    int idxGarageButtonY = reader.GetOrdinal("GarageButtonLocY");
                    int idxGarageProductsX = reader.GetOrdinal("GarageProductsLocX");
                    int idxGarageProductsY = reader.GetOrdinal("GarageProductsLocY");

                    while (reader.Read())
                    {
                        var entity = new HouseEntity();

                        if (!reader.IsDBNull(idxId)) entity.HouseId = reader.GetInt32(idxId);
                        if (!reader.IsDBNull(idxName)) entity.Name = reader.GetString(idxName);
                        if (!reader.IsDBNull(idxPrice)) entity.Price = reader.GetDecimal(idxPrice);
                        if (!reader.IsDBNull(idxForSale)) entity.ForSale = reader.GetInt32(idxForSale);
                        if (!reader.IsDBNull(idxFrontImage)) entity.FrontImage = reader.GetString(idxFrontImage);
                        if (!reader.IsDBNull(idxLivingRoom)) entity.LivingRoomImage = reader.GetString(idxLivingRoom);
                        if (!reader.IsDBNull(idxGarage)) entity.GarageImage = reader.GetString(idxGarage);
                        if (!reader.IsDBNull(idxOwnerName)) entity.OwnerName = reader.GetString(idxOwnerName);
                        if (!reader.IsDBNull(idxGarageButtonX)) entity.GarageButtonLocX = reader.GetDouble(idxGarageButtonX);
                        if (!reader.IsDBNull(idxGarageButtonY)) entity.GarageButtonLocY = reader.GetDouble(idxGarageButtonY);
                        if (!reader.IsDBNull(idxGarageProductsX)) entity.GarageProductsLocX = reader.GetDouble(idxGarageProductsX);
                        if (!reader.IsDBNull(idxGarageProductsY)) entity.GarageProductsLocY = reader.GetDouble(idxGarageProductsY);

                        houses.Add(entity);
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    throw;
                }

                return houses;
            }

            public async void Upsert(HouseEntity entity)
            {
                try
                {
                    // If no id provided, try to find by name
                    if (entity.HouseId == 0 && !string.IsNullOrEmpty(entity.Name))
                    {
                        var foundByName = await GetHouseByNameAsync(entity.Name).ConfigureAwait(false);
                        if (foundByName != null && foundByName.HouseId != 0)
                            entity.HouseId = foundByName.HouseId;
                    }

                    var found = await GetHouseByIdAsync(entity.HouseId).ConfigureAwait(false);
                    var connection = adoNetHelper.GetConnection();

                    if (found != null && found.HouseId != 0)
                    {
                        // Update
                        using var command = connection.CreateCommand();
                        command.CommandText = @"UPDATE House SET 
                        Name = @Name, 
                        Price = @Price,
                        ForSale = @ForSale,
                        FrontImage = @FrontImage, 
                        LivingRoomImage = @LivingRoomImage, 
                        GarageImage = @GarageImage, 
                        OwnerName = @OwnerName,
                        GarageButtonLocX = @GarageButtonLocX,
                        GarageButtonLocY = @GarageButtonLocY,
                        GarageProductsLocX = @GarageProductsLocX,
                        GarageProductsLocY = @GarageProductsLocY
                        WHERE HouseId = @Id";
                        command.Parameters.Add(new SqliteParameter("@Name", entity.Name ?? string.Empty));
                        command.Parameters.Add(new SqliteParameter("@Price", entity.Price));
                        command.Parameters.Add(new SqliteParameter("@ForSale", entity.ForSale));
                        command.Parameters.Add(new SqliteParameter("@FrontImage", entity.FrontImage ?? string.Empty));
                        command.Parameters.Add(new SqliteParameter("@LivingRoomImage", entity.LivingRoomImage ?? string.Empty));
                        command.Parameters.Add(new SqliteParameter("@GarageImage", entity.GarageImage ?? string.Empty));
                        command.Parameters.Add(new SqliteParameter("@OwnerName", entity.OwnerName ?? string.Empty));
                        command.Parameters.Add(new SqliteParameter("@GarageButtonLocX", entity.GarageButtonLocX));
                        command.Parameters.Add(new SqliteParameter("@GarageButtonLocY", entity.GarageButtonLocY));
                        command.Parameters.Add(new SqliteParameter("@GarageProductsLocX", entity.GarageProductsLocX));
                        command.Parameters.Add(new SqliteParameter("@GarageProductsLocY", entity.GarageProductsLocY));
                        command.Parameters.Add(new SqliteParameter("@Id", entity.HouseId));
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        // Insert
                        using var command = connection.CreateCommand();
                        command.CommandText = @"INSERT INTO House 
                        (Name, Price, ForSale, FrontImage, LivingRoomImage, GarageImage, OwnerName, 
                        GarageButtonLocX, GarageButtonLocY, GarageProductsLocX, GarageProductsLocY) 
                        VALUES 
                        (@Name, @Price, @ForSale, @FrontImage, @LivingRoomImage, @GarageImage, @OwnerName, 
                        @GarageButtonLocX, @GarageButtonLocY, @GarageProductsLocX, @GarageProductsLocY)";
                        command.Parameters.Add(new SqliteParameter("@Name", entity.Name ?? string.Empty));
                        command.Parameters.Add(new SqliteParameter("@Price", entity.Price));
                        command.Parameters.Add(new SqliteParameter("@ForSale", entity.ForSale));
                        command.Parameters.Add(new SqliteParameter("@FrontImage", entity.FrontImage ?? string.Empty));
                        command.Parameters.Add(new SqliteParameter("@LivingRoomImage", entity.LivingRoomImage ?? string.Empty));
                        command.Parameters.Add(new SqliteParameter("@GarageImage", entity.GarageImage ?? string.Empty));
                        command.Parameters.Add(new SqliteParameter("@OwnerName", entity.OwnerName ?? string.Empty));
                        command.Parameters.Add(new SqliteParameter("@GarageButtonLocX", entity.GarageButtonLocX));
                        command.Parameters.Add(new SqliteParameter("@GarageButtonLocY", entity.GarageButtonLocY));
                        command.Parameters.Add(new SqliteParameter("@GarageProductsLocX", entity.GarageProductsLocX));
                        command.Parameters.Add(new SqliteParameter("@GarageProductsLocY", entity.GarageProductsLocY));
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public async Task DeleteAsync(int id)
            {
                try
                {
                    var connection = adoNetHelper.GetConnection();
                    using var command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM House WHERE HouseId = @Id";
                    command.Parameters.Add(new SqliteParameter("@Id", id));
                    command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public async Task UpdateGarageLocationsAsync(int houseId, double garageButtonX, double garageButtonY,
                double garageProductsX, double garageProductsY)
            {
                try
                {
                    var connection = adoNetHelper.GetConnection();
                    using var command = connection.CreateCommand();
                    command.CommandText = @"UPDATE House SET 
                    GarageButtonLocX = @GarageButtonLocX,
                    GarageButtonLocY = @GarageButtonLocY,
                    GarageProductsLocX = @GarageProductsLocX,
                    GarageProductsLocY = @GarageProductsLocY
                    WHERE HouseId = @Id";
                    command.Parameters.Add(new SqliteParameter("@GarageButtonLocX", garageButtonX));
                    command.Parameters.Add(new SqliteParameter("@GarageButtonLocY", garageButtonY));
                    command.Parameters.Add(new SqliteParameter("@GarageProductsLocX", garageProductsX));
                    command.Parameters.Add(new SqliteParameter("@GarageProductsLocY", garageProductsY));
                    command.Parameters.Add(new SqliteParameter("@Id", houseId));
                    command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public async Task UpdateOwnerAsync(int houseId, string ownerName)
            {
                try
                {
                    var connection = adoNetHelper.GetConnection();
                    using var command = connection.CreateCommand();
                    command.CommandText = "UPDATE House SET OwnerName = @OwnerName WHERE HouseId = @Id";
                    command.Parameters.Add(new SqliteParameter("@OwnerName", ownerName ?? string.Empty));
                    command.Parameters.Add(new SqliteParameter("@Id", houseId));
                    command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public async Task UpdateForSaleAsync(int houseId, int forSale)
            {
                try
                {
                    var connection = adoNetHelper.GetConnection();
                    using var command = connection.CreateCommand();
                    command.CommandText = "UPDATE House SET ForSale = @ForSale WHERE HouseId = @Id";
                    command.Parameters.Add(new SqliteParameter("@ForSale", forSale));
                    command.Parameters.Add(new SqliteParameter("@Id", houseId));
                    command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }



