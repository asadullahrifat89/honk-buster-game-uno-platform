using Microsoft.UI;

namespace HonkBusterGame
{
    public partial class PromptOrientationChangeScreen : HoveringTitleScreen
    {
        #region Fields       

        private readonly TextBlock _titleScreenText;

        #endregion

        #region Ctor

        public PromptOrientationChangeScreen(
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

            //CornerRadius = new CornerRadius(5);

            IsometricDisplacement = Constants.DEFAULT_ISOMETRIC_DISPLACEMENT;
            Speed = Constants.DEFAULT_CONSTRUCT_SPEED;
            DropShadowDistance = Constants.DEFAULT_DROP_SHADOW_DISTANCE;

            #region Base Container

            Grid rootGrid = new();

            rootGrid.Children.Add(new Border()
            {
                //Background = new SolidColorBrush(Colors.Goldenrod),
                CornerRadius = new CornerRadius(15),
                Opacity = Constants.DEFAULT_HOVERING_SCREEN_OPACITY,
                //BorderBrush = new SolidColorBrush(Colors.White),
                //BorderThickness = new Thickness(Constants.DEFAULT_CONTROLLER_KEY_BORDER_THICKNESS),
            });

            StackPanel container = new()
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            #endregion

            #region Content

            _titleScreenText = new TextBlock() // title screen text
            {
                Text =
                "Pls" + Environment.NewLine +
                "Rotate" + Environment.NewLine +
                "Your Screen",
                FontSize = Constants.DEFAULT_GUI_FONT_SIZE - 5,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(5),
                Foreground = new SolidColorBrush(Colors.White),
                TextWrapping = TextWrapping.Wrap,
            };

            container.Children.Add(_titleScreenText);

            #endregion

            rootGrid.Children.Add(container);
            SetContent(rootGrid);
        }

        #endregion
    }
}
