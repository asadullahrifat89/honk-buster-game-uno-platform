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

    //    #region Properties

    //    //private double width = 0;
    //    //public double ImageWidth
    //    //{
    //    //    get { return width; }
    //    //    set
    //    //    {
    //    //        width = value;

    //    //        if (_htmlImage is not null)
    //    //            _htmlImage.Width = width;
    //    //    }
    //    //}


    //    //private double height = 0;
    //    //public double ImageHeight
    //    //{
    //    //    get { return height; }
    //    //    set
    //    //    {
    //    //        height = value;

    //    //        if (_htmlImage is not null)
    //    //            _htmlImage.Height = height;
    //    //    }
    //    //}

    //    //private double brightness = 100;
    //    //public double ImageBrightness
    //    //{
    //    //    get { return brightness; }
    //    //    set
    //    //    {
    //    //        brightness = value;

    //    //        if (_htmlImage is not null)
    //    //            _htmlImage.Brightness = brightness;
    //    //    }
    //    //}



    //    //private string id = string.Empty;
    //    //public string Id
    //    //{
    //    //    get { return id; }
    //    //    set
    //    //    {
    //    //        id = value;
    //    //        if (ImgContent is not null)
    //    //            ImgContent.Id = id;
    //    //    }
    //    //}

    //    //private string source = string.Empty;
    //    //public string ImageSource
    //    //{
    //    //    get { return source; }
    //    //    set
    //    //    {
    //    //        source = value;
    //    //        if (ImgContent is not null)
    //    //            ImgContent.Source = source;
    //    //    }
    //    //}

    //    //private double blur = 0;
    //    //public double ImageBlur
    //    //{
    //    //    get { return blur; }
    //    //    set
    //    //    {
    //    //        blur = value;

    //    //        if (ImgContent is not null)
    //    //            ImgContent.Blur = blur;
    //    //    }
    //    //}


    //    //private double contrast = 100;
    //    //public double ImageContrast
    //    //{
    //    //    get { return contrast; }
    //    //    set
    //    //    {
    //    //        contrast = value;

    //    //        if (ImgContent is not null)
    //    //            ImgContent.Contrast = contrast;
    //    //    }
    //    //}

    //    //private double saturation = 100;
    //    //public double ImageSaturation
    //    //{
    //    //    get { return saturation; }
    //    //    set
    //    //    {
    //    //        saturation = value;

    //    //        if (ImgContent is not null)
    //    //            ImgContent.Saturation = saturation;
    //    //    }
    //    //}


    //    //private double sepia = 0;
    //    //public double ImageSepia
    //    //{
    //    //    get { return sepia; }
    //    //    set
    //    //    {
    //    //        sepia = value;

    //    //        if (ImgContent is not null)
    //    //            ImgContent.Sepia = sepia;
    //    //    }
    //    //}


    //    //private double invert = 0;
    //    //public double ImageInvert
    //    //{
    //    //    get { return invert; }
    //    //    set
    //    //    {
    //    //        invert = value;

    //    //        if (ImgContent is not null)
    //    //            ImgContent.Invert = invert;
    //    //    }
    //    //}


    //    //private double hue = 0;
    //    //public double ImageHue
    //    //{
    //    //    get { return hue; }
    //    //    set
    //    //    {
    //    //        hue = value;

    //    //        if (ImgContent is not null)
    //    //            ImgContent.Hue = hue;
    //    //    }
    //    //}


    //    //private double opacity = 1;
    //    //public double ImageOpacity
    //    //{
    //    //    get { return opacity; }
    //    //    set
    //    //    {
    //    //        opacity = value;

    //    //        if (ImgContent is not null)
    //    //            ImgContent.Opacity = opacity;
    //    //    }
    //    //}


    //    //private double rotation = 0;
    //    //public double ImageRotation
    //    //{
    //    //    get { return rotation; }
    //    //    set
    //    //    {
    //    //        rotation = value;

    //    //        if (ImgContent is not null)
    //    //            ImgContent.Rotation = rotation;
    //    //    }
    //    //}


    //    //private double scaleX = 1;
    //    //public double ImageScaleX
    //    //{
    //    //    get { return scaleX; }
    //    //    set
    //    //    {
    //    //        scaleX = value;

    //    //        if (ImgContent is not null)
    //    //            ImgContent.ScaleX = scaleX;
    //    //    }
    //    //}


    //    //private double scaleY = 1;
    //    //public double ImageScaleY
    //    //{
    //    //    get { return scaleY; }
    //    //    set
    //    //    {
    //    //        scaleY = value;

    //    //        if (ImgContent is not null)
    //    //            ImgContent.ScaleY = scaleY;
    //    //    }
    //    //}


    //    //private double skewX = 0;
    //    //public double ImageSkewX
    //    //{
    //    //    get { return skewX; }
    //    //    set
    //    //    {
    //    //        skewX = value;

    //    //        if (ImgContent is not null)
    //    //            ImgContent.SkewX = skewX;
    //    //    }
    //    //}


    //    //private double skewY = 0;
    //    //public double ImageSkewY
    //    //{
    //    //    get { return skewY; }
    //    //    set
    //    //    {
    //    //        skewY = value;

    //    //        if (ImgContent is not null)
    //    //            ImgContent.SkewY = skewY;
    //    //    }
    //    //}

    //    #endregion
    //} 
    #endregion


}
