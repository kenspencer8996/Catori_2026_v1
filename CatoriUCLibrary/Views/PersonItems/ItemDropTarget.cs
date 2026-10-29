using System.Windows;
using CatoriUCLibrary.Objects.PersonItems;

namespace CatoriUCLibrary.Views;

public static class ItemDropTarget
{
    public const string DragAnchorFormat = "Catori.ItemPointerAnchor";
    public const string DragSizeFormat = "Catori.ItemDisplaySize";
    public static readonly DependencyProperty SurfaceProperty = DependencyProperty.RegisterAttached(
        "Surface", typeof(ItemSceneSurface), typeof(ItemDropTarget), new PropertyMetadata(null, InventoryChanged));

    public static ItemSceneSurface? GetSurface(DependencyObject element)
    {
        return (ItemSceneSurface?)element.GetValue(SurfaceProperty);
    }

    public static void SetSurface(DependencyObject element, ItemSceneSurface? value)
    {
        element.SetValue(SurfaceProperty, value);
    }

    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
        "IsEnabled", typeof(bool), typeof(ItemDropTarget), new PropertyMetadata(false, EnabledChanged));

    public static bool GetIsEnabled(DependencyObject element)
    {
        return (bool)element.GetValue(IsEnabledProperty);
    }

    public static void SetIsEnabled(DependencyObject element, bool value)
    {
        element.SetValue(IsEnabledProperty, value);
    }

    private static void EnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        SetInventory(sender, (bool)e.NewValue ? (sender is PersonUC person ? person.CarriedItems : GetInventory(sender) ?? new PersonItemInventory()) : null);
    }

    public static readonly DependencyProperty InventoryProperty = DependencyProperty.RegisterAttached(
        "Inventory", typeof(PersonItemInventory), typeof(ItemDropTarget), new PropertyMetadata(null, InventoryChanged));

    public static PersonItemInventory? GetInventory(DependencyObject element)
    {
        return (PersonItemInventory?)element.GetValue(InventoryProperty);
    }

    public static void SetInventory(DependencyObject element, PersonItemInventory? value)
    {
        element.SetValue(InventoryProperty, value);
    }

    public static bool TryReceive(DependencyObject target, PersonItem item)
    {
        var inventory = GetInventory(target);
        if (inventory?.CanReceive(item) != true)
        {
            return false;
        }
        if (!ItemPurchase.TryAcquire(item, target))
        {
            return false;
        }
        inventory.Receive(item);
        return true;
    }

    private static void InventoryChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not UIElement element)
        {
            return;
        }
        element.PreviewDragOver -= DragOver;
        element.PreviewDrop -= Drop;
        if (e.NewValue != null)
        {
            element.AllowDrop = true;
            element.PreviewDragOver += DragOver;
            element.PreviewDrop += Drop;
        }
    }

    private static void DragOver(object sender, DragEventArgs e)
    {
        if (HasInnerContainer(sender, e.OriginalSource as DependencyObject))
        {
            return;
        }
        var inventory = GetInventory((DependencyObject)sender);
        var item = PersonItemDrag.Read(e.Data);
        if (item == null)
        {
            return;
        }
        e.Effects = GetSurface((DependencyObject)sender) != null || inventory?.CanReceive(item) == true
            ? DragDropEffects.Move : DragDropEffects.None;
        e.Handled = true;
    }

    private static void Drop(object sender, DragEventArgs e)
    {
        if (HasInnerContainer(sender, e.OriginalSource as DependencyObject))
        {
            return;
        }
        var inventory = GetInventory((DependencyObject)sender);
        var item = PersonItemDrag.Read(e.Data);
        if (item == null)
        {
            return;
        }
        e.Effects = DragDropEffects.None;
        var surface = GetSurface((DependencyObject)sender);
        Size size = e.Data.GetData(DragSizeFormat) is Size displaySize ? displaySize : new Size(48, 64);
        bool received = surface != null
            ? surface.TryReceive(item, e.GetPosition(surface), size, e.Data.GetData(DragAnchorFormat) is Point anchor ? anchor : new Point(.5, .5))
            : TryReceive((DependencyObject)sender, item);
        if (received)
        {
            e.Effects = DragDropEffects.Move;
        }
        e.Handled = true;
    }

    private static bool HasInnerContainer(object target, DependencyObject? source)
    {
        for (var current = source; current != null && !ReferenceEquals(current, target);
            current = current is System.Windows.Media.Visual or System.Windows.Media.Media3D.Visual3D
                ? System.Windows.Media.VisualTreeHelper.GetParent(current) : LogicalTreeHelper.GetParent(current))
        {
            if (current is PersonUC || current is ItemUC { IsContainer: true } || GetInventory(current) != null || GetSurface(current) != null)
            {
                return true;
            }
        }
        return false;
    }
}

