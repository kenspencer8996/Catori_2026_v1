using CatoriUCLibrary.Views.Pacman.GameObjects;

using System.Windows;

using System.Windows.Controls;

using System.Windows.Input;

using System.Windows.Threading;

namespace CatoriUCLibrary.Views;

public partial class PacmanUC : UserControl
{

    private readonly GameState game = new();

    private readonly ControllerInput controller = new();

    private readonly DispatcherTimer timer = new() {
 Interval = TimeSpan.FromMilliseconds(30) }
;

    private DateTime lastMove;

    private Window? inputWindow;

    public PacmanUC()
    {

        InitializeComponent();
 Surface.Game = game;
 game.NewGame();

        Loaded += OnLoaded;
 Unloaded += OnUnloaded;
 PreviewKeyDown += OnKeyDown;
 timer.Tick += OnTick;

        UpdateDisplay();

    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {

        inputWindow = Window.GetWindow(this);

        if (inputWindow != null)
        {

            inputWindow.PreviewKeyDown += OnWindowKeyDown;

        }

        Focus();

        lastMove = DateTime.UtcNow;

        timer.Start();

    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {

        timer.Stop();

        if (inputWindow != null)
        {

            inputWindow.PreviewKeyDown -= OnWindowKeyDown;

            inputWindow = null;

        }

    }

    private void OnWindowKeyDown(object sender, KeyEventArgs e)
    {

        if (IsVisible)
        {

            OnKeyDown(sender, e);

        }

    }

    private void OnTick(object? sender,EventArgs e)
    {

        if (!IsVisible || inputWindow?.IsActive != true)
        {

            lastMove = DateTime.UtcNow;

            return;

        }

        controller.Poll();

        if (controller.Direction >= 0) game.Requested = controller.Direction;

        if (controller.PausePressed) game.Paused = !game.Paused;

        if (controller.RestartPressed) game.NewGame();

        if (DateTime.UtcNow-lastMove >= TimeSpan.FromMilliseconds(Math.Max(90,150-game.Level*5))) {
 game.Advance();
 lastMove=DateTime.UtcNow;
 }

        UpdateDisplay();

    }

    private void OnKeyDown(object sender,KeyEventArgs e)
    {

        switch(e.Key)
        {

            case Key.Up: case Key.W: game.Requested=0;
 break;

            case Key.Right: case Key.D: game.Requested=1;
 break;

            case Key.Down: case Key.S: game.Requested=2;
 break;

            case Key.Left: case Key.A: game.Requested=3;
 break;

            case Key.P: case Key.Escape: if (!e.IsRepeat) game.Paused=!game.Paused;
 break;

            case Key.R: if (!e.IsRepeat) game.NewGame();
 break;

            default: return;

        }

        e.Handled=true;
 UpdateDisplay();

    }

    private void NewGame_Click(object sender,RoutedEventArgs e) {
 game.NewGame();
 Focus();
 UpdateDisplay();
 }

    private void UpdateDisplay()
    {

        ScoreText.Text=$"SCORE  {game.Score:00000}     LIVES  {game.Lives}     LEVEL  {game.Level}";

        StatusText.Text=game.Ended?"Game over · Press R or A to play again":game.Paused?"Paused":game.Powered?"Power up! Chase the ghosts":"Collect every pellet. Avoid the ghosts.";

        ControllerText.Text=controller.Connected?"Controller connected":"Keyboard ready · Connect an Xbox-compatible controller anytime";

        Surface.InvalidateVisual();

    }

}

