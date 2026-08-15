using CatoriApp.Core.Objects.Production;

namespace CatoriApp.Core.Objects.Arguments
{
    public class AnimationCompleteMessage
    {
        public string AnimationName { get; }
        public string PartName { get; init; }

        public string? TargetName { get; init; }
        public string? NextAction { get; init; }
        public ProductionAction Action { get; init; }
        public FrameworkElement? Part { get; init; }
        public AnimationCompleteMessage(string animationName,
            string partName,string targetName,string nextAction,FrameworkElement? part=null)
        {
            AnimationName = animationName;
            PartName = partName;
            TargetName = targetName;
            NextAction = nextAction;
            Action=Enum.TryParse<ProductionAction>(nextAction,true,out var action)?action:ProductionAction.None;
            Part=part;
        }

        public AnimationCompleteMessage(string animationName,string partName,string targetName,
            ProductionAction action,FrameworkElement? part=null)
            :this(animationName,partName,targetName,action==ProductionAction.None?string.Empty:action.ToString(),part) { }
    }
}
