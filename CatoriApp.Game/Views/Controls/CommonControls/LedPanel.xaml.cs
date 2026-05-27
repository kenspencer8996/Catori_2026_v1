using System.Collections.ObjectModel;
using System.Windows.Threading;
namespace CatoriApp.Game.Views.Controls.CommonControls
{
    public partial class LedPanel : UserControl
    {
        private readonly DispatcherTimer _timer = new();
        private readonly Random _rng = new();
        private int _chaseIndex;
        public event EventHandler<EventArgs>? MouseDown;
        public LedPanel()
        {
            InitializeComponent();

            _timer.Tick += (_, __) => Tick();
            Loaded += (_, __) => RebuildLights();
            Unloaded += (_, __) => _timer.Stop();
        }
        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (MouseDown != null)
                MouseDown(sender, e);
        }
        // Exposed collection for ItemsControl binding
        public ObservableCollection<LedLight> Lights { get; } = new();

        #region Dependency Properties

        public static readonly DependencyProperty LightCountProperty =
            DependencyProperty.Register(nameof(LightCount), typeof(int), typeof(LedPanel),
                new PropertyMetadata(5, (_, __) => ((LedPanel)_).RebuildLights()));

        public int LightCount
        {
            get => (int)GetValue(LightCountProperty);
            set => SetValue(LightCountProperty, value);
        }

        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register(nameof(Interval), typeof(TimeSpan), typeof(LedPanel),
                new PropertyMetadata(TimeSpan.FromMilliseconds(400), (_, __) => ((LedPanel)_).ApplyTimer()));

        public TimeSpan Interval
        {
            get => (TimeSpan)GetValue(IntervalProperty);
            set => SetValue(IntervalProperty, value);
        }

        public static readonly DependencyProperty IsRunningProperty =
            DependencyProperty.Register(nameof(IsRunning), typeof(bool), typeof(LedPanel),
                new PropertyMetadata(true, (_, __) => ((LedPanel)_).ApplyTimer()));

        public bool IsRunning
        {
            get => (bool)GetValue(IsRunningProperty);
            set => SetValue(IsRunningProperty, value);
        }

        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register(nameof(Mode), typeof(BlinkMode), typeof(LedPanel),
                new PropertyMetadata(BlinkMode.Random, (_, __) => ((LedPanel)_).ResetState()));

        public BlinkMode Mode
        {
            get => (BlinkMode)GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        public static readonly DependencyProperty OnBrushProperty =
            DependencyProperty.Register(nameof(OnBrush), typeof(Brush), typeof(LedPanel),
                new PropertyMetadata(Brushes.LimeGreen, (_, __) => ((LedPanel)_).ApplyBrushes()));

        public Brush OnBrush
        {
            get => (Brush)GetValue(OnBrushProperty);
            set => SetValue(OnBrushProperty, value);
        }

        public static readonly DependencyProperty OffBrushProperty =
            DependencyProperty.Register(nameof(OffBrush), typeof(Brush), typeof(LedPanel),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(40, 40, 40)), (_, __) => ((LedPanel)_).ApplyBrushes()));

        public Brush OffBrush
        {
            get => (Brush)GetValue(OffBrushProperty);
            set => SetValue(OffBrushProperty, value);
        }

        #endregion

        private void RebuildLights()
        {
            Lights.Clear();
            var count = Math.Max(1, LightCount);

            for (int i = 0; i < count; i++)
            {
                Lights.Add(new LedLight
                {
                    IsOn = false,
                    OnBrush = OnBrush,
                    OffBrush = OffBrush
                });
            }

            ResetState();
            ApplyTimer();
        }

        private void ApplyBrushes()
        {
            foreach (var l in Lights)
            {
                l.OnBrush = OnBrush;
                l.OffBrush = OffBrush;
            }
        }

        private void ApplyTimer()
        {
            _timer.Interval = Interval <= TimeSpan.Zero ? TimeSpan.FromMilliseconds(250) : Interval;

            if (!IsLoaded)
                return;

            if (IsRunning)
                _timer.Start();
            else
                _timer.Stop();
        }

        private void ResetState()
        {
            _chaseIndex = 0;
            foreach (var l in Lights) l.IsOn = false;
        }

        private void Tick()
        {
            if (Lights.Count == 0) return;

            switch (Mode)
            {
                case BlinkMode.All:
                    {
                        bool anyOn = false;
                        for (int i = 0; i < Lights.Count; i++)
                            anyOn |= Lights[i].IsOn;

                        bool next = !anyOn; // toggle all
                        for (int i = 0; i < Lights.Count; i++)
                            Lights[i].IsOn = next;
                        break;
                    }

                case BlinkMode.Chase:
                    {
                        for (int i = 0; i < Lights.Count; i++)
                            Lights[i].IsOn = (i == _chaseIndex);

                        _chaseIndex = (_chaseIndex + 1) % Lights.Count;
                        break;
                    }

                case BlinkMode.Random:
                default:
                    {
                        // randomly toggle a subset
                        for (int i = 0; i < Lights.Count; i++)
                        {
                            // 30% chance to flip each light
                            if (_rng.NextDouble() < 0.30)
                                Lights[i].IsOn = !Lights[i].IsOn;
                        }
                        break;
                    }
            }
        }

      
    }
}


