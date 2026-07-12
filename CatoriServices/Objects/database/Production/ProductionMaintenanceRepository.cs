using CatoriServices.Objects.Entities.Production;
using Microsoft.Data.Sqlite;

namespace CatoriServices.Objects.database.Production
{
    public class ProductionMaintenanceRepository
    {
        private readonly string _connectionString;

        public ProductionMaintenanceRepository()
        {
            _connectionString = "Data Source=" + GlobalServices.Database + " ;";

            using var conn = GetConnection();
            conn.Open();
            EnsureSchema(conn);
        }

        private SqliteConnection GetConnection()
            => new SqliteConnection(_connectionString);

        public async Task<List<FactoryCapabilityEntity>> GetAllFactoryCapabilitiesAsync()
        {
            using var conn = GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqliteCommand("SELECT * FROM FactoryCapability ORDER BY CapabilityType, CapabilityName, LocationId", conn);
            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<FactoryCapabilityEntity>();
            while (await reader.ReadAsync())
                list.Add(MapFactoryCapability(reader));

            return list;
        }

        public async Task<long> SaveFactoryCapabilityAsync(FactoryCapabilityEntity entity)
        {
            return entity.FactoryCapabilityId <= 0
                ? await InsertFactoryCapabilityAsync(entity)
                : await UpdateFactoryCapabilityAsync(entity);
        }

        private async Task<long> InsertFactoryCapabilityAsync(FactoryCapabilityEntity entity)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqliteCommand(@"
                INSERT INTO FactoryCapability (LocationId, CapabilityType, CapabilityName, IsEnabled)
                VALUES (@LocationId, @CapabilityType, @CapabilityName, @IsEnabled);
                SELECT last_insert_rowid();", conn);
            AddFactoryCapabilityParameters(cmd, entity);
            entity.FactoryCapabilityId = Convert.ToInt64(await cmd.ExecuteScalarAsync());
            return entity.FactoryCapabilityId;
        }

        private async Task<long> UpdateFactoryCapabilityAsync(FactoryCapabilityEntity entity)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqliteCommand(@"
                UPDATE FactoryCapability
                SET LocationId = @LocationId,
                    CapabilityType = @CapabilityType,
                    CapabilityName = @CapabilityName,
                    IsEnabled = @IsEnabled
                WHERE FactoryCapabilityId = @FactoryCapabilityId", conn);
            AddFactoryCapabilityParameters(cmd, entity);
            cmd.Parameters.AddWithValue("@FactoryCapabilityId", entity.FactoryCapabilityId);
            await cmd.ExecuteNonQueryAsync();
            return entity.FactoryCapabilityId;
        }

        public async Task<List<ProductProduceRequirementEntity>> GetAllProductProduceRequirementsAsync()
        {
            using var conn = GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqliteCommand("SELECT * FROM ProductProduceRequirement ORDER BY productId, RequiredCapabilityType, RequiredCapabilityName", conn);
            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<ProductProduceRequirementEntity>();
            while (await reader.ReadAsync())
                list.Add(MapProductProduceRequirement(reader));

            return list;
        }

        public async Task<long> SaveProductProduceRequirementAsync(ProductProduceRequirementEntity entity)
        {
            return entity.ProductProduceRequirementIdId <= 0
                ? await InsertProductProduceRequirementAsync(entity)
                : await UpdateProductProduceRequirementAsync(entity);
        }

        private async Task<long> InsertProductProduceRequirementAsync(ProductProduceRequirementEntity entity)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqliteCommand(@"
                INSERT INTO ProductProduceRequirement (productId, RequiredCapabilityType, RequiredCapabilityName, MinimumFactoryLevel)
                VALUES (@ProductId, @RequiredCapabilityType, @RequiredCapabilityName, @MinimumFactoryLevel);
                SELECT last_insert_rowid();", conn);
            AddProductProduceRequirementParameters(cmd, entity);
            entity.ProductProduceRequirementIdId = Convert.ToInt64(await cmd.ExecuteScalarAsync());
            return entity.ProductProduceRequirementIdId;
        }

