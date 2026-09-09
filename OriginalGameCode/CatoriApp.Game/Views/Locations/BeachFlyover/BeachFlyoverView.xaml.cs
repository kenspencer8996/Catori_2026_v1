using CatoriApp.Game.Controllers.Locations.BeachFlyover;
using System.Windows;
using System.Windows.Controls;

namespace CatoriApp.Game.Views.Locations.BeachFlyover;

public partial class BeachFlyoverView : Window
{
    private readonly BeachFlyoverViewController _controller;
    public BeachFlyoverView(){InitializeComponent();_controller=new BeachFlyoverViewController(this);}
    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        _controller.Close();
    }

    private void MainCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        Canvas.SetLeft(ExitButton, Math.Max(0, e.NewSize.Width - ExitButton.Width - 8));
    }
}
