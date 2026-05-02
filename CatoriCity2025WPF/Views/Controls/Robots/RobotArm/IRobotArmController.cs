using System.Drawing;

namespace CatoriCity2025WPF.Views.Controls.Robots.RobotArm.Robot
{
    public interface IRobotArmController
    {
        Task MoveToPointAsync();
        Task CloseGripperAsync();
        Task OpenGripperAsync();
        Task MoveHomeAsync();
    }
}
