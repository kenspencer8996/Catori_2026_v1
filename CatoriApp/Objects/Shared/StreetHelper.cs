using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CatoriApp.Objects.Shared
{
    internal class StreetHelper
    {
        public static void SetGlobalStreetvalues(double canvaswidth, double youhgt, double mikhgt,
            double teahgt, double teawdth)
        {
            double widthModifier = 160;
            double mainwidth = CityScapeGlobal.CityScapeViewWidth;
            double mainheight = CityScapeGlobal.CityScapeViewHeight;
            double canvastopwstreet = 100 + youhgt;
            double streetwidthhalf = (CityScapeGlobal.StreetWidth / 2);
            GlobalGeo.YouStreetloc.LocationStartXY.x = 100;
            GlobalGeo.YouStreetloc.LocationStartXY.y = 100;
            GlobalGeo.YouStreetloc.LocationEndXY.x = canvaswidth - widthModifier;
            GlobalGeo.YouStreetloc.LocationEndXY.y = 100;

            GlobalGeo.MikAveLoc.LocationStartXY.x = 100;
            GlobalGeo.MikAveLoc.LocationStartXY.y = mainheight - (100 + mikhgt);
            GlobalGeo.MikAveLoc.LocationEndXY.x = canvaswidth - widthModifier;
            GlobalGeo.MikAveLoc.LocationEndXY.y = mainheight - (100 + mikhgt); ;

            GlobalGeo.YodelLaneStreetLoc.LocationStartXY.x = 100;
            GlobalGeo.YodelLaneStreetLoc.LocationStartXY.y = canvastopwstreet;
            GlobalGeo.YodelLaneStreetLoc.LocationEndXY.x = 100;
            GlobalGeo.YodelLaneStreetLoc.LocationEndXY.y = GlobalGeo.MikAveLoc.LocationStartXY.y; ;

            double moostartx = mainwidth * .53;

            GlobalGeo.MooDrLoc.LocationStartXY.x = moostartx;
            GlobalGeo.MooDrLoc.LocationStartXY.y = canvastopwstreet;
            GlobalGeo.MooDrLoc.LocationEndXY.x = moostartx;
            GlobalGeo.MooDrLoc.LocationEndXY.y = GlobalGeo.MikAveLoc.LocationStartXY.y; ;

            double teastartx = mainwidth - (100 + teawdth + 40);
            GlobalGeo.TeaStreetLoc.LocationStartXY.x = teastartx;
            GlobalGeo.TeaStreetLoc.LocationStartXY.y = 100;
            GlobalGeo.TeaStreetLoc.LocationEndXY.x = teastartx;
            GlobalGeo.TeaStreetLoc.LocationEndXY.y = GlobalGeo.MikAveLoc.LocationStartXY.y + youhgt;

            CityScapeGlobal.IntersectuonYouYodel.x = GlobalGeo.YouStreetloc.LocationStartXY.x + streetwidthhalf;
            CityScapeGlobal.IntersectuonYouTea.x = GlobalGeo.TeaStreetLoc.LocationEndXY.x + streetwidthhalf;
            CityScapeGlobal.IntersectuonYouMoo.x = GlobalGeo.MooDrLoc.LocationEndXY.x + streetwidthhalf;
            CityScapeGlobal.IntersectuonMikYodel.x = GlobalGeo.MikAveLoc.LocationEndXY.x + streetwidthhalf;
            CityScapeGlobal.IntersectuonMikMoo.x = GlobalGeo.MooDrLoc.LocationEndXY.x + streetwidthhalf;
            CityScapeGlobal.IntersectuonMikTea.x = GlobalGeo.TeaStreetLoc.LocationEndXY.x + streetwidthhalf;

            CityScapeGlobal.IntersectuonYouYodel.y = GlobalGeo.YouStreetloc.LocationEndXY.y + streetwidthhalf; ;
            CityScapeGlobal.IntersectuonYouTea.y = GlobalGeo.YouStreetloc.LocationEndXY.y + streetwidthhalf;
            CityScapeGlobal.IntersectuonYouMoo.y = GlobalGeo.YouStreetloc.LocationEndXY.y + streetwidthhalf;
            CityScapeGlobal.IntersectuonMikYodel.y = GlobalGeo.MikAveLoc.LocationEndXY.y + streetwidthhalf;
            CityScapeGlobal.IntersectuonMikMoo.y = GlobalGeo.MikAveLoc.LocationEndXY.y + streetwidthhalf;
            CityScapeGlobal.IntersectuonMikTea.y = GlobalGeo.MikAveLoc.LocationEndXY.y + streetwidthhalf;
        }
    }
}


