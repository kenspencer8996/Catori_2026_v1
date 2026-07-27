using System;
using System.Collections.Generic;
using System.Text;

namespace CatoriApp.Game.Controllers.LocationSubControllers
{
    public class LocationConVeyorTable1SubController : LocationSubControllerBase, ILocationSubController
    {
        public LocationConVeyorTable1SubController(FactoryInterior_UC view)
        {
            this.view = view;
        }
        public void LoadPaths()
        {
           base.LoadPaths(_layoutitem);

        }

        protected  override  void OnAnimationCompleted(AnimationCompleteMessage message)
        {   
            
        }

        public void StartProduction()
        {
            
        }

        public void StopProduction()
        {
            
        }
    }
}
