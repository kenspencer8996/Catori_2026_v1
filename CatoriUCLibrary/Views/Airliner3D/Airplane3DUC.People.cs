using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CatoriUCLibrary.Views.Person;
using CatoriApp.Game.Objects.AnimationOnPath;

namespace CatoriUCLibrary.Views.Airliner3D;

public partial class Airplane3DUC : CatoriApp.Core.Objects.DragDrop.IDropTarget
{
    private readonly Dictionary<string, (PersonUC Person, Canvas Scene)> _boardedPeople = new();

    public event EventHandler? PassengerDropCompleted;

    public bool CanDrop(CatoriApp.Core.Objects.DragDrop.IDraggable element)
    {
        return element is PersonUC person && CanBoardPerson(person.Role);
    }

    public void OnDrop(CatoriApp.Core.Objects.DragDrop.IDraggable element)
    {
        if (element is PersonUC person)
        {
            TryBoardPerson(person);
        }
    }

    public Point GetSnapPoint(CatoriApp.Core.Objects.DragDrop.IDraggable element)
    {
        double left = Canvas.GetLeft(element.Visual);
        double top = Canvas.GetTop(element.Visual);
        return new Point(double.IsNaN(left) ? 0 : left, double.IsNaN(top) ? 0 : top);
    }

    public void HighlightOn()
    {
    }

    public void HighlightOff()
    {
    }

    public bool CanBoardPerson(PersonRole role)
    {
        return !IsPathActive && !LeftEngineRunning && !RightEngineRunning
            && (role == PersonRole.Pilot && BoardedPilot is null
                || role == PersonRole.Passenger && BoardedPilot is not null && BoardedPassenger is null);
    }

    public bool TryBoardPerson(PersonUC person, bool requestDeparture = true)
    {
        Dispatcher.VerifyAccess();
        ArgumentNullException.ThrowIfNull(person);
        if (!CanBoardPerson(person.Role) || FindPeopleScene() is not Canvas scene)
        {
            return false;
        }
        var role = person.Role == PersonRole.Pilot ? AirplaneOccupantRole.Pilot : AirplaneOccupantRole.Passenger;
        var occupant = new AirplaneOccupant(Guid.NewGuid().ToString("N"), person.Name, role);
        _boardedPeople.Add(occupant.Id, (person, scene));
        if (role == AirplaneOccupantRole.Pilot)
        {
            BoardedPilot = occupant;
        }
        else
        {
            BoardedPassenger = occupant;
        }
        person.Visibility = Visibility.Collapsed;
        SetTripState(role == AirplaneOccupantRole.Pilot ? AirplaneTripState.WaitingForPassenger : AirplaneTripState.ReadyToTaxi);
        OccupantBoarded?.Invoke(occupant);
        if (requestDeparture && role == AirplaneOccupantRole.Passenger)
        {
            PassengerDropCompleted?.Invoke(this, EventArgs.Empty);
        }
        return true;
    }

    private Canvas? FindPeopleScene()
    {
        DependencyObject? parent = VisualTreeHelper.GetParent(this);
        while (parent is not null && parent is not Canvas)
        {
            parent = VisualTreeHelper.GetParent(parent);
        }
        return parent as Canvas;
    }

    public IReadOnlyList<PersonUC> TakeBoardedPeople()
    {
        Dispatcher.VerifyAccess();
        var people = new[] { BoardedPilot, BoardedPassenger }
            .Where(occupant => occupant is not null && _boardedPeople.ContainsKey(occupant.Id))
            .Select(occupant => _boardedPeople[occupant!.Id].Person).ToList();
        if (people.Count > 0)
        {
            _boardedPeople.Clear();
            BoardedPilot = null;
            BoardedPassenger = null;
        }
        return people;
    }

    private void ReleaseBoardedPerson(AirplaneOccupant? occupant, int index)
    {
        if (occupant is null || !_boardedPeople.Remove(occupant.Id, out var entry))
        {
            return;
        }
        var scene = FindPeopleScene() ?? entry.Scene;
        var person = entry.Person;
        double planeWidth = ActualWidth > 0 ? ActualWidth : Width;
        double planeHeight = ActualHeight > 0 ? ActualHeight : Height;
        Rect bounds = IsDescendantOf(scene)
            ? TransformToAncestor(scene).TransformBounds(new Rect(0, 0, planeWidth, planeHeight))
            : new Rect(0, 0, 0, 0);
        GameAnimationHelper.DetachFromParent(person);
        person.RenderTransform = Transform.Identity;
        person.CurrentActivity = default;
        person.HideHeldTicket();
        person.Visibility = Visibility.Visible;
        double width = double.IsFinite(person.Width) && person.Width > 0 ? person.Width : 47;
        double height = double.IsFinite(person.Height) && person.Height > 0 ? person.Height : 63;
        double sceneWidth = scene.ActualWidth > 0 ? scene.ActualWidth : scene.Width;
        double sceneHeight = scene.ActualHeight > 0 ? scene.ActualHeight : scene.Height;
        Canvas.SetLeft(person, Math.Clamp(bounds.Left + bounds.Width * .6 + index * (width + 10), 0, Math.Max(0, sceneWidth - width)));
        Canvas.SetTop(person, Math.Clamp(bounds.Bottom + 12 - height, 0, Math.Max(0, sceneHeight - height)));
        Panel.SetZIndex(person, Math.Max(3000, Panel.GetZIndex(this) + 1));
        scene.Children.Add(person);
    }
}
