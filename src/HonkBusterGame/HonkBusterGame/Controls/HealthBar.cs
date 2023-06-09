﻿using Windows.UI;

namespace HonkBusterGame
{
    public partial class HealthBar : Border
    {
        #region Fields

        private readonly ProgressBar _progressBar = new()
        {
            Width = 60,
            Height = 10,
            Value = 0,
            Maximum = 0,
            Minimum = 0,
            Margin = new Thickness(0, 0, 5, 0)
        };

        private readonly ImageContainer _imageContainer;
        private readonly StackPanel _container;

        #endregion

        #region Ctor

        public HealthBar()
        {
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Center;

            _container = new()
            {
                VerticalAlignment = VerticalAlignment.Center,
                Orientation = Orientation.Horizontal
            };

            _imageContainer = new ImageContainer(uri: Constants.CONSTRUCT_TEMPLATES.FirstOrDefault(x => x.ConstructType == ConstructType.HEALTH_PICKUP).Uri, width: 30, height: 30)
            {
                //Stretch = Stretch.Uniform,
                Margin = new Thickness(5)
            };

            _container.Children.Add(_imageContainer);
            _container.Children.Add(_progressBar);

            Child = _container;

            CornerRadius = new CornerRadius(5);
            //BorderThickness = new Thickness(4);
            //Background = new SolidColorBrush(Colors.Goldenrod);
            //BorderBrush = new SolidColorBrush(Colors.White);

            Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Properties

        public bool HasHealth => _progressBar.Value > 0;

        #endregion

        #region Methods

        public void SetIcon(Uri uri)
        {
            _imageContainer.SetSource(uri);
        }

        public void SetMaxiumHealth(double value)
        {
            _progressBar.Maximum = value;
        }

        public double GetMaxiumHealth()
        {
            return _progressBar.Maximum;
        }

        public void SetValue(double value)
        {
            _progressBar.Value = value;
            Visibility = _progressBar.Value > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public void SetBarColor(Color color)
        {
            _progressBar.Foreground = new SolidColorBrush(color);
        }

        public double GetValue()
        {
            return _progressBar.Value;
        }

        public void Reset()
        {
            SetValue(0);
        }

        #endregion
    }
}
