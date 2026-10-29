using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CatoriApp.Game.Objects.AnimationOnPath;

namespace CatoriUCLibrary.Views;

public partial class AirplaneUC
{
    public PersonUC? BoardedPilot { get; private set; }
    public PersonUC? BoardedPassenger { get; private set; }
    public bool AreEnginesRunning => _leftEngineCancellation != null || _rightEngineCancellation != null;
    private bool disembarking;

    private void RememberBoardedPilot(PersonUC person)
    {
        BoardedPilot = person;
    }

    private void RememberBoardedPassenger(PersonUC person)
    {
        BoardedPassenger = person;
    }

    public void StopEngines()
    {
        _leftEngineCancellation?.Cancel();
        _leftEngineCancellation?.Dispose();
        _leftEngineCancellation = null;
        _rightEngineCancellation?.Cancel();
        _rightEngineCancellation?.Dispose();
        _rightEngineCancellation = null;
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

    public async Task ParkAndDisembarkAsync(Canvas destination, AirplaneUC? crewPlane = null, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(destination);
        if (disembarking)
        {
            return;
        }
        disembarking = true;
        crewPlane ??= this;
        try
        {
            StopEngines();
            crewPlane.StopEngines();
            var people = new[] { crewPlane.BoardedPassenger, crewPlane.BoardedPilot }
                .OfType<PersonUC>().Distinct().ToArray();
            await SetDoorOpenAsync(true, TimeSpan.FromMilliseconds(300), token);
            double planeWidth = ActualWidth > 0 ? ActualWidth : Width;
            double planeHeight = ActualHeight > 0 ? ActualHeight : Height;
            Rect bounds = TransformToAncestor(destination).TransformBounds(new Rect(0, 0, planeWidth, planeHeight));
            double sceneWidth = destination.ActualWidth > 0 ? destination.ActualWidth : destination.Width;
            double sceneHeight = destination.ActualHeight > 0 ? destination.ActualHeight : destination.Height;
            double offset = 0;
            foreach (var person in people)
            {
                GameAnimationHelper.DetachFromParent(person);
                person.RenderTransform = Transform.Identity;
                person.CurrentActivity = default;
                person.HideHeldTicket();
                person.Visibility = Visibility.Visible;
                double width = person.Width > 0 && double.IsFinite(person.Width) ? person.Width : 47;
                double height = person.Height > 0 && double.IsFinite(person.Height) ? person.Height : 63;
                person.Width = width;
                person.Height = height;
                Canvas.SetLeft(person, Math.Clamp(bounds.Left + bounds.Width * .6 + offset, 0, Math.Max(0, sceneWidth - width)));
                Canvas.SetTop(person, Math.Clamp(bounds.Bottom + 12 - height, 0, Math.Max(0, sceneHeight - height)));
                Panel.SetZIndex(person, Math.Max(3000, Panel.GetZIndex(this) + 1));
                destination.Children.Add(person);
                offset += width + 10;
            }
            crewPlane.ClearBoardedCrew();
            ClearBoardedCrew();
        }
        finally
        {
            disembarking = false;
        }
    }

    private void ClearBoardedCrew()
    {
        BoardedPilot = null;
        BoardedPassenger = null;
        HasPilot = false;
        HasPassenger = false;
        PilotView.Opacity = 0;
        PassengerView.Opacity = 0;
        PilotWindshield.Opacity = 0;
        if (FindName("PassengerWindow") is UIElement passengerWindow)
        {
            passengerWindow.Opacity = 0;
        }
    }
}
