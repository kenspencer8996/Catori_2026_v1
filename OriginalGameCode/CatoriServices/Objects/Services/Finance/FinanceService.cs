using Microsoft.Data.Sqlite;
using CatoriServices.Objects.Core;
using CatoriServices.Objects.database.Finance;

namespace CatoriServices.Objects.Services.Finance;

public enum LocationRateType { Hourly,Piece }
public sealed record LocationEarningRate(long LocationId,long RateCents,LocationRateType RateType);
public sealed record MoneyOperationResult(long WalletBalanceCents,long? BankBalanceCents=null,long? RecordId=null);

public sealed class FinanceService
{
    private readonly string _databasePath;
    public FinanceService(string? databasePath=null)
    {
        _databasePath=databasePath??GlobalServices.Database;
        RuntimeFinanceSchemaMigrator.Migrate(_databasePath);
    }

    public LocationEarningRate? GetLocationRate(long locationId)
    {
        using var connection=Open();using var command=connection.CreateCommand();
        command.CommandText=@"SELECT RateCents,RateType FROM LocationEarningRate
WHERE LocationId=@locationId AND IsActive=1 AND EffectiveFrom<=datetime('now')
ORDER BY EffectiveFrom DESC,LocationEarningRateId DESC LIMIT 1;";
        command.Parameters.AddWithValue("@locationId",locationId);
        using var reader=command.ExecuteReader();
        return reader.Read()?new(locationId,reader.GetInt64(0),Enum.Parse<LocationRateType>(reader.GetString(1),true)):null;
    }

