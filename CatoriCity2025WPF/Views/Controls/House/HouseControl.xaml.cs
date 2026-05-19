using CatoriApp.Controllers;
using CatoriApp.Objects.Arguments;
using CatoriApp.Objects.DragDrop;
using CatoriApp.Objects.Messages;
using CatorisControlLibrary.Objects;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows.Input;

namespace CatoriApp.Views.Controls
{
    /// <summary>
    /// Interaction logic for HouseControl.xaml
    /// </summary>
    public partial class HouseControl : UserControl, IDropAddToUC
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
        PersonControl _personControl;
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

            Width = CityScapeGlobal.buildingsize;
            Height = CityScapeGlobal.buildingsize;
            DataContext = _houseViewModel;

            if (_houseViewModel.OwnerName == "Catori")
            {
                LoadOwnerImage();
            }

            string tooltipimagepath = _houseViewModel.HouseImageFileName;

            ImageTextToolTip toolTip = new ImageTextToolTip
            {
                Title = _houseViewModel.Name,
                Description = "View the living room and find the garage by clicking tharrow.",
                Icon = UIUtility.GetImageControl(tooltipimagepath, 32, 32, 0).Source
            };
            this.ToolTip = toolTip;
            WeakReferenceMessenger.Default.Register<HouseSoldMessage>(this, (r, m) =>
            {
                try
                {
                    if (_houseViewModel.HouseId == m.House.HouseId)
                    {
                        _houseViewModel = m.House;
                        if (_houseViewModel.Name == m.House.Name)
                        {
                            LoadOwnerImage();
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            });

        }
        private void LoadOwnerImage()
        {
            string tooltipimagepath = Imagehelper.GetImagePath("houses//CatoriMailBox.png");

            string imagePath = Imagehelper.GetImagePath("houses//CatoriMailBox.png");
            MailBoxImage.Source = UIUtility.GetImageControl(imagePath, 15, 15, 0).Source;

            ImageTextToolTip toolTip = new ImageTextToolTip
            {
                Title = "Catori's house",
                Description = "Catori owns this house.",
                Icon = UIUtility.GetImageControl(tooltipimagepath, 32, 32, 0).Source
            };
            MailBoxImage.ToolTip = toolTip;
            MailBoxImage.Opacity = 1;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (CityScapeGlobal.ShowAllBordersIfAvailable)
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
            DetailHouseInsideView view = new DetailHouseInsideView(_houseViewModel,CityScapeGlobal.CityScapeViewWidth,CityScapeGlobal.CityScapeViewHeight);
            view.Owner = CityScapeGlobal.CityScapeView;
            view.Show();
        }
        private void HouseControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double newWidth = e.NewSize.Width;
            double newHeight = e.NewSize.Height;
            double newimagesize = newWidth - 25;
            HouseImage.Width = newimagesize;
            HouseImage.Height = newimagesize;
 
        }

        private void fundsThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {

        }

        private void fundsThumb_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            //var data = new DataObject(DataFormats.StringFormat, Funds);
            //DragDrop.DoDragDrop(fundsThumb, data, DragDropEffects.Move); // Here the exception occurs
        }
        

        public void AddPersonModel(PersonViewModel model)
        {
            try
            {
                //_houseViewModel.PersonCurrentImagePath = model.StaticImageFilePath;
                //PersonImage.Source = _houseViewModel.CurrentImage.Source;
                //PersonImage.ToolTip = model.Name;
                //PersonImage.Visibility = Visibility.Visible;
                //PersonName = model.Name;
                //if (model.Funds > 0)
                //    Funds = model.Funds;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private void MainLayout_Drop(object sender, DragEventArgs e)
        {
            var data = e.Data;
            if (data != null)
            {
                string dataasstring = data.GetData(DataFormats.StringFormat).ToString();
                if (dataasstring == null || dataasstring == "" || dataasstring == "0.0")
                    return;
                //personmodel = GenericSerializer.Deserialize<PersonViewModel>(dataasstring);
                //_houseViewModel.PersonCurrentImagePath = personmodel.StaticImageFilePath;
                //PersonImage.Source = _houseViewModel.CurrentImage.Source;
                //PersonImage.ToolTip = personmodel.Name;
                //PersonImage.Visibility = Visibility.Visible;
                //PersonName = personmodel.Name;
                //if (personmodel.Funds > 0)
                //    Funds = personmodel.Funds;

            }
        }

        private void PersonImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            cLogger.Log($"HouseControl PersonImage_MouseDown before if :  {PersonName}");
            if (PersonName != null && PersonName != "")
            {
                //personmodel.Name = PersonName;
                ////     model.PersonCurrentImagePath = _houseViewModel.PersonCurrentImagePath;
                //personmodel.Funds = Funds;
                //personmodel.StaticImageFilePath = _houseViewModel.PersonCurrentImagePath;
                //string modelstring = GenericSerializer.Serialize<PersonViewModel>(personmodel);
                //DataObject data = new DataObject();
                //data.SetText(modelstring);
                //DragDrop.DoDragDrop(PersonImage, modelstring, DragDropEffects.Move);
                //PersonImage.Visibility = Visibility.Collapsed;
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
            //FundsDetailView fundsDetailView = new FundsDetailView(personmodel);
            //fundsDetailView.Owner = GlobalStuff.MainView;
            //fundsDetailView.ShowDialog();
        }

      
        public void AddDroppedElement(UIElement element)
        {
            _personControl = element as PersonControl;
            element.Visibility = Visibility.Hidden;
        }

        public void AddDroppedElement(IDraggable element)
        {
        }
    }
}

