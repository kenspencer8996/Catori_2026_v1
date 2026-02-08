using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Effects;

namespace CatoriCity2025WPF.Objects
{
    public class ImageTextToolTip : ToolTip
    {
        private readonly Image _iconImage;
        private readonly TextBlock _titleText;
        private readonly TextBlock _descriptionText;

        public ImageTextToolTip(double iconwidth = 32, double iconheight = 32)
        {
            Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
            HasDropShadow = false;
            Padding = new Thickness(0);
            Background = Brushes.Transparent;
            BorderThickness = new Thickness(0);

            // Root border (bubble)
            var bubble = new Border
            {
                Background = new SolidColorBrush(Colors.DarkBlue),
                BorderBrush = new SolidColorBrush(Color.FromArgb(50, 0, 0, 0)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(10),
                Effect = new DropShadowEffect
                {
                    BlurRadius = 18,
                    ShadowDepth = 6,
                    Opacity = 0.35
                }
            };

            // Layout grid
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Icon
            _iconImage = new Image
            {
                Width = iconwidth,
                Height = iconheight,
                Margin = new Thickness(0, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Top
            };
            Grid.SetColumn(_iconImage, 0);

            // Text stack
            var textStack = new StackPanel
            {
                Orientation = Orientation.Vertical
            };
            Grid.SetColumn(textStack, 1);

            _titleText = new TextBlock
            {
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = Brushes.White,
                Margin = new Thickness(0, 0, 0, 4)
            };

            _descriptionText = new TextBlock
            {
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromArgb(220, 255, 255, 255)),
                TextWrapping = TextWrapping.Wrap,
                Width = 240
            };

            textStack.Children.Add(_titleText);
            textStack.Children.Add(_descriptionText);

            grid.Children.Add(_iconImage);
            grid.Children.Add(textStack);

            bubble.Child = grid;
            Content = bubble;
        }

        // ======================
        // Properties
        // ======================

        public ImageSource Icon
        {
            get => _iconImage.Source;
            set
            {
                _iconImage.Source = value;
                _iconImage.Visibility = value == null ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public string Title
        {
            get => _titleText.Text;
            set
            {
                _titleText.Text = value ?? string.Empty;
                _titleText.Visibility = string.IsNullOrWhiteSpace(value)
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
        }

        public string Description
        {
            get => _descriptionText.Text;
            set
            {
                _descriptionText.Text = value ?? string.Empty;
                _descriptionText.Visibility = string.IsNullOrWhiteSpace(value)
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
        }
    }
}
