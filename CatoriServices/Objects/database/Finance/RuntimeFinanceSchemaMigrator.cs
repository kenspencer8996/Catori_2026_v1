using Microsoft.Data.Sqlite;

namespace CatoriServices.Objects.database.Finance;

public static class RuntimeFinanceSchemaMigrator
{
    public static void Migrate(string databasePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);
        using var connection=new SqliteConnection(new SqliteConnectionStringBuilder
        { DataSource=databasePath,ForeignKeys=true }.ToString());
        connection.Open();
        using var transaction=connection.BeginTransaction();
        foreach(string sql in Statements)Execute(connection,transaction,sql);
        transaction.Commit();
    }

    private static void Execute(SqliteConnection connection,SqliteTransaction transaction,string sql)
    {
        using var command=connection.CreateCommand();
        command.Transaction=transaction;command.CommandText=sql;command.ExecuteNonQuery();
    }

    private static readonly string[] Statements=
    [
        "CREATE TABLE IF NOT EXISTS SchemaVersion(Name TEXT PRIMARY KEY,Version INTEGER NOT NULL,AppliedAt TEXT NOT NULL DEFAULT(datetime('now')));",
        @"CREATE TABLE IF NOT EXISTS LocationEarningRate(
LocationEarningRateId INTEGER PRIMARY KEY AUTOINCREMENT,
LocationId INTEGER NOT NULL,
RateCents INTEGER NOT NULL CHECK(RateCents>=0),
RateType TEXT NOT NULL CHECK(RateType IN ('Hourly','Piece')),
EffectiveFrom TEXT NOT NULL DEFAULT(datetime('now')),
IsActive INTEGER NOT NULL DEFAULT 1,
FOREIGN KEY(LocationId) REFERENCES Location(LocationId) ON DELETE CASCADE);",
        "CREATE INDEX IF NOT EXISTS IX_LocationEarningRate_Current ON LocationEarningRate(LocationId,IsActive,EffectiveFrom DESC);",
        @"CREATE TABLE IF NOT EXISTS WalletTransaction(
WalletTransactionId INTEGER PRIMARY KEY AUTOINCREMENT,
PersonId INTEGER NOT NULL,AmountCents INTEGER NOT NULL,
TransactionType TEXT NOT NULL,LocationId INTEGER NULL,
ReferenceType TEXT NULL,ReferenceId TEXT NULL,Description TEXT NULL,
CreatedAt TEXT NOT NULL DEFAULT(datetime('now')),
FOREIGN KEY(PersonId) REFERENCES Person(PersonID),
FOREIGN KEY(LocationId) REFERENCES Location(LocationId));",
        @"CREATE TABLE IF NOT EXISTS BankTransaction(
BankTransactionId INTEGER PRIMARY KEY AUTOINCREMENT,
DepositId INTEGER NOT NULL,AmountCents INTEGER NOT NULL,
TransactionType TEXT NOT NULL,WalletTransactionId INTEGER NULL,
ReferenceType TEXT NULL,ReferenceId TEXT NULL,Description TEXT NULL,
CreatedAt TEXT NOT NULL DEFAULT(datetime('now')),
FOREIGN KEY(DepositId) REFERENCES Deposit(DepositId),
FOREIGN KEY(WalletTransactionId) REFERENCES WalletTransaction(WalletTransactionId));",
        @"CREATE TABLE IF NOT EXISTS BankruptcyCase(
BankruptcyCaseId INTEGER PRIMARY KEY AUTOINCREMENT,PersonId INTEGER NOT NULL,
Status TEXT NOT NULL CHECK(Status IN ('Filed','Discharged','Dismissed')),
FiledAt TEXT NOT NULL DEFAULT(datetime('now')),DischargedAt TEXT NULL,
AssetCents INTEGER NOT NULL DEFAULT 0,LiabilityCents INTEGER NOT NULL DEFAULT 0,
Reason TEXT NULL,FOREIGN KEY(PersonId) REFERENCES Person(PersonID));",
        "CREATE UNIQUE INDEX IF NOT EXISTS UX_BankruptcyCase_Active ON BankruptcyCase(PersonId) WHERE Status='Filed';",
        @"CREATE TABLE IF NOT EXISTS Loan(
LoanId INTEGER PRIMARY KEY AUTOINCREMENT,PersonId INTEGER NOT NULL,BankId INTEGER NOT NULL,
OriginalPrincipalCents INTEGER NOT NULL CHECK(OriginalPrincipalCents>0),
OutstandingPrincipalCents INTEGER NOT NULL CHECK(OutstandingPrincipalCents>=0),
InterestRate REAL NOT NULL CHECK(InterestRate>=0),Status TEXT NOT NULL,
IsBankruptcyLoan INTEGER NOT NULL DEFAULT 0,IssuedAt TEXT NOT NULL DEFAULT(datetime('now')),
DueAt TEXT NULL,FOREIGN KEY(PersonId) REFERENCES Person(PersonID),FOREIGN KEY(BankId) REFERENCES Bank(bankid));",
        @"INSERT INTO LocationEarningRate(LocationId,RateCents,RateType)
SELECT LocationId,100,'Piece' FROM Location l
WHERE lower(COALESCE(l.LocationType,''))='factory'
AND NOT EXISTS(SELECT 1 FROM LocationEarningRate r WHERE r.LocationId=l.LocationId AND r.IsActive=1);",
        "INSERT INTO SchemaVersion(Name,Version,AppliedAt) VALUES('FinanceRuntime',1,datetime('now')) ON CONFLICT(Name) DO UPDATE SET Version=excluded.Version,AppliedAt=excluded.AppliedAt;"
    ];
}
