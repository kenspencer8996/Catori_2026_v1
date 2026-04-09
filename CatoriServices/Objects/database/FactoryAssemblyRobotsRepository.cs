using CatoriServices.Objects.Entities;
using CityAppServices;
using Microsoft.Data.Sqlite;
using System.Text;

namespace CatoriServices.Objects.database
{
    public class FactoryAssemblyRobotsRepository
    {
        private readonly string _connectionString;

        public FactoryAssemblyRobotsRepository()
        {
            _connectionString = "Data Source=" + GlobalServices.Database + " ;";
        }

        private SqliteConnection GetConnection()
        {
            return new SqliteConnection(_connectionString);
        }

        public List<FactoryAssemblyRobotEntity> GetAll()
        {
            var list = new List<FactoryAssemblyRobotEntity>();

            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = "SELECT Id, Name, FactoryLayoutId, RobotType, Xloc, Yloc, Width, Height,FactoryInteriorName FROM FactoryAssemblyRobots";

                using (var cmd = new SqliteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new FactoryAssemblyRobotEntity
                        {
                            Id = reader.GetInt64(0),
                            Name = reader.GetString(1),
                            FactoryLayoutId = reader.GetInt64(2),
                            RobotType = reader.GetString(3),
                            Xloc = reader.GetInt64(4),
                            Yloc = reader.GetInt64(5),
                            Width = reader.GetInt64(6),
                            Height = reader.GetInt64(7),
                            FactoryInteriorName = reader.GetString(8)
                        });
                    }
                }
            }

            return list;
        }
        public List<FactoryAssemblyRobotEntity> GetbyFactoryLayoutId(long id)
        {
            var list = new List<FactoryAssemblyRobotEntity>();

            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = "SELECT Id, Name, FactoryLayoutId, RobotType, Xloc, Yloc, Width, Height,FactoryInteriorName FROM FactoryAssemblyRobots";
                sql += $" WHERE FactoryLayoutId = {id}";

                using (var cmd = new SqliteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new FactoryAssemblyRobotEntity
                        {
                            Id = reader.GetInt64(0),
                            Name = reader.GetString(1),
                            FactoryLayoutId = reader.GetInt64(2),
                            RobotType = reader.GetString(3),
                            Xloc = reader.GetInt64(4),
                            Yloc = reader.GetInt64(5),
                            Width = reader.GetInt64(6),
                            Height = reader.GetInt64(7),
                            FactoryInteriorName = reader.GetString(8)
                        });
                    }
                }
            }

            return list;
        }
        public FactoryAssemblyRobotEntity? GetById(long id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = $"SELECT Id, Name, FactoryLayoutId, RobotType, Xloc, Yloc, Width, Height,FactoryInteriorName " +
                             $"FROM FactoryAssemblyRobots WHERE Id = {id}";

                using (var cmd = new SqliteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new FactoryAssemblyRobotEntity
                        {
                            Id = reader.GetInt64(0),
                            Name = reader.GetString(1),
                            FactoryLayoutId = reader.GetInt64(2),
                            RobotType = reader.GetString(3),
                            Xloc = reader.GetInt64(4),
                            Yloc = reader.GetInt64(5),
                            Width = reader.GetInt64(6),
                            Height = reader.GetInt64(7),
                            FactoryInteriorName = reader.GetString(8)
                        };
                    }
                }
            }

            return null;
        }

        public long Insert(FactoryAssemblyRobotEntity robot)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string sql =
                    $"INSERT INTO FactoryAssemblyRobots (Name, FactoryLayoutId, RobotType, Xloc, Yloc, Width, Height,FactoryInteriorName) " +
                    $"VALUES ('{robot.Name}', {robot.FactoryLayoutId}, '{robot.RobotType}', {robot.Xloc}, {robot.Yloc}, {robot.Width}, {robot.Height},'{robot.FactoryInteriorName}'); " +
                    $"SELECT last_insert_rowid();";

                using (var cmd = new SqliteCommand(sql, conn))
                {
                    return (long)cmd.ExecuteScalar();
                }
            }
        }

        public void Update(FactoryAssemblyRobotEntity robot)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string sql =
                    $"UPDATE FactoryAssemblyRobots SET " +
                    $"Name = '{robot.Name}', " +
                    $"FactoryLayoutId = {robot.FactoryLayoutId}, " +
                    $"RobotType = '{robot.RobotType}', " +
                    $"Xloc = {robot.Xloc}, " +
                    $"Yloc = {robot.Yloc}, " +
                    $"Width = {robot.Width}, " +
                    $"Height = {robot.Height}, " +
                    $"FactoryInteriorName = '{robot.FactoryInteriorName}' " +
                    $"WHERE Id = {robot.Id}" ;

                using (var cmd = new SqliteCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(long id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = $"DELETE FROM FactoryAssemblyRobots WHERE Id = {id}";

                using (var cmd = new SqliteCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
