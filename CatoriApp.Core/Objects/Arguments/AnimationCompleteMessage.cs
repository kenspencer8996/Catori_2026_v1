using System;
using System.Collections.Generic;
using System.Text;

namespace CatoriApp.Core.Objects.Arguments
{
    public class AnimationCompleteMessage
    {
        public string AnimationName { get; }
        public string LocationName { get; }

        public AnimationCompleteMessage(
            string animationName,
            string locationName)
        {
            AnimationName = animationName;
            LocationName = locationName;
        }
    }
}
