namespace CatoriServices.Objects.Entities.City
{
    public class LoadStreetsEventArgs : EventArgs
    {
        public LoadStreetsEventArgs() 
        {
            try
            {
                        
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }
    }
}

