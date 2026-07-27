using System;
using System.Collections.Generic;
using System.Text;

namespace CatoriApp.Game.Controllers.LocationSubControllers
{
    public class LocationInterior1SubController : LocationSubControllerBase, ILocationSubController
    {
        public LocationInterior1SubController(FactoryInterior_UC view)
        {
            base.view = view;
        }
        public void LoadPaths()
        {
            base.LoadPaths
        }

        public void OnAnimationCompleted()
        {
            throw new NotImplementedException();
        }

        public void StartProduction()
        {
            throw new NotImplementedException();
        }

        public void StopProduction()
        {
            throw new NotImplementedException();
        }
    }
}
