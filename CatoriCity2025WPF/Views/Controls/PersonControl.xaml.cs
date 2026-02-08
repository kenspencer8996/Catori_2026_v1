using CommunityToolkit.Mvvm.Messaging;

namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for PersonControl.xaml
    /// </summary>
    public partial class PersonControl : UserControl
    {
        PersonService personservice;

        PersonViewModel _person;
        public PersonControl(PersonViewModel person)
        {
            InitializeComponent();
            _person = person;
            personservice = new PersonService();
            if (GlobalStuff.ShowAllBordersIfAvailable)
            {
                MainBorder.BorderThickness = new Thickness(1);
                MainBorder.BorderBrush = Brushes.Red;
            }

            PersonImage.Source = UIUtility.GetImageControl(_person.StaticImageFilePath, 50, 50, 0).Source;
            WeakReferenceMessenger.Default.Register<LeaveWorkArg>(this, (r, m) =>
            {
                //check if the person is in this house
                if (m.Person.Name == _person.Name)
                {
                   
                    this.Visibility = Visibility.Visible;
                    personservice.UpsertPerson(m.Person);
                }
            });
        }

        private void DepositMenu_Click(object sender, RoutedEventArgs e)
        {
            
            FundsDetailView fundsDetailView = new FundsDetailView(_person);
            fundsDetailView.Owner = GlobalStuff.MainView;
            fundsDetailView.ShowDialog();
        }

        private void UserControl_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        
        }
    }
}
