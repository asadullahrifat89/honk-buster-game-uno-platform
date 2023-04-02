using Windows.Foundation;

namespace HonkBusterGame
{
    public partial class MafiaBoss : UfoBossBase
    {
        #region Fields

        private readonly Random _random;
        private readonly Uri[] _mafia_boss_idle_uris;
        private readonly Uri[] _mafia_boss_hit_uris;
        private readonly Uri[] _mafia_boss_win_uris;

        private readonly double _grace = 7;
        private readonly double _lag = 125;

        private double _changeMovementPatternDelay;

        private readonly ImageElement _content_image;

        private double _hitStanceDelay;
        private readonly double _hitStanceDelayDefault = 1.5;

        private double _winStanceDelay;
        private readonly double _winStanceDelayDefault = 8;

        private MovementDirection _movementDirection;

        #endregion

        #region Ctor

        public MafiaBoss(
           Action<Construct> animateAction,
           Action<Construct> recycleAction)
        {
            ConstructType = ConstructType.MAFIA_BOSS;

            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            _random = new Random();

            _mafia_boss_idle_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.MAFIA_BOSS_IDLE).Select(x => x.Uri).ToArray();
            _mafia_boss_hit_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.MAFIA_BOSS_HIT).Select(x => x.Uri).ToArray();
            _mafia_boss_win_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.MAFIA_BOSS_WIN).Select(x => x.Uri).ToArray();

            SetConstructSize(ConstructType);

            var uri = ConstructExtensions.GetRandomContentUri(_mafia_boss_idle_uris);
            _content_image = new(uri: uri, width: this.Width, height: this.Height);
            _content_image.SetDropShadow(offsetX: 0, offsetY: 0, blurRadius: 7, color: "#dc451c");

            SetChild(_content_image);

            IsometricDisplacement = Constants.DEFAULT_ISOMETRIC_DISPLACEMENT;
            DropShadowDistance = Constants.DEFAULT_DROP_SHADOW_DISTANCE;
        }

        #endregion

        #region Properties

        public MafiaBossMovementPattern MovementPattern { get; set; }

        private BossStance MafiaBossStance { get; set; } = BossStance.Idle;

        #endregion

        #region Methods

        public new void Reset()
        {
            base.Reset();

            MafiaBossStance = BossStance.Idle;

            var uri = ConstructExtensions.GetRandomContentUri(_mafia_boss_idle_uris);
            _content_image.SetSource(uri);

            RandomizeMovementPattern();
            SetScaleTransform(1);
        }

        public void SetHitStance()
        {
            if (MafiaBossStance != BossStance.Win)
            {
                MafiaBossStance = BossStance.Hit;

                var uri = ConstructExtensions.GetRandomContentUri(_mafia_boss_hit_uris);
                _content_image.SetSource(uri);

                _hitStanceDelay = _hitStanceDelayDefault;
            }
        }

        public void SetWinStance()
        {
            MafiaBossStance = BossStance.Win;
            var uri = ConstructExtensions.GetRandomContentUri(_mafia_boss_win_uris);
            _content_image.SetSource(uri);
            _winStanceDelay = _winStanceDelayDefault;
        }

        public void SetIdleStance()
        {
            MafiaBossStance = BossStance.Idle;
            var uri = ConstructExtensions.GetRandomContentUri(_mafia_boss_idle_uris);
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
                case MafiaBossMovementPattern.PLAYER_SEEKING:
                    SeekPlayer(playerPoint);
                    break;
                case MafiaBossMovementPattern.RECTANGULAR_SQUARE:
                    MoveInRectangularSquares(
                        speed: speed,
                        sceneWidth: sceneWidth,
                        sceneHeight: sceneHeight);
                    break;
                case MafiaBossMovementPattern.RIGHT_LEFT:
                    MoveRightLeft(
                        speed: speed,
                        sceneWidth: sceneWidth,
                        sceneHeight: sceneHeight);
                    break;
                case MafiaBossMovementPattern.UP_DOWN:
                    MoveUpDown(
                        speed: speed,
                        sceneWidth: sceneWidth,
                        sceneHeight: sceneHeight);
                    break;
            }
        }

        private bool MoveInRectangularSquares(double speed, double sceneWidth, double sceneHeight)
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

            if (_movementDirection == MovementDirection.Up)
            {
                MoveUp(speed);

                if (GetTop() - Height / 2 < 0)
                {
                    _movementDirection = MovementDirection.Right;
                }
            }
            else
            {
                if (_movementDirection == MovementDirection.Right)
                {
                    MoveRight(speed);

                    if (GetRight() > sceneWidth)
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
                            _movementDirection = MovementDirection.Left;
                        }
                    }
                    else
                    {
                        if (_movementDirection == MovementDirection.Left)
                        {
                            MoveLeft(speed);

                            if (GetLeft() - Width < 0)
                            {
                                _movementDirection = MovementDirection.Up;
                            }
                        }
                    }
                }
            }

            return false;
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
            MovementPattern = (MafiaBossMovementPattern)_random.Next(Enum.GetNames(typeof(MafiaBossMovementPattern)).Length);
        }

        #endregion
    }

    public enum MafiaBossMovementPattern
    {
        PLAYER_SEEKING,
        RECTANGULAR_SQUARE,
        RIGHT_LEFT,
        UP_DOWN,
    }
}
