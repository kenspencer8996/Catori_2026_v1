using System.Windows;

using System.Windows.Controls;

using System.Windows.Input;

using System.Windows.Media;

using System.Windows.Media.Animation;

using System.Windows.Shapes;

using System.Windows.Threading;

using CatoriUCLibrary.Views.SpaceshipGame.GameObjects;


using CatoriUCLibrary.Views.SpaceshipGame.GameObjects;

namespace CatoriUCLibrary.Views;


public partial class SpaceshipGameUC : UserControl
{

    private Window? inputWindow;

    private ushort previousButtons;

    private const double PlayerSpeed = 5.5;

    private const double ShipSpeed = 2.25;

    private readonly DispatcherTimer _timer = new() {
 Interval = TimeSpan.FromMilliseconds(16) }
;

    private readonly HashSet<Key> _keys = [];

    private readonly List<Projectile> _projectiles = [];

    private readonly Random _random = new();

    private double _playerX, _playerY, _shipX, _shipY;

    private double _approachX, _approachY;

    private DateTime _nextApproachChange;

    private int _playerHealth, _shipHealth, _hitsRequired;

    private DateTime _lastEnemyShot;

    private DateTime _lastPlayerShot;

    private DateTime _invulnerableUntil;

    private double _controllerX, _controllerY;

    private bool _controllerFire;

    private bool _ended;

    private bool _isDragging;

    private Point _dragOffset;


    public SpaceshipGameUC()
    {

        InitializeComponent();

        _timer.Tick += GameTick;

        SizeChanged += (_, _) => KeepAstronautInBounds();

    }


    private void OnLoaded(object sender, RoutedEventArgs e)
    {

        GameStatusStore.Load();

        inputWindow = Window.GetWindow(this);

        if (inputWindow != null)
        {

            inputWindow.PreviewKeyDown += OnWindowKeyDown;

            inputWindow.PreviewKeyUp += OnWindowKeyUp;

        }

        ResetGame();

        Focus();

        Keyboard.Focus(this);

    }


