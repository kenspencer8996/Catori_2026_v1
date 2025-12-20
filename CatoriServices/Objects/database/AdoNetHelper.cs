using Microsoft.Data.Sqlite;

namespace CityAppServices.Objects.database
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
