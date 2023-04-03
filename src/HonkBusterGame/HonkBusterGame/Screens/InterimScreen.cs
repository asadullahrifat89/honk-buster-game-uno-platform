using Microsoft.UI;

namespace HonkBusterGame
{
    public partial class InterimScreen : HoveringTitleScreen
    {
        #region Fields

        private readonly TextBlock _titleScreenText;

        private double _messageOnScreenDelay;
        private readonly double _messageOnScreenDelayDefault = 20;

        #endregion

        #region Ctor

        public InterimScreen(
            Action<Construct> animateAction,
            Action<Construct> recycleAction)
        {
            ConstructType = ConstructType.INTERIM_SCREEN;

            var size = Constants.CONSTRUCT_SIZES.FirstOrDefault(x => x.ConstructType == ConstructType.INTERIM_SCREEN);

            var width = size.Width;
            var height = size.Height;

            SetSize(width: width, height: height);

            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            //CornerRadius = new CornerRadius(5);

            IsometricDisplacement = Constants.DEFAULT_ISOMETRIC_DISPLACEMENT;
            Speed = Constants.DEFAULT_CONSTRUCT_SPEED;
            DropShadowDistance = Constants.DEFAULT_DROP_SHADOW_DISTANCE;

            _titleScreenText = new TextBlock()
            {
                Text = "Honk Trooper",
                FontSize = Constants.DEFAULT_GUI_FONT_SIZE,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 5),
                Foreground = new SolidColorBrush(Colors.White),
            };

            SetContent(_titleScreenText);
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
            _titleScreenText.Text = title;
        }

        #endregion
    }
}
