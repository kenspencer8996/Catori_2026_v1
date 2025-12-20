using Dapper;
using Microsoft.Data.Sqlite;

namespace CityAppServices.Objects.database
{
    public class LoggingRepository
    {
        AdoNetHelper adoNetHelper = new AdoNetHelper();

        public async Task<List<LogEntity>> GetLogAsync()
        {
            List<LogEntity> Log = new List<LogEntity>();
            try
            {
                var connection = adoNetHelper.GetConnection();
                var sql = "SELECT * FROM Logging";
                var results = await connection.QueryAsync<LogEntity>(sql);
                if (results != null && results.Any())
                {
                    Log = results.ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return Log;
        }
   
        public async void UpsertLogging(LogEntity Logging)
        {
            bool result = false;
            var connection = adoNetHelper.GetConnection();

                try
                {
                    string sqlraw = "INSERT INTO Logging ( ClassName,MethodName,Message,ImageType) ";
                    sqlraw += "VALUES('" + Logging.ClassName + "','" + Logging.MethodName + "',";
                    sqlraw += "'" + Logging.Message + "'," + Logging.RunTime + ")";
                    var command = new SqliteCommand();
                    command.Connection = connection;
                    command.CommandText = sqlraw;
                    command.ExecuteNonQuery();

                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }
    }

