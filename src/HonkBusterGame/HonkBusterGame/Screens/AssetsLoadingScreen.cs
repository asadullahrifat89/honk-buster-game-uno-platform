using Microsoft.UI;

namespace HonkBusterGame
{
    public partial class AssetsLoadingScreen : HoveringTitleScreen
    {
        #region Fields

        private readonly ProgressBar _progressBar;

        private readonly TextBlock _sub_title_text;

        private bool _assets_loaded;

        #endregion

        #region Ctor

        public AssetsLoadingScreen(
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
                Background = new SolidColorBrush(Colors.DeepSkyBlue),
                CornerRadius = new CornerRadius(15),
                Opacity = Constants.DEFAULT_HOVERING_SCREEN_OPACITY,
                //BorderBrush = new SolidColorBrush(Colors.White),
                //BorderThickness = new Thickness(Constants.DEFAULT_CONTROLLER_KEY_BORDER_THICKNESS),
            });

            Grid container = new() { VerticalAlignment = VerticalAlignment.Center };
            container.RowDefinitions.Add(new RowDefinition());
            container.RowDefinitions.Add(new RowDefinition());

            #endregion

            #region Content

            #region Loading Bar

            _progressBar = new()
            {
                Width = 200,
                Height = 10,
                Value = 0,
                Maximum = 0,
                Minimum = 0,
                Margin = new Thickness(5),
                Foreground = new SolidColorBrush(Colors.Crimson),
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            Grid.SetRow(_progressBar, 0);

            container.Children.Add(_progressBar);

            #endregion

            #region SubTitle

            _sub_title_text = new TextBlock()
            {
                FontSize = Constants.DEFAULT_GUI_FONT_SIZE - 7,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 5),
                Foreground = new SolidColorBrush(Colors.White),
            };

            Grid.SetRow(_sub_title_text, 1);

            container.Children.Add(_sub_title_text);

            #endregion 

            #endregion

            rootGrid.Children.Add(container);
            SetChild(rootGrid);
        }

        #endregion

        #region Methods

        public void SetSubTitle(string subTitle)
        {
            _sub_title_text.Text = subTitle;
        }

        public async Task PreloadAssets(Action preloadedAction)
        {
            if (_assets_loaded)
            {
                preloadedAction();
            }
            else
            {
                _progressBar.IsIndeterminate = false;
                _progressBar.ShowPaused = false;
                _progressBar.Value = 0;
                _progressBar.Minimum = 0;

                _progressBar.Maximum = Constants.CONSTRUCT_TEMPLATES.Length;

                SetSubTitle($"... Loading Assets ({_progressBar.Value:00}/{_progressBar.Maximum:00}) ...");

                await AssetsPreCache.PreloadImageAssets(() =>
                {
                    _progressBar.Value++;
                    SetSubTitle($"... Loading Assets ({_progressBar.Value:00}/{_progressBar.Maximum:00}) ...");
                });

                if (_progressBar.Value == _progressBar.Maximum)
                {
                    await Task.Delay(300);
                    _assets_loaded = true;
                    preloadedAction();
                }
            }
        }

        #endregion
    }
}
