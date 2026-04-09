using CatoriServices.Objects.Entities;
using CityAppServices;
using Microsoft.Data.Sqlite;

namespace CatoriServices.Objects.database
{
    public partial class FactoryLayoutPointRepository
    {


        public void Insert(FactoryLayoutPointEntity point)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            string sql = $@"
            INSERT INTO FactoryLayoutPoint
            ( FactoryLayoutId, PointIndex, PointType, XLoc, YLoc, XLocEnd, YLocEnd)
            VALUES (
                
                {point.FactoryLayoutId},
                {point.PointIndex},
                {(point.PointType != null ? $"'{point.PointType.Replace("'", "''")}'" : "NULL")},
                {point.XLoc},
                {point.YLoc},
                {(point.XLocEnd)},
                {(point.YLocEnd)}
            );";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        public void Update(FactoryLayoutPointEntity point)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            string sql = $@"
            UPDATE FactoryLayoutPoint
            SET 
                FactoryLayoutId = {point.FactoryLayoutId},
                PointIndex = {point.PointIndex},
                PointType = {(point.PointType != null ? $"'{point.PointType.Replace("'", "''")}'" : "NULL")},
                XLoc = {point.XLoc},
                YLoc = {point.YLoc},
                XLocEnd = {(point.XLocEnd)},
                YLocEnd = {(point.YLocEnd)}
            WHERE FactoryLayoutPointId = {point.FactoryLayoutPointId};";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        public void Delete(long id)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            string sql = $"DELETE FROM FactoryLayoutPoint WHERE FactoryLayoutPointId = {id};";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        public FactoryLayoutPointEntity? GetById(long id)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            string sql = $"SELECT * FROM FactoryLayoutPoint WHERE FactoryLayoutPointId = {id};";

            using var cmd = new SqliteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            return GetEntityFromReader(reader);

        }

        public List<FactoryLayoutPointEntity> GetAll()
        {
            var list = new List<FactoryLayoutPointEntity>();

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            string sql = "SELECT * FROM FactoryLayoutPoint;";

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