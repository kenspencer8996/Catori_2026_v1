using CatoriApp.Game.Controllers;
using CatoriApp.Core.Objects;
using CatoriServices.Objects;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows.Controls;
namespace CatoriApp.Game.Views.Controls.Police
{
    /// <summary>
    /// Interaction logic for CarPo_ice.xaml
    /// </summary>
    public partial class CarPoliceOffRoadControl : UserControl
    {
        RobberyMessage _robberyMessage;
        CarPoliceOffRoadControlController _controller;
        public CarPoliceOffRoadControl()
        {
            InitializeComponent();
            double x = Canvas.GetLeft(this);
            double y = Canvas.GetTop(this);

            _controller = new CarPoliceOffRoadControlController(this);
            WeakReferenceMessenger.Default.Register<RobberyMessage>(this, (r, m) =>
            {
                cLogger.Log(this.Name + " WeakReferenceMessenger called : " + " ");
                _robberyMessage = m;
            });
        }

        public string RobberName { get; internal set; }

        internal void ChangeDirection(PositionsEWNSEnum north)
        {
        }

        internal void FireMoveCarEvent(BusinessControlAndLocXYEntity businessloc)
        {
        }
    }
}



