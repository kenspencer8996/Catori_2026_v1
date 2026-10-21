using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CatoriUCLibrary.Objects.PersonItems;

namespace CatoriUCLibrary.Views;

public static class ItemPurchase
{
    public static bool TryAcquire(PersonItem item, DependencyObject target)
    {
        if (!item.IsForSale)
        {
            return true;
        }
        PersonUC? buyer = FindBuyer(target);
        if (buyer == null)
        {
            MessageBox.Show("Move an avatar into the scene before buying an item.", "Purchase");
            return false;
        }
        if (!AvatarMoney.TryPurchase(buyer, item.Price, "item:" + item.Id, item.ItemName))
        {
            return false;
        }
        item.IsForSale = false;
        return true;
    }

    public static PersonUC? FindBuyer(DependencyObject target)
    {
        for (DependencyObject? current = target; current != null; current = current is Visual
            ? VisualTreeHelper.GetParent(current) : LogicalTreeHelper.GetParent(current))
        {
            if (current is PersonUC avatar)
            {
                return avatar;
            }
            if (current is Canvas scene)
            {
                var avatars = scene.Children.OfType<PersonUC>().Where(person => person.IsVisible
                    && (person.Role.ToString() is "Avatar" or "Passenger")).ToList();
                if (avatars.Count != 0)
                {
                    Point point = target is UIElement visual ? visual.TranslatePoint(new Point(), scene) : new Point();
                    return avatars.OrderBy(person => (person.TranslatePoint(new Point(person.ActualWidth / 2, person.ActualHeight / 2), scene) - point).Length).First();
                }
            }
        }
        return null;
    }
}
