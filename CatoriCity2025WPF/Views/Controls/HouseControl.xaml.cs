using CatoriCity2025WPF.Controllers;
using CatoriCity2025WPF.Objects;
using CatoriCity2025WPF.Objects.Arguments;
using CatoriCity2025WPF.Objects.Services;
using CatoriCity2025WPF.ViewModels;
using CatorisControlLibrary.Objects;
using CatoriServices.Objects;
using CityAppServices.Objects.Entities;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for HouseControl.xaml
    /// </summary>
    public partial class HouseControl : UserControl
    {
        HouseControlController _controller;
        private string _ImageKitchenFileName;
        public List<FundsViewModel> FundsModels;
        private string _houseImageName = "";
        private Border _border = new Border();
        public event EventHandler<BuildingOpenEventArgs> OnBuildingOpenContent;
        public string _houseName { get; set; }
        public Label _houseLabel { get; set; }
        private double _houseWidth;
        private double _houseHeight;
        private int _houseOriginalZindex;
        Button expandimageButton = new Button();
        public HouseViewModel _houseViewModel;
        private string PersonName = "";
        private decimal Funds = 0;
        PersonService personservice;
        PersonViewModel personmodel = new PersonViewModel();

        public string HouseImageName
        {
            get
            {
                return _houseImageName;
            }
            set
            {
                _houseImageName = value;
                //_houseImage.Source = _houseImageName;
            }

        }
        public string HouseName
        {
            get
            {
                return _houseName;
            }
            set
            {
                _houseName = value;
            }
        }
        public string ImageKitchenFileName
        {
            get
            {
                return _ImageKitchenFileName;

            }
            set
            {
                _ImageKitchenFileName = value;

            }
        }

        public string ImageGarageFileName { get; set; }
        public string ImageLivingFileName { get; set; }
        private LotEntity ContentLocationLot;

        public HouseControl(HouseViewModel houseViewModel)
        {
            InitializeComponent();
            _controller = new HouseControlController(this);
            _houseViewModel = houseViewModel;
            HouseImageName = _houseViewModel.HouseImageFileName;

            Width = GlobalStuff.buildingsize;
            Height = GlobalStuff.buildingsize;
            DataContext = _houseViewModel;
            personservice = new PersonService();

            if (_houseViewModel.PersonCurrentImagePath != null && _houseViewModel.PersonCurrentImagePath != "")
            {
                PersonImage.Source = UIUtility.GetImageControl(_houseViewModel.PersonCurrentImagePath, Width, Height, 0).Source;
            }
            else
            {
                PersonImage.Source = null;
            }
            fundsThumb.Visibility = Visibility.Hidden;

            WeakReferenceMessenger.Default.Register<LeaveWorkArg>(this, (r, m) =>
            {
                //check if the person is in this house
                if (m.Person.Name == PersonName)
                {
                    PersonImage.Visibility = Visibility.Visible;
                    fundsThumb.Visibility = Visibility.Visible;
                    personservice.UpsertPerson(m.Person);
                }
            });
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

            if (_houseImageName != "")
                LoadImage(HouseImageTypeEnum.Normal);
        }
        public void LoadImage(HouseImageTypeEnum houseImageTypeEnum)
        {

            switch (houseImageTypeEnum)
            {
                case HouseImageTypeEnum.Normal:
                    if (_houseImageName != "")
                    {
                        string imagePath = Imagehelper.GetImagePath(_houseImageName);
                        HouseImage.Source = UIUtility.GetImageControl(imagePath, Width, Height, 0).Source;
                    }
                    else
                    {
                        HouseImage.Source = null;
                    }
                    break;
                case HouseImageTypeEnum.Kitchen:
                    if (_ImageKitchenFileName != "")
                    {
                        string imagePath = Imagehelper.GetImagePath(_ImageKitchenFileName);
                        HouseImage.Source = UIUtility.GetImageControl(imagePath, Width, Height, 0).Source;
                    }
                    else
                    {
                        HouseImage.Source = null;
                    }
                    break;
                case HouseImageTypeEnum.LivingRoom:
                    if (ImageLivingFileName != "")
                    {
                        string imagePath = Imagehelper.GetImagePath(ImageLivingFileName);
                        HouseImage.Source = UIUtility.GetImageControl(imagePath, Width, Height, 0).Source;
                    }
                    else
                    {
                        HouseImage.Source = null;
                    }
                    break;
                case HouseImageTypeEnum.Garage:
                    if (ImageGarageFileName != "")
                    {
                        string imagePath = Imagehelper.GetImagePath(ImageGarageFileName);
                        HouseImage.Source = UIUtility.GetImageControl(imagePath, Width, Height, 0).Source;
                    }
                    else
                    {
                        HouseImage.Source = null;
                    }
                    break;
            }
        }



        private void ExpandButton_Click(object sender, RoutedEventArgs e)
        {
            DetailHouseInsideView view = new DetailHouseInsideView(_houseViewModel);
            view.Owner = GlobalStuff.MainView;
            view.Show();
        }
        private void HouseControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double newWidth = e.NewSize.Width;
            double newHeight = e.NewSize.Height;
            double newimagesize = newWidth - 25;
            HouseImage.Width = newimagesize;
            HouseImage.Height = newimagesize;
            Canvas.SetLeft(PersonImage, newimagesize);
            Canvas.SetLeft(fundsThumb, newimagesize);

        }

        private void fundsThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {

        }

        private void fundsThumb_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            var data = new DataObject(DataFormats.StringFormat, Funds);
            DragDrop.DoDragDrop(fundsThumb, data, DragDropEffects.Move); // Here the exception occurs
        }
        public void AddPersonModel(PersonViewModel model)
        {
            personmodel = model;
            _houseViewModel.PersonCurrentImagePath = model.StaticImageFilePath;
            PersonImage.Source = _houseViewModel.CurrentImage.Source;
            PersonImage.ToolTip = model.Name;
            PersonImage.Visibility = Visibility.Visible;
            PersonName = model.Name;
            if (model.Funds > 0)
                Funds = model.Funds;
        }
        private void MainLayout_Drop(object sender, DragEventArgs e)
        {
            var data = e.Data;
            if (data != null)
            {
                string dataasstring = data.GetData(DataFormats.StringFormat).ToString();
                if (dataasstring == null || dataasstring == "" || dataasstring == "0.0")
                    return;
                personmodel = GenericSerializer.Deserialize<PersonViewModel>(dataasstring);
                _houseViewModel.PersonCurrentImagePath = personmodel.StaticImageFilePath;
                PersonImage.Source = _houseViewModel.CurrentImage.Source;
                PersonImage.ToolTip = personmodel.Name;
                PersonImage.Visibility = Visibility.Visible;
                PersonName = personmodel.Name;
                if (personmodel.Funds > 0)
                    Funds = personmodel.Funds;

            }
        }

        private void PersonImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            cLogger.Log($"HouseControl PersonImage_MouseDown before if :  {PersonName}");
            if (PersonName != null && PersonName != "")
            {
                personmodel.Name = PersonName;
                //     model.PersonCurrentImagePath = _houseViewModel.PersonCurrentImagePath;
                personmodel.Funds = Funds;
                personmodel.StaticImageFilePath = _houseViewModel.PersonCurrentImagePath;
                string modelstring = GenericSerializer.Serializer<PersonViewModel>(personmodel);
                DataObject data = new DataObject();
                data.SetText(modelstring);
                DragDrop.DoDragDrop(PersonImage, modelstring, DragDropEffects.Move);
                PersonImage.Visibility = Visibility.Collapsed;
                cLogger.Log($"HouseControl PersonImage_MouseDown if :  {PersonName}");
            }
            else
            {
                cLogger.Log($"HouseControl PersonImage_MouseDown else :  {_houseViewModel.Name}");
            }
            e.Handled = false;
        }

        private void fundsThumb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FundsDetailView fundsDetailView = new FundsDetailView(personmodel);
            fundsDetailView.Owner = GlobalStuff.MainView;
            fundsDetailView.ShowDialog();
        }
    }
}