    private void OnUnloaded(object sender, RoutedEventArgs e)
    {

        _timer.Stop();

        _keys.Clear();

        if (inputWindow != null)
        {

            inputWindow.PreviewKeyDown -= OnWindowKeyDown;

            inputWindow.PreviewKeyUp -= OnWindowKeyUp;

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

    private void OnWindowKeyUp(object sender, KeyEventArgs e)
    {

        OnKeyUp(sender, e);

    }


    private void ResetGame()
    {

        foreach (var projectile in _projectiles) GameCanvas.Children.Remove(projectile.Shape);

        _projectiles.Clear();

        _keys.Clear();

        _isDragging = false;

        _ended = false;

        _playerHealth = 5;

        _hitsRequired = GameStatusStore.Current.Games!.AstronautRadarChase!.RequiredHits;

        _shipHealth = _hitsRequired;

        _lastEnemyShot = DateTime.MinValue;

        _lastPlayerShot = DateTime.MinValue;

        EnemyShip.Visibility = Visibility.Visible;

        EndPanel.Visibility = Visibility.Collapsed;

        Radar.Visibility = Visibility.Collapsed;

        InstructionText.Text = "Drag or use arrow keys to move • Get behind the ship and press SPACE or controller A to fire";


        var width = Math.Max(700, ActualWidth);

        var height = Math.Max(500, ActualHeight);

        _playerX = width * .68;

        _playerY = height * .5;

        _shipX = -EnemyShip.Width - 90;

        _shipY = height * .42;

        ChooseNewApproach();

        UpdateHud();

        PositionActors();

        _timer.Start();

    }


    private void GameTick(object? sender, EventArgs e)
    {

        if (!IsVisible || inputWindow?.IsActive != true)
        {

            _keys.Clear();

            return;

        }

        var reading = ControllerInput.Read();

        ushort pressed = (ushort)(reading.Buttons & ~previousButtons);

        previousButtons = reading.Buttons;

        ApplyController(reading, (pressed & 0x10) != 0);

        if (_ended || GameCanvas.ActualWidth < 100 || GameCanvas.ActualHeight < 100) return;

        MovePlayerWithKeyboard();

        if (_controllerFire) FirePlayerLaser();

        MoveShip();

        MoveProjectiles();

        PositionActors();

        UpdateRadar();

    }


    public void ApplyController(CatoriUCLibrary.Views.SpaceshipGame.GameObjects.ControllerReading reading, bool restart)
    {

        _controllerX = reading.X;

        _controllerY = reading.Y;

        _controllerFire = reading.Fire;

        if (restart)
            ResetGame();

    }


    private void MovePlayerWithKeyboard()
    {

        double dx = (_keys.Contains(Key.Right) ? 1 : 0) - (_keys.Contains(Key.Left) ? 1 : 0);

        double dy = (_keys.Contains(Key.Down) ? 1 : 0) - (_keys.Contains(Key.Up) ? 1 : 0);

        dx = Math.Clamp(dx + _controllerX, -1, 1);

        dy = Math.Clamp(dy + _controllerY, -1, 1);

        if (dx == 0 && dy == 0) return;

        double length = Math.Sqrt(dx * dx + dy * dy);

        if (length > 1) {
 dx /= length;
 dy /= length;
 }

        Astronaut.FaceDirection(dx, dy);

        _playerX = Math.Clamp(_playerX + dx * PlayerSpeed, 0, GameCanvas.ActualWidth - Astronaut.Width);

        _playerY = Math.Clamp(_playerY + dy * PlayerSpeed, 56, GameCanvas.ActualHeight - Astronaut.Height - 44);

    }


    private void MoveShip()
    {

        if (DateTime.UtcNow >= _nextApproachChange ||
            Math.Abs(_approachX - _shipX) + Math.Abs(_approachY - _shipY) < 35)
            ChooseNewApproach();


        // Each approach is a different waypoint: direct, diagonal, flank, or off-screen pass.
        var dx = _approachX - _shipX;

        var dy = _approachY - _shipY;

        var distance = Math.Max(1, Math.Sqrt(dx * dx + dy * dy));

        var velocityX = dx / distance * ShipSpeed;

        var velocityY = dy / distance * ShipSpeed;

        _shipX += velocityX;

        _shipY += velocityY;

        EnemyShip.FaceDirection(velocityX, velocityY);


        // Keep the ship in a larger world, not inside the visible canvas.
        _shipX = Math.Clamp(_shipX, -EnemyShip.Width - 240, GameCanvas.ActualWidth + 240);

        _shipY = Math.Clamp(_shipY, -EnemyShip.Height - 240, GameCanvas.ActualHeight + 240);


        if (DateTime.UtcNow - _lastEnemyShot >= TimeSpan.FromMilliseconds(650))
        {

            AddProjectile(_shipX + EnemyShip.Width, _shipY + EnemyShip.Height / 2,
                _playerX + Astronaut.Width / 2, _playerY + Astronaut.Height / 2, false);

            _lastEnemyShot = DateTime.UtcNow;

        }

    }


    private void ChooseNewApproach()
    {

        var width = Math.Max(700, GameCanvas.ActualWidth);

        var height = Math.Max(500, GameCanvas.ActualHeight);

        var playerCenterX = _playerX + Astronaut.Width / 2;

        var playerCenterY = _playerY + Astronaut.Height / 2;


        switch (_random.Next(5))
        {

            case 0: // Direct pursuit from behind.
                _approachX = playerCenterX - EnemyShip.Width - _random.Next(80, 190);

                _approachY = playerCenterY - EnemyShip.Height / 2;

                break;

            case 1: // High diagonal swoop.
                _approachX = playerCenterX + _random.Next(-220, 221);

                _approachY = playerCenterY - _random.Next(150, 310);

                break;

            case 2: // Low diagonal swoop.
                _approachX = playerCenterX + _random.Next(-220, 221);

                _approachY = playerCenterY + _random.Next(150, 310);

                break;

            case 3: // Wide angular orbit around the astronaut.
                var angle = _random.NextDouble() * Math.PI * 2;

                var radius = _random.Next(190, 390);

                _approachX = playerCenterX + Math.Cos(angle) * radius - EnemyShip.Width / 2;

                _approachY = playerCenterY + Math.Sin(angle) * radius - EnemyShip.Height / 2;

                break;

            default: // Reposition beyond a random edge for a fresh attack run.
                var side = _random.Next(4);

                _approachX = side switch
                {

                    0 => -EnemyShip.Width - _random.Next(20, 190),
                    1 => width + _random.Next(20, 190),
                    _ => _random.NextDouble() * width
                }
;

                _approachY = side switch
                {

                    2 => -EnemyShip.Height - _random.Next(20, 190),
                    3 => height + _random.Next(20, 190),
                    _ => _random.NextDouble() * height
                }
;

                break;

        }


        _approachX = Math.Clamp(_approachX, -EnemyShip.Width - 240, width + 240);

        _approachY = Math.Clamp(_approachY, -EnemyShip.Height - 240, height + 240);

        _nextApproachChange = DateTime.UtcNow.AddMilliseconds(_random.Next(2200, 4700));

    }


    private void UpdateRadar()
    {

        var width = GameCanvas.ActualWidth;

        var height = GameCanvas.ActualHeight;

        var shipRight = _shipX + EnemyShip.Width;

        var shipBottom = _shipY + EnemyShip.Height;

        var outside = shipRight < 0 || _shipX > width || shipBottom < 0 || _shipY > height;

        var dx = shipRight < 0 ? -shipRight : _shipX > width ? _shipX - width : 0;

        var dy = shipBottom < 0 ? -shipBottom : _shipY > height ? _shipY - height : 0;

        var distance = Math.Sqrt(dx * dx + dy * dy);

        Radar.Visibility = outside && distance <= 100 ? Visibility.Visible : Visibility.Collapsed;

        if (Radar.Visibility == Visibility.Visible)
            Radar.Track(new Point(_shipX + EnemyShip.Width / 2, _shipY + EnemyShip.Height / 2),
                new Size(width, height), distance);

    }


    private void FirePlayerLaser()
    {

        if (_ended || DateTime.UtcNow - _lastPlayerShot < TimeSpan.FromMilliseconds(280)) return;

        var playerCenterX = _playerX + Astronaut.Width / 2;

        var playerCenterY = _playerY + Astronaut.Height / 2;

        var shipCenterY = _shipY + EnemyShip.Height / 2;

        var behindShip = playerCenterX < _shipX && Math.Abs(playerCenterY - shipCenterY) < 120;

        if (!behindShip)
        {

            InstructionText.Text = "Move behind the ship (to its left), line up, then press SPACE or controller A.";

            return;

        }

        AddProjectile(playerCenterX, playerCenterY, _shipX + EnemyShip.Width / 2, shipCenterY, true);

        _lastPlayerShot = DateTime.UtcNow;

        InstructionText.Text = "Target locked — keep firing from behind!";

    }


    private void AddProjectile(double x, double y, double targetX, double targetY, bool friendly)
    {

        var dx = targetX - x;

        var dy = targetY - y;

        var distance = Math.Max(1, Math.Sqrt(dx * dx + dy * dy));

        var shot = new Ellipse
        {

            Width = friendly ? 18 : 22, Height = 7,
            Fill = friendly ? Brushes.Cyan : Brushes.Red,
            Effect = new System.Windows.Media.Effects.DropShadowEffect
            {
 Color = friendly ? Colors.Cyan : Colors.Red, BlurRadius = 12, ShadowDepth = 0 }

        }
;

        GameCanvas.Children.Add(shot);

        Canvas.SetZIndex(shot, 3);

        _projectiles.Add(new Projectile(shot, x, y, dx / distance * 8, dy / distance * 8, friendly));

    }


    private void MoveProjectiles()
    {

        for (var i = _projectiles.Count - 1;
 i >= 0;
 i--)
        {

            var shot = _projectiles[i];

            shot.X += shot.Dx;

            shot.Y += shot.Dy;

            Canvas.SetLeft(shot.Shape, shot.X);

            Canvas.SetTop(shot.Shape, shot.Y);

            var target = shot.Friendly
                ? new Rect(_shipX, _shipY, EnemyShip.Width, EnemyShip.Height)
                : new Rect(_playerX, _playerY, Astronaut.Width, Astronaut.Height);

            var hit = target.IntersectsWith(new Rect(shot.X, shot.Y, shot.Shape.Width, shot.Shape.Height));

            if (hit && (shot.Friendly || DateTime.UtcNow >= _invulnerableUntil))
            {

                if (shot.Friendly) _shipHealth--;

                else {
 _playerHealth--;
 _invulnerableUntil = DateTime.UtcNow.AddMilliseconds(700);
 }

                RemoveProjectile(i);

                UpdateHud();

                if (_shipHealth <= 0) ExplodeShip();

                else if (_playerHealth <= 0) EndGame(false);

            }

            else if (shot.X < -400 || shot.Y < -400 ||
                     shot.X > GameCanvas.ActualWidth + 400 || shot.Y > GameCanvas.ActualHeight + 400)
                RemoveProjectile(i);

        }

    }


    private void RemoveProjectile(int index)
    {

        GameCanvas.Children.Remove(_projectiles[index].Shape);

        _projectiles.RemoveAt(index);

    }


    private void ExplodeShip()
    {

        if (_ended) return;

        _ended = true;

        _timer.Stop();

        Radar.Visibility = Visibility.Collapsed;

        EnemyShip.Visibility = Visibility.Collapsed;

        var status = GameStatusStore.Current.Games!.AstronautRadarChase!;

        status.Wins++;

        status.RequiredHits = _hitsRequired + 1;

        GameStatusStore.Save();


        var blast = new Ellipse {
 Width = 28, Height = 28, Fill = Brushes.White,
            RenderTransformOrigin = new Point(.5, .5), RenderTransform = new ScaleTransform(1, 1) }
;

        Canvas.SetLeft(blast, _shipX + EnemyShip.Width / 2 - 14);

        Canvas.SetTop(blast, _shipY + EnemyShip.Height / 2 - 14);

        Canvas.SetZIndex(blast, 20);

        GameCanvas.Children.Add(blast);

        var scale = (ScaleTransform)blast.RenderTransform;

        scale.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(1, 9, TimeSpan.FromMilliseconds(600)));

        scale.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(1, 9, TimeSpan.FromMilliseconds(600)));

        blast.BeginAnimation(OpacityProperty, new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(750)));

