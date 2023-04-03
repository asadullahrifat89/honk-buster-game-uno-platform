namespace HonkBusterGame
{
    public partial class RoadMark : MovableConstruct
    {
        #region Fields

        private readonly ImageElement _content_image;
        private readonly Uri[] _tree_uris;

        #endregion

        #region Ctor

        public RoadMark(
            Action<Construct> animateAction,
            Action<Construct> recycleAction)
        {
            ConstructType = ConstructType.ROAD_MARK;

            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            _tree_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.ROAD_MARK).Select(x => x.Uri).ToArray();

            SetConstructSize(ConstructType);

            var uri = ConstructExtensions.GetRandomContentUri(_tree_uris);
            _content_image = new(uri: uri, width: this.Width, height: this.Height);
            _content_image.SetDropShadow(offsetX: 0, offsetY: 0, blurRadius: 5);
            SetContent(_content_image);

            SetSkewY(36);
            SetRotation(-63.5);

            Speed = Constants.DEFAULT_CONSTRUCT_SPEED;
            IsometricDisplacement = Constants.DEFAULT_ISOMETRIC_DISPLACEMENT;
        }

        #endregion

        public void Reset()
        {
            if (Scene.IsInNightMode)
            {
                _content_image.ImageBrightness = 50;
            }
            else
            {
                _content_image.ImageBrightness = 100;
            }
        }
    }
}
