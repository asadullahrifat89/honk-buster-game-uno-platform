using Microsoft.UI;

namespace HonkBusterGame
{
    public partial class GameStartScreen : HoveringTitleScreen
    {
        #region Fields       

        private readonly TextBlock _title_text;
        private readonly TextBlock _sub_title_text;
        private readonly ImageContainer _imageContainer;

        private readonly AudioStub _audioStub;

        #endregion

        #region Ctor

        public GameStartScreen
            (Action<GameObject> animateAction,
            Action<GameObject> recycleAction,
            Action playAction)
        {
            ConstructType = ConstructType.TITLE_SCREEN;

            _audioStub = new AudioStub((SoundType.GAME_START, 1, false));

            var size = Constants.CONSTRUCT_SIZES.FirstOrDefault(x => x.ConstructType == ConstructType.TITLE_SCREEN);

            var width = size.Width * 1.8;
            var height = size.Height * 1.5;

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
                Background = new SolidColorBrush(Colors.Gray),
                CornerRadius = new CornerRadius(5),
                Opacity = Constants.DEFAULT_HOVERING_SCREEN_OPACITY - 0.3,                
            });

            rootGrid.Children.Add(new Border()
            {               
                Child = new ImageContainer(uri: Constants.CONSTRUCT_TEMPLATES.FirstOrDefault(x => x.ConstructType == ConstructType.GAME_COVER_IMAGE).Uri, width: width, height: height) { Margin = new Thickness(10), }
            });

            Grid container = new() { VerticalAlignment = VerticalAlignment.Center };
            container.RowDefinitions.Add(new RowDefinition());
            container.RowDefinitions.Add(new RowDefinition());
            container.RowDefinitions.Add(new RowDefinition());
            container.RowDefinitions.Add(new RowDefinition());

            #endregion

            #region Content

            #region Image

            //var playerUris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.PLAYER_BALLOON).Select(x => x.Uri).ToArray();

            //var uri = ConstructExtensions.GetRandomContentUri(playerUris);
            //_imageContainer = new(uri: uri, width: 110, height: 110)
            //{
            //    HorizontalAlignment = HorizontalAlignment.Center,                
            //    Margin = new Thickness(0, 10, 0, 5)
            //};

            //Grid.SetRow(_imageContainer, 0);

            //container.Children.Add(_imageContainer);


            //_imageContainer = new(uri: Constants.CONSTRUCT_TEMPLATES.FirstOrDefault(x => x.ConstructType == ConstructType.GAME_COVER_IMAGE).Uri, width: width, height: height) { Margin = new Thickness(10), };

            //Grid.SetRow(_imageContainer, 0);
            //Grid.SetRowSpan(_imageContainer, 4);

            //container.Children.Add(_imageContainer);

            #endregion

            #region Title

            _title_text = new TextBlock()
            {
                FontSize = Constants.DEFAULT_GUI_FONT_SIZE,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 5),
                Foreground = new SolidColorBrush(Colors.White),
            };

            Grid.SetRow(_title_text, 1);

            container.Children.Add(_title_text);

            #endregion

            #region SubTitle

            _sub_title_text = new TextBlock()
            {
                FontSize = Constants.DEFAULT_GUI_FONT_SIZE - 7,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 5),
                Foreground = new SolidColorBrush(Colors.White),
            };

            Grid.SetRow(_sub_title_text, 2);

            container.Children.Add(_sub_title_text);

            #endregion

            #region Play Button

            Button playButton = new()
            {
                Background = new SolidColorBrush(Colors.Goldenrod),
                Height = Constants.DEFAULT_CONTROLLER_KEY_SIZE - 5,
                Width = Constants.DEFAULT_CONTROLLER_KEY_SIZE * 3.5,
                CornerRadius = new CornerRadius(Constants.DEFAULT_CONTROLLER_KEY_CORNER_RADIUS - 20),
                Content = new SymbolIcon()
                {
                    Symbol = Symbol.Play,
                },
                BorderBrush = new SolidColorBrush(Colors.White),
                BorderThickness = new Thickness(Constants.DEFAULT_CONTROLLER_KEY_BORDER_THICKNESS),
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush(Colors.White),
                Margin = new Thickness(0, 0, 0, 5),
            };

            playButton.Click += (s, e) =>
            {
                _audioStub.Play(SoundType.GAME_START);
                playAction();
            };

            Grid.SetRow(playButton, 3);

            container.Children.Add(playButton);

            #endregion

            #endregion

            rootGrid.Children.Add(container);
            SetContent(rootGrid);
        }

        #endregion

        #region Methods

        public void SetContent(Uri uri)
        {
            //_imageContainer.SetSource(uri);
        }

        public void SetTitle(string title)
        {
            _title_text.Text = title;
        }

        public void SetSubTitle(string subTitle)
        {
            _sub_title_text.Text = subTitle;

            if (string.IsNullOrEmpty(subTitle))
                _sub_title_text.Visibility = Visibility.Collapsed;
        }

        #endregion
    }
}
