using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CatoriCity2025WPF.Views.Controls.Robots.RobotArm
{
    /// <summary>
    /// Interaction logic for RobotTestView.xaml
    /// </summary>
    public partial class RobotTestView : Window
    {
        public RobotTestView()
        {
            InitializeComponent();
            Robotarm.SetupRobot(RobotColorEnum.Blue);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
