namespace HonkBusterGame
{
    public partial class HealthPickup : MovableBase
    {
        #region Fields

        private readonly Uri[] _health_uris;
        private readonly ImageContainer _imageContainer;
        private readonly AudioStub _audioStub;

        #endregion

        #region Ctor

        public HealthPickup(
            Action<GameObject> animateAction,
            Action<GameObject> recycleAction)
        {
            ConstructType = ConstructType.HEALTH_PICKUP;

            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            SetConstructSize(ConstructType);

            _health_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.HEALTH_PICKUP).Select(x => x.Uri).ToArray();

            var uri = ConstructExtensions.GetRandomContentUri(_health_uris);
            _imageContainer = new(uri: uri, width: this.Width, height: this.Height);
            _imageContainer.SetDropShadow(offsetX: 0, offsetY: 0, blurRadius: 6, color: "#f24d3a");

            SetContent(_imageContainer);

            Speed = Constants.DEFAULT_CONSTRUCT_SPEED - 2;
            DropShadowDistance = Constants.DEFAULT_DROP_SHADOW_DISTANCE;
            IsometricDisplacement = Constants.DEFAULT_ISOMETRIC_DISPLACEMENT;

            _audioStub = new AudioStub((SoundType.HEALTH_PICKUP, 1, false));
        }

        #endregion

        #region Methods

        public static bool ShouldGenerate(double playerHealth)
        {
            return playerHealth <= 50;
        }

        public void Reset()
        {
            IsPickedUp = false;
            SetScaleTransform(1);
        }

        public void PickedUp()
        {
            _audioStub.Play(SoundType.HEALTH_PICKUP);

            IsPickedUp = true;
        }

        #endregion

        #region Properties

        public bool IsPickedUp { get; set; }

        #endregion
    }
}
