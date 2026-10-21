using CatoriUCLibrary.Views.SpaceInvaders.GameObjects;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace CatoriUCLibrary.Views
{
    /// <summary>
    /// Interaction logic for SpaceInvadersUC.xaml
    /// </summary>
    public partial class SpaceInvadersUC : UserControl
    {
        private readonly SpaceInvadersGameEngine engine = new();
        private readonly Stopwatch clock = new();
        private readonly DispatcherTimer timer = new() { Interval = TimeSpan.FromMilliseconds(16) };
        private double previousTime;
        private Window? inputWindow;

        public SpaceInvadersUC()
        {
            InitializeComponent();
            Playfield.Engine = engine;
            timer.Tick += Tick;
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            IsVisibleChanged += OnVisibilityChanged;
            Playfield.LostKeyboardFocus += (_, _) => PauseGame();
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {

            inputWindow = Window.GetWindow(this);
            if (inputWindow != null)
            {
                inputWindow.PreviewKeyDown += Window_PreviewKeyDown;
                inputWindow.PreviewKeyUp += Window_PreviewKeyUp;
            }
            previousTime = 0;
            clock.Restart();
            timer.Start();
            Playfield.Focus();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (inputWindow != null)
            {
                inputWindow.PreviewKeyDown -= Window_PreviewKeyDown;
                inputWindow.PreviewKeyUp -= Window_PreviewKeyUp;
                inputWindow = null;
            }
            timer.Stop();
            clock.Stop();
            PauseGame();
        }

        private void OnVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
                {
                    if (IsVisible)
                    {
                        Playfield.Focus();
                    }
                }));
            }
            else
            {
                PauseGame();
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (IsVisible)
            {
                Playfield_KeyDown(sender, e);
                if (e.Handled)
                {
                    Playfield.Focus();
                }
            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (IsVisible)
            {
                Playfield_KeyUp(sender, e);
            }
        }

        private void Tick(object? sender, EventArgs e)
        {
            UpdateGamepad();
            double now = clock.Elapsed.TotalSeconds;
            engine.Update(now - previousTime);
            previousTime = now;
            Playfield.InvalidateVisual();
        }
        private void UpdateGamepad()
        {
            var reading = SpaceInvadersControllerInput.Read();
            bool keyboardActive = Playfield.IsKeyboardFocusWithin;

            engine.Left = false;
            engine.Right = false;
            engine.Fire = false;

            if (reading.Left)
                engine.Left = true;

            if (reading.Right)
                engine.Right = true;

            if (reading.Fire)
                engine.Fire = true;

            if (keyboardActive)
            {
                if (Keyboard.IsKeyDown(Key.Left) || Keyboard.IsKeyDown(Key.A))
                    engine.Left = true;

                if (Keyboard.IsKeyDown(Key.Right) || Keyboard.IsKeyDown(Key.D))
                    engine.Right = true;

                if (Keyboard.IsKeyDown(Key.Space))
                    engine.Fire = true;
            }
            //// Recompute every frame so released controls cannot leave input latched.
            //var reading = SpaceInvadersControllerInput.Read();
            //bool keyboardActive = Playfield.IsKeyboardFocusWithin;

            //engine.Left = reading.Left
            //    || (keyboardActive && (Keyboard.IsKeyDown(Key.Left) || Keyboard.IsKeyDown(Key.A)));
            //engine.Right = reading.Right
            //    || (keyboardActive && (Keyboard.IsKeyDown(Key.Right) || Keyboard.IsKeyDown(Key.D)));
            //engine.Fire = reading.Fire
            //    || (keyboardActive && Keyboard.IsKeyDown(Key.Space));
        }
        public void PauseGame()
        {
            if (engine.State is SpaceInvadersGameState.Playing or SpaceInvadersGameState.WaveCleared)
            {
                engine.TogglePause();
            }
            engine.ClearInput();
            Playfield.InvalidateVisual();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Playfield.Focus();
            engine.Start();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            engine.TogglePause();
            Playfield.Focus();
        }

        private void Playfield_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Playfield.Focus();
        }

        private void Playfield_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                case Key.A:
                    engine.Left = true;
                    break;
                case Key.Right:
                case Key.D:
                    engine.Right = true;
                    break;
                case Key.Space:
                    engine.Fire = true;
                    break;
                case Key.P:
                case Key.Escape:
                    if (!e.IsRepeat)
                    {
                        engine.TogglePause();
                    }
                    break;
                case Key.Enter:
                    if (!e.IsRepeat && engine.State is SpaceInvadersGameState.Ready or SpaceInvadersGameState.GameOver)
                    {
                        engine.Start();
                    }
                    break;
                case Key.R:
                    if (!e.IsRepeat)
                    {
                        engine.Start();
                    }
                    break;
                default:
                    return;
            }
            e.Handled = true;
        }

        private void Playfield_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.Left or Key.A)
            {
                engine.Left = Keyboard.IsKeyDown(Key.Left) || Keyboard.IsKeyDown(Key.A);
            }
            if (e.Key is Key.Right or Key.D)
            {
                engine.Right = Keyboard.IsKeyDown(Key.Right) || Keyboard.IsKeyDown(Key.D);
            }
            if (e.Key == Key.Space)
            {
                engine.Fire = false;
            }
        }
    }
}
