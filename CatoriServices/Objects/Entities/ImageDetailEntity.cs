namespace CityAppServices.Objects.Entities
{
    public class ImageDetailEntity
    {
        public int ImageId { get; set; } = 0;
        public string FilePath { get; set; } = "";
        public string Name 
        {
            get
            {
                return System.IO.Path.GetFileNameWithoutExtension(FilePath);    
            }
            set
            {
                FilePath = System.IO.Path.Combine(GlobalServices.ImageFolder , value);
            }
        } 
        public string NamePart { get; set; } = string.Empty;
        public Int32 NumberPart { get; set; } = 0;
        public int SequenceNumber { get; set; } = 0;    
        public string TrailingPart { get; set; } = string.Empty;
        public ImageEnum ImageRole { get; set; }
        public ImageDetailEntity() { }

    }
}
