using Dapper;
using Microsoft.Data.Sqlite;
namespace CatoriServices.Objects.database.Core
{
    internal class DatabaseHelper
    {
        internal DatabaseHelper()
        {

        }

        internal SqliteConnection GetConnection()
        {
            string connectionString = $"Data Source=" + GlobalServices.Database;
            SqliteConnection connection = null;
            try
            {
                connection = new SqliteConnection(connectionString);
                connection.Open();

                Console.WriteLine("Connected to the SQLite database!");

            }
            catch (SqliteException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {

                throw;
            }
            return connection;
        }
    }
}


