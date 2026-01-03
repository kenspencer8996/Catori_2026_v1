using ExecuteSQLStatementsFromFIle;
using Microsoft.Data.Sqlite;
using System.Diagnostics;
using System.Reflection.Metadata;

if (args.Length < 2)
{
    Console.WriteLine("Usage: SqliteBatchRunner <dbPath> <inputFile> [--no-tx] [--stop-on-error] [--log <logFile>]");
    return 2;
}

string dbPath = args[0];
string inputFile = args[1];

bool useTransaction = true;
bool stopOnError = false;
string? logFile = Path.Combine( Path.GetDirectoryName(args[1]), "SqliteBatchRunner.log");

for (int i = 2; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--no-tx":
            useTransaction = false;
            break;
        case "--stop-on-error":
            stopOnError = true;
            break;
        case "--log":
            if (i + 1 >= args.Length)
            {
                Console.WriteLine("Missing value after --log");
                return 2;
            }
            logFile = args[++i];
            break;
    }
}

if (!File.Exists(inputFile))
{
    Console.WriteLine($"Input file not found: {inputFile}");
    return 2;
}

Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(logFile ?? "x.log")) ?? ".");

var log = new LogWriter(logFile);
log.Info($"Started at {DateTimeOffset.Now:O}");
log.Info($"DB: {Path.GetFullPath(dbPath)}");
log.Info($"Input: {Path.GetFullPath(inputFile)}");
log.Info($"Options: transaction={(useTransaction ? "ON" : "OFF")}, stopOnError={(stopOnError ? "ON" : "OFF")}");

int okCount = 0;
int failCount = 0;
int stmtCount = 0;

try
{
    var csb = new SqliteConnectionStringBuilder
    {
        DataSource = dbPath,
        Mode = SqliteOpenMode.ReadWriteCreate
    };

    using var conn = new SqliteConnection(csb.ConnectionString);
    conn.Open();

    using var tx = useTransaction ? conn.BeginTransaction() : null;

    int lineNo = 0;
    foreach (var rawLine in File.ReadLines(inputFile))
    {
        lineNo++;
        var line = rawLine.Trim();

        if (string.IsNullOrWhiteSpace(line))
            continue;

        // Ignore full-line comments
        if (line.StartsWith("--") || line.StartsWith("#"))
            continue;

        // Split this single non-blank line into statements
        var statements = SplitSqlStatementsParent.SplitSqlStatements(line);

        int batchIndex = 0;
        foreach (var sql in statements)
        {
            batchIndex++;
            var trimmed = sql.Trim();
            if (trimmed.Length == 0)
                continue;

            stmtCount++;
            var sw = Stopwatch.StartNew();

            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = trimmed;
                if (tx != null) cmd.Transaction = tx;

                int affected = cmd.ExecuteNonQuery(); // Works for DDL/DML. SELECT returns -1.
                sw.Stop();

                okCount++;
                log.Ok($"OK   line={lineNo} stmt={batchIndex} ms={sw.ElapsedMilliseconds} affected={affected} :: {General.OneLine(trimmed)}");
            }
            catch (Exception ex)
            {
                sw.Stop();
                failCount++;
                log.Fail($"FAIL line={lineNo} stmt={batchIndex} ms={sw.ElapsedMilliseconds} :: {General.OneLine(trimmed)}");
                log.Fail($"     {ex.GetType().Name}: {ex.Message}");

                if (stopOnError)
                {
                    if (tx != null)
                    {
                        try { tx.Rollback(); } catch { /* ignore */ }
                    }
                    log.Info("Stopping due to --stop-on-error.");
                    return 1;
                }
            }
        }
    }

    if (tx != null)
    {
        if (failCount == 0)
        {
            tx.Commit();
            log.Info("Transaction committed.");
        }
        else
        {
            // You can change this behavior if you want partial commits.
            tx.Rollback();
            log.Info("Transaction rolled back (because there were failures). Use --no-tx to allow partial success.");
        }
    }

    log.Info($"Finished. Statements={stmtCount}, OK={okCount}, FAIL={failCount}");
    return failCount == 0 ? 0 : 1;
}
catch (Exception ex)
{
    log.Fail($"FATAL: {ex.GetType().Name}: {ex.Message}");
    return 1;
}
    