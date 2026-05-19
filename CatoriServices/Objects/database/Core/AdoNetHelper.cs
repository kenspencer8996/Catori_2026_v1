using Microsoft.Data.Sqlite;
namespace CatoriServices.Objects.database.Core
{
    internal class AdoNetHelper
    {
        DatabaseHelper databaseHelper;
        public AdoNetHelper()
        {
            databaseHelper = new DatabaseHelper();
           // databaseHelper.CheckOrCreateDB();
        }
        public SqliteConnection GetConnection()
        {
            SqliteConnection connection = databaseHelper.GetConnection();
            return connection;
        }
    }
}