        var finish = new DispatcherTimer {
 Interval = TimeSpan.FromMilliseconds(800) }
;

        finish.Tick += (_, _) => {
 finish.Stop();
 GameCanvas.Children.Remove(blast);
 EndGame(true);
 }
;

        finish.Start();

    }


    private void EndGame(bool won)
    {

        _ended = true;

        _timer.Stop();

        Radar.Visibility = Visibility.Collapsed;

        EndTitle.Text = won ? "MISSION COMPLETE" : "ASTRONAUT DOWN";

        EndTitle.Foreground = won ? Brushes.Cyan : Brushes.OrangeRed;

        EndMessage.Text = won ? "The radar-guided pursuit is over. The ship was destroyed!" : "The ship's lasers caught you.";

        EndPanel.Visibility = Visibility.Visible;

    }


    private void UpdateHud()
    {

        HealthText.Text = $"Astronaut: {new string('♥', Math.Max(0, _playerHealth))}";

        ShipHealthText.Text = $"Ship: {_shipHealth}/{_hitsRequired}";

    }


    private void PositionActors()
    {

        Canvas.SetLeft(Astronaut, _playerX);
 Canvas.SetTop(Astronaut, _playerY);

        Canvas.SetLeft(EnemyShip, _shipX);
 Canvas.SetTop(EnemyShip, _shipY);

        Astronaut.Opacity = DateTime.UtcNow < _invulnerableUntil ? .45 : 1;

    }


    private void KeepAstronautInBounds()
    {

        if (GameCanvas.ActualWidth <= 0 || GameCanvas.ActualHeight <= 0) return;

        _playerX = Math.Clamp(_playerX, 0, Math.Max(0, GameCanvas.ActualWidth - Astronaut.Width));

        _playerY = Math.Clamp(_playerY, 56, Math.Max(56, GameCanvas.ActualHeight - Astronaut.Height - 44));

    }


    private void OnKeyDown(object sender, KeyEventArgs e)
    {

        if (e.Key == Key.Enter && !e.IsRepeat)
        {

            ResetGame();

            e.Handled = true;

        }

        if (e.Key is Key.Left or Key.Right or Key.Up or Key.Down) {
 _keys.Add(e.Key);
 e.Handled = true;
 }

        if (e.Key == Key.Space) {
 FirePlayerLaser();
 e.Handled = true;
 }

    }


    private void OnKeyUp(object sender, KeyEventArgs e)
    {

        _keys.Remove(e.Key);

        if (e.Key is Key.Left or Key.Right or Key.Up or Key.Down) e.Handled = true;

    }


    private void Astronaut_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {

        if (_ended) return;

        _isDragging = true;

        var pointer = e.GetPosition(GameCanvas);

        _dragOffset = new Point(pointer.X - _playerX, pointer.Y - _playerY);

        Astronaut.CaptureMouse();

        Focus();
 Keyboard.Focus(this);

        e.Handled = true;

    }


    private void Astronaut_MouseMove(object sender, MouseEventArgs e)
    {

        if (!_isDragging || e.LeftButton != MouseButtonState.Pressed) return;

        var pointer = e.GetPosition(GameCanvas);

        var nextX = Math.Clamp(pointer.X - _dragOffset.X, 0, GameCanvas.ActualWidth - Astronaut.Width);

        var nextY = Math.Clamp(pointer.Y - _dragOffset.Y, 56, GameCanvas.ActualHeight - Astronaut.Height - 44);

        Astronaut.FaceDirection(nextX - _playerX, nextY - _playerY);

        _playerX = nextX;

        _playerY = nextY;

        PositionActors();

        e.Handled = true;

    }


    private void Astronaut_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {

        if (!_isDragging) return;

        _isDragging = false;

        Astronaut.ReleaseMouseCapture();

        e.Handled = true;

    }


    private void Restart_Click(object sender, RoutedEventArgs e) {
 ResetGame();
 Focus();
 Keyboard.Focus(this);
 }



}

