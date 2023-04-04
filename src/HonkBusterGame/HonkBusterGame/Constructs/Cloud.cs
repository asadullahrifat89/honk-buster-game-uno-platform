namespace HonkBusterGame
{
    public partial class Cloud : AnimableBase
    {
        #region Fields

        private readonly Uri[] _cloudUris;

        private readonly ImageContainer _imageContainer;        
        private readonly Random _random;

        #endregion

        #region Ctor

        public Cloud(
            Action<GameObject> animateAction,
            Action<GameObject> recycleAction)
        {
            ConstructType = ConstructType.CLOUD;

            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            _random = new Random();

            _cloudUris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.CLOUD).Select(x => x.Uri).ToArray();

            SetConstructSize(ConstructType);

            var uri = ConstructExtensions.GetRandomContentUri(_cloudUris);
            _imageContainer = new(uri: uri, width: this.Width, height: this.Height);

            _imageContainer.SetBlur(6);

            SetContent(_imageContainer);

            IsometricDisplacement = Constants.DEFAULT_ISOMETRIC_DISPLACEMENT;
            DropShadowDistance = Constants.DEFAULT_DROP_SHADOW_DISTANCE + 200;
        }

        #endregion

        #region Methods

        public void Reset()
        {
            Speed = _random.Next(Constants.DEFAULT_CONSTRUCT_SPEED - 2, Constants.DEFAULT_CONSTRUCT_SPEED);

            var uri = ConstructExtensions.GetRandomContentUri(_cloudUris);
            _imageContainer.SetSource(uri);
        }

        #endregion
    }
}
