namespace HonkBusterGame
{
    public partial class Cloud : AnimableConstruct
    {
        #region Fields

        private readonly ImageElement _content_image;
        private readonly Uri[] _cloud_uris;
        private readonly Random _random;

        #endregion

        #region Ctor

        public Cloud(
            Action<Construct> animateAction,
            Action<Construct> recycleAction)
        {
            ConstructType = ConstructType.CLOUD;

            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            _random = new Random();

            _cloud_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.CLOUD).Select(x => x.Uri).ToArray();

            SetConstructSize(ConstructType);

            var uri = ConstructExtensions.GetRandomContentUri(_cloud_uris);
            _content_image = new(uri: uri, width: this.Width, height: this.Height);

            _content_image.SetBlur(6);

            SetContent(_content_image);

            IsometricDisplacement = Constants.DEFAULT_ISOMETRIC_DISPLACEMENT;
            DropShadowDistance = Constants.DEFAULT_DROP_SHADOW_DISTANCE + 200;
        }

        #endregion

        #region Methods

        public void Reset()
        {
            Speed = _random.Next(Constants.DEFAULT_CONSTRUCT_SPEED - 2, Constants.DEFAULT_CONSTRUCT_SPEED);

            var uri = ConstructExtensions.GetRandomContentUri(_cloud_uris);
            _content_image.SetSource(uri);
        }

        #endregion
    }
}
