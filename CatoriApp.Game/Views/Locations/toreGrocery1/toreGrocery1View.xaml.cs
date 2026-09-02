using CatoriApp.Game.Controllers.Locations.toreGrocery1;
using System.Windows;
using System.Windows.Controls;

namespace CatoriApp.Game.Views.Locations.toreGrocery1;

public partial class toreGrocery1View : Window
{
    private readonly toreGrocery1ViewController _controller;
    public toreGrocery1View()
    {
        InitializeComponent();
        _controller=new toreGrocery1ViewController(this);
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
