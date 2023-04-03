namespace HonkBusterGame
{
    public partial class RoadSideTree : MovableBase
    {
        #region Fields

        private readonly ImageContainer _imageContainer;
        private readonly Uri[] _tree_uris;

        #endregion

        #region Ctor

        public RoadSideTree(
            Action<GameObject> animateAction,
            Action<GameObject> recycleAction)
        {
            ConstructType = ConstructType.ROAD_SIDE_TREE;

            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            _tree_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.ROAD_SIDE_TREE).Select(x => x.Uri).ToArray();

            SetConstructSize(ConstructType);

            var uri = ConstructExtensions.GetRandomContentUri(_tree_uris);
            _imageContainer = new(uri: uri, width: this.Width, height: this.Height);
            _imageContainer.SetDropShadow(offsetX: 0, offsetY: 15, blurRadius: 5);
            
            SetContent(_imageContainer);

            Speed = Constants.DEFAULT_CONSTRUCT_SPEED;
            IsometricDisplacement = Constants.DEFAULT_ISOMETRIC_DISPLACEMENT;
            DropShadowDistance = -22;
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
