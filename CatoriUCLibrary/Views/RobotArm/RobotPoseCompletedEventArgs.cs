namespace CatoriUCLibrary.Views.RobotArm;

public sealed class RobotPoseCompletedEventArgs : EventArgs
{
    public RobotPoseCompletedEventArgs(string poseName, int poseIndex, int poseCount, RobotPose pose)
    {
        PoseName = poseName;
        PoseIndex = poseIndex;
        PoseCount = poseCount;
        Pose = pose;
    }

    public string PoseName { get; }
    public int PoseIndex { get; }
    public int PoseNumber => PoseIndex + 1;
    public int PoseCount { get; }
    public RobotPose Pose { get; }
}
