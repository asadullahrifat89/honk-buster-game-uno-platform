namespace HonkBusterGame
{
    public partial class UfoEnemyRocket : AnimableConstruct
    {
        #region Fields

        private readonly Uri[] _bomb_uris;
        private readonly Uri[] _bomb_blast_uris;

        private readonly ImageElement _content_image;

        private double _autoBlastDelay;
        private readonly double _autoBlastDelayDefault = 12;

        private readonly AudioStub _audioStub;

        #endregion

        #region Ctor

        public UfoEnemyRocket(
           Action<GameObject> animateAction,
           Action<GameObject> recycleAction)
        {
            ConstructType = ConstructType.UFO_ENEMY_ROCKET;

            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            _bomb_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.UFO_ENEMY_ROCKET).Select(x => x.Uri).ToArray();
            _bomb_blast_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.BLAST).Select(x => x.Uri).ToArray();

            SetConstructSize(ConstructType);

            var uri = ConstructExtensions.GetRandomContentUri(_bomb_uris);
            _content_image = new(uri: uri, width: this.Width, height: this.Height);
            _content_image.SetDropShadow(offsetX: 0, offsetY: 0, blurRadius: 6, color: "#ffd608");

            SetContent(_content_image);

            //BorderThickness = new Microsoft.UI.Xaml.Thickness(Constants.DEFAULT_BLAST_RING_BORDER_THICKNESS);
            //CornerRadius = new Microsoft.UI.Xaml.CornerRadius(Constants.DEFAULT_BLAST_RING_CORNER_RADIUS);

            IsometricDisplacement = Constants.DEFAULT_ISOMETRIC_DISPLACEMENT;
            DropShadowDistance = Constants.DEFAULT_DROP_SHADOW_DISTANCE + 10;

            _audioStub = new AudioStub((SoundType.ORB_LAUNCH, 0.5, false), (SoundType.ROCKET_BLAST, 1, false));

        }

        #endregion

        #region Methods

        public void Reset()
        {
            _audioStub.Play(SoundType.ORB_LAUNCH);

            SetOpacity(1);
            Speed = Constants.DEFAULT_CONSTRUCT_SPEED + 2;
            SetScaleTransform(1);

            //BorderBrush = new SolidColorBrush(Colors.Transparent);

            IsBlasting = false;

            var uri = ConstructExtensions.GetRandomContentUri(_bomb_uris);
            _content_image.SetSource(uri);

            _autoBlastDelay = _autoBlastDelayDefault;
        }

        public void Reposition(UfoEnemy ufoEnemy)
        {
            SetPosition(
                left: (ufoEnemy.GetLeft() + ufoEnemy.Width / 2) - Width / 2,
                top: ufoEnemy.GetBottom() - (50));
        }

        public void SetBlast()
        {
            _audioStub.Play(SoundType.ROCKET_BLAST);
            Speed = Constants.DEFAULT_CONSTRUCT_SPEED - 1;

            SetScaleTransform(Constants.DEFAULT_BLAST_SHRINK_SCALE);

            //BorderBrush = new SolidColorBrush(Colors.Goldenrod);

            var uri = ConstructExtensions.GetRandomContentUri(_bomb_blast_uris);
            _content_image.SetSource(uri);

            IsBlasting = true;
        }

        public bool AutoBlast()
        {
            _autoBlastDelay -= 0.1;

            if (_autoBlastDelay <= 0)
                return true;

            return false;
        }

        #endregion
    }
}