        private async Task<long> UpdateProductProduceRequirementAsync(ProductProduceRequirementEntity entity)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqliteCommand(@"
                UPDATE ProductProduceRequirement
                SET productId = @ProductId,
                    RequiredCapabilityType = @RequiredCapabilityType,
                    RequiredCapabilityName = @RequiredCapabilityName,
                    MinimumFactoryLevel = @MinimumFactoryLevel
                WHERE ProductProduceRequirementIdId = @ProductProduceRequirementIdId", conn);
            AddProductProduceRequirementParameters(cmd, entity);
            cmd.Parameters.AddWithValue("@ProductProduceRequirementIdId", entity.ProductProduceRequirementIdId);
            await cmd.ExecuteNonQueryAsync();
            return entity.ProductProduceRequirementIdId;
        }

        public async Task<List<LocationUnlockRuleEntity>> GetAllLocationUnlockRulesAsync()
        {
            using var conn = GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqliteCommand("SELECT * FROM LocationUnlockRule ORDER BY LocationId, LocationUnlockRuleId", conn);
            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<LocationUnlockRuleEntity>();
            while (await reader.ReadAsync())
                list.Add(MapLocationUnlockRule(reader));

            return list;
        }

        public async Task<long> SaveLocationUnlockRuleAsync(LocationUnlockRuleEntity entity)
        {
            return entity.LocationUnlockRuleId <= 0
                ? await InsertLocationUnlockRuleAsync(entity)
                : await UpdateLocationUnlockRuleAsync(entity);
        }

        private async Task<long> InsertLocationUnlockRuleAsync(LocationUnlockRuleEntity entity)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqliteCommand(@"
                INSERT INTO LocationUnlockRule (LocationId, RequiredMoney, Requiredproduct_idLocationId, RequiredProductCount, UnlockDescription)
                VALUES (@LocationId, @RequiredMoney, @RequiredProductIdLocationId, @RequiredProductCount, @UnlockDescription);
                SELECT last_insert_rowid();", conn);
            AddLocationUnlockRuleParameters(cmd, entity);
            entity.LocationUnlockRuleId = Convert.ToInt64(await cmd.ExecuteScalarAsync());
            return entity.LocationUnlockRuleId;
        }

        private async Task<long> UpdateLocationUnlockRuleAsync(LocationUnlockRuleEntity entity)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqliteCommand(@"
                UPDATE LocationUnlockRule
                SET LocationId = @LocationId,
                    RequiredMoney = @RequiredMoney,
                    Requiredproduct_idLocationId = @RequiredProductIdLocationId,
                    RequiredProductCount = @RequiredProductCount,
                    UnlockDescription = @UnlockDescription
                WHERE LocationUnlockRuleId = @LocationUnlockRuleId", conn);
            AddLocationUnlockRuleParameters(cmd, entity);
            cmd.Parameters.AddWithValue("@LocationUnlockRuleId", entity.LocationUnlockRuleId);
            await cmd.ExecuteNonQueryAsync();
            return entity.LocationUnlockRuleId;
        }

