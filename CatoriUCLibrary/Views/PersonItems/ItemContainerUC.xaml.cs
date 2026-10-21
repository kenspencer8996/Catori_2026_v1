using System.Windows;
using System.Windows.Controls;
using CatoriUCLibrary.Objects.PersonItems;

namespace CatoriUCLibrary.Views;

public partial class ItemContainerUC : UserControl
{
    public PersonItemInventory Inventory { get; } = new();

    public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register(
        nameof(ItemWidth), typeof(double), typeof(ItemContainerUC), new PropertyMetadata(64d));

    public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register(
        nameof(ItemHeight), typeof(double), typeof(ItemContainerUC), new PropertyMetadata(80d));

    public double ItemWidth
    {
        get => (double)GetValue(ItemWidthProperty);
        set => SetValue(ItemWidthProperty, value);
    }

    public double ItemHeight
    {
        get => (double)GetValue(ItemHeightProperty);
        set => SetValue(ItemHeightProperty, value);
    }

    public ItemContainerUC()
    {
        InitializeComponent();
    }
}
