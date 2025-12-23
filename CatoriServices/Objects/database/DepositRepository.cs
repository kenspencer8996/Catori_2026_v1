using CatoriServices.Objects.Entities;
using CityAppServices.Objects.database;
using System.Data;

namespace CatoriServices.Objects.database
{


    public class DepositRepository
    {
        AdoNetHelper adoNetHelper = new AdoNetHelper();
        SqlHelper sqlhelper = new SqlHelper();

        public async Task<List<DepositEntity>> GetDepositsAsync()
        {
            List<DepositEntity> deposits = new List<DepositEntity>();
            try
            {
                string sql = "SELECT DepositId, d.BankId, PersonId, Amount,d.businessname FROM Deposit d " ;
                IDataReader reader = sqlhelper.GetReader(sql);

                int idxDepositId = reader.GetOrdinal("DepositId");
                int idxBankId = reader.GetOrdinal("BankId");
                int idxPersonId = reader.GetOrdinal("PersonId");
                int idxAmount = reader.GetOrdinal("Amount");
                int idxBusinessName = reader.GetOrdinal("businessname");

                while (reader.Read())
                {
                    DepositEntity entity = new DepositEntity();
                    if (!reader.IsDBNull(idxDepositId)) entity.DepositId = reader.GetInt32(idxDepositId);
                    if (!reader.IsDBNull(idxBankId)) entity.BankId = reader.GetInt32(idxBankId);
                    if (!reader.IsDBNull(idxPersonId)) entity.PersonId = reader.GetInt32(idxPersonId);
                    if (!reader.IsDBNull(idxAmount)) entity.Amount = reader.GetDecimal(idxAmount);
                    if (!reader.IsDBNull(idxBusinessName)) entity.BusinessName = reader.GetString(idxBusinessName);

                    deposits.Add(entity);
                }

                reader.Close();
            }
            catch
            {
                throw;
            }

            return deposits;
        }
        public async Task<List<DepositEntity>> GetDepositsForPersonAsync(int personid)
        {
            List<DepositEntity> deposits = new List<DepositEntity>();
            try
            {
                string sql = "SELECT DepositId, d.BankId, PersonId, Amount,d.businessname FROM Deposit d ";
                sql += " where personid = " + personid;
                IDataReader reader = sqlhelper.GetReader(sql);

                int idxDepositId = reader.GetOrdinal("DepositId");
                int idxBankId = reader.GetOrdinal("BankId");
                int idxPersonId = reader.GetOrdinal("PersonId");
                int idxAmount = reader.GetOrdinal("Amount");
                int idxBusinessName = reader.GetOrdinal("businessname");

                while (reader.Read())
                {
                    DepositEntity entity = new DepositEntity();
                    if (!reader.IsDBNull(idxDepositId)) entity.DepositId = reader.GetInt32(idxDepositId);
                    if (!reader.IsDBNull(idxBankId)) entity.BankId = reader.GetInt32(idxBankId);
                    if (!reader.IsDBNull(idxPersonId)) entity.PersonId = reader.GetInt32(idxPersonId);
                    if (!reader.IsDBNull(idxAmount)) entity.Amount = reader.GetDecimal(idxAmount);
                    if (!reader.IsDBNull(idxBusinessName)) entity.BusinessName = reader.GetString(idxBusinessName);

                    deposits.Add(entity);
                }

                reader.Close();
            }
            catch
            {
                throw;
            }

            return deposits;
        }
        public async Task<DepositEntity> GetDepositByIdAsync(int depositId)
        {
            DepositEntity entity = new DepositEntity();
            try
            {
                string sql = $"SELECT DepositId, BankId,businessname, PersonId, Amount FROM Deposit WHERE DepositId = {depositId}";
                IDataReader reader = sqlhelper.GetReader(sql);

                int idxDepositId = reader.GetOrdinal("DepositId");
                int idxBankId = reader.GetOrdinal("BankId");
                int idxPersonId = reader.GetOrdinal("PersonId");
                int idxAmount = reader.GetOrdinal("Amount");
                int idxBusinessName = reader.GetOrdinal("businessname");

                while (reader.Read())
                {
                    if (!reader.IsDBNull(idxDepositId)) entity.DepositId = reader.GetInt32(idxDepositId);
                    if (!reader.IsDBNull(idxBankId)) entity.BankId = reader.GetInt32(idxBankId);
                    if (!reader.IsDBNull(idxPersonId)) entity.PersonId = reader.GetInt32(idxPersonId);
                    if (!reader.IsDBNull(idxAmount)) entity.Amount = reader.GetDecimal(idxAmount);
                    if (!reader.IsDBNull(idxBusinessName)) entity.BusinessName = reader.GetString(idxBusinessName);
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
        public async void Upsert(DepositEntity entity)
        {
            try
            {
                if (entity.DepositId == 0)
                {
                    var deposits = await GetDepositsForPersonAsync(entity.PersonId);
                    var forbank = deposits.FirstOrDefault(d => d.BankId == entity.BankId);
                    if (forbank != null)
                    {
                        entity.DepositId = forbank.DepositId;
                    }
                }
                DepositEntity foundDeposit = await GetDepositByIdAsync(entity.DepositId);
                var connection = adoNetHelper.GetConnection();

                if (foundDeposit != null && foundDeposit.DepositId != 0)
                {
                    // Update
                    using var command = connection.CreateCommand();
                    command.CommandText = "UPDATE Deposit SET BankId = @BankId, PersonId = @personId, Amount = @amount WHERE DepositId = @depositId";
                    command.Parameters.AddWithValue("@BankId", entity.BankId);
                    command.Parameters.AddWithValue("@personId", entity.PersonId);
                    command.Parameters.AddWithValue("@amount", entity.Amount);
                    command.Parameters.AddWithValue("@depositId", entity.DepositId);
                    command.Parameters.AddWithValue("@businessname", entity.BusinessName);
                    command.ExecuteNonQuery();
                }
                else
                {
                    // Insert
                    using var command = connection.CreateCommand();
                    command.CommandText = "INSERT INTO Deposit (BankId, PersonId, Amount,businessname) VALUES (@BankId, @personId, @amount,@businessname);";
                    command.Parameters.AddWithValue("@BankId", entity.BankId);
                    command.Parameters.AddWithValue("@personId", entity.PersonId);
                    command.Parameters.AddWithValue("@amount", entity.Amount);
                    command.Parameters.AddWithValue("@businessname", entity.BusinessName);
                    command.ExecuteNonQuery();
                }
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<DepositEntity>> GetDepositsForBankAsync(int bankId)
        {
            List<DepositEntity> deposits = new List<DepositEntity>();
            try
            {
                string sql = "SELECT DepositId, d.BankId, PersonId, Amount,d.businessname FROM Deposit d ";
                sql += " where d.BankId = " + bankId;
                IDataReader reader = sqlhelper.GetReader(sql);

                int idxDepositId = reader.GetOrdinal("DepositId");
                int idxBankId = reader.GetOrdinal("BankId");
                int idxPersonId = reader.GetOrdinal("PersonId");
                int idxAmount = reader.GetOrdinal("Amount");
                int idxBusinessName = reader.GetOrdinal("businessname");

                while (reader.Read())
                {
                    DepositEntity entity = new DepositEntity();
                    if (!reader.IsDBNull(idxDepositId)) entity.DepositId = reader.GetInt32(idxDepositId);
                    if (!reader.IsDBNull(idxBankId)) entity.BankId = reader.GetInt32(idxBankId);
                    if (!reader.IsDBNull(idxPersonId)) entity.PersonId = reader.GetInt32(idxPersonId);
                    if (!reader.IsDBNull(idxAmount)) entity.Amount = reader.GetDecimal(idxAmount);
                    if (!reader.IsDBNull(idxBusinessName)) entity.BusinessName = reader.GetString(idxBusinessName);

                    deposits.Add(entity);
                }

                reader.Close();
            }
            catch
            {
                throw;
            }

            return deposits;
        }
    }
}