        public static void EnsureSchema(SqliteConnection conn)
        {
            using var cmd = new SqliteCommand(@"
                CREATE TABLE IF NOT EXISTS FactoryCapability (
                    FactoryCapabilityId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                    LocationId bigint NOT NULL,
                    CapabilityType text NOT NULL,
                    CapabilityName text NULL,
                    IsEnabled bigint DEFAULT (1) NOT NULL
                );

                CREATE TABLE IF NOT EXISTS ProductProduceRequirement (
                    ProductProduceRequirementIdId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                    productId bigint NOT NULL,
                    RequiredCapabilityType text NOT NULL,
                    RequiredCapabilityName text NULL,
                    MinimumFactoryLevel bigint DEFAULT (1) NOT NULL
                );

                CREATE TABLE IF NOT EXISTS LocationUnlockRule (
                    LocationUnlockRuleId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                    LocationId bigint NOT NULL,
                    RequiredMoney real DEFAULT (0) NOT NULL,
                    Requiredproduct_idLocationId bigint NULL,
                    RequiredProductCount bigint DEFAULT (0) NOT NULL,
                    UnlockDescription text NULL
                );", conn);
            cmd.ExecuteNonQuery();
        }

        private static FactoryCapabilityEntity MapFactoryCapability(SqliteDataReader reader)
        {
            return new FactoryCapabilityEntity
            {
                FactoryCapabilityId = reader.GetInt64(reader.GetOrdinal("FactoryCapabilityId")),
                LocationId = reader.GetInt64(reader.GetOrdinal("LocationId")),
                CapabilityType = reader.GetString(reader.GetOrdinal("CapabilityType")),
                CapabilityName = GetNullableString(reader, "CapabilityName"),
                IsEnabled = reader.GetInt64(reader.GetOrdinal("IsEnabled")) != 0
            };
        }

        private static ProductProduceRequirementEntity MapProductProduceRequirement(SqliteDataReader reader)
        {
            return new ProductProduceRequirementEntity
            {
                ProductProduceRequirementIdId = reader.GetInt64(reader.GetOrdinal("ProductProduceRequirementIdId")),
                ProductId = reader.GetInt64(reader.GetOrdinal("productId")),
                RequiredCapabilityType = reader.GetString(reader.GetOrdinal("RequiredCapabilityType")),
                RequiredCapabilityName = GetNullableString(reader, "RequiredCapabilityName"),
                MinimumFactoryLevel = reader.GetInt64(reader.GetOrdinal("MinimumFactoryLevel"))
            };
        }

        private static LocationUnlockRuleEntity MapLocationUnlockRule(SqliteDataReader reader)
        {
            var requiredProductOrdinal = reader.GetOrdinal("Requiredproduct_idLocationId");
            return new LocationUnlockRuleEntity
            {
                LocationUnlockRuleId = reader.GetInt64(reader.GetOrdinal("LocationUnlockRuleId")),
                LocationId = reader.GetInt64(reader.GetOrdinal("LocationId")),
                RequiredMoney = reader.GetDouble(reader.GetOrdinal("RequiredMoney")),
                RequiredProductIdLocationId = reader.IsDBNull(requiredProductOrdinal) ? null : reader.GetInt64(requiredProductOrdinal),
                RequiredProductCount = reader.GetInt64(reader.GetOrdinal("RequiredProductCount")),
                UnlockDescription = GetNullableString(reader, "UnlockDescription")
            };
        }

        private static void AddFactoryCapabilityParameters(SqliteCommand cmd, FactoryCapabilityEntity entity)
        {
            cmd.Parameters.AddWithValue("@LocationId", entity.LocationId);
            cmd.Parameters.AddWithValue("@CapabilityType", entity.CapabilityType.Trim());
            cmd.Parameters.AddWithValue("@CapabilityName", ToDbValue(entity.CapabilityName));
            cmd.Parameters.AddWithValue("@IsEnabled", entity.IsEnabled ? 1 : 0);
        }

        private static void AddProductProduceRequirementParameters(SqliteCommand cmd, ProductProduceRequirementEntity entity)
        {
            cmd.Parameters.AddWithValue("@ProductId", entity.ProductId);
            cmd.Parameters.AddWithValue("@RequiredCapabilityType", entity.RequiredCapabilityType.Trim());
            cmd.Parameters.AddWithValue("@RequiredCapabilityName", ToDbValue(entity.RequiredCapabilityName));
            cmd.Parameters.AddWithValue("@MinimumFactoryLevel", entity.MinimumFactoryLevel);
        }

        private static void AddLocationUnlockRuleParameters(SqliteCommand cmd, LocationUnlockRuleEntity entity)
        {
            cmd.Parameters.AddWithValue("@LocationId", entity.LocationId);
            cmd.Parameters.AddWithValue("@RequiredMoney", entity.RequiredMoney);
            cmd.Parameters.AddWithValue("@RequiredProductIdLocationId", entity.RequiredProductIdLocationId.HasValue ? entity.RequiredProductIdLocationId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@RequiredProductCount", entity.RequiredProductCount);
            cmd.Parameters.AddWithValue("@UnlockDescription", ToDbValue(entity.UnlockDescription));
        }

        private static string? GetNullableString(SqliteDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }

        private static object ToDbValue(string? value)
            => string.IsNullOrWhiteSpace(value) ? DBNull.Value : value.Trim();
    }
}