using Uno.Foundation;
using Uno.UI.Runtime.WebAssembly;

namespace HonkBusterGame
{
    [HtmlElement("img")]
    public sealed class ImgElement : FrameworkElement
    {
        #region Fields

        private Uri _uri;

        #endregion

        #region Ctor

        public ImgElement()
        {
            this.SetHtmlAttribute("draggable", "false");
            this.SetHtmlAttribute("loading", "lazy");
            this.SetCssStyle("object-fit", "contain");
        }

        #endregion

        #region Properties

        private string _Id = string.Empty;

        public string Id
        {
            get { return _Id; }
            set
            {
                _Id = value;

                if (!_Id.IsNullOrBlank())
                {
                    this.SetHtmlAttribute("id", _Id);
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

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(string), typeof(ImgElement), new PropertyMetadata(default(string), OnSourceChanged));

        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        private static void OnSourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (dependencyObject is ImgElement image)
            {
                var encodedSource = WebAssemblyRuntime.EscapeJs("" + args.NewValue);
                image.SetHtmlAttribute("src", encodedSource);
                image.SetProperties();
            }
        }

        #endregion

        #region Methods

        public string GetCssFilter()
        {
            return $"grayscale({grayscale}%) contrast({contrast}%) brightness({brightness}%) saturate({saturation}%) sepia({sepia}%) invert({invert}%) hue-rotate({hue}deg) blur({blur}px)";
        }

        public void SetProperties()
        {
            this.SetCssStyle(("filter", GetCssFilter() + $" drop-shadow(0 0 0.75rem crimson)"), ("opacity", $"{opacity}"), ("transform", $"rotate({rotation}deg) scaleX({scaleX}) scaleY({scaleY})"));
        }

        #endregion
    }
}
