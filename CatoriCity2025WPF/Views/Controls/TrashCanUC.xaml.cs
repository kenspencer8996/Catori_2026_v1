using CatoriCity2025WPF.Objects.Arguments;
using System.Windows;
using System.Windows.Controls;

namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for TrashCanUC.xaml
    /// </summary>
    public partial class TrashCanUC : UserControl
    {
        public event EventHandler<DeleteLandscapeArg> OnDeleteUC;

        public TrashCanUC()
        {
            InitializeComponent();
        }

        private void UserControl_Drop(object sender, DragEventArgs e)
        {
            LandscapeObjectControl landscapeObjectControl = (LandscapeObjectControl)sender;
            DeleteLandscapeArg deleteUC = new DeleteLandscapeArg(landscapeObjectControl);
            if (OnDeleteUC != null)
                OnDeleteUC(this, deleteUC);
        }

        private void UserControl_DragEnter(object sender, DragEventArgs e)
        {

        }
    }
}
