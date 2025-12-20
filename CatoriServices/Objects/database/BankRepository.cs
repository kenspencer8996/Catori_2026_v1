using CatoriServices.Objects.Entities;
using CityAppServices.Objects.database;
using System.Data;

namespace CatoriServices.Objects.database
{
    public class BankRepository
    {

        AdoNetHelper adoNetHelper = new AdoNetHelper();
        SqlHelper sqlhelper = new SqlHelper();

        public async Task<List<BankEntity>> GetBanksAsync()
        {
            List<BankEntity> banks = new List<BankEntity>();
            try
            {
                string sql = "SELECT bankid, BusinesskeyImageNameWOExtension, ImageFileName, Description FROM Bank";
                IDataReader reader = sqlhelper.GetReader(sql);

                int idxBankId = reader.GetOrdinal("bankid");
                int idxBusinessKey = reader.GetOrdinal("BusinesskeyImageNameWOExtension");
                int idxImageFile = reader.GetOrdinal("ImageFileName");
                int idxDescription = reader.GetOrdinal("Description");

                while (reader.Read())
                {
                    BankEntity entity = new BankEntity();
                    if (!reader.IsDBNull(idxBankId)) entity.BankId = reader.GetInt32(idxBankId);
                    if (!reader.IsDBNull(idxBusinessKey)) entity.BusinesskeyImageNameWOExtension = reader.GetString(idxBusinessKey);
                    if (!reader.IsDBNull(idxImageFile)) entity.ImageFileName = reader.GetString(idxImageFile);
                    if (!reader.IsDBNull(idxDescription)) entity.Description = reader.GetString(idxDescription);

                    banks.Add(entity);
                }

                reader.Close();
            }
            catch
            {
                throw;
            }

            return banks;
        }

        public async Task<BankEntity> GetBankByIdAsync(int bankId)
        {
            BankEntity entity = new BankEntity();
            try
            {
                string sql = $"SELECT bankid, BusinesskeyImageNameWOExtension, ImageFileName, Description FROM Bank WHERE bankid = {bankId}";
                IDataReader reader = sqlhelper.GetReader(sql);

                int idxBankId = reader.GetOrdinal("bankid");
                int idxBusinessKey = reader.GetOrdinal("BusinesskeyImageNameWOExtension");
                int idxImageFile = reader.GetOrdinal("ImageFileName");
                int idxDescription = reader.GetOrdinal("Description");

                while (reader.Read())
                {
                    if (!reader.IsDBNull(idxBankId)) entity.BankId = reader.GetInt32(idxBankId);
                    if (!reader.IsDBNull(idxBusinessKey)) entity.BusinesskeyImageNameWOExtension = reader.GetString(idxBusinessKey);
                    if (!reader.IsDBNull(idxImageFile)) entity.ImageFileName = reader.GetString(idxImageFile);
                    if (!reader.IsDBNull(idxDescription)) entity.Description = reader.GetString(idxDescription);
                }

                reader.Close();
            }
            catch
            {
                throw;
            }

            return entity;
        }

        // Matches pattern in existing templates (async void used in template). Consider changing to Task in future.
        public async void Upsert(BankEntity entity)
        {
            try
            {
                BankEntity found = await GetBankByIdAsync(entity.BankId);
                var connection = adoNetHelper.GetConnection();

                if (found != null && found.BankId != 0)
                {
                    // Update
                    using var command = connection.CreateCommand();
                    command.CommandText = "UPDATE Bank SET BusinesskeyImageNameWOExtension = @businessKey, ImageFileName = @imageFile, Description = @description WHERE bankid = @bankId";
                    command.Parameters.AddWithValue("@businessKey", entity.BusinesskeyImageNameWOExtension);
                    command.Parameters.AddWithValue("@imageFile", entity.ImageFileName);
                    command.Parameters.AddWithValue("@description", entity.Description);
                    command.Parameters.AddWithValue("@bankId", entity.BankId);
                    command.ExecuteNonQuery();
                }
                else
                {
                    // Insert
                    using var command = connection.CreateCommand();
                    command.CommandText = "INSERT INTO Bank (BusinesskeyImageNameWOExtension, ImageFileName, Description) VALUES (@businessKey, @imageFile, @description);";
                    command.Parameters.AddWithValue("@businessKey", entity.BusinesskeyImageNameWOExtension);
                    command.Parameters.AddWithValue("@imageFile", entity.ImageFileName);
                    command.Parameters.AddWithValue("@description", entity.Description);
                    command.ExecuteNonQuery();
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
