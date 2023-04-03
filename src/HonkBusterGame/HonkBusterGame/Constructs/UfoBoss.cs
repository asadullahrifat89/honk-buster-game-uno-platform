using Windows.Foundation;

namespace HonkBusterGame
{
    public partial class UfoBoss : UfoBossBase
    {
        #region Fields

        private readonly Random _random;
        private readonly Uri[] _ufo_boss_idle_uris;
        private readonly Uri[] _ufo_boss_hit_uris;
        private readonly Uri[] _ufo_boss_win_uris;

        private readonly double _grace = 7;
        private readonly double _lag = 125;

        private double _changeMovementPatternDelay;

        private readonly ImageContainer _content_image;

        private double _hitStanceDelay;
        private readonly double _hitStanceDelayDefault = 1.5;

        private double _winStanceDelay;
        private readonly double _winStanceDelayDefault = 8;

        private MovementDirection _movementDirection;

        #endregion

        #region Ctor

        public UfoBoss(
            Action<GameObject> animateAction,
            Action<GameObject> recycleAction)
        {
            ConstructType = ConstructType.UFO_BOSS;

            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            _random = new Random();

            _ufo_boss_idle_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.UFO_BOSS_IDLE).Select(x => x.Uri).ToArray();
            _ufo_boss_hit_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.UFO_BOSS_HIT).Select(x => x.Uri).ToArray();
            _ufo_boss_win_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.UFO_BOSS_WIN).Select(x => x.Uri).ToArray();

            SetConstructSize(ConstructType);

            var uri = ConstructExtensions.GetRandomContentUri(_ufo_boss_idle_uris);
            _content_image = new(uri: uri, width: this.Width, height: this.Height);
            //_content_image.SetDropShadow(offsetX: 0, offsetY: 0, blurRadius: 7, color: "#f73e3e");

            SetContent(_content_image);

            IsometricDisplacement = Constants.DEFAULT_ISOMETRIC_DISPLACEMENT;
            DropShadowDistance = Constants.DEFAULT_DROP_SHADOW_DISTANCE;
        }

        #endregion

        #region Properties

        public UfoBossMovementPattern MovementPattern { get; set; }

        private BossStance UfoBossStance { get; set; } = BossStance.Idle;

        #endregion

        #region Methods

        public new void Reset()
        {
            base.Reset();

            UfoBossStance = BossStance.Idle;

            var uri = ConstructExtensions.GetRandomContentUri(_ufo_boss_idle_uris);
            _content_image.SetSource(uri);

            RandomizeMovementPattern();
            SetScaleTransform(1);
        }

        public void SetHitStance()
        {
            if (UfoBossStance != BossStance.Win)
            {
                UfoBossStance = BossStance.Hit;

                var uri = ConstructExtensions.GetRandomContentUri(_ufo_boss_hit_uris);
                _content_image.SetSource(uri);

                _hitStanceDelay = _hitStanceDelayDefault;
            }
        }

        public void SetWinStance()
        {
            UfoBossStance = BossStance.Win;
            var uri = ConstructExtensions.GetRandomContentUri(_ufo_boss_win_uris);
            _content_image.SetSource(uri);
            _winStanceDelay = _winStanceDelayDefault;
        }

        public void SetIdleStance()
        {
            UfoBossStance = BossStance.Idle;
            var uri = ConstructExtensions.GetRandomContentUri(_ufo_boss_idle_uris);
            _content_image.SetSource(uri);
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
            double sceneHeight,
            Rect playerPoint)
        {
            switch (MovementPattern)
            {
                case UfoBossMovementPattern.PLAYER_SEEKING:
                    SeekPlayer(playerPoint);
                    break;
                case UfoBossMovementPattern.ISOMETRIC_SQUARE:
                    MoveInIsometricSquares(
                        speed: speed,
                        sceneWidth: sceneWidth,
                        sceneHeight: sceneHeight);
                    break;
                case UfoBossMovementPattern.UPRIGHT_DOWNLEFT:
                    MoveUpRightDownLeft(
                        speed: speed,
                        sceneWidth: sceneWidth,
                        sceneHeight: sceneHeight);
                    break;
                case UfoBossMovementPattern.UPLEFT_DOWNRIGHT:
                    MoveUpLeftDownRight(
                        speed: speed,
                        sceneWidth: sceneWidth,
                        sceneHeight: sceneHeight);
                    break;
                case UfoBossMovementPattern.RIGHT_LEFT:
                    MoveRightLeft(
                        speed: speed,
                        sceneWidth: sceneWidth,
                        sceneHeight: sceneHeight);
                    break;
                case UfoBossMovementPattern.UP_DOWN:
                    MoveUpDown(
                        speed: speed,
                        sceneWidth: sceneWidth,
                        sceneHeight: sceneHeight);
                    break;
            }
        }

        private void SeekPlayer(Rect playerPoint)
        {
            _changeMovementPatternDelay -= 0.1;

            if (_changeMovementPatternDelay < 0)
            {
                RandomizeMovementPattern();
            }

            double left = GetLeft();
            double top = GetTop();

            double playerMiddleX = left + Width / 2;
            double playerMiddleY = top + Height / 2;

            // move up
            if (playerPoint.Y < playerMiddleY - _grace)
            {
                var distance = Math.Abs(playerPoint.Y - playerMiddleY);
                double speed = GetFlightSpeed(distance);

                SetTop(top - speed);
            }

            // move left
            if (playerPoint.X < playerMiddleX - _grace)
            {
                var distance = Math.Abs(playerPoint.X - playerMiddleX);
                double speed = GetFlightSpeed(distance);

                SetLeft(left - speed);
            }

            // move down
            if (playerPoint.Y > playerMiddleY + _grace)
            {
                var distance = Math.Abs(playerPoint.Y - playerMiddleY);
                double speed = GetFlightSpeed(distance);

                SetTop(top + speed);
            }

            // move right
            if (playerPoint.X > playerMiddleX + _grace)
            {
                var distance = Math.Abs(playerPoint.X - playerMiddleX);
                double speed = GetFlightSpeed(distance);

                SetLeft(left + speed);
            }
        }

        private double GetFlightSpeed(double distance)
        {
            var flightSpeed = distance / _lag;
            return flightSpeed;
        }

        private bool MoveInIsometricSquares(double speed, double sceneWidth, double sceneHeight)
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

                    if (GetTop() < 0)
                    {
                        _movementDirection = MovementDirection.DownRight;
                    }
                }
                else
                {
                    if (_movementDirection == MovementDirection.DownRight)
                    {
                        MoveDownRight(speed);

                        if (GetRight() > sceneWidth || GetBottom() > sceneHeight)
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
                                _movementDirection = MovementDirection.UpLeft;
                            }
                        }
                        else
                        {
                            if (_movementDirection == MovementDirection.UpLeft)
                            {
                                MoveUpLeft(speed);

                                if (GetTop() < 0 || GetLeft() < 0)
                                {
                                    _movementDirection = MovementDirection.UpRight;
                                }
                            }
                        }
                    }
                }
            }

            return false;
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

        private bool MoveUpLeftDownRight(double speed, double sceneWidth, double sceneHeight)
        {
            _changeMovementPatternDelay -= 0.1;

            if (_changeMovementPatternDelay < 0)
            {
                RandomizeMovementPattern();
                return true;
            }

            if (IsAttacking && _movementDirection == MovementDirection.None)
            {
                _movementDirection = MovementDirection.UpLeft;
            }
            else
            {
                IsAttacking = true;
            }

            if (IsAttacking)
            {
                if (_movementDirection == MovementDirection.UpLeft)
                {
                    MoveUpLeft(speed);

                    if (GetTop() < 0 || GetLeft() < 0)
                    {
                        _movementDirection = MovementDirection.DownRight;
                    }
                }
                else
                {
                    if (_movementDirection == MovementDirection.DownRight)
                    {
                        MoveDownRight(speed);

                        if (GetRight() > sceneWidth || GetBottom() > sceneHeight)
                        {
                            _movementDirection = MovementDirection.UpLeft;
                        }
                    }
                }
            }

            return false;
        }

        private bool MoveRightLeft(double speed, double sceneWidth, double sceneHeight)
        {
            _changeMovementPatternDelay -= 0.1;

            if (_changeMovementPatternDelay < 0)
            {
                RandomizeMovementPattern();
                return true;
            }

            if (IsAttacking && _movementDirection == MovementDirection.None)
            {
                _movementDirection = MovementDirection.Right;
            }
            else
            {
                IsAttacking = true;
            }

            if (IsAttacking)
            {
                if (_movementDirection == MovementDirection.Right)
                {
                    MoveRight(speed);

                    if (GetRight() > sceneWidth)
                    {
                        _movementDirection = MovementDirection.Left;
                    }
                }
                else
                {
                    if (_movementDirection == MovementDirection.Left)
                    {
                        MoveLeft(speed);

                        if (GetLeft() < 0)
                        {
                            _movementDirection = MovementDirection.Right;
                        }
                    }
                }
            }

            return false;
        }

        private bool MoveUpDown(double speed, double sceneWidth, double sceneHeight)
        {
            _changeMovementPatternDelay -= 0.1;

            if (_changeMovementPatternDelay < 0)
            {
                RandomizeMovementPattern();
                return true;
            }

            if (IsAttacking && _movementDirection == MovementDirection.None)
            {
                _movementDirection = MovementDirection.Up;
            }
            else
            {
                IsAttacking = true;
            }

            if (IsAttacking)
            {
                if (_movementDirection == MovementDirection.Up)
                {
                    MoveUp(speed);

                    if (GetTop() - Height / 2 < 0)
                    {
                        _movementDirection = MovementDirection.Down;
                    }
                }
                else
                {
                    if (_movementDirection == MovementDirection.Down)
                    {
                        MoveDown(speed);

                        if (GetBottom() > sceneHeight)
                        {
                            _movementDirection = MovementDirection.Up;
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
            MovementPattern = (UfoBossMovementPattern)_random.Next(Enum.GetNames(typeof(UfoBossMovementPattern)).Length);
        }

        #endregion
    }

    public enum UfoBossMovementPattern
    {
        PLAYER_SEEKING,
        ISOMETRIC_SQUARE,
        UPRIGHT_DOWNLEFT,
        UPLEFT_DOWNRIGHT,
        RIGHT_LEFT,
        UP_DOWN,
    }

    public enum BossStance
    {
        Idle,
        Hit,
        Win,
    }
}
