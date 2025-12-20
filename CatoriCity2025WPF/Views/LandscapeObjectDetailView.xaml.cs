using CatoriCity2025WPF.Objects;
using CatoriCity2025WPF.ViewModels;
using Microsoft.Win32;
using System.Windows;

namespace CatoriCity2025WPF.Views
{
    /// <summary>
    /// Interaction logic for LandscapeObjectDetailView.xaml
    /// </summary>
    public partial class LandscapeObjectDetailView : Window
    {
        LandscapeObjectViewModel _model;
        public LandscapeObjectDetailView(LandscapeObjectViewModel model)
        {
            InitializeComponent();
            _model = model;
            this.DataContext = _model;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            CheckValidName();
            Hide();
        }

        private void CheckValidName()
        {
            string result = "";
            string testname = _model.Name;
            int counter = 1;
            do
            {
               var found = from l in GlobalStuff.LandscapeObjects where l.Name == testname select l;
               if (found.Count() == 0)
                {
                    result = testname;
                }
                else
                {
                    counter++;
                   // name already exists, append a number to make it unique
                    testname  += counter;
                    result = testname;
                }

            }
            while (string.IsNullOrEmpty(result));
            _model.Name = result;
        }
        private void GetImageNamButton_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select a File",
                Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg|All Files (*.*)|*.*",
                InitialDirectory = GlobalStuff.ImageFolder,
                Multiselect = false // Set to true if you want to allow multiple file selection
            };

            // Show dialog and check if the user selected a file
            if (openFileDialog.ShowDialog() == true)
            {
                _model.ImageName = openFileDialog.FileName;
            }
        }



        private void NameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void DescriptionTextbox_GotFocus(object sender, RoutedEventArgs e)
        {
            CheckValidName();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            _model.DeleteModel = true;
            Hide();
        }
    }
}
