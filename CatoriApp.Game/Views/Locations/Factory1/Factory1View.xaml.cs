using CatoriApp.Game.Controllers.Locations.Factory1;
using System.Windows;
using System.Windows.Controls;

namespace CatoriApp.Game.Views.Locations.Factory1;

public partial class Factory1View : Window
{
    private readonly Factory1ViewController _controller;
    public Factory1View()
    {
        InitializeComponent();
        _controller=new Factory1ViewController(this);
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
