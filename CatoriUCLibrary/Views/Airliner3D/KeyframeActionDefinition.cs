namespace CatoriUCLibrary.Views.Airliner3D
{
    public class KeyframeActionDefinition
    {
        public int Order { get; set; }
        public ActionEnum Action { get; set; }
        public   double Angle { get; set; }
        public double Scale { get; set; } = 1;
        public double Duration { get; set; }
    }
}
