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
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace CatoriApp.Views.Controls.CommonControls
{
    /// <summary>
    /// Interaction logic for CabinetCornerControl.xaml
    /// </summary>
    public partial class CabinetCornerControl : UserControl
    {
        public CabinetCornerControl()
        {
            InitializeComponent();
        }

        private void Canvas_MouseEnter(object sender, MouseEventArgs e)
        {
            cLogger.Log(" CabinetCornerControl Canvas_MouseEnter ");
        }

        private void Canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            cLogger.Log(" CabinetCornerControl Canvas_MouseLeave ");
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            cLogger.Log(" CabinetCornerControl Canvas_MouseUp ");

        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            cLogger.Log(" CabinetCornerControl UserControl_MouseEnter ");

        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            cLogger.Log(" CabinetCornerControl UserControl_MouseLeave ");

        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            cLogger.Log(" CabinetCornerControl UserControl_MouseUp ");

        }
    }
}


