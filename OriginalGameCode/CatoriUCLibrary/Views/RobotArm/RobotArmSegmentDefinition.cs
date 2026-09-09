namespace CatoriUCLibrary.Views.RobotArm
{
    public class RobotArmSegmentDefinition
    {
        public string Name { get; set; } = string.Empty;
        public string ImagePath { get; set; } = "robotArmLongBlue.png";
        public double Width { get; set; } = 140;
        public double Height { get; set; } = 40;
        public double JointLength { get; set; } = 124;
        public double InitialAngle { get; set; } = -90;
    }
}
