using System;
using System.Collections.Generic;
using System.Text;

namespace CatoriApp.Game.Controllers.LocationSubControllers
{
    public class Locationinterior2SubController : LocationSubControllerBase, ILocationSubController
    {
        long _locationid;
        public Locationinterior2SubController(FactoryInterior_UC view,long locationid) 
        {
            base.view = view;
            _locationid = locationid;
            LoadPaths();
        }
        public void LoadPaths()
        {
            
        }

        protected override void OnAnimationCompleted(AnimationCompleteMessage message)
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

        public void OnAnimationCompleted()
        {
            throw new NotImplementedException();
        }
    }
}
