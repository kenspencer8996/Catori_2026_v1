namespace CatoriApp.Core.Objects.Arguments
{
    public class AnimationCompleteMessage
    {
        public string AnimationName { get; }

        public AnimationCompleteMessage(string animationName)
        {
            AnimationName = animationName;
        }
    }
}
