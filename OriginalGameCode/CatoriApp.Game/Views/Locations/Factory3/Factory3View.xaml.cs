using CatoriApp.Game.Controllers.Locations.Factory3;
using System.Windows;
using System.Windows.Controls;

namespace CatoriApp.Game.Views.Locations.Factory3;

public partial class Factory3View : Window
{
    private readonly Factory3ViewController _controller;
    public Factory3View()
    {
        InitializeComponent();
        _controller=new Factory3ViewController(this);
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
