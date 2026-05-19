
using System.Drawing;
namespace CatoriApp.Views.Controls.Person
{
    
    /// <summary>
    /// Interaction logic for Pers_onShopperControl.xaml
    /// </summary>
    public partial class PersonShopperControl : UserControl
    {
        public PersonShopperControl()
        {
            InitializeComponent();
        }
      
        public PersonViewModel Model { get; internal set; }

        private void MainBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }
    }
}


