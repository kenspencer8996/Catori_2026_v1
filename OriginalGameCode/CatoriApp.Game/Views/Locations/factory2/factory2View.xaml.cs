using CatoriApp.Game.Controllers.Locations.factory2;
using System.Windows;
using System.Windows.Controls;

namespace CatoriApp.Game.Views.Locations.factory2;

public partial class factory2View : Window
{
    private readonly factory2ViewController _controller;
    public factory2View()
    {
        InitializeComponent();
        _controller=new factory2ViewController(this);
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
