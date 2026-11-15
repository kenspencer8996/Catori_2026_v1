namespace CatoriUCLibrary.Views.Airliner3D
{
    public class KeyframeActionDefinition
    {
        public int Order { get; set; }
        public ActionEnum Action { get; set; }
        public   double Angle { get; set; }
        public double Scale { get; set; } = 1;
        public double Duration { get; set; }
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault)]
        public HeadingTurnDirection TurnDirection { get; set; } = HeadingTurnDirection.Shortest;
        public AirplaneCameraPreset CameraPreset { get; set; } = AirplaneCameraPreset.ThreeQuarter;
        public double ElevationDegrees { get; set; }
    }
}
