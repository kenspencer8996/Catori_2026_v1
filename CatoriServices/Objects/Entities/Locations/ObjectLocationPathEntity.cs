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
             APath.Add(aPath);
        }

        public void Clear() { 
        APath = new List<LocationEnum>();
        }
    }
}

