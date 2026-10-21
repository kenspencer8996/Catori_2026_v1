using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CatoriUCLibrary.Views;

public partial class PersonCarriedItems : UserControl
{
    public PersonCarriedItems()
    {
        InitializeComponent();
        Loaded += EnablePersonDrops;
    }

    private void EnablePersonDrops(object sender, RoutedEventArgs e)
    {
        for (DependencyObject? parent = VisualTreeHelper.GetParent(this); parent != null; parent = VisualTreeHelper.GetParent(parent))
        {
            if (parent is PersonUC person)
            {
                ItemDropTarget.SetIsEnabled(person, true);
                return;
            }
        }
    }
}
