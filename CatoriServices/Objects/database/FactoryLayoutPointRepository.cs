using CatoriServices.Objects.Entities;
using CityAppServices;
using Microsoft.Data.Sqlite;

namespace CatoriServices.Objects.database
{
    public partial class FactoryLayoutPointRepository
    {
        public  readonly string _connectionString;

        public FactoryLayoutPointRepository()
        {
            _connectionString = "Data Source=" + GlobalServices.Database + " ;"; ;
        }
        private FactoryLayoutPointEntity GetEntityFromReader(SqliteDataReader reader)
        {
            {
                return new FactoryLayoutPointEntity
                {
                    FactoryLayoutPointId = reader.GetInt64(reader.GetOrdinal("FactoryLayoutPointId")),
                    FactoryLayoutId = reader.GetInt64(reader.GetOrdinal("FactoryLayoutId")),
                    PointIndex = reader.IsDBNull(reader.GetOrdinal("PointIndex")) ? 0 : reader.GetInt64(reader.GetOrdinal("PointIndex")),
                    PointType = reader.IsDBNull(reader.GetOrdinal("PointType")) ? null : reader.GetString(reader.GetOrdinal("PointType")),
                    XLoc = reader.GetInt64(reader.GetOrdinal("XLoc")),
                    YLoc = reader.GetInt64(reader.GetOrdinal("YLoc")),
                    XLocEnd = reader.IsDBNull(reader.GetOrdinal("XLocEnd")) ? 0 : reader.GetInt64(reader.GetOrdinal("XLocEnd")),
                    YLocEnd = reader.IsDBNull(reader.GetOrdinal("YLocEnd")) ? 0 : reader.GetInt64(reader.GetOrdinal("YLocEnd"))
                };
            }
        }
        public List<FactoryLayoutPointEntity> GetListByLayoutId(long id)
        {
            var list = new List<FactoryLayoutPointEntity>();

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            string sql = $"SELECT * FROM FactoryLayoutPoint WHERE FactoryLayoutId = {id};";

            using var cmd = new SqliteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(GetEntityFromReader(reader));

            }

            return list;
        }
        public List<FactoryLayoutPointEntity> GetByFactoryInteriorName(string name)
        {
            var list = new List<FactoryLayoutPointEntity>();

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            string sql = $"SELECT * FROM FactoryLayoutPoint WHERE FactoryLayoutId = '{name}';";

            using var cmd = new SqliteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(GetEntityFromReader(reader));
            }

            return list;
        }

        public List<FactoryLayoutPointEntity> GetByLayoutId(long layoutId)
        {
            var list = new List<FactoryLayoutPointEntity>();

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            string sql = $@"
            SELECT *
            FROM FactoryLayoutPoint
            WHERE FactoryLayoutId = {layoutId}
            ORDER BY PointIndex;";

            using var cmd = new SqliteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(GetEntityFromReader(reader));

            }

            return list;
        }

    }
}
