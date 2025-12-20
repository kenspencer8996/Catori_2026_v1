using CatoriServices.Objects;
using CityAppServices;
using Microsoft.Data.Sqlite;
using System.Data;

namespace CityAppServices.Objects.database
{
    internal class SqlHelper
    {
       internal IDataReader GetReader(String query)
        {
            cLogger.Log("SqlHelper.GetReader: " + query);            
            SqliteDataReader reader = null;
            try
            {
                string connectionString = "Data Source=" + GlobalServices.Database + " ;";
                {
                    SqliteConnection connection = new SqliteConnection(connectionString);
                    
                    SqliteCommand command = new SqliteCommand(query, connection);
                    
                    command.CommandText = query;
                    command.Connection = connection;
                    command.Connection.Open();
                    reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return reader;
        }
    }
}
