using CatoriCity2025WPF.ViewModels;
using CatoriCity2025WPF.Views.Controls;
using CatoriServices.Objects;
using CatoriServices.Objects.Entities;
using System.Globalization;
using System.Windows.Controls;

namespace CatoriCity2025WPF.Objects
{
    public class GeometryHelper
    {
        private List<string> PathGeometries = new List<string>();

        public GeometryHelper()
        {
            LoadGeometries();
        }
        public string GetRandomGeometry()
        {
            Random random = new Random();
            int index = random.Next(PathGeometries.Count);
            return PathGeometries[index];
        }
        private void LoadGeometries()
        {
            PathGeometries.Add("Curve");
            PathGeometries.Add("Line");
        }
        internal int GetRandomIntAtOrBelowMax(int max)
        {
            Random random = new Random();
            int minValue = 1; // Minimum value for the random number

            int randomNumber = random.Next(minValue, max); // minValue is 
            return randomNumber;
        }
        private int GetNumberOfPathStops()
        {
            int totalcount = GlobalStuff.LandscapeObjects.Count;
            Random random = new Random();
            int minValue = 3; // Minimum value for the random number
            if (totalcount <= 3)
            {
                minValue = 1;
            }
            int maxValue = totalcount; // Maximum value for the random number
            int randomNumber = random.Next(minValue, maxValue); // minValue is 
            return randomNumber;
        }
        public LandScapeWithBank_ReturnEntity GetShuffledDeckOfLandscapeObjects()
        {
            SethomeObject();
            SetNextObject();
            int top = GetNumberOfPathStops();
            top = 0;
            List<LandscapeObjectViewModel> selectedlist = new List<LandscapeObjectViewModel>();
            if (top == 0)
                selectedlist = GlobalStuff.LandscapeObjects.ToList();
            else
                selectedlist = GlobalStuff.LandscapeObjects.Take(top).ToList();

            LandScapeWithBank_ReturnEntity returnEntity = CreateShuffledDeck(selectedlist);

            return returnEntity;
        }

