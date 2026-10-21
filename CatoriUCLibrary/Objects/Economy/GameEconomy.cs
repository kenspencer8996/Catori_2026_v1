using System.IO;
using System.Text.Json;

namespace CatoriUCLibrary.Objects.Economy;

public sealed class GameEconomy
{
    private readonly object sync = new();
    private readonly string savePath;
    private readonly MasterGameConfiguration config;
    private static readonly Lazy<GameEconomy> current = new(CreateDefault);
    public static GameEconomy Current => current.Value;
    public event Action<string, decimal>? BalanceChanged;
    public string SavePath => savePath;

    public GameEconomy(MasterGameConfiguration configuration, string path)
    {
        config = configuration;
        savePath = Path.GetFullPath(path);
    }

    private static GameEconomy CreateDefault()
    {
        var configuration = MasterGameConfiguration.Load(Path.Combine(AppContext.BaseDirectory, "master.game.json"));
        string key = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(configuration.GameId)))[..16];
        return new GameEconomy(configuration, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Catori", key, "economy.json"));
    }

    private EconomySave Read()
    {
        if (!File.Exists(savePath))
        {
            return new EconomySave();
        }
        var save = JsonSerializer.Deserialize<EconomySave>(File.ReadAllText(savePath))
            ?? throw new InvalidDataException("The economy save is empty.");
        if (save.Version != 1 || save.Balances == null || save.Transactions == null || save.Balances.Any(pair => pair.Value < 0))
        {
            throw new InvalidDataException("The economy save is invalid. Restore its .bak file before continuing.");
        }
        return save;
    }

    private void Write(EconomySave save)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);
        string temporary = savePath + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                JsonSerializer.Serialize(stream, save, new JsonSerializerOptions { WriteIndented = true });
                stream.Flush(true);
            }
            if (File.Exists(savePath))
            {
                File.Replace(temporary, savePath, savePath + ".bak");
            }
            else
            {
                File.Move(temporary, savePath);
            }
        }
        finally
        {
            if (File.Exists(temporary))
            {
                File.Delete(temporary);
            }
        }
    }

    private T Locked<T>(Func<T> action)
    {
        string mutexName = "Local\\CatoriEconomy" + Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(savePath.ToUpperInvariant())))[..24];
        using var mutex = new Mutex(false, mutexName);
        bool owned = false;
        try
        {
            try
            {
                owned = mutex.WaitOne(TimeSpan.FromSeconds(10));
            }
            catch (AbandonedMutexException)
            {
                owned = true;
            }
            if (!owned)
            {
                throw new IOException("The balance save is busy. Try again.");
            }
            lock (sync)
            {
                return action();
            }
        }
        finally
        {
            if (owned)
            {
                mutex.ReleaseMutex();
            }
        }
    }

    public decimal GetBalance(string avatarId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(avatarId);
        return Locked(() =>
        {
            var save = Read();
            if (!save.Balances.TryGetValue(avatarId, out decimal balance))
            {
                balance = config.DefaultBalance;
                save.Balances.Add(avatarId, balance);
                Write(save);
            }
            return balance;
        });
    }

    public bool TryPurchase(string avatarId, decimal amount, string transactionId, string description = "Purchase")
    {
        return Change(avatarId, amount, transactionId, description, false);
    }

    public bool CreditWork(string avatarId, decimal amount, string transactionId, string description = "Work completed")
    {
        return Change(avatarId, amount, transactionId, description, true);
    }

    private bool Change(string avatarId, decimal amount, string transactionId, string description, bool credit)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(avatarId);
        ArgumentException.ThrowIfNullOrWhiteSpace(transactionId);
        if (amount < 0 || amount != decimal.Round(amount, 2))
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amounts must be non-negative with at most two decimal places.");
        }
        decimal result = 0;
        bool success = Locked(() =>
        {
            var save = Read();
            decimal change = credit ? amount : -amount;
            if (save.Transactions.TryGetValue(transactionId, out var previous))
            {
                if (previous.AvatarId != avatarId || previous.Amount != change)
                {
                    throw new InvalidOperationException("Transaction ID was already used for a different payment.");
                }
                result = save.Balances[avatarId];
                return true;
            }
            decimal balance = save.Balances.GetValueOrDefault(avatarId, config.DefaultBalance);
            if (!credit && balance < amount)
            {
                return false;
            }
            result = checked(balance + change);
            save.Balances[avatarId] = result;
            save.Transactions[transactionId] = new MoneyTransaction
            {
                AvatarId = avatarId,
                Amount = change,
                Description = description,
                TimestampUtc = DateTime.UtcNow
            };
            Write(save);
            return true;
        });
        if (success)
        {
            BalanceChanged?.Invoke(avatarId, result);
        }
        return success;
    }
}
