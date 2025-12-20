using CatoriCity2025WPF.Objects;

namespace CatoriServices.Objects.Entities
{
    public class LandscapeObjectEntity
    {
        public int LandScapeObjectID { get; set; } = 0;
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public StreetsEnum Street { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public double xActual { get; set; }
        public double yActual { get; set; }
        public string ImageName { get; set; } = "";
        public Int32 GroupId { get; set; }
        public bool HomeObject { get; set; } = false;
        public bool NextFromHomeObject { get; set; } = false;
        public string FeatureNote { get; set; } = "";
    }
}
