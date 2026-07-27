using CatoriApp.MachineLayoutDesigner.ViewModels.Robots;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatoriApp.Game.Controllers.LocationSubControllers
{
    public class LocationSubControllerBase 
    {
        public FactoryInterior_UC view;
        public LocationService locationService = new LocationService();
        public LocationLayoutItemService _layoutItemService = new LocationLayoutItemService();
        public List<RobotPoseViewModel> robotposes { get; private set; }
        public MachineLayoutDesignerViewModel machineLayoutDesignermodel;
        public RobotPoseService _poseservice = new RobotPoseService();
        public readonly MachineLayoutDesignerService _robotdesignservice = new MachineLayoutDesignerService();
        public long _locationId;
        public LocationSubControllerBase(long locationId, FactoryInterior_UC view)
        {
            _locationId = locationId;
            this.view = view;
            {
                WeakReferenceMessenger.Default.Register<
                    AnimationCompleteMessage>(
                    this,HandleAnimationCompleted);
            }
        }

        private void HandleAnimationCompleted(
            object recipient,AnimationCompleteMessage message)
        {
            OnAnimationCompleted(message);
        }

        protected virtual void OnAnimationCompleted(
            AnimationCompleteMessage message)
        {
        }

        public virtual void Dispose()
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }
      
    }
}
