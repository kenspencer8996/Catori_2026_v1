using System;
using System.Windows;
using System.Windows.Media.Animation;

using System.Windows;
using System.Windows.Media.Animation;

namespace CatoriCity2025WPF.Views.Controls.Robots.RobotArm
{
    class Animaterobot
    {
        public void AnimateToPose(RobotArmControl arm,
        double a1, double a2, double a3, double a4,
        double seconds = 0.5)
    {
        var storyboard = new Storyboard();
        var duration = TimeSpan.FromSeconds(seconds);

        //AddAngleAnimation(storyboard, arm, nameof(RobotArm.Joint1Angle), a1, duration);
        //AddAngleAnimation(storyboard, arm, nameof(RobotArm.Joint2Angle), a2, duration);
        //AddAngleAnimation(storyboard, arm, nameof(RobotArm.Joint3Angle), a3, duration);
        //AddAngleAnimation(storyboard, arm, nameof(RobotArm.Joint4Angle), a4, duration);

        storyboard.Begin();
    }

    private void AddAngleAnimation(Storyboard sb, DependencyObject target, string propertyName, double to, TimeSpan duration)
    {
        var anim = new DoubleAnimation
        {
            To = to,
            Duration = duration
        };

        Storyboard.SetTarget(anim, target);
        Storyboard.SetTargetProperty(anim, new PropertyPath(propertyName));
        sb.Children.Add(anim);
    }
}
}
