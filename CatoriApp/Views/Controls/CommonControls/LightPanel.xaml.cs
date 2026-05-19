using System.Windows.Threading;
namespace CatoriApp.Views.Controls.CommonControls
{
    /// <summary>
    /// Interaction logic for LightPanel.xaml
    /// </summary>
    public partial class LightPanel : UserControl
    {
        private readonly DispatcherTimer _flickerTimerLight1;
        private readonly DispatcherTimer _flickerTimerLight2;
        public event EventHandler PanelTriggered;
        private bool _isOn;
        List<Brush> _brushes;
        int iLight1 = 0;
        int iLight2 = 1;
        public LightPanel()
        {
            InitializeComponent();
            _flickerTimerLight1 = new DispatcherTimer();
            _flickerTimerLight1.Interval = TimeSpan.FromMilliseconds(50); // 20 Hz flicker
            _flickerTimerLight1.Tick += FlickerTick;
            _flickerTimerLight2 = new DispatcherTimer();
            _flickerTimerLight2.Interval = TimeSpan.FromMilliseconds(40); // 20 Hz flicker
            _flickerTimerLight2.Tick += FlickerTick2;
            _brushes = new List<Brush>();
            _brushes.Add(Brushes.Red);
            _brushes.Add(Brushes.Green);
            _brushes.Add(Brushes.Blue);
            _brushes.Add(Brushes.Yellow);
            _brushes.Add(Brushes.White);
            _brushes.Add(Brushes.LightCyan);

        }

        private void FlickerTick2(object? sender, EventArgs e)
        {
            _isOn = !_isOn;
            Brush thisColor;
            var onColor = _brushes[iLight2];
            var offColor = Brushes.Transparent;

            Light2.Fill = _isOn ? onColor : offColor;
            if (iLight2 < _brushes.Count - 1)
                iLight2++;
            else
                iLight2 = 0;
        }

        private void FlickerTick(object? sender, EventArgs e)
        {
            _isOn = !_isOn;

            var onColor = _brushes[iLight1]; 
            var offColor = Brushes.Transparent;

            Light1.Fill = _isOn ? onColor : offColor;

            if (iLight1 < _brushes.Count - 1)
                iLight1++;
            else
                iLight1 = 0;
        }

        public void StartFlicker()
        {
            _flickerTimerLight1.Start();
            _flickerTimerLight2.Start();
        }
        public void StopFlicker()
        {
            _flickerTimerLight1.Stop();
            _flickerTimerLight2.Stop();
            _isOn = false;

            Light1.Fill = Brushes.Transparent;
            Light2.Fill = Brushes.Transparent;
        }

        private void UserControl_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        }

        private void UserControl_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (PanelTriggered != null)
                PanelTriggered?.Invoke(this, EventArgs.Empty);
        }
    }

            
 }