        private void SethomeObject()
        {
            for (int i = GlobalStuff.LandscapeObjects.Count() - 1; i > -1; i--)
            {
                try
                {
                    LandscapeObjectViewModel model = GlobalStuff.LandscapeObjects[i];
                    if (model.HomeObject == true)
                    {
                        GlobalStuff.HomeLandscapeObject = model;
                        //GlobalStuff.LandscapeObjects.RemoveAt(i);
                        break;
                    }

                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
        private void SetNextObject()
        {
            for (int i = GlobalStuff.LandscapeObjects.Count() - 1; i > -1; i--)
            {
                LandscapeObjectViewModel model = GlobalStuff.LandscapeObjects[i];
                if (model.NextFromHomeObject == true)
                {
                    GlobalStuff.NextFromHomeObject = model;
                    //GlobalStuff.LandscapeObjects.RemoveAt(i);
                    break;
                }
            }
        }
        private LandScapeWithBank_ReturnEntity CreateShuffledDeck(List<LandscapeObjectViewModel> items)
        {
            List<LandscapeObjectViewModel> selectedojects = items;
            var random = new Random();
            Stack<LandscapeObjectViewModel> stack = new Stack<LandscapeObjectViewModel>();
            List<LandscapeObjectViewModel> financials = new List<LandscapeObjectViewModel>();
            financials = GetFinancialLocations();
            int financialCount = financials.Count();
            LandscapeObjectViewModel financial;
            if (financialCount > 1)
            {
                int randomIndex = random.Next(0, financialCount);
                financial = financials[randomIndex];
            }
            else
                financial = financials[0];
            double startUCx = financial.xActual;
            double startUCy = financial.yActual;
            string info = "CreateShuffledDeck bank (UC) name " + financial.Name + " x " + startUCx + " y " + startUCy;
            cLogger.Log(info);

            stack.Push(financial); // Push the financial location to the top of the stack.
            while (selectedojects.Count() > 0)
            {  // Get the next item at random.
                var randomIndex = random.Next(0, selectedojects.Count);
                var randomItem = selectedojects[randomIndex];
                // Remove the item from the list and push it to the top of the deck.
                randomItem.SetCenter();
                selectedojects.RemoveAt(randomIndex);
                if (randomItem.xActual > 100 && randomItem.yActual > 100)
                    stack.Push(randomItem);
            }
            string infoheader = "------ stack items -----"; ;
            cLogger.Log(infoheader);
            foreach (var item in stack)
            {
                string infoItem = "  item name " + item.Name + " x " + item.xActual + " y " + item.yActual;
                cLogger.Log(infoItem);
            }
            string infof = "  count " + stack.Count() + " items in stack after shuffling.";
            cLogger.Log(infof);      
            infoheader = "------ end -----"; ;
            cLogger.Log(infoheader);

            LandScapeWithBank_ReturnEntity returnEntity = new LandScapeWithBank_ReturnEntity();
            returnEntity.LandscapeObjectsStack = stack;
            returnEntity.financial = financial;
            return returnEntity;
        }

        private List<LandscapeObjectViewModel> GetFinancialLocations()
        {
            List<LandscapeObjectViewModel> results = new List<LandscapeObjectViewModel>();
            int i = 0;
            foreach (LotControl item in GlobalStuff.FinancialLotCobtrols)
            {
                LandscapeObjectEntity entity = new LandscapeObjectEntity();
                entity.Name = "Financial " + i;
                LandscapeObjectViewModel model = new LandscapeObjectViewModel();
                model.EntityToModel(entity);
                model.Entity.Street = item.Street;
                model.xActual = Canvas.GetLeft(item);
                model.yActual = Canvas.GetTop(item);
                results.Add(model);
                i++;
            }
            return results;
        }

        internal Stack<LandscapeObjectViewModel> SetupGeometryForPath(
            Stack<LandscapeObjectViewModel> LandscapeObjectsForPathToBank, Int32 x, Int32 y)
        {
            //     LandscapeObjectsForPathToFinance.Push(GlobalStuff.HomeLandscapeObject);
            Stack<LandscapeObjectViewModel> results = new Stack<LandscapeObjectViewModel>();

            string info = "-------------------- SetupGeometryForPath ------------------------------------------------------------" + Environment.NewLine;
            info += " start approach  " + " x " + x + " y " + y + Environment.NewLine;
            info += " total LandscapeObjectsForPathToFinance " + LandscapeObjectsForPathToBank.Count();
            cLogger.Log(info);
            int i = 1;
            LandscapeObjectViewModel lastItem = null;
            while (LandscapeObjectsForPathToBank.Count > 0)
            // for (int i = 0; i < (LandscapeObjectsForPathToFinance.Count()); i++)
            {
                try
                {

                    string infoinsideloop = "===========================================" + Environment.NewLine;
                    cLogger.Log(infoinsideloop);
                    LandscapeObjectViewModel item = LandscapeObjectsForPathToBank.Pop();
                    lastItem = item;
                    LandscapeObjectViewModel nextmodek = null;
                    bool nextok = false;
                    try
                    {
                        nextmodek = LandscapeObjectsForPathToBank.Peek();
                        item.NextLandscapeObjectName = nextmodek.Name;
                        nextok = true;
                    }
                    catch (Exception exinner)
                    {
                        infoinsideloop += " exception 1st exception" + exinner.Message ;
                        cLogger.Log(infoinsideloop);
                        nextok = false;
                    }
                    var realucs = from uc in GlobalStuff.LandscapeUCs
                                  where uc.Name == item.Name
                                  select uc;
                    var realnextucs = from uc in GlobalStuff.LandscapeUCs
                                      where uc.Name == nextmodek.Name
                                      select uc;
                    bool oktouseitem = DetermineIfItemValid(realucs.FirstOrDefault());
                    if (oktouseitem ==false) 
                    {
                        realucs = new List<LandscapeObjectControl>();
                    }
                    LandscapeObjectControl realitemControl = new LandscapeObjectControl();
                    LandscapeObjectControl realnextitemControl = new LandscapeObjectControl();
                    if (realucs.Any())
                    {
                        realitemControl = realucs.FirstOrDefault();
                    }
                    if (realucs.Any())
                    {
                        realnextitemControl = realucs.FirstOrDefault();
                    }

                    if (nextok)
                    {
                        info = "    pop name " + item.Name + " next item " + " x " + item.xActual + " y " + item.yActual;
                        info += "   peek next " + nextmodek.Name + " x " + nextmodek.xActual + " y " + nextmodek.yActual;
                        cLogger.Log(info);
                        double itmwidth = item.Width;
                        double itmheight = item.Height;
                        double nexwidth = nextmodek.Width;
                        double nexheight = nextmodek.Height;
                        if (nexwidth < 1)
                            nexwidth = 100;
                        if (nexheight < 1)
                            nexheight = 100;
                        if (itmheight < 1)
                            itmheight = 100;
                        if (itmwidth < 1)
                            itmwidth = 100;
                        double widthhalf = itmwidth / 2;
                        double heighthalf = itmheight / 2;
                        item.xActual = GetValidDouble(Canvas.GetLeft(realitemControl) + itmwidth);
                        item.yActual = GetValidDouble(Canvas.GetTop(realitemControl) + itmheight);
                        item.xActual = GetValidDouble(Canvas.GetLeft(realnextitemControl));
                        item.yActual = GetValidDouble(Canvas.GetTop(realnextitemControl));
                        double xend = nextmodek.xActual + nexwidth / 2;
                        double yend = nextmodek.yActual + nexheight / 2;
                        double xstart = item.xActual + itmwidth / 2; ;
                        double ystart = item.yActual + itmheight / 2;

                        double xdistance = 0;
                        double ydistance = 0;
                        if (xend > xstart)
                            xdistance = xend - xstart;
                        else if (xend < xstart)
                            xdistance = xstart - xend;
                        if (yend > ystart)
                            ydistance = yend - ystart;
                        else if (xend < xstart)
                            xdistance = ystart - yend;

                        int numberOfStops = GetNumberOfPathStops();
                        if (GlobalStuff.PathSegmentsInnerToCreate > 0)
                            numberOfStops = GlobalStuff.PathSegmentsInnerToCreate;
                        if (numberOfStops < 1)
                            numberOfStops = 1; // Ensure at least one segment is created.
                        double xstep = xdistance / numberOfStops;
                        double ystep = ydistance / numberOfStops;
                        double xstartStep = xstart;
                        double ystartStep = ystart;
                        double xendStep = 0;
                        double yendStep = 0;
                        string info3 = "   (UC) numberOfStops " + numberOfStops + " xstart " + xstart + "(UC Next) xe " + xend + " (UC) ystart " + ystart + " (UC Next) yend " + yend;
                        cLogger.Log(info3);
                        for (int ii = 0; ii < numberOfStops; ii++)
                        {
                            try
                            {
                                PathPositionModel pathPosition = new PathPositionModel();
                                pathPosition.GeometryType = GetRandomGeometry();
                                pathPosition.startx = xstartStep;
                                pathPosition.starty = ystart;
                                if (ii == numberOfStops - 1)
                                {
                                    xendStep = xend;
                                    yendStep = yend;
                                }
                                else
                                {
                                    if (xend > xstart)
                                    {
                                        xstartStep += xstep;
                                        xendStep = xstartStep + xstep;
                                    }
                                    else if (xend < xstart)
                                    {

                                        {
                                            xstartStep -= xstep;
                                            xendStep = xstartStep - xstep;
                                        }
                                    }
                                    if (yend > ystart)
                                    {
                                        ystartStep += ystep;
                                        yendStep = ystartStep + ystep;
                                    }
                                    else if (yend < ystart)
                                    {
                                        ystartStep -= ystep;
                                        yendStep = ystartStep - ystep;
                                    }
                                }
                            
                            pathPosition.endx = xendStep;
                            pathPosition.endy = yendStep;
                            pathPosition.SetCenter();
                            xstartStep = pathPosition.endx;
                            ystartStep = pathPosition.endy;
                            item.PathPositions.Push(pathPosition);
                            info = "     position segment strtx " + pathPosition.startx;
                            info += " strty " + pathPosition.starty + " endx " + pathPosition.endx + " endy  " + pathPosition.endy;
                            cLogger.Log(info);
                                }
                            catch (Exception ex)
                            {

                                throw;
                            }
                        }
                    }
                    results.Push(item);
                }
                catch (Exception ex)
                {
                    throw new Exception("SetupGeometryForPath: Error setting up geometry for path: " + ex.Message);
                }

                string infoinside = "===========================================";
                cLogger.Log(infoinside);

                i++;
            }

            info = " total results " + results.Count();
            cLogger.Log(info);
            string info2 = "-------------------** end SetupGeometryForPath **-------------------------------------------------------------" + Environment.NewLine;
            cLogger.Log(info);

            return results;
        }

        private bool DetermineIfItemValid(LandscapeObjectControl control)
        {
            bool result = true;
            string[] invalidutems = { "tent", "tractor" };
            if (control == null)
                return false;
            foreach (var item in invalidutems)
            {
                if (control.Name.ToLower().Contains(item.ToLower()))
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        private double GetValidDouble(double value)
        {
            if (double.IsNaN(value))
                value = 0;

            return value;
        }
    }
}

