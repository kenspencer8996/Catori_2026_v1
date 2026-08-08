using System.Drawing;

namespace CatoriApp.Core.Objects.Arguments
{
    public class AnimationCompleteMessage
    {
        public string AnimationName { get; }
        public string PartName { get; init; }

        public string? TargetName { get; init; }
        public string? NextAction { get; init; }
        public AnimationCompleteMessage(string animationName,
            string partName,string targetName,string nextAction)
        {
            AnimationName = animationName;
            PartName = partName;
            TargetName = targetName;
            NextAction = nextAction;
        }
    }
}
