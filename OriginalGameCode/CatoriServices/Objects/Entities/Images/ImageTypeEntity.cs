using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CatoriServices.Objects.Entities.Images
{
    public class ImageTypeEntity
    {
        public ImageContentEnum Imagetype { get; set; }

        public string Name { get; set; }
        public string imagesource
        {
            get
            {
                return Name.Replace(".svg", ".jpg");
            }
        }
        public string lastpart { get; set; }
        public int number { get; set; }
    }
}

