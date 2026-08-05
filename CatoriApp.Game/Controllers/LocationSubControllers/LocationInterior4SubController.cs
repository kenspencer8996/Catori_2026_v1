using System;
using System.Collections.Generic;
using System.Text;

namespace CatoriApp.Game.Controllers.LocationSubControllers
{
    public class LocationInterior4SubController : LocationSubControllerBase
    {
        public LocationInterior4SubController(long locationId, FactoryInterior_UC view) 
            : base(locationId, view)
        {
            base._view = view;
            _locationId = locationId;
            LoadLayoutItems();

        }
    }
}
