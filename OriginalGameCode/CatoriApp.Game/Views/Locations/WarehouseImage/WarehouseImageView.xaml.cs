using CatoriApp.Game.Controllers.Locations.WarehouseImage;
using System.Windows;
using System.Windows.Controls;

namespace CatoriApp.Game.Views.Locations.WarehouseImage;

public partial class WarehouseImageView : Window
{
    private readonly WarehouseImageViewController _controller;
    public WarehouseImageView()
    {
        InitializeComponent();
        _controller=new WarehouseImageViewController(this);
    }

    private void ExitButton_Click(object sender,RoutedEventArgs e)
    {
        _controller.Close();
    }

    private void MainCanvas_SizeChanged(object sender,SizeChangedEventArgs e)
    {
        Canvas.SetLeft(ExitButton,Math.Max(0,e.NewSize.Width-ExitButton.Width-8));
    }
}
