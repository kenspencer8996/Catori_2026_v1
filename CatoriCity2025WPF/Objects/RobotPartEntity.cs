using CatoriUCLibrary;

namespace CatoriApp.Objects
{
    public class RobotPartEntity
    {
        public RobotPartType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public RobotColorEnum Color { get; set; }
    }
}

