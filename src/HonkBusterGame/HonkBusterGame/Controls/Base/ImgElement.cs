using Uno.Foundation;
using Uno.UI.Runtime.WebAssembly;

namespace HonkBusterGame
{
    [HtmlElement("img")]
    public sealed class ImgElement : FrameworkElement
    {
        #region Ctor

        public ImgElement()
        {
            this.SetHtmlAttribute("draggable", "false");
            this.SetHtmlAttribute("loading", "lazy");
            this.SetCssStyle("object-fit", "contain");
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

                if (!id.IsNullOrBlank())
                {
                    this.SetHtmlAttribute("id", id);
                }
            }
        }


        private double grayscale = 0;

        public double Grayscale
        {
            get { return grayscale; }
            set
            {
                grayscale = value;
                SetProperties();
            }
        }

        private double contrast = 100;

        public double Contrast
        {
            get { return contrast; }
            set
            {
                contrast = value;
                SetProperties();
            }
        }

        private double brightness = 100;

        public double Brightness
        {
            get { return brightness; }
            set
            {
                brightness = value;
                SetProperties();
            }
        }

        private double saturation = 100;

        public double Saturation
        {
            get { return saturation; }
            set
            {
                saturation = value;
                SetProperties();
            }
        }

        private double sepia = 0;

        public double Sepia
        {
            get { return sepia; }
            set
            {
                sepia = value;
                SetProperties();
            }
        }

        private double invert = 0;

        public double Invert
        {
            get { return invert; }
            set
            {
                invert = value;
                SetProperties();
            }
        }

        private double hue = 0;

        public double Hue
        {
            get { return hue; }
            set
            {
                hue = value;
                SetProperties();
            }
        }

        private double blur = 0;

        public double Blur
        {
            get { return blur; }
            set
            {
                blur = value;
                SetProperties();
            }
        }

        private double opacity = 1;

        public new double Opacity
        {
            get { return opacity; }
            set
            {
                opacity = value;
                SetProperties();
            }
        }

        private double rotation = 0;

        public new double Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;
                SetProperties();
            }
        }

        private double scaleX = 1;

        public double ScaleX
        {
            get { return scaleX; }
            set
            {
                scaleX = value;
                SetProperties();
            }
        }

        private double scaleY = 1;

        public double ScaleY
        {
            get { return scaleY; }
            set
            {
                scaleY = value;
                SetProperties();
            }
        }

        private double skewX = 0;

        public double SkewX
        {
            get { return skewX; }
            set
            {
                skewX = value;
                SetProperties();
            }
        }

        private double skewY = 0;

        public double SkewY
        {
            get { return skewY; }
            set
            {
                skewY = value;
                SetProperties();
            }
        }

        private int dropShadowX = 0;

        public int DropShadowX
        {
            get { return dropShadowX; }
            set
            {
                dropShadowX = value;
                SetProperties();
            }
        }

        private int dropShadowY = 0;

        public int DropShadowY
        {
            get { return dropShadowY; }
            set
            {
                dropShadowY = value;
                SetProperties();
            }
        }

        private int dropShadowBlur = 0;

        public int DropShadowBlurRadius
        {
            get { return dropShadowBlur; }
            set
            {
                dropShadowBlur = value;
                SetProperties();
            }
        }

        private string dropShadowColor = "#202020";

        public string DropShadowColor
        {
            get { return dropShadowColor; }
            set
            {
                dropShadowColor = value;
                SetProperties();
            }
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(string), typeof(ImgElement), new PropertyMetadata(default(string), (s, e) =>
        {
            if (s is ImgElement image)
            {
                var encodedSource = WebAssemblyRuntime.EscapeJs("" + e.NewValue);
                image.SetHtmlAttribute("src", encodedSource);
                image.SetProperties();
            }

        }));

        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }



        public new static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(ImgElement), new PropertyMetadata(default(double), (s, e) =>
        {
            if (s is ImgElement image)
            {
                var encodedWidth = e.NewValue.ToString();
                image.SetHtmlAttribute("width", encodedWidth);
            }
        }));

        public new double Width
        {
            get => (double)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        public new static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(double), typeof(ImgElement), new PropertyMetadata(default(double), (s, e) =>
        {
            if (s is ImgElement image)
            {
                var encodedHeight = e.NewValue.ToString();
                image.SetHtmlAttribute("height", encodedHeight);
            }
        }));

        public new double Height
        {
            get => (double)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        #endregion

        #region Methods

        public string GetCssFilter()
        {
            return $"grayscale({grayscale}%) " +
                $"contrast({contrast}%) " +
                $"brightness({brightness}%) " +
                $"saturate({saturation}%) " +
                $"sepia({sepia}%) " +
                $"invert({invert}%) " +
                $"hue-rotate({hue}deg) " +
                $"blur({blur}px) " +
                $"drop-shadow({dropShadowX}px {dropShadowY}px {dropShadowBlur}px {dropShadowColor})";
        }

        public void SetProperties()
        {
            this.SetCssStyle(
                ("filter", GetCssFilter()),
                ("opacity", $"{opacity}"),
                ("transform", $"rotate({rotation}deg) scaleX({scaleX}) scaleY({scaleY}) skew({skewX}deg,{skewY}deg)"));
        }

        #endregion
    }
}
