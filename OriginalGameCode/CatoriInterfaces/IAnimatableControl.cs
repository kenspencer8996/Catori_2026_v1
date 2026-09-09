namespace CatoriInterfaces
{
    public interface IAnimatableControl
    {
        void AnimationStarted();
        void AnimationCompleted();
        string ControlImagePath { get; set; }
    }
}
