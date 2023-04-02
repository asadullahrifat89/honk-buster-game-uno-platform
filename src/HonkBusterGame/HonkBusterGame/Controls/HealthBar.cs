using Microsoft.UI.Xaml.Media.Imaging;
using Windows.UI;

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

        private readonly Image _content_image;
        private readonly BitmapImage _bitmapImage;

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

            _bitmapImage = new BitmapImage();

            _content_image = new()
            {
                Height = 30,
                Width = 30,
                Stretch = Stretch.Uniform,
                Margin = new Thickness(5),
                Source = _bitmapImage,
            };

            _container.Children.Add(_content_image);
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
            _bitmapImage.UriSource = uri;
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
