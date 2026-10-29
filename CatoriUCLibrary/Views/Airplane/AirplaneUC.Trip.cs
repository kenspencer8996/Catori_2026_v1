using System.Windows;
using System.Windows.Media;

namespace CatoriUCLibrary.Views;

public partial class AirplaneUC
{
    public void ResetForNextTrip()
    {
        // ApplySettings cancels both engine loops, empties the seats and rebuilds the authored part pose.
        ApplySettings(_settings);
        Reset();
        _boardingInProgress = false;
        ClearBoardedCrew();
        PilotView.Opacity = 0;
        PassengerView.Opacity = 0;
        PilotWindshield.Opacity = 0;
        if (FindName("PassengerWindow") is UIElement passengerWindow)
        {
            passengerWindow.Opacity = 0;
        }
        foreach (var smoke in new[] { LeftSmoke, RightSmoke })
        {
            smoke.BeginAnimation(OpacityProperty, null);
            smoke.Opacity = 0;
            if (smoke.RenderTransform is TranslateTransform translation)
            {
                translation.BeginAnimation(TranslateTransform.XProperty, null);
                translation.X = 0;
            }
        }
    }
}
