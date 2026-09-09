using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CatoriServices.Objects.Entities.House
{
    public class LotEntity
    {
        public string ContentName { get; set; } = "";
        public double x { get; set; }
        public double y { get; set; }
        public int LotNumber { get; set; }
        public StreetsEnum StreetName { get; set; } 
        public LotSizeEnum LotSize { get; set; }
        public BuildingFacingEnum FrontFace { get; set; }
        public double FrontFaceDegrees { get; set; } = 180;
        public LotPositionEnum lotPosition { get; set; }
    }
}



