using CatoriApp.Core.Objects.DragDrop;
using CatoriApp.Core.Objects.Messages;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows.Threading;
namespace CatoriApp.Game.Views.Controls.Locations.Factory
{
    /// <summary>
    /// Interaction logic for BusinessControl.xaml
    /// </summary>
    public partial class FactoryControl : UserControl, IDropAddToUC
    {
        private string _imagename = "";
        public double RotationDegrees { get; set; }
        public BusinessTypeEnum BusinessType { get; set; }
        DispatcherTimer _payTimer;
        private decimal _payRate = 25;
        private int _worktimer = 0;
        private int _worktimerMax = 2;
        public int InteriorSelector = 0;
        private LeaveEnerEnum LeaveEnter;
        PersonViewModel _personViewModel;
        private bool IsPersonMouseUp = false;   
        public FactoryControl(int interiorSelector)
        {
            InitializeComponent();
            InteriorSelector = interiorSelector;

            WeakReferenceMessenger.Default.Register<PersonMouseMessage>(this, (r, m) =>
            {
                cLogger.Log(this.Name + " WeakReferenceMessenger called : " + "MouseState " + m.MouseState);
                if (m.MouseState == MouseStatesEnum.Up)
                {
                    IsPersonMouseUp = true;
                    _personViewModel = m.Person;
                }
            });
        }
        public decimal PayRate
        {
            get { return _payRate; }
            set
            {
                if (_payRate != value)
                {
                    _payRate = value;
                }
            }
        }   

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _payTimer = new DispatcherTimer();
                _payTimer.Tick += new EventHandler(DispatcherTimer_Tick);
                _payTimer.Interval = new TimeSpan(0, 0, 10);
                _payTimer.Start();
                if (CityScapeGlobal.ShowAllBordersIfAvailable)
                {
                    MainBorder.BorderThickness = new Thickness(2);
                }
                else 
                {
                    MainBorder.BorderThickness = new Thickness(0);
                    //border.Background = ;
                }
                string path = Imagehelper.GetImagePath("gearanimated.gif");
    
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        public void AddPerson(PersonViewModel person)
        {
            _personViewModel = person;
            StartWork();
        }   
        private void DispatcherTimer_Tick(object? sender, EventArgs e)
        {
        }
        private void BusinessContent_SizeChanged(object? sender, EventArgs e)
        {
            //border.WidthRequest = this.WidthRequest;
            //border.HeightRequest = this.HeightRequest;
        }

       
        public string businessName { get; set; }
        public string RobberName { get; set; }
        public string BusinessImageName
        {
            get
            {
                return _imagename;
            }
            set
            {
                _imagename = value;
                BusinessImage.Source = UIUtility.GetImageControl(_imagename, Width, Height, 0).Source;
            }
        }

        public LotEntity Lot { get; internal set; }

        public void RotateBusinessImage(double degrees)
        {
            //RotationDegrees = degrees;
            //BusinessImage.RotateTo(degrees);
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }
        public void StartWork()
        {
            //CityScapeGlobal.SetFactoryWorking(_personViewModel.StaticImageFilePath, InteriorSelector);

        }
        private void MainLayout_Drop(object sender, DragEventArgs e)
        {
            //var data = e.Data;
            //string dataasstring;
            //if (data.GetDataPresent(DataFormats.StringFormat))
            //{
            //    dataasstring = data.GetData(DataFormats.StringFormat).ToString();
            //    if (dataasstring != "" && dataasstring != "00" && dataasstring != "0.0")
            //    {
            //        dataasstring = data.GetData(DataFormats.StringFormat).ToString();
            //    }
            //    else
            //    {
            //        return;
            //    }
            //    PersonViewModel model = GenericSerializer.Deserialize<PersonViewModel>(dataasstring);
            //    _personViewModel = model;
            //    GlobalStuff.SetFactoryWorking(_personViewModel.StaticImageFilePath,InteriorSelector);
                
            //    //string _personcurrentImage = _personViewModel.StaticImageFilePath;
            //    //PersonImage.Source = UIUtility.GetImageControl(_personcurrentImage, 50, 50, 0).Source;
            //    StartPay();
            //}
        }
      

        private void FactoryControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            cLogger.Log("FactoryControl_MouseEnter");
            
        }

        private void FactoryControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            IsPersonMouseUp = false;
            cLogger.Log("FactoryControl_Leave");
         }

        private void FactoryControl_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            cLogger.Log("FactoryControl_Mouse");

        }

    
        private void BusinessUC_PreviewDrop(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

     

       
       

        public void AddDroppedElement(UIElement element)
        {
            _personViewModel = (element as PersonControl)._person;
            AddPerson(_personViewModel);
             FactoryMouseMessage args = new FactoryMouseMessage();
            args.LeaveEnter = LeaveEnerEnum.Enter;
            args.FactoryControlInstance = this;
            WeakReferenceMessenger.Default.Send<FactoryMouseMessage>(args);

        }

        public void AddDroppedElement(IDraggable element)
        {
            FactoryView view = new FactoryView(InteriorSelector);
            view.Owner = Application.Current.MainWindow;
            view.ShowDialog();
        }
    }
}


