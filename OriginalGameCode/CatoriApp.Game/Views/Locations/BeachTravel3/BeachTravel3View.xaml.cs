using CatoriApp.Game.Controllers.Locations.BeachTravel3;
using System.Windows;
using System.Windows.Controls;

namespace CatoriApp.Game.Views.Locations.BeachTravel3;

public partial class BeachTravel3View : Window
{
    private readonly BeachTravel3ViewController _controller;
    public BeachTravel3View(){InitializeComponent();_controller=new BeachTravel3ViewController(this);}
    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        _controller.Close();
    }

    private void MainCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        Canvas.SetLeft(ExitButton, Math.Max(0, e.NewSize.Width - ExitButton.Width - 8));
    }
}
