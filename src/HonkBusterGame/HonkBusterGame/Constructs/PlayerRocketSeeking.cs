namespace HonkBusterGame
{
    public partial class PlayerRocketSeeking : SeekingRocketObject
    {
        #region Fields

        private readonly Uri[] _bomb_uris;
        private readonly Uri[] _bomb_blast_uris;

        private readonly ImageContainer _content_image;

        private readonly AudioStub _audioStub;

        private double _autoBlastDelay;
        private readonly double _autoBlastDelayDefault = 25;

        #endregion

        #region Ctor

        public PlayerRocketSeeking(
            Action<GameObject> animateAction,
            Action<GameObject> recycleAction)
        {
            ConstructType = ConstructType.PLAYER_ROCKET_SEEKING;

            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            _bomb_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.PLAYER_ROCKET_SEEKING).Select(x => x.Uri).ToArray();
            _bomb_blast_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.BLAST).Select(x => x.Uri).ToArray();

            SetConstructSize(ConstructType);

            var uri = ConstructExtensions.GetRandomContentUri(_bomb_uris);
            _content_image = new(uri: uri, width: this.Width, height: this.Height);
            _content_image.SetDropShadow(offsetX: 0, offsetY: 0, blurRadius: 6, color: "#e8e8e8");

            SetContent(_content_image);

            //BorderThickness = new Microsoft.UI.Xaml.Thickness(Constants.DEFAULT_BLAST_RING_BORDER_THICKNESS);
            //CornerRadius = new Microsoft.UI.Xaml.CornerRadius(Constants.DEFAULT_BLAST_RING_CORNER_RADIUS);

            Speed = Constants.DEFAULT_CONSTRUCT_SPEED;
            IsometricDisplacement = Constants.DEFAULT_ISOMETRIC_DISPLACEMENT;
            DropShadowDistance = Constants.DEFAULT_DROP_SHADOW_DISTANCE;

            _audioStub = new AudioStub((SoundType.SEEKER_ROCKET_LAUNCH, 0.7, false), (SoundType.ROCKET_BLAST, 1, false));
        }

        #endregion

        #region Properties

        public bool IsBlasting { get; set; }

        #endregion

        #region Methods

        public void Reset()
        {
            _audioStub.Play(SoundType.SEEKER_ROCKET_LAUNCH);

            SetOpacity(1);

            var uri = ConstructExtensions.GetRandomContentUri(_bomb_uris);
            _content_image.SetSource(uri);

            //BorderBrush = new SolidColorBrush(Colors.Transparent);

            SetScaleTransform(1);
            SetRotation(0);

            IsBlasting = false;

            _autoBlastDelay = _autoBlastDelayDefault;
        }

        public void Reposition(PlayerBalloon player)
        {
            SetPosition(
                left: (player.GetLeft() + player.Width / 2) - Width / 2,
                top: player.GetBottom() - (40));
        }

        public bool AutoBlast()
        {
            _autoBlastDelay -= 0.1;

            if (_autoBlastDelay <= 0)
                return true;

            return false;
        }

        public void SetBlast()
        {
            _audioStub.Play(SoundType.ROCKET_BLAST);

            SetRotation(0);
            SetScaleTransform(Constants.DEFAULT_BLAST_SHRINK_SCALE);

            //BorderBrush = new SolidColorBrush(Colors.Crimson);

            var uri = ConstructExtensions.GetRandomContentUri(_bomb_blast_uris);
            _content_image.SetSource(uri);
            IsBlasting = true;
        }

        #endregion
    }
}
