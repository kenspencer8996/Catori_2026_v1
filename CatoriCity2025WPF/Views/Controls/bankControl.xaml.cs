using CatoriCity2025WPF.Objects;
using CatoriCity2025WPF.Objects.Arguments;
using CatoriCity2025WPF.Objects.Services;
using CatoriCity2025WPF.ViewModels;
using CityAppServices.Objects.Entities;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for BusinessControl.xaml
    /// </summary>
    public partial class BankControl : UserControl
    {
        public BankViewModel Model = new BankViewModel();
        private string _imagename = "";
        public double RotationDegrees { get; set; }
        public BusinessTypeEnum BusinessType { get; set; }
        public FundsViewModel Funds = new FundsViewModel();
        public double ParentLeft = -1;
        public double Parentop = -1;
        DepositService depositservice;

        public BankControl(double left,double top)
        {
            InitializeComponent();
            ParentLeft = left;
            Parentop = top;
            depositservice = new DepositService();
            //WeakReferenceMessenger.Default.Register<DepositMessageArgument>(this, (r, m) =>
            //{

            //    cLogger.Log(this.Name + " WeakReferenceMessenger called : " + " ");
            //    if (m.BankId == Model.BankId)
            //    {
            //        Funds.Money += m.Amount;
            //        SaveDeposit(m.Amount,m.Person.PersonId,m.BusinessName);
            //        SendMessageDepositeMade(Funds);
            //        SenMessageToBadGuys(Funds);
            //    }
            //});
        }
        private void SenMessageToBadGuys(FundsViewModel funds)
        {
            MessageToRobbersOnDeposit messagetoRobbers =
                new MessageToRobbersOnDeposit(Model.BankId, ParentLeft, Parentop, funds.Money);
            
            //Send message to the main window to update the funds
            WeakReferenceMessenger.Default.Send<MessageToRobbersOnDeposit>(messagetoRobbers);
        }

        public void SaveDeposit(decimal amount,int personid,string businessname)
        {
            DepositViewModel _depositViewModel = new DepositViewModel();
            _depositViewModel.BankId = Model.BankId;
            _depositViewModel.Amount = amount;
            _depositViewModel.PersonId = personid;
            _depositViewModel.BusinessName = businessname;
            
            depositservice.UpsertAsync(_depositViewModel.Entity);
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (GlobalStuff.ShowAllBordersIfAvailable)
            {
                MainBorder.BorderThickness = new Thickness(2);
            }
            else 
            {
                MainBorder.BorderThickness = new Thickness(0);
                //border.Background = ;
            }

        }

        private void BusinessContent_SizeChanged(object? sender, EventArgs e)
        {
            //border.WidthRequest = this.WidthRequest;
            //border.HeightRequest = this.HeightRequest;
        }

       
        public string businessName { get; set; }
        public string BankKey { get; set; }
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
            double newWidth = e.NewSize.Width;
            double newHeight = e.NewSize.Height;
            BusinessImage.Width = newWidth - 20;
            BusinessImage.Height = newHeight;
        }

        private void MainLayout_Drop(object sender, DragEventArgs e)
        {
         
        }
        private void SendMessageDepositeMade(FundsViewModel funds)
        {
            MessageSaveDepositArgument messageSaveDeposit= new MessageSaveDepositArgument();
            messageSaveDeposit.Deposit.BankId =Model.BankId;
            //Send message to the main window to update the funds
            WeakReferenceMessenger.Default.Send< MessageSaveDepositArgument>(messageSaveDeposit);
        }
        private void BusinessUC_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void ShowDepositsButton_Click(object sender, RoutedEventArgs e)
        {
            BankDepositsView bankDepositsView = new BankDepositsView(Model);
            bankDepositsView.Owner = Application.Current.MainWindow;
            bankDepositsView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            bankDepositsView.ShowDialog();

        }

        private void MainLayout_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Size NewSize = e.NewSize;
            Canvas.SetLeft(ShowDepositsButton, NewSize.Width - ShowDepositsButton.Width );
            Canvas.SetTop(ShowDepositsButton, 0);
            
        }
    }
}
