using System;
using System.Collections.Generic;
using System.Text;

namespace CatoriShared.AnimationHelpers
{
    public class PathAnimationOptions
    {
        public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(5);
        public RepeatBehavior RepeatBehavior { get; set; } = new RepeatBehavior(1);
        public bool AutoReverse { get; set; }
        public bool AutoStart { get; set; } = true;
        /// <summary>
        /// Animates the supplied control in place instead of reparenting it into
        /// a ContentControl host. Use this for controls already on the canvas.
        /// </summary>
        public bool AnimateExistingControl { get; set; }
        public TimeSpan StartDelay { get; set; }
        /// <summary>
        /// Keeps the part at the final path point until a connected path or robot
        /// takes ownership of it.
        /// </summary>
        public bool HoldAtEndUntilHandoff { get; set; } = true;
        public bool RotateWithPath { get; set; } = true;
        public bool CenterOnPath { get; set; } = true;
        /// <summary>
        /// Optional normalized point on an existing control that follows the path.
        /// For example, (0.5, 1) keeps the bottom-center on a ground path.
        /// </summary>
        public Point? ControlAnchorOnPath { get; set; }
        public bool IsHitTestVisible { get; set; }
        public double Opacity { get; set; } = 1;
        public double SpeedRatio { get; set; } = 1;
        public double AccelerationRatio { get; set; }
        public double DecelerationRatio { get; set; }
        public double? ControlWidth { get; set; }
        public double? ControlHeight { get; set; }
        public int ZIndex { get; set; } = 2002;
        public Point RenderTransformOrigin { get; set; } = new(0.5, 0.5);
        public object? Tag { get; set; }
        public Action<FrameworkElement>? Started { get; set; }
        public Action<FrameworkElement>? Completed { get; set; }
        /// <summary>
        /// Displays the animation geometry. Intended for design/debug views; game paths are hidden by default.
        /// </summary>
        public bool ShowPath { get; set; }
        /// <summary>
        /// Initial scale of the control.
        /// 1.0 = normal size.
        /// 0 = disabled.
        /// 0.25 = starts at 25% and grows to 100%.
        /// </summary>
        public double InitialScale { get; set; } = 0;
    }
}
