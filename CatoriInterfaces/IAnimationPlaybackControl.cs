namespace CatoriInterfaces;

public interface IAnimationPlaybackControl
{
    void PauseAnimations();
    void ResumeAnimations();
    void AdvanceAnimations(TimeSpan elapsed);
}
