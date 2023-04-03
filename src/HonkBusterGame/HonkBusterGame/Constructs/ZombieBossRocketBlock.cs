namespace HonkBusterGame
{
    public partial class ZombieBossRocketBlock : AnimableHealthyBase
    {
        #region Fields

        private readonly Random _random;

        private readonly Uri[] _bomb_uris;
        private readonly Uri[] _bomb_blast_uris;

        private readonly ImageContainer _imageContainer;

        private double _autoBlastDelay;
        private readonly double _autoBlastDelayDefault = 14;

        private readonly AudioStub _audioStub;

        #endregion

        #region Ctor

        public ZombieBossRocketBlock(
          Action<GameObject> animateAction,
          Action<GameObject> recycleAction)
        {
            ConstructType = ConstructType.ZOMBIE_BOSS_ROCKET_BLOCK;

            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            _random = new Random();

            _bomb_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.ZOMBIE_BOSS_ROCKET_BLOCK).Select(x => x.Uri).ToArray();
            _bomb_blast_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.BLAST).Select(x => x.Uri).ToArray();

            SetConstructSize(ConstructType);

            var uri = ConstructExtensions.GetRandomContentUri(_bomb_uris);
            _imageContainer = new(uri: uri, width: this.Width, height: this.Height);
            _imageContainer.SetDropShadow(offsetX: 0, offsetY: 0, blurRadius: 6, color: "#ffd11a");

            SetContent(_imageContainer);

            //BorderThickness = new Microsoft.UI.Xaml.Thickness(Constants.DEFAULT_BLAST_RING_BORDER_THICKNESS);
            //CornerRadius = new Microsoft.UI.Xaml.CornerRadius(Constants.DEFAULT_BLAST_RING_CORNER_RADIUS * Width);

            IsometricDisplacement = Constants.DEFAULT_ISOMETRIC_DISPLACEMENT;
            DropShadowDistance = Constants.DEFAULT_DROP_SHADOW_DISTANCE + 10;

            _audioStub = new AudioStub((SoundType.ORB_LAUNCH, 0.3, false), (SoundType.ROCKET_BLAST, 1, false));
        }

        #endregion

        #region Properties

        public bool IsDestroyed => Health <= 0;

        #endregion

        #region Methods

        public void Reset()
        {
            _audioStub.Play(SoundType.ORB_LAUNCH);

            SetOpacity(1);
            SetScaleTransform(1);

            var uri = ConstructExtensions.GetRandomContentUri(_bomb_uris);
            _imageContainer.SetSource(uri);

            //BorderBrush = new SolidColorBrush(Colors.Transparent);

            Health = HitPoint * _random.Next(3);
            Speed = Constants.DEFAULT_CONSTRUCT_SPEED + 1.5;

            IsBlasting = false;

            _autoBlastDelay = _autoBlastDelayDefault;
        }

        public void Reposition()
        {
            var topOrLeft = _random.Next(2); // generate top and left corner lane wise vehicles
            var lane = ScreenExtensions.Height < 450 ? _random.Next(3) : _random.Next(4); // generate number of lanes based on screen height
            var randomY = _random.Next(-10, 10);

            switch (topOrLeft)
            {
                case 0:
                    {
                        var xLaneWidth = Constants.DEFAULT_SCENE_WIDTH / 4;

                        switch (lane)
                        {
                            case 0:
                                {
                                    SetPosition(
                                        left: 0 - Width / 2,
                                        top: (Height * -1) + randomY);
                                }
                                break;
                            case 1:
                                {
                                    SetPosition(
                                        left: (xLaneWidth - Width / 1.5),
                                        top: (Height * -1) + randomY);
                                }
                                break;
                            case 2:
                                {
                                    SetPosition(
                                       left: (xLaneWidth * 2 - Width / 1.5),
                                       top: (Height * -1) + randomY);
                                }
                                break;
                            case 3:
                                {
                                    SetPosition(
                                       left: (xLaneWidth * 3 - Width / 1.5),
                                       top: (Height * -1) + randomY);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case 1:
                    {
                        var yLaneHeight = Constants.DEFAULT_SCENE_HEIGHT / 6;

                        switch (lane)
                        {
                            case 0:
                                {
                                    SetPosition(
                                        left: Width * -1,
                                        top: (0 - Height / 2) + randomY);
                                }
                                break;
                            case 1:
                                {
                                    SetPosition(
                                        left: Width * -1,
                                        top: (yLaneHeight - Height / 3) + randomY);
                                }
                                break;
                            case 2:
                                {
                                    SetPosition(
                                       left: Width * -1,
                                       top: (yLaneHeight * 2 - Height / 3) + randomY);
                                }
                                break;
                            case 3:
                                {
                                    SetPosition(
                                       left: Width * -1,
                                       top: (yLaneHeight * 3 - Height / 3) + randomY);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public void LooseHealth()
        {
            Health -= HitPoint;

            if (IsDestroyed)
            {
                SetBlast();
            }
        }

        public void SetBlast()
        {
            _audioStub.Play(SoundType.ROCKET_BLAST);
            Speed = Constants.DEFAULT_CONSTRUCT_SPEED - 1;

            SetScaleTransform(Constants.DEFAULT_BLAST_SHRINK_SCALE - 0.2);

            //BorderBrush = new SolidColorBrush(Colors.DarkGreen);

            var uri = ConstructExtensions.GetRandomContentUri(_bomb_blast_uris);
            _imageContainer.SetSource(uri);

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
