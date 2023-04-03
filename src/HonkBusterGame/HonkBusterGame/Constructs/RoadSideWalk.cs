namespace HonkBusterGame
{
    public partial class RoadSideWalk : MovableBase
    {
        #region Fields

        private readonly ImageContainer _imageContainer;
        private readonly Uri[] _side_walk_uris;

        #endregion

        #region Ctor

        public RoadSideWalk(
            Action<GameObject> animateAction,
            Action<GameObject> recycleAction)
        {
            ConstructType = ConstructType.ROAD_SIDE_WALK;

            _side_walk_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.ROAD_SIDE_WALK).Select(x => x.Uri).ToArray();

            SetConstructSize(ConstructType);

            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            var uri = ConstructExtensions.GetRandomContentUri(_side_walk_uris);
            _imageContainer = new(uri: uri, width: this.Width, height: this.Height);
            _imageContainer.SetDropShadow(offsetX: -7, offsetY: 0, blurRadius: 2);
            SetContent(_imageContainer);

            //BorderBrush = Application.Current.Resources["BorderColor"] as SolidColorBrush;
            //BorderThickness = new Thickness(Constants.DEFAULT_CONTROLLER_KEY_BORDER_THICKNESS, 0);

            SetSkewY(36);
            SetRotation(-63.5);

            IsometricDisplacement = Constants.DEFAULT_ISOMETRIC_DISPLACEMENT;
            Speed = Constants.DEFAULT_CONSTRUCT_SPEED;
        }

        #endregion

        #region Methods

        public void Reset()
        {
            _imageContainer.SetBrighness(GameView.IsInNightMode ? 50 : 100);
        }

        #endregion
    }
}
