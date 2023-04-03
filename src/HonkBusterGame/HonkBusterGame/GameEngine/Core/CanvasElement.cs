using Uno.UI.Runtime.WebAssembly;

namespace HonkBusterGame
{
    [HtmlElement("canvas")]
    public sealed class CanvasElement : FrameworkElement
    {
        #region Ctor
        
        public CanvasElement()
        {

        }

        #endregion

        #region Properties


        private string id = "gamearea";
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

        #endregion

        #region Dependency Properties      

        public new double Width
        {
            get => (double)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        public new static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(CanvasElement), new PropertyMetadata(default(double), (s, e) =>
        {
            if (s is CanvasElement image)
            {
                var encodedWidth = e.NewValue.ToString();
                image.SetHtmlAttribute("width", encodedWidth);
            }
        }));

        public new double Height
        {
            get => (double)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        public new static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(double), typeof(CanvasElement), new PropertyMetadata(default(double), (s, e) =>
        {
            if (s is CanvasElement image)
            {
                var encodedHeight = e.NewValue.ToString();
                image.SetHtmlAttribute("height", encodedHeight);
            }
        }));

        #endregion

        #region Methods



        #endregion
    }
}
