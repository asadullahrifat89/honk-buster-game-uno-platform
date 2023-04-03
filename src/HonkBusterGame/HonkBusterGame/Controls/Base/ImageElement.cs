namespace HonkBusterGame
{
    public partial class ImageElement : Border
    {
        #region Fields

        private readonly ImgElement _imgElement;
        private readonly string _baseUrl = string.Empty;
        private Uri _uri;

        #endregion

        #region Ctor

        public ImageElement(Uri uri, double width, double height)
        {
            _uri = uri;

            var indexUrl = "./"; //Uno.Foundation.WebAssemblyRuntime.InvokeJS("window.location.href;");
            var appPackageId = Environment.GetEnvironmentVariable("UNO_BOOTSTRAP_APP_BASE");

            _baseUrl = $"{indexUrl}{appPackageId}";
            _imgElement = new ImgElement();

            RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
            Width = width;
            Height = height;
            CanDrag = false;
            Child = _imgElement;

            SetId(uri);
            SetSource(uri);
        }

        #endregion

        #region Properties

        //private string id = string.Empty;

        //public string Id
        //{
        //    get { return id; }
        //    set
        //    {
        //        id = value;
        //        if (_imgElement is not null)
        //            _imgElement.Id = id;
        //    }
        //}

        private double contrast = 100;

        public double ImageContrast
        {
            get { return contrast; }
            set
            {
                contrast = value;

                if (_imgElement is not null)
                    _imgElement.Contrast = contrast;
            }
        }

        private double brightness = 100;

        public double ImageBrightness
        {
            get { return brightness; }
            set
            {
                brightness = value;

                if (_imgElement is not null)
                    _imgElement.Brightness = brightness;
            }
        }

        private double saturation = 100;

        public double ImageSaturation
        {
            get { return saturation; }
            set
            {
                saturation = value;

                if (_imgElement is not null)
                    _imgElement.Saturation = saturation;
            }
        }

        private double sepia = 0;

        public double ImageSepia
        {
            get { return sepia; }
            set
            {
                sepia = value;

                if (_imgElement is not null)
                    _imgElement.Sepia = sepia;
            }
        }

        private double invert = 0;

        public double ImageInvert
        {
            get { return invert; }
            set
            {
                invert = value;

                if (_imgElement is not null)
                    _imgElement.Invert = invert;
            }
        }

        private double hue = 0;

        public double ImageHue
        {
            get { return hue; }
            set
            {
                hue = value;

                if (_imgElement is not null)
                    _imgElement.Hue = hue;
            }
        }

        private double opacity = 1;

        public double ImageOpacity
        {
            get { return opacity; }
            set
            {
                opacity = value;

                if (_imgElement is not null)
                    _imgElement.Opacity = opacity;
            }
        }

        private double rotation = 0;

        public double ImageRotation
        {
            get { return rotation; }
            set
            {
                rotation = value;

                if (_imgElement is not null)
                    _imgElement.Rotation = rotation;
            }
        }

        private double scaleX = 1;

        public double ImageScaleX
        {
            get { return scaleX; }
            set
            {
                scaleX = value;

                if (_imgElement is not null)
                    _imgElement.ScaleX = scaleX;
            }
        }

        private double scaleY = 1;

        public double ImageScaleY
        {
            get { return scaleY; }
            set
            {
                scaleY = value;

                if (_imgElement is not null)
                    _imgElement.ScaleY = scaleY;
            }
        }

        #endregion

        #region Methods

        public void SetId(Uri uri)
        {
            if (_imgElement is not null)
            {
                var id = uri.AbsoluteUri.Replace("ms-appx:///HonkBusterGame/Assets/Images/", "");
                _imgElement.Id = id;
            }
        }

        public string GetId()
        {
            if (_imgElement is not null)
                return _imgElement.Id;
            else
                return string.Empty;
        }

        public void SetGrayscale(double grayscale)
        {
            if (_imgElement is not null)
                _imgElement.Grayscale = grayscale;
        }

        public void SetDropShadow(int offsetX, int offsetY, int blurRadius, string color = "#202020")
        {
            if (_imgElement is not null)
            {
                _imgElement.DropShadowX = offsetX;
                _imgElement.DropShadowY = offsetY;
                _imgElement.DropShadowBlurRadius = blurRadius;
                _imgElement.DropShadowColor = color;
            }
        }

        public void SetBlur(double blur)
        {
            if (_imgElement is not null)
                _imgElement.Blur = blur;
        }

        public void SetSource(Uri uri)
        {
            if (_imgElement is not null)
            {
                var source = $"{_baseUrl}/{uri.AbsoluteUri.Replace("ms-appx:///", "")}";
                _imgElement.Source = source;

                _uri = uri;
            }
        }

        public Uri GetSourceUri()
        {
            return _uri;
        }

        #endregion
    }

    //public partial class ImageElement : Image
    //{
    //    private readonly BitmapImage _bitmapImage;

    //    private Uri _uri;

    //    public ImageElement(Uri uri, double width, double height)
    //    {
    //        _uri = uri;
    //        _bitmapImage = new BitmapImage(uriSource: _uri);
    //        Source = _bitmapImage;
    //        Width = width;
    //        Height = height;
    //        CanDrag = false;
    //    }

    //    public void SetSource(Uri uri)
    //    {
    //        _uri = uri;
    //        _bitmapImage.UriSource = _uri;
    //    }

    //    public Uri GetSourceUri()
    //    {
    //        return _uri;
    //    }
    //}
}