    public void SetLocationRate(long locationId,decimal rate,LocationRateType rateType)
    {
        long cents=ToCents(rate);if(cents<0)throw new ArgumentOutOfRangeException(nameof(rate));
        using var connection=Open();using var transaction=connection.BeginTransaction();
        Execute(connection,transaction,"UPDATE LocationEarningRate SET IsActive=0 WHERE LocationId=@locationId AND IsActive=1;",("@locationId",locationId));
        Execute(connection,transaction,@"INSERT INTO LocationEarningRate(LocationId,RateCents,RateType)
VALUES(@locationId,@cents,@type);",("@locationId",locationId),("@cents",cents),("@type",rateType.ToString()));
        transaction.Commit();
    }

    public MoneyOperationResult CreditFactoryDrop(int personId,long locationId,string partName)
    {
        var rate=GetLocationRate(locationId)??throw new InvalidOperationException($"Location {locationId} has no active earning rate.");
        long cents=rate.RateType==LocationRateType.Piece?rate.RateCents:
            Math.Max(1,(long)Math.Round(rate.RateCents/60m,MidpointRounding.AwayFromZero));
        return ChangeWallet(personId,cents,"FactoryEarning",locationId,"FactoryDrop",partName,
            $"Produced {partName} at location {locationId} ({rate.RateType}).");
    }

    public MoneyOperationResult CreditFoundMoney(int personId, decimal amount, long? locationId, string description)
    {
        return ChangeWallet(personId, ToPositiveCents(amount), "Found", locationId, "FoundMoney", null, description);
    }

    public MoneyOperationResult PurchaseTravelTicket(int personId,decimal amount,string travelMode,string destinationKey,string destinationName)
    {
        if(string.IsNullOrWhiteSpace(travelMode))throw new ArgumentException("Travel mode is required.",nameof(travelMode));
        if(string.IsNullOrWhiteSpace(destinationKey))throw new ArgumentException("Destination is required.",nameof(destinationKey));
        long cents=ToPositiveCents(amount);using var connection=Open();using var transaction=connection.BeginTransaction();
        long wallet=ReadWalletCents(connection,transaction,personId);
        if(wallet<cents)throw new InvalidOperationException($"You need {FromCents(cents):C} for the {destinationName} ticket.");
        long balance=wallet-cents;SetWalletCents(connection,transaction,personId,balance);
        long transactionId=InsertWalletTransaction(connection,transaction,personId,-cents,"TravelTicket",null,
            travelMode,destinationKey,$"{travelMode} ticket to {destinationName}.");
        transaction.Commit();return new(balance,null,transactionId);
    }

    public MoneyOperationResult DepositToBank(int personId,int bankId,decimal amount,string? businessName=null)
    {
        long cents=ToPositiveCents(amount);using var connection=Open();using var transaction=connection.BeginTransaction();
        long wallet=ReadWalletCents(connection,transaction,personId);
        if(wallet<cents)throw new InvalidOperationException("The wallet does not contain enough money for this deposit.");
        SetWalletCents(connection,transaction,personId,wallet-cents);
        long walletTransactionId=InsertWalletTransaction(connection,transaction,personId,-cents,"BankDeposit",null,"Bank",bankId.ToString(),$"Deposit to bank {bankId}.");
        Execute(connection,transaction,@"INSERT INTO Deposit(PersonId,Amount,businessname,BankId)
SELECT @personId,0,@name,@bankId WHERE NOT EXISTS(
SELECT 1 FROM Deposit WHERE PersonId=@personId AND BankId=@bankId);",
            ("@personId",personId),("@name",businessName),("@bankId",bankId));
        using var find=connection.CreateCommand();find.Transaction=transaction;
        find.CommandText="SELECT DepositId,Amount FROM Deposit WHERE PersonId=@personId AND BankId=@bankId ORDER BY DepositId LIMIT 1;";
        find.Parameters.AddWithValue("@personId",personId);find.Parameters.AddWithValue("@bankId",bankId);
        using var reader=find.ExecuteReader();if(!reader.Read())throw new InvalidOperationException("Bank account could not be created.");
        long depositId=reader.GetInt64(0);long bankCents=ToCents(reader.GetDecimal(1));reader.Close();
        Execute(connection,transaction,"UPDATE Deposit SET Amount=@amount,businessname=COALESCE(@name,businessname) WHERE DepositId=@id;",
            ("@amount",FromCents(bankCents+cents)),("@name",businessName),("@id",depositId));
        Execute(connection,transaction,@"INSERT INTO BankTransaction(DepositId,AmountCents,TransactionType,WalletTransactionId,ReferenceType,ReferenceId,Description)
VALUES(@depositId,@cents,'Deposit',@walletTransactionId,'WalletTransaction',@referenceId,'Wallet deposit');",
            ("@depositId",depositId),("@cents",cents),("@walletTransactionId",walletTransactionId),("@referenceId",walletTransactionId.ToString()));
        transaction.Commit();return new(wallet-cents,bankCents+cents,depositId);
    }

    public long FileBankruptcy(int personId,string? reason=null)
    {
        using var connection=Open();using var transaction=connection.BeginTransaction();
        long assets=ReadWalletCents(connection,transaction,personId)+ReadBankTotalCents(connection,transaction,personId);
        long liabilities=ReadLoanTotalCents(connection,transaction,personId);
        Execute(connection,transaction,@"INSERT INTO BankruptcyCase(PersonId,Status,AssetCents,LiabilityCents,Reason)
VALUES(@personId,'Filed',@assets,@liabilities,@reason);",("@personId",personId),("@assets",assets),("@liabilities",liabilities),("@reason",reason));
        long id=LastInsertId(connection,transaction);transaction.Commit();return id;
    }

    public MoneyOperationResult IssueBankruptcyLoan(int personId,int bankId,decimal principal,decimal interestRate)
    {
        long cents=ToPositiveCents(principal);if(interestRate<0)throw new ArgumentOutOfRangeException(nameof(interestRate));
        using var connection=Open();using var transaction=connection.BeginTransaction();
        if(ScalarLong(connection,transaction,"SELECT COUNT(*) FROM BankruptcyCase WHERE PersonId=@personId AND Status='Filed';",("@personId",personId))==0)
            throw new InvalidOperationException("A bankruptcy recovery loan requires an active bankruptcy case.");
        Execute(connection,transaction,@"INSERT INTO Loan(PersonId,BankId,OriginalPrincipalCents,OutstandingPrincipalCents,InterestRate,Status,IsBankruptcyLoan)
VALUES(@personId,@bankId,@cents,@cents,@rate,'Active',1);",("@personId",personId),("@bankId",bankId),("@cents",cents),("@rate",interestRate));
        long loanId=LastInsertId(connection,transaction);long wallet=ReadWalletCents(connection,transaction,personId)+cents;
        SetWalletCents(connection,transaction,personId,wallet);
        InsertWalletTransaction(connection,transaction,personId,cents,"LoanProceeds",null,"Loan",loanId.ToString(),"Bankruptcy recovery loan proceeds.");
        transaction.Commit();return new(wallet,null,loanId);
    }

    private MoneyOperationResult ChangeWallet(int personId,long cents,string type,long? locationId,string? referenceType,string? referenceId,string? description)
    {
        using var connection=Open();using var transaction=connection.BeginTransaction();
        long balance=ReadWalletCents(connection,transaction,personId)+cents;SetWalletCents(connection,transaction,personId,balance);
        long id=InsertWalletTransaction(connection,transaction,personId,cents,type,locationId,referenceType,referenceId,description);
        transaction.Commit();return new(balance,null,id);
    }

    private SqliteConnection Open(){var c=new SqliteConnection(new SqliteConnectionStringBuilder{DataSource=_databasePath,ForeignKeys=true}.ToString());c.Open();return c;}
    private static long ReadWalletCents(SqliteConnection c, SqliteTransaction t, int id)
    {
        return ScalarLong(c, t, "SELECT CAST(ROUND(COALESCE(Funds,0)*100) AS INTEGER) FROM Person WHERE PersonID=@id;", ("@id", id));
    }

    private static long ReadBankTotalCents(SqliteConnection c, SqliteTransaction t, int id)
    {
        return ScalarLong(c, t, "SELECT CAST(ROUND(COALESCE(SUM(Amount),0)*100) AS INTEGER) FROM Deposit WHERE PersonId=@id;", ("@id", id));
    }

    private static long ReadLoanTotalCents(SqliteConnection c, SqliteTransaction t, int id)
    {
        return ScalarLong(c, t, "SELECT COALESCE(SUM(OutstandingPrincipalCents),0) FROM Loan WHERE PersonId=@id AND Status='Active';", ("@id", id));
    }

    private static void SetWalletCents(SqliteConnection c,SqliteTransaction t,int id,long cents)
    { if(Execute(c,t,"UPDATE Person SET Funds=@funds WHERE PersonID=@id;",("@funds",FromCents(cents)),("@id",id))!=1)throw new InvalidOperationException($"Person {id} was not found."); }
    private static long InsertWalletTransaction(SqliteConnection c,SqliteTransaction t,int personId,long cents,string type,long? locationId,string? referenceType,string? referenceId,string? description)
    { Execute(c,t,@"INSERT INTO WalletTransaction(PersonId,AmountCents,TransactionType,LocationId,ReferenceType,ReferenceId,Description)
VALUES(@personId,@cents,@type,@locationId,@referenceType,@referenceId,@description);",("@personId",personId),("@cents",cents),("@type",type),("@locationId",locationId),("@referenceType",referenceType),("@referenceId",referenceId),("@description",description));return LastInsertId(c,t); }
    private static long LastInsertId(SqliteConnection c, SqliteTransaction t)
    {
        return ScalarLong(c, t, "SELECT last_insert_rowid();");
    }

    private static int Execute(SqliteConnection c,SqliteTransaction t,string sql,params (string Name,object? Value)[] values)
    { using var command=c.CreateCommand();command.Transaction=t;command.CommandText=sql;foreach(var v in values)command.Parameters.AddWithValue(v.Name,v.Value??DBNull.Value);return command.ExecuteNonQuery(); }
    private static long ScalarLong(SqliteConnection c,SqliteTransaction t,string sql,params (string Name,object? Value)[] values)
    { using var command=c.CreateCommand();command.Transaction=t;command.CommandText=sql;foreach(var v in values)command.Parameters.AddWithValue(v.Name,v.Value??DBNull.Value);object? result=command.ExecuteScalar();if(result==null||result==DBNull.Value)throw new InvalidOperationException("Required financial record was not found.");return Convert.ToInt64(result); }
    public static long ToCents(decimal amount)
    {
        return checked((long)Math.Round(amount * 100m, MidpointRounding.AwayFromZero));
    }

    public static decimal FromCents(long cents)
    {
        return cents / 100m;
    }

    private static long ToPositiveCents(decimal amount){long cents=ToCents(amount);if(cents<=0)throw new ArgumentOutOfRangeException(nameof(amount),"Amount must be positive.");return cents;}
}
