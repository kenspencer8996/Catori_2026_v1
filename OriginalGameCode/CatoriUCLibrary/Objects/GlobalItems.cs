namespace CatoriUCLibrary.Views.RobotArm
{
    public class GlobalItems
    {
        public static LayoutDesignerSettings AppSettings { get; set; } = new();
    }

    public class LayoutDesignerSettings
    {
        public string LastFileName { get; set; } = "";
        public string LastItemType { get; set; } = "";
    }
}
