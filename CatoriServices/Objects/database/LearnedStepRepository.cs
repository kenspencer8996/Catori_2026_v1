using CatoriServices.Objects.Entities;
using Microsoft.Data.Sqlite;

namespace CityAppServices.Objects.database
{
    public class LearnedStepRepository
    {
        private readonly string _connectionString;

        public LearnedStepRepository()
        {
            _connectionString = "Data Source=" + GlobalServices.Database + " ;";
        }

        public async Task<LearnedStepEntity?> GetByIdAsync(int learnedStepId)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT LearnedStepId, Name, StepNumber, DisplayName, IsComplete, TreasureStep, ParentName
                FROM LearnedStep
                WHERE LearnedStepId = @LearnedStepId";
            command.Parameters.AddWithValue("@LearnedStepId", learnedStepId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new LearnedStepEntity
                {
                    LearnedStepId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    StepNumber = reader.GetInt32(2),
                    DisplayName = reader.GetString(3),
                    IsComplete = reader.GetBoolean(4),
                    TreasureStep = reader.GetString(5),
                    ParentName = reader.GetString(6)
                };
            }

            return null;
        }

        public async Task<List<LearnedStepEntity>> GetAllAsync()
        {
            var steps = new List<LearnedStepEntity>();

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT LearnedStepId, Name, StepNumber, DisplayName, IsComplete, TreasureStep, ParentName
                FROM LearnedStep
                ORDER BY StepNumber";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var stepentity = new LearnedStepEntity();

                stepentity.LearnedStepId = reader.GetInt32(0);
                stepentity.Name = reader.GetString(1);
                stepentity.StepNumber = reader.GetInt32(2);
                stepentity.DisplayName = reader.GetString(3);
                stepentity.IsComplete = reader.GetBoolean(4);
                stepentity.TreasureStep = reader.GetString(5);
                stepentity.ParentName = reader.GetString(6);
            
                steps.Add(stepentity);
            }

            return steps;
        }

        public async Task<int> InsertAsync(LearnedStepEntity step)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO LearnedStep (Name, StepNumber, DisplayName, IsComplete, TreasureStep, ParentName)
                VALUES (@Name, @StepNumber, @DisplayName, @IsComplete, @TreasureStep, @ParentName);
                SELECT last_insert_rowid();";

            command.Parameters.AddWithValue("@Name", step.Name ?? string.Empty);
            command.Parameters.AddWithValue("@StepNumber", step.StepNumber);
            command.Parameters.AddWithValue("@DisplayName", step.DisplayName);
            command.Parameters.AddWithValue("@IsComplete", step.IsComplete);
            command.Parameters.AddWithValue("@TreasureStep", step.TreasureStep ?? string.Empty);
            command.Parameters.AddWithValue("@ParentName", step.ParentName ?? string.Empty);

            var result = await command.ExecuteScalarAsync();
            step.LearnedStepId = Convert.ToInt32(result);
            return step.LearnedStepId;
        }

        public async Task<int> UpdateAsync(LearnedStepEntity step)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE LearnedStep
                SET Name = @Name,
                    StepNumber = @StepNumber,
                    DisplayName = @DisplayName,
                    IsComplete = @IsComplete,
                    TreasureStep = @TreasureStep,
                    ParentName = @ParentName
                WHERE LearnedStepId = @LearnedStepId";

            command.Parameters.AddWithValue("@LearnedStepId", step.LearnedStepId);
            command.Parameters.AddWithValue("@Name", step.Name);
            command.Parameters.AddWithValue("@StepNumber", step.StepNumber);
            command.Parameters.AddWithValue("@DisplayName", step.DisplayName);
            command.Parameters.AddWithValue("@IsComplete", step.IsComplete);
            command.Parameters.AddWithValue("@TreasureStep", step.TreasureStep ?? string.Empty);
            command.Parameters.AddWithValue("@ParentName", step.ParentName ?? string.Empty);

            return await command.ExecuteNonQueryAsync();
        }

        public async Task<int> DeleteAsync(int learnedStepId)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM LearnedStep WHERE LearnedStepId = @LearnedStepId";
            command.Parameters.AddWithValue("@LearnedStepId", learnedStepId);

            return await command.ExecuteNonQueryAsync();
        }
    }


}