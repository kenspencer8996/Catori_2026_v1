using System;
using System.Collections.Generic;
using System.Text;

namespace CatoriApp.Game.Controllers.LocationSubControllers
{
    public interface ILocationSubController
    {
        public abstract void StartProduction();
        public abstract void StopProduction();
        public abstract void LoadPaths();
        protected void OnAnimationCompleted();

    }
}
