using System;
using System.Collections.Generic;
using System.Text;

namespace CatoriApp.Game.Controllers.LocationSubControllers
{
    public class LocationConVeyorTable1SubController : LocationSubControllerBase, ILocationSubController
    {
        public LocationConVeyorTable1SubController(FactoryInterior_UC view,long locationId) : base(locationId, view)
        {
            this._view = view;
            LoadPaths();
        }
        public void LoadPaths()
        {
           LoadPaths();

        }

        protected  override  void OnAnimationCompleted(AnimationCompleteMessage message)
        {   
            
        }

        public void OnAnimationCompleted()
        {
            // Implement interface method
        }

        public void StartProduction()
        {
            
        }

        public void StopProduction()
        {
            
        }
    }
}
