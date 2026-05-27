using Dapper;
using Microsoft.Data.Sqlite;
namespace CatoriServices.Objects.database.Core
{
    internal class DatabaseHelper
    {
        internal DatabaseHelper()
        {
            try
            {
                        
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
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
                cLogger.Log(ex.ToString());
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());

                throw;
            }
            return connection;
        }
    }
}


