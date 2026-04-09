using CatoriServices.Objects.Entities;
using CityAppServices;
using Microsoft.Data.Sqlite;

namespace CatoriServices.Objects.database
{
    public class LearnedStepRepository
    {
        private readonly string _connectionString;

        public LearnedStepRepository()
        {
            _connectionString = "Data Source=" + GlobalServices.Database + " ;";
        }

        private SqliteConnection GetConnection()
        {
            return new SqliteConnection(_connectionString);
        }

        public List<LearnedStepEntity> GetAll()
        {
            var list = new List<LearnedStepEntity>();

            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = "SELECT FactoryInteriorId, FactoryInteriorName, StepNumber, DisplayName, IsComplete FROM LearnedStep";

                using (var cmd = new SqliteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new LearnedStepEntity
                        {
                            FactoryInteriorId = reader.GetInt32(0),
                            FactoryInteriorName = reader.GetString(1),
                            StepNumber = reader.GetInt32(2),
                            DisplayName = reader.GetString(3),
                            IsComplete = reader.GetInt32(4) == 1
                        });
                    }
                }
            }

            return list;
        }

        public List<LearnedStepEntity> GetByFactoryInteriorFactoryName(string factoryInteriorname)
        {
            var list = new List<LearnedStepEntity>();

            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = "SELECT FactoryInteriorId, FactoryInteriorName, StepNumber, DisplayName, IsComplete FROM LearnedStep WHERE FactoryInteriorId = @FactoryInteriorId ORDER BY StepNumber";

                using (var cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@factoryInteriorname", factoryInteriorname);
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new LearnedStepEntity
                            {
                                FactoryInteriorId = reader.GetInt32(0),
                                FactoryInteriorName = reader.GetString(1),
                                StepNumber = reader.GetInt32(2),
                                DisplayName = reader.GetString(3),
                                IsComplete = reader.GetInt32(4) == 1
                            });
                        }
                    }
                }
            }

            return list;
        }

        public LearnedStepEntity? GetByFactoryInteriorIdAndStepNumber(int factoryInteriorId, int stepNumber)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = "SELECT FactoryInteriorId, FactoryInteriorName, StepNumber, DisplayName, IsComplete FROM LearnedStep WHERE FactoryInteriorId = @FactoryInteriorId AND StepNumber = @StepNumber";

                using (var cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@FactoryInteriorId", factoryInteriorId);
                    cmd.Parameters.AddWithValue("@StepNumber", stepNumber);
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new LearnedStepEntity
                            {
                                FactoryInteriorId = reader.GetInt32(0),
                                FactoryInteriorName = reader.GetString(1),
                                StepNumber = reader.GetInt32(2),
                                DisplayName = reader.GetString(3),
                                IsComplete = reader.GetInt32(4) == 1
                            };
                        }
                    }
                }
            }

            return null;
        }

        public void Insert(LearnedStepEntity step)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string sql = "INSERT INTO LearnedStep (FactoryInteriorId, FactoryInteriorName, StepNumber, DisplayName, IsComplete) " +
                            "VALUES (@FactoryInteriorId, @FactoryInteriorName, @StepNumber, @DisplayName, @IsComplete)";

                using (var cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@FactoryInteriorId", step.FactoryInteriorId);
                    cmd.Parameters.AddWithValue("@FactoryInteriorName", step.FactoryInteriorName);
                    cmd.Parameters.AddWithValue("@StepNumber", step.StepNumber);
                    cmd.Parameters.AddWithValue("@DisplayName", step.DisplayName);
                    cmd.Parameters.AddWithValue("@IsComplete", step.IsComplete ? 1 : 0);
                    
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(LearnedStepEntity step)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string sql = "UPDATE LearnedStep SET " +
                            "FactoryInteriorName = @FactoryInteriorName, " +
                            "DisplayName = @DisplayName, " +
                            "IsComplete = @IsComplete " +
                            "WHERE FactoryInteriorId = @FactoryInteriorId AND StepNumber = @StepNumber";

                using (var cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@FactoryInteriorId", step.FactoryInteriorId);
                    cmd.Parameters.AddWithValue("@FactoryInteriorName", step.FactoryInteriorName);
                    cmd.Parameters.AddWithValue("@StepNumber", step.StepNumber);
                    cmd.Parameters.AddWithValue("@DisplayName", step.DisplayName);
                    cmd.Parameters.AddWithValue("@IsComplete", step.IsComplete ? 1 : 0);
                    
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateIsComplete(int factoryInteriorId, int stepNumber, bool isComplete)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string sql = "UPDATE LearnedStep SET IsComplete = @IsComplete " +
                            "WHERE FactoryInteriorId = @FactoryInteriorId AND StepNumber = @StepNumber";

                using (var cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@FactoryInteriorId", factoryInteriorId);
                    cmd.Parameters.AddWithValue("@StepNumber", stepNumber);
                    cmd.Parameters.AddWithValue("@IsComplete", isComplete ? 1 : 0);
                    
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int factoryInteriorId, int stepNumber)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = "DELETE FROM LearnedStep WHERE FactoryInteriorId = @FactoryInteriorId AND StepNumber = @StepNumber";

                using (var cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@FactoryInteriorId", factoryInteriorId);
                    cmd.Parameters.AddWithValue("@StepNumber", stepNumber);
                    
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteAllByFactoryInteriorId(int factoryInteriorId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = "DELETE FROM LearnedStep WHERE FactoryInteriorId = @FactoryInteriorId";

                using (var cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@FactoryInteriorId", factoryInteriorId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
