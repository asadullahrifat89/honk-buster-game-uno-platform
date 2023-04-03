using Microsoft.UI;

namespace HonkBusterGame
{
    public partial class InterimScreen : HoveringTitleScreen
    {
        #region Fields

        private readonly TextBlock _title_text;

        private double _messageOnScreenDelay;
        private readonly double _messageOnScreenDelayDefault = 20;

        #endregion

        #region Ctor

        public InterimScreen(
            Action<Construct> animateAction,
            Action<Construct> recycleAction)
        {
            ConstructType = ConstructType.TITLE_SCREEN;

            var size = Constants.CONSTRUCT_SIZES.FirstOrDefault(x => x.ConstructType == ConstructType.TITLE_SCREEN);

            var width = size.Width;
            var height = size.Height;

            SetSize(width: width, height: height);

            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            IsometricDisplacement = Constants.DEFAULT_ISOMETRIC_DISPLACEMENT;
            Speed = Constants.DEFAULT_CONSTRUCT_SPEED;
            DropShadowDistance = Constants.DEFAULT_DROP_SHADOW_DISTANCE;

            #region Base Container

            Grid rootGrid = new();

            rootGrid.Children.Add(new Border()
            {
                //Background = new SolidColorBrush(Colors.DeepSkyBlue),
                CornerRadius = new CornerRadius(15),
                //Opacity = Constants.DEFAULT_HOVERING_SCREEN_OPACITY,
            });

            Grid container = new() { VerticalAlignment = VerticalAlignment.Center };
            container.RowDefinitions.Add(new RowDefinition());
            //container.RowDefinitions.Add(new RowDefinition());
            //container.RowDefinitions.Add(new RowDefinition());
            //container.RowDefinitions.Add(new RowDefinition());

            #endregion

            #region Content            

            #region Title

            _title_text = new TextBlock()
            {
                FontSize = Constants.DEFAULT_GUI_FONT_SIZE,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 5),
                Foreground = new SolidColorBrush(Colors.White),
            };

            Grid.SetRow(_title_text, 0);

            container.Children.Add(_title_text);

            #endregion

            #endregion

            rootGrid.Children.Add(container);
            SetContent(rootGrid);
        }

        #endregion

        #region Properties

        public bool IsDepleted => _messageOnScreenDelay <= 0;

        #endregion

        #region Methods

        public new void Reset()
        {
            _messageOnScreenDelay = _messageOnScreenDelayDefault;
        }

        public bool DepleteOnScreenDelay()
        {
            _messageOnScreenDelay -= 0.1;
            return true;
        }

        public void SetTitle(string title)
        {
            _title_text.Text = title;
        }

        #endregion
    }
}
