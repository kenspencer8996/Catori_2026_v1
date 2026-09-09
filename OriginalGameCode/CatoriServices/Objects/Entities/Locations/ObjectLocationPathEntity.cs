using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CatoriServices.Objects.Entities.Locations
{
    public class ObjectLocationPathEntity
    {
        public List<LocationEnum> APath { get; set; } = new List<LocationEnum>();
        public void Add(LocationEnum aPath)
        {
            try
            {
                             APath.Add(aPath);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public void Clear() {
            try
            {
                        APath = new List<LocationEnum>();
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }
    }
}

