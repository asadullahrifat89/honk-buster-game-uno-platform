namespace HonkBusterGame
{
    public partial class Honk : Construct
    {
        #region Fields

        private readonly Uri[] _honk_uris;
        private readonly ImageElement _content_image;
        private readonly AudioStub _audioStub;

        #endregion

        #region Ctor

        public Honk(
            Action<Construct> animateAction,
            Action<Construct> recycleAction)
        {
            ConstructType = ConstructType.HONK;

            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            _honk_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.HONK).Select(x => x.Uri).ToArray();

            SetConstructSize(ConstructType);

            var uri = ConstructExtensions.GetRandomContentUri(_honk_uris);
            _content_image = new(uri: uri, width: this.Width, height: this.Height);

            SetChild(_content_image);

            _audioStub = new AudioStub((SoundType.HONK, 0.5, false));
        }

        #endregion

        #region Methods

        public void Reset()
        {
            _audioStub.Play(SoundType.HONK);
            Opacity = 1;
        }

        public void Reposition(Construct source)
        {
            var hitBox = source.GetCloseHitBox();

            SetPosition(
                left: hitBox.Left - source.Width / 3,
                top: hitBox.Top - (25));
        }

        #endregion
    }
}
