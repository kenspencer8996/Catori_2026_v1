using CatoriCity2025WPF.Objects;
using CatoriCity2025WPF.Objects.Arguments;
using System.Reflection;
using System.Windows.Controls;

namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for ColorPickerUserUC.xaml
    /// </summary>
    public partial class ColorPickerUC : UserControl
    {
        public event EventHandler<ColorChangedArgs> OnColorChanged;
        private string _selectedColor;
        public ColorPickerUC()
        {
            InitializeComponent();
        }
        public string SelectedColor
        {
            get { return _selectedColor; }
        }
        public string ControlToChangeName { get; set; }
        private void cboColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string _selectedColor = (string)cboColors.SelectedValue;
           
            ColorChangedArgs changedArgs = new ColorChangedArgs(ControlToChangeName, _selectedColor);
            OnColorChanged?.Invoke(this, changedArgs);
        }
    }
}
