using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace CatoriCity2025WPF.Views.Controls.Robots.RobotArm.Robot
{
   public class FactoryPart
    {
        public FrameworkElement Visual { get; set; }
        public string PartCode { get; set; }
        public bool IsVisible
        {
            get => Visual.Visibility == Visibility.Visible;
            set => Visual.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
