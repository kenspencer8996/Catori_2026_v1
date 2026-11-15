using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CatoriUCLibrary.Views;

namespace CatoriUCLibrary.Objects.Economy;

public static class EconomyKeyboardShortcuts
{
    private static bool registered;

    public static void Register()
    {
        if (registered)
        {
            return;
        }
        registered = true;
        EventManager.RegisterClassHandler(typeof(Window), Keyboard.PreviewKeyDownEvent,
            new KeyEventHandler(KeyDown), true);
    }

    public static bool HandleGesture(GameEconomy economy, Key key, ModifierKeys modifiers, bool isRepeat)
    {
        if (key != Key.T || modifiers != ModifierKeys.Control || isRepeat)
        {
            return false;
        }
        economy.ResetBalances();
        return true;
    }

    private static void KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Handled || e.Key != Key.T || Keyboard.Modifiers != ModifierKeys.Control || e.IsRepeat)
        {
            return;
        }
        e.Handled = true;
        try
        {
            if (Application.Current is { } application)
            {
                foreach (Window window in application.Windows)
                {
                    EnsureAvatarAccounts(window);
                }
            }
            HandleGesture(GameEconomy.Current, e.Key, Keyboard.Modifiers, e.IsRepeat);
        }
        catch (Exception exception)
        {
            CatoriShared.Diagnostics.GameSafetyLog.Error("Economy", "Could not reset avatar funds.", exception);
            if (sender is Window owner)
            {
                MessageBox.Show(owner, $"Could not reset funds.\n{exception.Message}", "Reset funds",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private static void EnsureAvatarAccounts(DependencyObject root)
    {
        if (root is PersonUC avatar)
        {
            if (string.IsNullOrWhiteSpace(avatar.MoneyAccountId) && !string.IsNullOrWhiteSpace(avatar.Name))
            {
                AvatarMoney.EnsureAccount(avatar);
            }
            return;
        }
        for (int index = 0; index < VisualTreeHelper.GetChildrenCount(root); index++)
        {
            EnsureAvatarAccounts(VisualTreeHelper.GetChild(root, index));
        }
    }
}
