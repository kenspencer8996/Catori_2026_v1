using CityAppServices.Objects;
using CityAppServices.Objects.Entities;

namespace CatoriCity2025WPF.Objects
{
    public class GlobalGeo
    {
        public static StreetsModel Streets = new StreetsModel();  
        public static StreetsEnum HouseStreets = StreetsEnum.YouStreet;
        public static StreetsEnum FinancialStreets = StreetsEnum.YodelLane;
        public static StreetsEnum FactoryStreets = StreetsEnum.Teastreet;
        public static StreetsEnum GovStreets = StreetsEnum.MikAve;
        public static StreetsEnum PoliceStreets = StreetsEnum.YouStreet;
        private static LotEntity _policeStationLocation = new LotEntity();
        public static LotEntity PoliceStationLocationEntity
        {
            get
            {
                return _policeStationLocation;
            }
            set
            {
                _policeStationLocation = value;
            }
        }
        public static int LotSize = 100;
        public static Int32 LandscapeObjecGroupid = 1;

        public static BusinessControlAndLocXYEntity YouStreetloc = new BusinessControlAndLocXYEntity();
        public static BusinessControlAndLocXYEntity MooDrLoc = new BusinessControlAndLocXYEntity();
        public static BusinessControlAndLocXYEntity YodelLaneStreetLoc = new BusinessControlAndLocXYEntity();
        public static BusinessControlAndLocXYEntity TeaStreetLoc = new BusinessControlAndLocXYEntity();
        public static BusinessControlAndLocXYEntity MikAveLoc = new BusinessControlAndLocXYEntity();
        public static int StLengthEW
        {
            get
            {
                return (int)(YouStreetloc.LocationEndXY.x - YouStreetloc.LocationStartXY.x);
            }
        }
       
        public static int StLengthNS
        {
            get
            {
                return (int)(MooDrLoc.LocationEndXY.y - MooDrLoc.LocationStartXY.y) + 100;
            }
        }
       

    }
}
