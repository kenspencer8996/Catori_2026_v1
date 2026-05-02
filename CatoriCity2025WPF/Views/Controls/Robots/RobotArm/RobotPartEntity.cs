namespace CatoriCity2025WPF.Views.Controls.Robots.RobotArm.Robot
{
    public class RobotPartEntity
    {
        public RobotPartEntity()
        {
        }
        public RobotPartType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public RobotColorEnum Color { get; set; }

    }
}
