using CatoriApp.Objects.Arguments;
using CatoriApp.Objects.DragDrop;
namespace CatoriApp.Views.Controls.Digging
{
    /// <summary>
    /// Interaction logic for DigInHoleControl.xaml
    /// </summary>
    public partial class DigInHoleControl : UserControl,IDropTarget
    {
        PersonControl personControl;
        public int totalSpotsCount;
        public DigInHoleControl(DragManager dragManager,Canvas hostCanvas)
        {
            InitializeComponent();

            personControl = new PersonControl(GlobalAllApps.CurrentPerson, dragManager, hostCanvas);
            MainLayoutForTreasure.Children.Add(personControl);
            personControl.DiggerCycleComplete += PersonControl_DiggerCycleComplete;
            Canvas.SetLeft(personControl, 50);
            Canvas.SetTop(personControl, 0);
            personControl.HidePerson();

            totalSpotsCount = SpotControl.DirtSpots.Count;
            SpotControl.StartDiggingEvent += SpotControl_StartDiggingEvent;
        }

        private void PersonControl_DiggerCycleComplete(object? sender, DiggerCompleteCycleArgs e)
        {
            SpotControl.DiggerCycleComplete();
        }

        private void SpotControl_StartDiggingEvent(object? sender, StartDiggingArgs e)
        {
           StartDigging();  
        }

        public void StartDigging()
        {
            personControl.ShowPerson();
            personControl.StartDiggingAnimation(totalSpotsCount);
        }
      

        public bool CanDrop(UIElement element)
        {
            return true;
        }

        public void OnDrop(UIElement element)
        {
            PersonControl person = element as PersonControl;
            if (person != null)
            {
                person.Opacity = 0;
            }
            StartDigging();
        }
    }
}


