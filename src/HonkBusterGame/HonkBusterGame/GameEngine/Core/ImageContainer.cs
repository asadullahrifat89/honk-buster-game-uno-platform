using Microsoft.UI.Xaml.Media.Imaging;
using Uno;

namespace HonkBusterGame
{
    #region Without Post Processing

    public partial class ImageContainer : Image // post processing off
    {
        private readonly BitmapImage _bitmapImage;

        private Uri _uri;

        public ImageContainer(Uri uri, double width, double height)
        {
            Width = width;
            Height = height;

            _uri = uri;
            _bitmapImage = new BitmapImage(uriSource: _uri);
            Source = _bitmapImage;

            CanDrag = false;
        }

        public void SetBrighness(double brightness)
        {
            Opacity = brightness / 100;
        }

        public void SetSource(Uri uri)
        {
            _uri = uri;
            _bitmapImage.UriSource = _uri;
        }

        public Uri GetSourceUri()
        {
            return _uri;
        }

        [NotImplemented]
        public void SetGrayscale(double grayscale)
        {

        }

        [NotImplemented]
        public void SetDropShadow(int offsetX, int offsetY, int blurRadius, string color = "#202020")
        {

        }

        [NotImplemented]
        public void SetBlur(double blur)
        {

        }
    }

    #endregion

    #region With Post Processing

    //public partial class ImageContainer : Border // post processing on
    //{
    //    #region Fields

    //    private readonly HtmlImageTag _htmlImage;
    //    private readonly string _baseUrl = string.Empty;
    //    private Uri _uri;

    //    #endregion

    //    #region Ctor

    //    public ImageContainer(Uri uri, double width, double height)
    //    {
    //        _uri = uri;

    //        var indexUrl = "./";
    //        var appPackageId = Environment.GetEnvironmentVariable("UNO_BOOTSTRAP_APP_BASE");

    //        _baseUrl = $"{indexUrl}{appPackageId}";
    //        _htmlImage = new HtmlImageTag()
    //        {
    //            Width = width,
    //            Height = height,
    //        };

    //        Width = width;
    //        Height = height;

    //        CanDrag = false;

    //        Child = _htmlImage;

    //        SetId(uri);
    //        SetSource(uri);
    //    }

    //    #endregion        

    //    #region Methods

    //    public void SetGrayscale(double grayscale)
    //    {
    //        _htmlImage.Grayscale = grayscale;
    //    }

    //    public void SetDropShadow(int offsetX, int offsetY, int blurRadius, string color = "#202020")
    //    {
    //        _htmlImage.DropShadowX = offsetX;
    //        _htmlImage.DropShadowY = offsetY;
    //        _htmlImage.DropShadowBlurRadius = blurRadius;
    //        _htmlImage.DropShadowColor = color;
    //    }

    //    public void SetBrighness(double brightness)
    //    {
    //        _htmlImage.Brightness = brightness;
    //    }

    //    public void SetBlur(double blur)
    //    {
    //        _htmlImage.Blur = blur;
    //    }

    //    public void SetSource(Uri uri)
    //    {
    //        _uri = uri;
    //        var source = $"{_baseUrl}/{uri.AbsoluteUri.Replace("ms-appx:///", "")}";
    //        _htmlImage.Source = source;
    //    }

    //    public void SetId(Uri uri)
    //    {
    //        var id = uri.AbsoluteUri.Replace("ms-appx:///HonkBusterGame/Assets/Images/", "").Replace(".png", "");
    //        _htmlImage.Id = id;
    //    }

    //    public Uri GetSourceUri()
    //    {
    //        return _uri;
    //    }

    //    #endregion
    //}

    #endregion


}
