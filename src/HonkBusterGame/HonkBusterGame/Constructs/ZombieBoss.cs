namespace HonkBusterGame
{
    public partial class ZombieBoss : UfoBossBase
    {
        #region Fields

        private readonly Random _random;
        private readonly Uri[] _zombieBossIdleUris;
        private readonly Uri[] _zombieBossHitUris;
        private readonly Uri[] _zombieBossWinUris;

        private double _changeMovementPatternDelay;

        private readonly ImageContainer _imageContainer;

        private double _hitStanceDelay;
        private readonly double _hitStanceDelayDefault = 1.5;

        private double _winStanceDelay;
        private readonly double _winStanceDelayDefault = 8;

        private MovementDirection _movementDirection;

        #endregion

        #region Ctor

        public ZombieBoss(
           Action<GameObject> animateAction,
           Action<GameObject> recycleAction)
        {
            ConstructType = ConstructType.ZOMBIE_BOSS;

            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            _random = new Random();

            _zombieBossIdleUris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.ZOMBIE_BOSS_IDLE).Select(x => x.Uri).ToArray();
            _zombieBossHitUris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.ZOMBIE_BOSS_HIT).Select(x => x.Uri).ToArray();
            _zombieBossWinUris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.ZOMBIE_BOSS_WIN).Select(x => x.Uri).ToArray();

            SetConstructSize(ConstructType);

            var uri = ConstructExtensions.GetRandomContentUri(_zombieBossIdleUris);
            _imageContainer = new(uri: uri, width: this.Width, height: this.Height);
            //_imageContainer.SetDropShadow(offsetX: 0, offsetY: 0, blurRadius: 7, color: "#544e36");

            SetContent(_imageContainer);

            IsometricDisplacement = Constants.DEFAULT_ISOMETRIC_DISPLACEMENT;
            DropShadowDistance = Constants.DEFAULT_DROP_SHADOW_DISTANCE;
        }

        #endregion

        #region Properties

        private BossStance ZombieBossStance { get; set; } = BossStance.Idle;

        #endregion

        #region Methods

        public new void Reset()
        {
            base.Reset();

            ZombieBossStance = BossStance.Idle;

            var uri = ConstructExtensions.GetRandomContentUri(_zombieBossIdleUris);
            _imageContainer.SetSource(uri);

            RandomizeMovementPattern();
            SetScaleTransform(1);
        }

        public void SetHitStance()
        {
            if (ZombieBossStance != BossStance.Win)
            {
                ZombieBossStance = BossStance.Hit;
                var uri = ConstructExtensions.GetRandomContentUri(_zombieBossHitUris);
                _imageContainer.SetSource(uri);
                _hitStanceDelay = _hitStanceDelayDefault;
            }
        }

        public void SetWinStance()
        {
            ZombieBossStance = BossStance.Win;
            var uri = ConstructExtensions.GetRandomContentUri(_zombieBossWinUris);
            _imageContainer.SetSource(uri);
            _winStanceDelay = _winStanceDelayDefault;
        }

        private void SetIdleStance()
        {
            ZombieBossStance = BossStance.Idle;
            var uri = ConstructExtensions.GetRandomContentUri(_zombieBossIdleUris);
            _imageContainer.SetSource(uri);
        }

        public void DepleteWinStance()
        {
            if (_winStanceDelay > 0)
            {
                _winStanceDelay -= 0.1;

                if (_winStanceDelay <= 0)
                {
                    SetIdleStance();
                }
            }
        }

        public void DepleteHitStance()
        {
            if (_hitStanceDelay > 0)
            {
                _hitStanceDelay -= 0.1;

                if (_hitStanceDelay <= 0)
                {
                    SetIdleStance();
                }
            }
        }

        public void Move(
           double speed,
           double sceneWidth,
           double sceneHeight)
        {
            MoveUpRightDownLeft(
                speed: speed,
                sceneWidth: sceneWidth,
                sceneHeight: sceneHeight);
        }

        private bool MoveUpRightDownLeft(double speed, double sceneWidth, double sceneHeight)
        {
            _changeMovementPatternDelay -= 0.1;

            if (_changeMovementPatternDelay < 0)
            {
                RandomizeMovementPattern();
                return true;
            }

            if (IsAttacking && _movementDirection == MovementDirection.None)
            {
                _movementDirection = MovementDirection.UpRight;
            }
            else
            {
                IsAttacking = true;
            }

            if (IsAttacking)
            {
                if (_movementDirection == MovementDirection.UpRight)
                {
                    MoveUpRight(speed);

                    if (GetTop() < 0 || GetLeft() > sceneWidth)
                    {
                        _movementDirection = MovementDirection.DownLeft;
                    }
                }
                else
                {
                    if (_movementDirection == MovementDirection.DownLeft)
                    {
                        MoveDownLeft(speed);

                        if (GetLeft() < 0 || GetBottom() > sceneHeight)
                        {
                            _movementDirection = MovementDirection.UpRight;
                        }
                    }
                }
            }

            return false;
        }

        private void RandomizeMovementPattern()
        {
            _changeMovementPatternDelay = _random.Next(40, 60);
            _movementDirection = MovementDirection.None;
            Speed = _random.Next(Constants.DEFAULT_CONSTRUCT_SPEED - 3, Constants.DEFAULT_CONSTRUCT_SPEED + 1);
        }

        #endregion
    }
}
