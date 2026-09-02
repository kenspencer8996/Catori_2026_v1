using CatoriApp.Game.Controllers.Locations.factory6;
using System.Windows;
using System.Windows.Controls;

namespace CatoriApp.Game.Views.Locations.factory6;

public partial class factory6View : Window
{
    private readonly factory6ViewController _controller;
    public factory6View()
    {
        InitializeComponent();
        _controller=new factory6ViewController(this);
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
