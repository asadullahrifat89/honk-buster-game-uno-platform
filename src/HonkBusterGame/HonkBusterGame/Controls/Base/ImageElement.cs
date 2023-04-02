using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace HonkBusterGame
{
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

    public partial class ImageElement : Border
    {
        #region Fields

        private readonly ImgElement ImgElement;
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

            RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
            ImgElement = new ImgElement();
            Child = ImgElement;
            Width = width;
            Height = height;
            CanDrag = false;

            SetSource(uri);
        }

        #endregion

        #region Properties

        private string id = string.Empty;

        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                if (ImgElement is not null)
                    ImgElement.Id = id;
            }
        }


        //private double grayscale = 0;

        //public double ImageGrayscale
        //{
        //    get { return grayscale; }
        //    set
        //    {
        //        grayscale = value;

        //        if (ImgElement is not null)
        //            ImgElement.Grayscale = grayscale;
        //    }
        //}

        private double contrast = 100;

        public double ImageContrast
        {
            get { return contrast; }
            set
            {
                contrast = value;

                if (ImgElement is not null)
                    ImgElement.Contrast = contrast;
            }
        }

        private double brightness = 100;

        public double ImageBrightness
        {
            get { return brightness; }
            set
            {
                brightness = value;

                if (ImgElement is not null)
                    ImgElement.Brightness = brightness;
            }
        }

        private double saturation = 100;

        public double ImageSaturation
        {
            get { return saturation; }
            set
            {
                saturation = value;

                if (ImgElement is not null)
                    ImgElement.Saturation = saturation;
            }
        }

        private double sepia = 0;

        public double ImageSepia
        {
            get { return sepia; }
            set
            {
                sepia = value;

                if (ImgElement is not null)
                    ImgElement.Sepia = sepia;
            }
        }

        private double invert = 0;

        public double ImageInvert
        {
            get { return invert; }
            set
            {
                invert = value;

                if (ImgElement is not null)
                    ImgElement.Invert = invert;
            }
        }

        private double hue = 0;

        public double ImageHue
        {
            get { return hue; }
            set
            {
                hue = value;

                if (ImgElement is not null)
                    ImgElement.Hue = hue;
            }
        }

        //private double blur = 0;

        //public double ImageBlur
        //{
        //    get { return blur; }
        //    set
        //    {
        //        blur = value;

        //        if (ImgElement is not null)
        //            ImgElement.Blur = blur;
        //    }
        //}

        private double opacity = 1;

        public double ImageOpacity
        {
            get { return opacity; }
            set
            {
                opacity = value;

                if (ImgElement is not null)
                    ImgElement.Opacity = opacity;
            }
        }

        private double rotation = 0;

        public double ImageRotation
        {
            get { return rotation; }
            set
            {
                rotation = value;

                if (ImgElement is not null)
                    ImgElement.Rotation = rotation;
            }
        }

        private double scaleX = 1;

        public double ImageScaleX
        {
            get { return scaleX; }
            set
            {
                scaleX = value;

                if (ImgElement is not null)
                    ImgElement.ScaleX = scaleX;
            }
        }

        private double scaleY = 1;

        public double ImageScaleY
        {
            get { return scaleY; }
            set
            {
                scaleY = value;

                if (ImgElement is not null)
                    ImgElement.ScaleY = scaleY;
            }
        }

        #endregion

        #region Dependency Properties

        //public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(string), typeof(ImageElement), new PropertyMetadata(default(string), OnSourceChanged));

        //public string Source
        //{
        //    get => (string)GetValue(SourceProperty);
        //    set => SetValue(SourceProperty, value);
        //}

        //private static void OnSourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        //{
        //    if (dependencyObject is ImageElement image)
        //    {
        //        if (image.ImgElement is not null)
        //        {
        //            image.ImgElement.Id = image.Id;
        //            image.ImgElement.Source = image.Source;
        //        }
        //    }
        //}

        #endregion

        #region Methods

        public void SetGrayscale(double grayscale)
        {
            if (ImgElement is not null)
                ImgElement.Grayscale = grayscale;
        }

        public void SetDropShadow(int offsetX, int offsetY, int blurRadius, string color = "#202020")
        {
            if (ImgElement is not null)
            {
                ImgElement.DropShadowX = offsetX;
                ImgElement.DropShadowY = offsetY;
                ImgElement.DropShadowBlurRadius = blurRadius;
                ImgElement.DropShadowColor = color;
            }
        }

        public void SetBlur(double blur)
        {
            if (ImgElement is not null)
                ImgElement.Blur = blur;
        }

        public void SetSource(Uri uri)
        {
            if (ImgElement is not null)
            {
                var source = $"{_baseUrl}/{uri.AbsoluteUri.Replace("ms-appx:///", "")}";
                ImgElement.Source = source;

                _uri = uri;
            }
        }

        public Uri GetSourceUri()
        {
            return _uri;
        }

        #endregion
    }
}
