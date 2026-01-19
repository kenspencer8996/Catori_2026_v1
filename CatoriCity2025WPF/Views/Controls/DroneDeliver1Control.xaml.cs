using CatoriCity2025WPF.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for DroneDeliver1.xaml
    /// </summary>
    public partial class DroneDeliver1UC : UserControl
    {
        private DroneDeliver1UCController _controller;
        public DroneDeliver1UC()
        {
            InitializeComponent();
            _controller = new DroneDeliver1UCController(this);

            // design-time placeholder
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                SetImage("pack://application:,,,/CatoriCity2025WPF;component/Images/dronewithbox.png");
            }
            else
            {
                Loaded += DroneDeliver1_Loaded;
            }
        }
        public static readonly DependencyProperty ImagePathProperty =  DependencyProperty.Register(
         nameof(ImagePath),
         typeof(string),
         typeof(DroneDeliver1UC),
         new PropertyMetadata(null, OnImagePathChanged));

        public string ImagePath
        {
            get => (string?)GetValue(ImagePathProperty) ?? string.Empty;
            set => SetValue(ImagePathProperty, value);
        }

       

        private void DroneDeliver1_Loaded(object? sender, RoutedEventArgs e)
        {
            // if ImagePath was set via DP before Loaded, apply it
            if (!string.IsNullOrEmpty(ImagePath))
                SetImage(ImagePath);
        }

        private static void OnImagePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DroneDeliver1UC control)
            {
                control.SetImage(e.NewValue as string);
            }
        }
        public void FlyToPickup()
        {
            ImagePath = System.IO.Path.Combine(GlobalStuff.ImageFolder, "DroneEmpty.png");
            _controller.FlyToPickup();
        }
        public void FlyToDroneParking()
        {
            ImagePath = System.IO.Path.Combine(GlobalStuff.ImageFolder, "DroneEmpty.png");
            _controller.FlyToDroneParking();
        }
        public void LiftOff()
        {
           ImagePath =System.IO.Path.Combine(GlobalStuff.ImageFolder, "dronewithbox.png");
            _controller.LiftOff();
        }
        public void FlyToHouse()
        {
            ImagePath = System.IO.Path.Combine(GlobalStuff.ImageFolder, "dronewithbox.png");
            _controller.FlyToHouse();
        }

        public void DeliverPackage()
        {
            ImagePath = System.IO.Path.Combine(GlobalStuff.ImageFolder, "dronewithbox.png");
            _controller.DeliverPackage();
        }
        /// <summary>
        /// Set image source. Accepts absolute/relative file path or pack URI.
        /// Uses existing UIUtility helper to build the image control source.
        /// </summary>
        public void SetImage(string? path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    // clear image
                    DroneImage.Source = null;
                    return;
                }

                // UIUtility.GetImageControl returns an Image; use its Source for consistency
                var img = UIUtility.GetImageControl(path, DroneImage.Width, DroneImage.Height, 0);
                if (img != null)
                {
                    DroneImage.Source = img.Source;
                }
                else
                {
                    DroneImage.Source = null;
                }
            }
            catch (Exception)
            {
                DroneImage.Source = null;
                throw;
            }
        }
    }
}
