namespace HonkBusterGame
{
    public partial class RoadSideBillboard : MovableBase
    {
        #region Fields

        private readonly ImageContainer _imageContainer;
        private readonly Uri[] _billboard_uris;

        #endregion

        #region Ctor

        public RoadSideBillboard(
            Action<GameObject> animateAction,
            Action<GameObject> recycleAction)
        {
            ConstructType = ConstructType.ROAD_SIDE_BILLBOARD;

            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            _billboard_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.ROAD_SIDE_BILLBOARD).Select(x => x.Uri).ToArray();

            SetConstructSize(ConstructType);

            var uri = ConstructExtensions.GetRandomContentUri(_billboard_uris);
            _imageContainer = new(uri: uri, width: this.Width, height: this.Height);
            _imageContainer.SetDropShadow(offsetX: 0, offsetY: 10, blurRadius: 2);

            SetContent(_imageContainer);

            Speed = Constants.DEFAULT_CONSTRUCT_SPEED;
            IsometricDisplacement = Constants.DEFAULT_ISOMETRIC_DISPLACEMENT;
            DropShadowDistance = -10;
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
