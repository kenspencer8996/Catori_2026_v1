namespace CatoriApp.Game.Controllers.LocationSubControllers
{
    public sealed class PassiveLocationSubController : LocationSubControllerBase, ILocationSubController
    {
        public PassiveLocationSubController(Location_UC view, long locationId)
            : base(locationId, view)
        {
        }

        public void StartProduction()
        {
        }

        public void StopProduction()
        {
        }

        public void LoadPaths()
        {
        }

        public void OnAnimationCompleted()
        {
        }
    }
}
