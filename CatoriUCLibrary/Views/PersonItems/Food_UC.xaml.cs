using CatoriInterfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CatoriUCLibrary.Views.PersonItems;

public partial class Food_UC : UserControl, IFood
{
    private Point _dragStart;

    public Food_UC()
    {
        InitializeComponent();
    }

    public string Name
    {
        get;
        set;
    } = "Food";

    private void Food_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _dragStart = e.GetPosition(this);
    }

    private void Food_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }
        Point current = e.GetPosition(this);
        if (Math.Abs(current.X - _dragStart.X) < SystemParameters.MinimumHorizontalDragDistance
            && Math.Abs(current.Y - _dragStart.Y) < SystemParameters.MinimumVerticalDragDistance)
        {
            return;
        }
        DragDrop.DoDragDrop(this, this, DragDropEffects.Copy);
    }
}
