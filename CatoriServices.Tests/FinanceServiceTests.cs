using CatoriServices.Objects.Services.Finance;
using Microsoft.Data.Sqlite;

namespace CatoriServices.Tests;

public sealed class FinanceServiceTests
{
    [Fact]
    public async Task Factory_drop_credits_wallet_and_ledger()
    {
        using var db=await CreateDatabase();var service=new FinanceService(db.DatabasePath);
        service.SetLocationRate(10,2.50m,LocationRateType.Piece);
        var result=service.CreditFactoryDrop(1,10,"Handle");
        Assert.Equal(1250,result.WalletBalanceCents);
        Assert.Equal(1,await Scalar(db.DatabasePath,"SELECT COUNT(*) FROM WalletTransaction WHERE TransactionType='FactoryEarning' AND AmountCents=250;"));
    }

    [Fact]
    public async Task Deposit_moves_money_atomically_and_writes_both_ledgers()
    {
        using var db=await CreateDatabase();var service=new FinanceService(db.DatabasePath);
        var result=service.DepositToBank(1,1,4.25m,"Test Bank");
        Assert.Equal(575,result.WalletBalanceCents);Assert.Equal(425,result.BankBalanceCents);
        Assert.Equal(1,await Scalar(db.DatabasePath,"SELECT COUNT(*) FROM WalletTransaction WHERE TransactionType='BankDeposit' AND AmountCents=-425;"));
        Assert.Equal(1,await Scalar(db.DatabasePath,"SELECT COUNT(*) FROM BankTransaction WHERE TransactionType='Deposit' AND AmountCents=425;"));
    }

    [Fact]
    public async Task Bankruptcy_enables_recovery_loan_and_credits_wallet()
    {
        using var db=await CreateDatabase();var service=new FinanceService(db.DatabasePath);
        long bankruptcyId=service.FileBankruptcy(1,"Unable to pay");
        var result=service.IssueBankruptcyLoan(1,1,20m,.05m);
        Assert.True(bankruptcyId>0);Assert.Equal(3000,result.WalletBalanceCents);
        Assert.Equal(1,await Scalar(db.DatabasePath,"SELECT COUNT(*) FROM Loan WHERE IsBankruptcyLoan=1 AND Status='Active';"));
    }

    private static async Task<SqliteTestDatabase> CreateDatabase()
    {
        var db=new SqliteTestDatabase();
        await db.ExecuteScriptAsync(@"
CREATE TABLE Person(PersonID INTEGER PRIMARY KEY,Funds REAL NULL);
CREATE TABLE Location(LocationId INTEGER PRIMARY KEY,LocationType TEXT NULL);
CREATE TABLE Bank(bankid INTEGER PRIMARY KEY);
CREATE TABLE Deposit(DepositId INTEGER PRIMARY KEY AUTOINCREMENT,PersonId INTEGER NOT NULL,Amount REAL NOT NULL,businessname TEXT NULL,BankId INTEGER NULL);
INSERT INTO Person VALUES(1,10.00);INSERT INTO Location VALUES(10,'Factory');INSERT INTO Bank VALUES(1);");
        return db;
    }

    private static async Task<long> Scalar(string path,string sql)
    {
        await using var connection=new SqliteConnection("Data Source="+path);await connection.OpenAsync();
        await using var command=new SqliteCommand(sql,connection);return Convert.ToInt64(await command.ExecuteScalarAsync());
    }
}
