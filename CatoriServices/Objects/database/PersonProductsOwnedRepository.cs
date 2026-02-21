using CatoriServices.Objects.Entities;
using CityAppServices.Objects.database;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatoriServices.Objects.database
{
    public class PersonProductsOwnedRepository
    {
        AdoNetHelper adoNetHelper = new AdoNetHelper();
        SqlHelper sqlhelper = new SqlHelper();
        public async Task<List<PersonProductsOwnedEntity>> GetByPersonIdWithShopItemDetailsAsync(int personId)
        {
            List<PersonProductsOwnedEntity> items = new List<PersonProductsOwnedEntity>();
            try
            {
                string sql = $@"SELECT 
                    ppo.PersonProductsOwnedId, 
                    ppo.PersonId, 
                    ppo.ShopItemId, 
                    ppo.Quantity,
                    si.Name,
                    si.ImageName
                FROM PersonProductsOwned ppo
                INNER JOIN ShopItem si ON ppo.ShopItemId = si.ShopItemId
                WHERE ppo.PersonId = {personId}";

                IDataReader reader = sqlhelper.GetReader(sql);

                int idxId = reader.GetOrdinal("PersonProductsOwnedId");
                int idxPersonId = reader.GetOrdinal("PersonId");
                int idxShopItemId = reader.GetOrdinal("ShopItemId");
                int idxQuantity = reader.GetOrdinal("Quantity");
                int idxName = reader.GetOrdinal("Name");
                int idxImageName = reader.GetOrdinal("ImageName");

                while (reader.Read())
                {
                    var entity = new PersonProductsOwnedEntity();

                    if (!reader.IsDBNull(idxId)) entity.PersonProductsOwnedId = reader.GetInt32(idxId);
                    if (!reader.IsDBNull(idxPersonId)) entity.PersonId = reader.GetInt32(idxPersonId);
                    if (!reader.IsDBNull(idxShopItemId)) entity.ShopItemId = reader.GetInt32(idxShopItemId);
                    if (!reader.IsDBNull(idxQuantity)) entity.Quantity = reader.GetInt32(idxQuantity);
                    if (!reader.IsDBNull(idxName)) entity.Name = reader.GetString(idxName);
                    if (!reader.IsDBNull(idxImageName)) entity.ImageName = reader.GetString(idxImageName);

                    items.Add(entity);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                throw;
            }

            return items;
        }
      
        public async Task<List<PersonProductsOwnedEntity>> GetAllAsync()
        {
            List<PersonProductsOwnedEntity> items = new List<PersonProductsOwnedEntity>();
            try
            {
                string sql = "SELECT PersonProductsOwnedId, PersonId, ShopItemId, Quantity FROM PersonProductsOwned";
                IDataReader reader = sqlhelper.GetReader(sql);

                int idxId = reader.GetOrdinal("PersonProductsOwnedId");
                int idxPersonId = reader.GetOrdinal("PersonId");
                int idxShopItemId = reader.GetOrdinal("ShopItemId");
                int idxQuantity = reader.GetOrdinal("Quantity");

                while (reader.Read())
                {
                    var entity = new PersonProductsOwnedEntity();

                    if (!reader.IsDBNull(idxId)) entity.PersonProductsOwnedId = reader.GetInt32(idxId);
                    if (!reader.IsDBNull(idxPersonId)) entity.PersonId = reader.GetInt32(idxPersonId);
                    if (!reader.IsDBNull(idxShopItemId)) entity.ShopItemId = reader.GetInt32(idxShopItemId);
                    if (!reader.IsDBNull(idxQuantity)) entity.Quantity = reader.GetInt32(idxQuantity);

                    items.Add(entity);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                throw;
            }

            return items;
        }

        public async Task<List<PersonProductsOwnedEntity>> GetByPersonIdAsync(int personId)
        {
            List<PersonProductsOwnedEntity> items = new List<PersonProductsOwnedEntity>();
            try
            {
                string sql = $"SELECT PersonProductsOwnedId, PersonId, ShopItemId, Quantity FROM PersonProductsOwned WHERE PersonId = {personId}";
                IDataReader reader = sqlhelper.GetReader(sql);

                int idxId = reader.GetOrdinal("PersonProductsOwnedId");
                int idxPersonId = reader.GetOrdinal("PersonId");
                int idxShopItemId = reader.GetOrdinal("ShopItemId");
                int idxQuantity = reader.GetOrdinal("Quantity");

                while (reader.Read())
                {
                    var entity = new PersonProductsOwnedEntity();

                    if (!reader.IsDBNull(idxId)) entity.PersonProductsOwnedId = reader.GetInt32(idxId);
                    if (!reader.IsDBNull(idxPersonId)) entity.PersonId = reader.GetInt32(idxPersonId);
                    if (!reader.IsDBNull(idxShopItemId)) entity.ShopItemId = reader.GetInt32(idxShopItemId);
                    if (!reader.IsDBNull(idxQuantity)) entity.Quantity = reader.GetInt32(idxQuantity);

                    items.Add(entity);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                throw;
            }

            return items;
        }

        public async Task<PersonProductsOwnedEntity> GetByIdAsync(int id)
        {
            PersonProductsOwnedEntity item = new PersonProductsOwnedEntity();
            try
            {
                string sql = $"SELECT PersonProductsOwnedId, PersonId, ShopItemId, Quantity FROM PersonProductsOwned WHERE PersonProductsOwnedId = {id}";
                IDataReader reader = sqlhelper.GetReader(sql);

                int idxId = reader.GetOrdinal("PersonProductsOwnedId");
                int idxPersonId = reader.GetOrdinal("PersonId");
                int idxShopItemId = reader.GetOrdinal("ShopItemId");
                int idxQuantity = reader.GetOrdinal("Quantity");

                while (reader.Read())
                {
                    if (!reader.IsDBNull(idxId)) item.PersonProductsOwnedId = reader.GetInt32(idxId);
                    if (!reader.IsDBNull(idxPersonId)) item.PersonId = reader.GetInt32(idxPersonId);
                    if (!reader.IsDBNull(idxShopItemId)) item.ShopItemId = reader.GetInt32(idxShopItemId);
                    if (!reader.IsDBNull(idxQuantity)) item.Quantity = reader.GetInt32(idxQuantity);
                }

                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }

            return item;
        }

        public async Task<PersonProductsOwnedEntity> GetByPersonAndShopItemAsync(int personId, int shopItemId)
        {
            PersonProductsOwnedEntity item = new PersonProductsOwnedEntity();
            try
            {
                string sql = $"SELECT PersonProductsOwnedId, PersonId, ShopItemId, Quantity FROM PersonProductsOwned WHERE PersonId = {personId} AND ShopItemId = {shopItemId}";
                IDataReader reader = sqlhelper.GetReader(sql);

                int idxId = reader.GetOrdinal("PersonProductsOwnedId");
                int idxPersonId = reader.GetOrdinal("PersonId");
                int idxShopItemId = reader.GetOrdinal("ShopItemId");
                int idxQuantity = reader.GetOrdinal("Quantity");

                while (reader.Read())
                {
                    if (!reader.IsDBNull(idxId)) item.PersonProductsOwnedId = reader.GetInt32(idxId);
                    if (!reader.IsDBNull(idxPersonId)) item.PersonId = reader.GetInt32(idxPersonId);
                    if (!reader.IsDBNull(idxShopItemId)) item.ShopItemId = reader.GetInt32(idxShopItemId);
                    if (!reader.IsDBNull(idxQuantity)) item.Quantity = reader.GetInt32(idxQuantity);
                }

                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }

            return item;
        }

        public async void Upsert(PersonProductsOwnedEntity entity)
        {
            try
            {
                // If no id provided, try to find by PersonId and ShopItemId
                if (entity.PersonProductsOwnedId == 0)
                {
                    var found = await GetByPersonAndShopItemAsync(entity.PersonId, entity.ShopItemId).ConfigureAwait(false);
                    if (found != null && found.PersonProductsOwnedId != 0)
                        entity.PersonProductsOwnedId = found.PersonProductsOwnedId;
                }

                var foundById = await GetByIdAsync(entity.PersonProductsOwnedId).ConfigureAwait(false);
                var connection = adoNetHelper.GetConnection();

                if (foundById != null && foundById.PersonProductsOwnedId != 0)
                {
                    // Update
                    using var command = connection.CreateCommand();
                    command.CommandText = "UPDATE PersonProductsOwned SET PersonId = @PersonId, ShopItemId = @ShopItemId, Quantity = @Quantity WHERE PersonProductsOwnedId = @Id";
                    command.Parameters.Add(new SqliteParameter("@PersonId", entity.PersonId));
                    command.Parameters.Add(new SqliteParameter("@ShopItemId", entity.ShopItemId));
                    command.Parameters.Add(new SqliteParameter("@Quantity", entity.Quantity));
                    command.Parameters.Add(new SqliteParameter("@Id", entity.PersonProductsOwnedId));
                    command.ExecuteNonQuery();
                }
                else
                {
                    // Insert
                    using var command = connection.CreateCommand();
                    command.CommandText = "INSERT INTO PersonProductsOwned (PersonId, ShopItemId, Quantity) VALUES (@PersonId, @ShopItemId, @Quantity)";
                    command.Parameters.Add(new SqliteParameter("@PersonId", entity.PersonId));
                    command.Parameters.Add(new SqliteParameter("@ShopItemId", entity.ShopItemId));
                    command.Parameters.Add(new SqliteParameter("@Quantity", entity.Quantity));
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
                command.CommandText = "DELETE FROM PersonProductsOwned WHERE PersonProductsOwnedId = @Id";
                command.Parameters.Add(new SqliteParameter("@Id", id));
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteByPersonAndShopItemAsync(int personId, int shopItemId)
        {
            try
            {
                var connection = adoNetHelper.GetConnection();
                using var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM PersonProductsOwned WHERE PersonId = @PersonId AND ShopItemId = @ShopItemId";
                command.Parameters.Add(new SqliteParameter("@PersonId", personId));
                command.Parameters.Add(new SqliteParameter("@ShopItemId", shopItemId));
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateQuantityAsync(int personId, int shopItemId, int newQuantity)
        {
            try
            {
                var connection = adoNetHelper.GetConnection();
                using var command = connection.CreateCommand();
                command.CommandText = "UPDATE PersonProductsOwned SET Quantity = @Quantity WHERE PersonId = @PersonId AND ShopItemId = @ShopItemId";
                command.Parameters.Add(new SqliteParameter("@Quantity", newQuantity));
                command.Parameters.Add(new SqliteParameter("@PersonId", personId));
                command.Parameters.Add(new SqliteParameter("@ShopItemId", shopItemId));
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddQuantityAsync(int personId, int shopItemId, int quantityToAdd)
        {
            try
            {
                var existing = await GetByPersonAndShopItemAsync(personId, shopItemId);

                if (existing != null && existing.PersonProductsOwnedId != 0)
                {
                    // Update existing
                    int newQuantity = existing.Quantity + quantityToAdd;
                    await UpdateQuantityAsync(personId, shopItemId, newQuantity);
                }
                else
                {
                    // Insert new
                    var newEntity = new PersonProductsOwnedEntity
                    {
                        PersonId = personId,
                        ShopItemId = shopItemId,
                        Quantity = quantityToAdd
                    };
                    Upsert(newEntity);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
