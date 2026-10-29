using System.Windows;
using System.Windows.Controls;
using CatoriUCLibrary.Objects.Economy;

namespace CatoriUCLibrary.Views;

public static class AvatarMoney
{
    public static readonly DependencyProperty WorkAmountProperty = DependencyProperty.RegisterAttached(
        "WorkAmount", typeof(decimal), typeof(AvatarMoney), new PropertyMetadata(0m));
    public static decimal GetWorkAmount(DependencyObject target)
    {
        return (decimal)target.GetValue(WorkAmountProperty);
    }
    public static void SetWorkAmount(DependencyObject target, decimal value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }
        target.SetValue(WorkAmountProperty, value);
    }
    private static void WorkCompleted(object? sender, CatoriUCLibrary.Views.Person.PersonActivityCompletedEventArgs e)
    {
        if (sender is PersonUC avatar && e.Activity == CatoriUCLibrary.Views.Person.PersonActivity.Work && GetWorkAmount(avatar) > 0)
        {
            CreditWork(avatar, GetWorkAmount(avatar), "work:" + Guid.NewGuid().ToString("N"));
        }
    }
    private static readonly List<WeakReference<DependencyObject>> targets = new();
    public static readonly DependencyProperty AccountIdProperty = DependencyProperty.RegisterAttached(
        "AccountId", typeof(string), typeof(AvatarMoney), new PropertyMetadata("", AccountChanged));
    private static readonly DependencyPropertyKey BalancePropertyKey = DependencyProperty.RegisterAttachedReadOnly(
        "Balance", typeof(decimal), typeof(AvatarMoney), new PropertyMetadata(0m));
    public static readonly DependencyProperty BalanceProperty = BalancePropertyKey.DependencyProperty;

    static AvatarMoney()
    {
        GameEconomy.Current.BalanceChanged += UpdateBalances;
    }

    public static string GetAccountId(DependencyObject target)
    {
        return (string)target.GetValue(AccountIdProperty);
    }
    public static void SetAccountId(DependencyObject target, string value)
    {
        target.SetValue(AccountIdProperty, value);
    }
    public static decimal GetBalance(DependencyObject target)
    {
        return (decimal)target.GetValue(BalanceProperty);
    }
    private static void AccountChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
        string id = (string)e.NewValue;
        if (string.IsNullOrWhiteSpace(id))
        {
            return;
        }
        target.SetValue(BalancePropertyKey, GameEconomy.Current.GetBalance(id));
        if (target is PersonUC person)
        {
            person.ActivityCompleted -= WorkCompleted;
            person.ActivityCompleted += WorkCompleted;
        }
        targets.RemoveAll(reference => !reference.TryGetTarget(out var entry) || ReferenceEquals(entry, target));
        targets.Add(new WeakReference<DependencyObject>(target));
    }
    private static void UpdateBalances(string id, decimal balance)
    {
        foreach (var reference in targets.ToArray())
        {
            if (reference.TryGetTarget(out var target))
            {
                target.Dispatcher.Invoke(() =>
                {
                    if (GetAccountId(target) == id)
                    {
                        target.SetValue(BalancePropertyKey, balance);
                    }
                });
            }
        }
    }
    public static void RefreshBalance(DependencyObject avatar)
    {
        string id = GetAccountId(avatar);
        if (!string.IsNullOrWhiteSpace(id))
        {
            avatar.SetValue(BalancePropertyKey, GameEconomy.Current.GetBalance(id));
        }
    }
    public static string EnsureAccount(PersonUC avatar)
    {
        string id = GetAccountId(avatar);
        if (string.IsNullOrWhiteSpace(id))
        {
            // Generated controls use authored IDs. This fallback is for hand-authored avatars.
            if (string.IsNullOrWhiteSpace(avatar.Name))
            {
                throw new InvalidOperationException("Avatar needs a persistent money account ID.");
            }
            id = "Avatar:" + avatar.Name;
            SetAccountId(avatar, id);
        }
        return id;
    }
    public static bool TryPurchase(PersonUC avatar, decimal amount, string transactionId, string description)
    {
        bool result = GameEconomy.Current.TryPurchase(EnsureAccount(avatar), amount, transactionId, description);
        if (!result)
        {
            MessageBox.Show("Insufficient funds.", "Purchase", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        return result;
    }
    public static void CreditWork(PersonUC avatar, decimal amount, string completionId)
    {
        GameEconomy.Current.CreditWork(EnsureAccount(avatar), amount, completionId);
    }
}
