using CatoriServices.Objects.Entities;
using CityAppServices.Objects.database;
using System.Data;

namespace CatoriServices.Objects.database
{
    public class BankCustomerFundsRepository
    {
        AdoNetHelper adoNetHelper = new AdoNetHelper();
        SqlHelper sqlhelper = new SqlHelper();

        public async Task<List<BankCustomerFundsEntity>> GetBankCustomerFundsAsync()
        {
            List<BankCustomerFundsEntity> results = new List<BankCustomerFundsEntity>();
            try
            {
                string sql = "SELECT BankCustomerFundsId, Amount, LastUpdated FROM BankCustomerFunds";
                IDataReader reader = sqlhelper.GetReader(sql);

                int idxId = reader.GetOrdinal("BankCustomerFundsId");
                int idxAmount = reader.GetOrdinal("Amount");
                int idxLastUpdated = reader.GetOrdinal("LastUpdated");

                while (reader.Read())
                {
                    BankCustomerFundsEntity entity = new BankCustomerFundsEntity();
                    if (!reader.IsDBNull(idxId)) entity.BankCustomerFundsId = reader.GetInt32(idxId);
                    if (!reader.IsDBNull(idxAmount)) entity.Amount = reader.GetDouble(idxAmount);
                    if (!reader.IsDBNull(idxLastUpdated)) entity.LastUpdated = reader.GetString(idxLastUpdated);

                    results.Add(entity);
                }

                reader.Close();
            }
            catch
            {
                throw;
            }

            return results;
        }

        public async Task<BankCustomerFundsEntity> GetBankCustomerFundsByIdAsync(int id)
        {
            BankCustomerFundsEntity entity = new BankCustomerFundsEntity();
            try
            {
                string sql = $"SELECT BankCustomerFundsId, Amount, LastUpdated FROM BankCustomerFunds WHERE BankCustomerFundsId = {id}";
                IDataReader reader = sqlhelper.GetReader(sql);

                int idxId = reader.GetOrdinal("BankCustomerFundsId");
                int idxAmount = reader.GetOrdinal("Amount");
                int idxLastUpdated = reader.GetOrdinal("LastUpdated");

                while (reader.Read())
                {
                    if (!reader.IsDBNull(idxId)) entity.BankCustomerFundsId = reader.GetInt32(idxId);
                    if (!reader.IsDBNull(idxAmount)) entity.Amount = reader.GetDouble(idxAmount);
                    if (!reader.IsDBNull(idxLastUpdated)) entity.LastUpdated = reader.GetString(idxLastUpdated);
                }

                reader.Close();
            }
            catch
            {
                throw;
            }

            return entity;
        }

        // Follows existing project pattern: async void used in repository templates.
        // Consider changing to Task for callers that need to await completion.
        public async void Upsert(BankCustomerFundsEntity entity)
        {
            try
            {
                BankCustomerFundsEntity found = await GetBankCustomerFundsByIdAsync(entity.BankCustomerFundsId);
                var connection = adoNetHelper.GetConnection();

                if (found != null && found.BankCustomerFundsId != 0)
                {
                    // Update
                    using var command = connection.CreateCommand();
                    command.CommandText = "UPDATE BankCustomerFunds SET Amount = @amount, LastUpdated = @lastUpdated WHERE BankCustomerFundsId = @id";
                    command.Parameters.AddWithValue("@amount", entity.Amount);
                    command.Parameters.AddWithValue("@lastUpdated", entity.LastUpdated ?? string.Empty);
                    command.Parameters.AddWithValue("@id", entity.BankCustomerFundsId);
                    command.ExecuteNonQuery();
                }
                else
                {
                    // Insert
                    using var command = connection.CreateCommand();
                    command.CommandText = "INSERT INTO BankCustomerFunds (Amount, LastUpdated) VALUES (@amount, @lastUpdated);";
                    command.Parameters.AddWithValue("@amount", entity.Amount);
                    command.Parameters.AddWithValue("@lastUpdated", entity.LastUpdated ?? string.Empty);
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
