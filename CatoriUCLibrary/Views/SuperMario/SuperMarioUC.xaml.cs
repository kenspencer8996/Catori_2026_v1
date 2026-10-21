using System.Diagnostics;

using System.Windows;

using System.Windows.Input;

using System.Windows.Threading;

using CatoriUCLibrary.Views.SuperMario.GameObjects;

using System.Windows.Controls;

namespace CatoriUCLibrary.Views;

public partial class SuperMarioUC : UserControl
{

    private readonly Engine engine=new();

    private readonly Gamepad pad=new();

    private readonly Stopwatch clock=new();

    private readonly DispatcherTimer timer=new(){
Interval=TimeSpan.FromMilliseconds(16)}
;

    private Window? inputWindow;

    private double previous;

    private ushort previousButtons;

    private readonly HashSet<Key> keys=[];

    public SuperMarioUC()
    {

        InitializeComponent();
Playfield.Engine=engine;

        Loaded += OnLoaded;

        Unloaded += OnUnloaded;

        Playfield.MouseDown+=(_,_)=>{
Playfield.Focus();
if(engine.State==PlayState.Ready)engine.Start();
}
;

        timer.Tick+=Tick;

    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {

        inputWindow = Window.GetWindow(this);

        if (inputWindow != null)
        {

            inputWindow.PreviewKeyDown += OnWindowKeyDown;

            inputWindow.PreviewKeyUp += OnWindowKeyUp;

            inputWindow.Deactivated += OnDeactivated;

        }

        clock.Restart();

        previous = 0;

        timer.Start();

        Playfield.Focus();

    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {

        timer.Stop();

        clock.Stop();

        keys.Clear();

        if (inputWindow != null)
        {

            inputWindow.PreviewKeyDown -= OnWindowKeyDown;

            inputWindow.PreviewKeyUp -= OnWindowKeyUp;

            inputWindow.Deactivated -= OnDeactivated;

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

        keys.Remove(e.Key);

    }

    private void OnDeactivated(object? sender, EventArgs e)
    {

        keys.Clear();

        if (engine.State == PlayState.Playing)
        {

            engine.Pause();

        }

    }

    private bool Held(params Key[] choices)=>choices.Any(keys.Contains);

    private void OnKeyDown(object sender,KeyEventArgs e)
    {

        keys.Add(e.Key);

        if(!e.IsRepeat)
        {

            if(e.Key==Key.R)engine.Start();

            if(e.Key is Key.P or Key.Escape)engine.Pause();

            if(e.Key==Key.Enter){
if(engine.State is PlayState.Ready or PlayState.GameOver or PlayState.Won)engine.Start();
else if(engine.State==PlayState.Paused)engine.Pause();
}

        }

        if(e.Key is Key.Left or Key.Right or Key.A or Key.D or Key.Space or Key.Z or Key.Enter or Key.R or Key.P or Key.Escape)e.Handled=true;

    }

    private void Tick(object? sender,EventArgs args)
    {

        var now=clock.Elapsed.TotalSeconds;
var dt=now-previous;
previous=now;
pad.Poll();

        var pressed=(ushort)(pad.Current.Buttons & ~previousButtons);
previousButtons=pad.Current.Buttons;

        if(IsVisible && inputWindow?.IsActive == true)
        {

            if((pressed&0x20)!=0)engine.Start();

            else if((pressed&0x10)!=0){
if(engine.State is PlayState.Ready or PlayState.GameOver or PlayState.Won)engine.Start();
else engine.Pause();
}

            else if((pressed&0x1000)!=0&&engine.State is PlayState.Ready or PlayState.GameOver or PlayState.Won)engine.Start();

            engine.Update(dt,Held(Key.Left,Key.A)||pad.Left,Held(Key.Right,Key.D)||pad.Right,Held(Key.Space,Key.Z)||pad.Down(0x1000),Held(Key.LeftShift,Key.RightShift)||pad.Down(0x4000));

        }

        ControllerStatus.Text=pad.Connected?"Controller connected · D-pad / stick: move · A: jump · X: run · Start: pause · Back: restart":"Controller disconnected · Keyboard ready · Connect an Xbox-compatible controller anytime";

        Playfield.InvalidateVisual();

    }

}

