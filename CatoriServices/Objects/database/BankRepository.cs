using CatoriServices.Objects.Entities;
using CityAppServices.Objects.database;
using System.Data;
using System.Security.Cryptography;

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
                //List<SqlliteColumnEntity> cols = GetBankColumns();

                string sql = "SELECT bankid, BusinesskeyImageNameWOExtension, ImageFileName, Description,interestrate FROM Bank";
                IDataReader reader = sqlhelper.GetReader(sql);

                int idxBankId = reader.GetOrdinal("bankid");
                int idxBusinessKey = reader.GetOrdinal("BusinesskeyImageNameWOExtension");
                int idxImageFile = reader.GetOrdinal("ImageFileName");
                int idxDescription = reader.GetOrdinal("Description");
                int idxInterestrate = reader.GetOrdinal("interestrate");

                while (reader.Read())
                {
                    BankEntity entity = new BankEntity();
                    if (!reader.IsDBNull(idxBankId)) entity.BankId = reader.GetInt32(idxBankId);
                    if (!reader.IsDBNull(idxBusinessKey)) entity.BusinesskeyImageNameWOExtension = reader.GetString(idxBusinessKey);
                    if (!reader.IsDBNull(idxImageFile)) entity.ImageFileName = reader.GetString(idxImageFile);
                    if (!reader.IsDBNull(idxDescription)) entity.Description = reader.GetString(idxDescription);
                    if (!reader.IsDBNull(idxInterestrate)) entity.Interestrate = reader.GetDecimal(idxInterestrate);

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
        private List<SqlliteColumnEntity> GetBankColumns()
        {
            List<SqlliteColumnEntity> columns = new List<SqlliteColumnEntity>();
            string sql = "PRAGMA table_info(\"bank\");";
            IDataReader reader = sqlhelper.GetReader(sql);
            int idxcid = reader.GetOrdinal("cid");
            int idxname = reader.GetOrdinal("name");
            int idxType = reader.GetOrdinal("type");
            int idxdflt_value = reader.GetOrdinal("dflt_value");
            int idxpk = reader.GetOrdinal("pk");

            while (reader.Read())
            {
                SqlliteColumnEntity entity = new SqlliteColumnEntity();
                if (!reader.IsDBNull(idxcid)) entity.cid = reader.GetInt32(idxcid);
                if (!reader.IsDBNull(idxname)) entity.name = reader.GetString(idxname);
                if (!reader.IsDBNull(idxType)) entity.type = reader.GetString(idxType);
                if (!reader.IsDBNull(idxdflt_value)) entity.dflt_value = reader.GetString(idxdflt_value);
                if (!reader.IsDBNull(idxpk)) entity.pk = reader.GetString(idxpk);

                columns.Add(entity);
            }
            return columns;
        }

        public async Task<BankEntity> GetBankByIdAsync(int bankId)
        {
            BankEntity entity = new BankEntity();
            try
            {
                string sql = $"SELECT bankid, BusinesskeyImageNameWOExtension, ImageFileName, Description,interestrate FROM Bank WHERE bankid = {bankId}";
                IDataReader reader = sqlhelper.GetReader(sql);

                int idxBankId = reader.GetOrdinal("bankid");
                int idxBusinessKey = reader.GetOrdinal("BusinesskeyImageNameWOExtension");
                int idxImageFile = reader.GetOrdinal("ImageFileName");
                int idxDescription = reader.GetOrdinal("Description");
                int idxInterestrate = reader.GetOrdinal("Interestrate");

                while (reader.Read())
                {
                    if (!reader.IsDBNull(idxBankId)) entity.BankId = reader.GetInt32(idxBankId);
                    if (!reader.IsDBNull(idxBusinessKey)) entity.BusinesskeyImageNameWOExtension = reader.GetString(idxBusinessKey);
                    if (!reader.IsDBNull(idxImageFile)) entity.ImageFileName = reader.GetString(idxImageFile);
                    if (!reader.IsDBNull(idxDescription)) entity.Description = reader.GetString(idxDescription);
                    if (!reader.IsDBNull(idxInterestrate)) entity.Interestrate = reader.GetDecimal(idxInterestrate);
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
                    command.CommandText = "UPDATE Bank SET BusinesskeyImageNameWOExtension = @businessKey, ImageFileName = @imageFile, Description = @description, interestrate = interestrate WHERE bankid = @bankId";
                    command.Parameters.AddWithValue("@businessKey", entity.BusinesskeyImageNameWOExtension);
                    command.Parameters.AddWithValue("@imageFile", entity.ImageFileName);
                    command.Parameters.AddWithValue("@description", entity.Description);
                    command.Parameters.AddWithValue("@bankId", entity.BankId);
                    command.Parameters.AddWithValue("@Interestrate", entity.Interestrate);
                    command.ExecuteNonQuery();
                }
                else
                {
                    // Insert
                    using var command = connection.CreateCommand();
                    command.CommandText = "INSERT INTO Bank (BusinesskeyImageNameWOExtension, ImageFileName, Description,interestrate) VALUES (@businessKey, @imageFile, @description);";
                    command.Parameters.AddWithValue("@businessKey", entity.BusinesskeyImageNameWOExtension);
                    command.Parameters.AddWithValue("@imageFile", entity.ImageFileName);
                    command.Parameters.AddWithValue("@description", entity.Description);
                    command.Parameters.AddWithValue("@Interestrate", entity.Interestrate);
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
