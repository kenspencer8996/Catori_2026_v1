using System;
using System.Collections.Generic;
using System.Text;

namespace CatoriApp.Game.Controllers.LocationSubControllers
{
    public class Locationinterior2SubController : LocationSubControllerBase, ILocationSubController
    {
         public Locationinterior2SubController(FactoryInterior_UC view,long locationid)
            : base(locationid, view)
        {
            base._view = view;
            _locationId = locationid;
            LoadLayoutItems();
        }
   

        protected override void OnAnimationCompleted(AnimationCompleteMessage message)
        {
            System.Diagnostics.Debug.WriteLine($"Animation completed: {message.AnimationName}");
        }

        public void StartProduction()
        {
            System.Diagnostics.Debug.WriteLine("Location 2 production has no configured animations.");
        }

        public void StopProduction()
        {
            System.Diagnostics.Debug.WriteLine("Location 2 production stopped.");
        }

        public void LoadPaths()
        {
            LoadLayoutItems().GetAwaiter().GetResult();
        }

        public void OnAnimationCompleted()
        {
            System.Diagnostics.Debug.WriteLine("Location 2 animation completed.");
        }
    }
}
