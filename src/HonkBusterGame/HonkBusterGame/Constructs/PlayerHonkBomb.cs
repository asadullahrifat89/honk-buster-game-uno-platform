namespace HonkBusterGame
{
    public partial class PlayerHonkBomb : MovableBase
    {
        #region Fields

        private Uri[] _bombUris;
        private readonly Uri[] _blastUris;
        private readonly Uri[] _bangUris;

        private readonly ImageContainer _imageContainer;
        private readonly AudioStub _audioStub;

        #endregion

        #region Ctor

        public PlayerHonkBomb(
            Action<GameObject> animateAction,
            Action<GameObject> recycleAction)
        {
            ConstructType = ConstructType.PLAYER_HONK_BOMB;

            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            _bombUris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.PLAYER_HONK_BOMB && x.Uri.OriginalString.Contains("cracker")).Select(x => x.Uri).ToArray();
            _blastUris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.BLAST).Select(x => x.Uri).ToArray();
            _bangUris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.BANG).Select(x => x.Uri).ToArray();

            SetConstructSize(ConstructType);

            var uri = ConstructExtensions.GetRandomContentUri(_bombUris);
            _imageContainer = new(uri: uri, width: this.Width, height: this.Height);
            _imageContainer.SetDropShadow(offsetX: 0, offsetY: 0, blurRadius: 6, color: "#ffae3e");

            SetContent(_imageContainer);

            IsometricDisplacement = Constants.DEFAULT_ISOMETRIC_DISPLACEMENT;
            Speed = Constants.DEFAULT_CONSTRUCT_SPEED + 1;
            DropShadowDistance = Constants.DEFAULT_DROP_SHADOW_DISTANCE - 15;

            _audioStub = new AudioStub((SoundType.CRACKER_DROP, 0.3, false), (SoundType.CRACKER_BLAST, 0.8, false), (SoundType.TRASH_CAN_HIT, 1, false));
        }

        #endregion

        #region Properties

        public bool IsBlasting { get; set; }

        private PlayerHonkBombTemplate HonkBombTemplate { get; set; }

        #endregion

        #region Methods

        public void Reset()
        {
            IsBlasting = false;

            var uri = ConstructExtensions.GetRandomContentUri(_bombUris);
            _imageContainer.SetSource(uri);

            _audioStub.Play(SoundType.CRACKER_DROP);

            SetOpacity(1);
            //BorderBrush = new SolidColorBrush(Colors.Transparent);

            SetScaleTransform(1);
            SetRotation(0);
        }

        public void SetHonkBombTemplate(PlayerHonkBombTemplate honkBombTemplate)
        {
            HonkBombTemplate = honkBombTemplate;

            switch (HonkBombTemplate)
            {
                case PlayerHonkBombTemplate.Cracker:
                    {
                        _bombUris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.PLAYER_HONK_BOMB && x.Uri.OriginalString.Contains("cracker")).Select(x => x.Uri).ToArray();
                    }
                    break;
                case PlayerHonkBombTemplate.TrashCan:
                    {
                        _bombUris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.PLAYER_HONK_BOMB && x.Uri.OriginalString.Contains("trash")).Select(x => x.Uri).ToArray();
                    }
                    break;
                default:
                    break;
            }

            var uri = ConstructExtensions.GetRandomContentUri(_bombUris);
            _imageContainer.SetSource(uri);
        }

        public void Reposition(PlayerBalloon player)
        {
            SetPosition(
                left: (player.GetLeft() + player.Width / 2) - Width / 2,
                top: player.GetBottom() - (35));
        }

        public void SetBlast()
        {
            Uri uri = null;

            switch (HonkBombTemplate)
            {
                case PlayerHonkBombTemplate.Cracker:
                    {
                        _audioStub.Play(SoundType.CRACKER_BLAST);
                        uri = ConstructExtensions.GetRandomContentUri(_blastUris);

                        //BorderBrush = new SolidColorBrush(Colors.Goldenrod);
                    }
                    break;
                case PlayerHonkBombTemplate.TrashCan:
                    {
                        _audioStub.Play(SoundType.TRASH_CAN_HIT);
                        uri = ConstructExtensions.GetRandomContentUri(_bangUris);

                        //BorderBrush = new SolidColorBrush(Colors.GreenYellow);
                    }
                    break;
                default:
                    break;
            }

            SetScaleTransform(Constants.DEFAULT_BLAST_SHRINK_SCALE);

            _imageContainer.SetSource(uri);
            IsBlasting = true;
        }

        #endregion
    }

    public enum PlayerHonkBombTemplate
    {
        Cracker,
        TrashCan,
    }
}
