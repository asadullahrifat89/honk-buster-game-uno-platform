namespace HonkBusterGame
{
    public partial class Honk : GameObject
    {
        #region Fields

        private readonly Uri[] _honkUris;
        private readonly ImageContainer _imageContainer;
        private readonly AudioStub _audioStub;

        #endregion

        #region Ctor

        public Honk(
            Action<GameObject> animateAction,
            Action<GameObject> recycleAction)
        {
            ConstructType = ConstructType.HONK;

            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            _honkUris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.HONK).Select(x => x.Uri).ToArray();

            SetConstructSize(ConstructType);

            var uri = ConstructExtensions.GetRandomContentUri(_honkUris);
            _imageContainer = new(uri: uri, width: this.Width, height: this.Height);

            SetContent(_imageContainer);

            _audioStub = new AudioStub((SoundType.HONK, 0.5, false));
        }

        #endregion

        #region Methods

        public void Reset()
        {
            _audioStub.Play(SoundType.HONK);
            SetOpacity(1);
        }

        public void Reposition(GameObject source)
        {
            var hitBox = source.GetCloseHitBox();

            SetPosition(
                left: hitBox.Left - source.Width / 3,
                top: hitBox.Top - (25));
        }

        #endregion
    }
}
