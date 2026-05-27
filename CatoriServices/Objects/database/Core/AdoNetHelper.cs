using Microsoft.Data.Sqlite;
namespace CatoriServices.Objects.database.Core
{
    internal class AdoNetHelper
    {
        DatabaseHelper databaseHelper;
        public AdoNetHelper()
        {
            try
            {
                            databaseHelper = new DatabaseHelper();
                           // databaseHelper.CheckOrCreateDB();
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }
        public SqliteConnection GetConnection()
        {
            try
            {
                            SqliteConnection connection = databaseHelper.GetConnection();
                            return connection;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }
    }
}

