using System.Windows;
using System.Windows.Controls;
using CatoriApp.Core.Objects.DragDrop;
namespace CatoriUCLibrary.Views;
public partial class PilotAirplaneUC : UserControl, IDropTarget
{
    private PilotCockpitWindow? cockpit;
    public PilotAirplaneUC()
    {
        InitializeComponent();
    }
    public bool CanDrop(IDraggable element)
    {
        return element is PersonUC && cockpit == null;
    }
    public void OnDrop(IDraggable element)
    {
        if (!CanDrop(element))
        {
            return;
        }
        var owner = Window.GetWindow(this);
        cockpit = new PilotCockpitWindow();
        cockpit.Owner = owner;
        cockpit.Closed += (_, _) =>
        {
            cockpit = null;
            owner?.Activate();
        };
        HighlightOff();
        Dispatcher.BeginInvoke(new Action(() => cockpit?.Show()));
    }
    public void HighlightOn()
    {
        DropHighlight.BorderThickness = new Thickness(3);
    }
    public void HighlightOff()
    {
        DropHighlight.BorderThickness = new Thickness(0);
    }
    public Point GetSnapPoint(IDraggable dragged)
    {
        double x = Canvas.GetLeft(this);
        double y = Canvas.GetTop(this);
        return new Point((double.IsNaN(x) ? 0 : x) + ActualWidth / 2, (double.IsNaN(y) ? 0 : y) + ActualHeight);
    }
}
