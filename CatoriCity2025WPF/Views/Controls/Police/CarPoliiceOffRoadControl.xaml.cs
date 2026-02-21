using CatoriCity2025WPF.Controllers;
using CatoriCity2025WPF.Objects;
using CatoriServices.Objects;
using CityAppServices.Objects.Entities;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows.Controls;

namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for CarPo_ice.xaml
    /// </summary>
    public partial class CarPoliiceOffRoadControl : UserControl
    {
        RobberyMessage _robberyMessage;
        CarPoliiceOffRoadControlController _controller;
        public CarPoliiceOffRoadControl()
        {
            InitializeComponent();
            double x = Canvas.GetLeft(this);
            double y = Canvas.GetTop(this);

            _controller = new CarPoliiceOffRoadControlController(this);
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
