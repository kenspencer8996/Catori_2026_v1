using CatoriServices.Objects.database;

namespace CatoriServices.Tests;

[Collection(GlobalTestStateCollection.Name)]
public sealed class LegacyRepositoryReadTests
{
    private readonly GlobalTestState _globalState;

    public LegacyRepositoryReadTests(GlobalTestState globalState)
    {
        _globalState = globalState;
    }

    [Fact]
    public async Task SettingsRepository_get_and_upsert_use_settings_table()
    {
        using var db = new SqliteTestDatabase();
        await db.ExecuteScriptAsync("""
            CREATE TABLE Settings (
                SettingID INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                StringSetting TEXT NOT NULL,
                IntSetting INTEGER NOT NULL
            );
            INSERT INTO Settings (Name, StringSetting, IntSetting) VALUES ('Volume', 'Medium', 5);
            """);
        _globalState.UseDatabase(db.DatabasePath);

        var repository = new SettingsRepository();

        var setting = repository.GetSettingbyName("Volume");
        Assert.Equal("Medium", setting.StringSetting);

        repository.UpsertSetting(new SettingEntity("Volume", "High", 10) { SettingID = setting.SettingID });
        var updated = repository.GetSettingbyName("Volume");
        Assert.Equal("High", updated.StringSetting);
        Assert.Equal(10, updated.IntSetting);
    }

    [Fact]
    public async Task BankRepository_reads_seeded_bank()
    {
        using var db = new SqliteTestDatabase();
        await db.ExecuteScriptAsync("""
            CREATE TABLE Bank (
                bankid INTEGER PRIMARY KEY,
                BusinesskeyImageNameWOExtension TEXT NOT NULL,
                ImageFileName TEXT NOT NULL,
                Description TEXT NOT NULL,
                interestrate NUMERIC NOT NULL
            );
            INSERT INTO Bank (bankid, BusinesskeyImageNameWOExtension, ImageFileName, Description, interestrate)
            VALUES (1, 'bank-key', 'bank.png', 'Main bank', 3.5);
            """);
        _globalState.UseDatabase(db.DatabasePath);

        var bank = await new BankRepository().GetBankByIdAsync(1);

        Assert.Equal(1, bank.BankId);
        Assert.Equal("bank-key", bank.BusinesskeyImageNameWOExtension);
        Assert.Equal("bank.png", bank.ImageFileName);
        Assert.Equal("Main bank", bank.Description);
        Assert.Equal(3.5m, bank.Interestrate);
    }

    [Fact]
    public async Task BankCustomerFundsRepository_reads_seeded_funds()
    {
        using var db = new SqliteTestDatabase();
        await db.ExecuteScriptAsync("""
            CREATE TABLE BankCustomerFunds (
                BankCustomerFundsId INTEGER PRIMARY KEY,
                Amount REAL NOT NULL,
                LastUpdated TEXT NOT NULL
            );
            INSERT INTO BankCustomerFunds (BankCustomerFundsId, Amount, LastUpdated)
            VALUES (7, 123.45, '2026-05-07');
            """);
        _globalState.UseDatabase(db.DatabasePath);

        var funds = await new BankCustomerFundsRepository().GetBankCustomerFundsByIdAsync(7);

        Assert.Equal(7, funds.BankCustomerFundsId);
        Assert.Equal(123.45, funds.Amount);
        Assert.Equal("2026-05-07", funds.LastUpdated);
    }
}

