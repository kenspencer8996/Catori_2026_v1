using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace CatoriUCLibrary.Views;

public sealed class AvatarMoneyWindowController
{
    private static readonly ConditionalWeakTable<Window, AvatarMoneyWindowController> controllers = new();
    private static bool displayRequested;
    private static WeakReference<AvatarMoneyWindowController>? activeController;
    private readonly Window owner;
    private readonly DispatcherTimer timer = new() { Interval = TimeSpan.FromMilliseconds(250) };
    private AvatarMoneyWindow? balanceWindow;
    private WeakReference<PersonUC>? selectedAvatar;
    private bool disposing;
    private bool positioned;

    private AvatarMoneyWindowController(Window window)
    {
        owner = window;
        owner.PreviewKeyDown += KeyDown;
        owner.PreviewMouseDown += MouseDown;
        owner.Activated += Activated;
        owner.IsVisibleChanged += VisibilityChanged;
        owner.Closed += Closed;
        timer.Tick += (_, _) => RefreshAvatar();
    }

    public static void Attach(Window window)
    {
        controllers.GetValue(window, key => new AvatarMoneyWindowController(key));
    }

    public static void Toggle(Window window)
    {
        var controller = controllers.GetValue(window, key => new AvatarMoneyWindowController(key));
        displayRequested = !displayRequested;
        if (!displayRequested && activeController?.TryGetTarget(out var active) == true)
        {
            active.balanceWindow?.Hide();
            active.timer.Stop();
        }
        controller.ApplyVisibility();
    }

    private void KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.M || e.IsRepeat || (Keyboard.Modifiers & ModifierKeys.Control) == 0)
        {
            return;
        }
        Toggle(owner);
        e.Handled = true;
    }

    private void MouseDown(object sender, MouseButtonEventArgs e)
    {
        for (var current = e.OriginalSource as DependencyObject; current != null;
            current = current is Visual ? VisualTreeHelper.GetParent(current) : LogicalTreeHelper.GetParent(current))
        {
            if (current is PersonUC avatar)
            {
                selectedAvatar = new WeakReference<PersonUC>(avatar);
                RefreshAvatar();
                return;
            }
        }
    }

    private void Activated(object? sender, EventArgs e)
    {
        ApplyVisibility();
    }

    private void VisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        ApplyVisibility();
    }

    private void ApplyVisibility()
    {
        if (!displayRequested || !owner.IsVisible)
        {
            balanceWindow?.Hide();
            timer.Stop();
            return;
        }
        if (activeController?.TryGetTarget(out var previous) == true && !ReferenceEquals(previous, this))
        {
            previous.balanceWindow?.Hide();
            previous.timer.Stop();
        }
        activeController = new WeakReference<AvatarMoneyWindowController>(this);
        if (balanceWindow == null)
        {
            balanceWindow = new AvatarMoneyWindow { Owner = owner };
            balanceWindow.Closing += BalanceWindowClosing;
            balanceWindow.PreviewKeyDown += KeyDown;
        }
        RefreshAvatar();
        if (!positioned)
        {
            var work = SystemParameters.WorkArea;
            double right = owner.WindowState == WindowState.Maximized ? work.Right : owner.Left + owner.ActualWidth;
            double top = owner.WindowState == WindowState.Maximized ? work.Top : owner.Top;
            balanceWindow.Left = Math.Clamp(right - balanceWindow.Width - 16, work.Left, Math.Max(work.Left, work.Right - balanceWindow.Width));
            balanceWindow.Top = Math.Clamp(top + 40, work.Top, Math.Max(work.Top, work.Bottom - balanceWindow.Height));
            positioned = true;
        }
        balanceWindow.Show();
        timer.Start();
    }

    private void RefreshAvatar()
    {
        if (balanceWindow == null)
        {
            return;
        }
        PersonUC? avatar = null;
        if (selectedAvatar?.TryGetTarget(out var selected) == true && selected.IsVisible && Window.GetWindow(selected) == owner)
        {
            avatar = selected;
        }
        avatar ??= FindAvatar(owner);
        if (avatar != null && !string.IsNullOrWhiteSpace(AvatarMoney.GetAccountId(avatar)))
        {
            // Refresh from disk too, so another game window/process cannot leave stale displayed balances.
            decimal balance = CatoriUCLibrary.Objects.Economy.GameEconomy.Current.GetBalance(AvatarMoney.GetAccountId(avatar));
            if (AvatarMoney.GetBalance(avatar) != balance)
            {
                AvatarMoney.RefreshBalance(avatar);
            }
        }
        balanceWindow.ShowAvatar(avatar);
    }

    private static PersonUC? FindAvatar(DependencyObject parent)
    {
        for (int index = 0; index < VisualTreeHelper.GetChildrenCount(parent); index++)
        {
            var child = VisualTreeHelper.GetChild(parent, index);
            if (child is UIElement { IsVisible: false })
            {
                continue;
            }
            if (child is PersonUC person && person.Role.ToString() is "Avatar" or "Passenger")
            {
                return person;
            }
            var nested = FindAvatar(child);
            if (nested != null)
            {
                return nested;
            }
        }
        return null;
    }

    private void BalanceWindowClosing(object? sender, CancelEventArgs e)
    {
        if (!disposing)
        {
            e.Cancel = true;
            displayRequested = false;
            ApplyVisibility();
        }
    }

    private void Closed(object? sender, EventArgs e)
    {
        disposing = true;
        timer.Stop();
        balanceWindow?.Close();
        owner.PreviewKeyDown -= KeyDown;
        owner.PreviewMouseDown -= MouseDown;
        owner.Activated -= Activated;
        owner.IsVisibleChanged -= VisibilityChanged;
        owner.Closed -= Closed;
        controllers.Remove(owner);
    }
}
