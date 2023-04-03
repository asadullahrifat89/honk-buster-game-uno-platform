namespace HonkBusterGame
{
    public partial class VehicleBoss : VehicleBossBase
    {
        #region Fields

        private readonly Random _random;

        private readonly Uri[] _vehicle_small_uris;
        private readonly Uri[] _vehicle_large_uris;

        private readonly ImageElement _content_image;
        private MovementDirection _movementDirection;

        private double _changeMovementPatternDelay;

        #endregion

        #region Ctor

        public VehicleBoss(
            Action<Construct> animateAction,
            Action<Construct> recycleAction)
        {
            ConstructType = ConstructType.VEHICLE_BOSS;

            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            _random = new Random();

            _vehicle_small_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.VEHICLE_ENEMY_SMALL).Select(x => x.Uri).ToArray();
            _vehicle_large_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.VEHICLE_ENEMY_LARGE).Select(x => x.Uri).ToArray();

            var vehicleType = _random.Next(2);
            Uri uri = null;

            switch (vehicleType)
            {
                case 0:
                    {
                        SetConstructSize(ConstructType.VEHICLE_ENEMY_SMALL);
                        uri = ConstructExtensions.GetRandomContentUri(_vehicle_small_uris);
                    }
                    break;
                case 1:
                    {
                        SetConstructSize(ConstructType.VEHICLE_ENEMY_LARGE);
                        uri = ConstructExtensions.GetRandomContentUri(_vehicle_large_uris);
                    }
                    break;
                default:
                    break;
            }

            _content_image = new(uri: uri, width: this.Width, height: this.Height);
            _content_image.SetDropShadow(offsetX: 0, offsetY: 10, blurRadius: 2);

            SetContent(_content_image);

            IsometricDisplacement = Constants.DEFAULT_ISOMETRIC_DISPLACEMENT;
        }

        #endregion

        #region Properties      

        public VehicleBossMovementPattern MovementPattern { get; set; }

        #endregion

        #region Methods

        public new void Reset()
        {
            base.Reset();

            var vehicleType = _random.Next(2);
            Uri uri = null;

            switch (vehicleType)
            {
                case 0:
                    {
                        SetConstructSize(ConstructType.VEHICLE_ENEMY_SMALL);
                        uri = ConstructExtensions.GetRandomContentUri(_vehicle_small_uris);
                    }
                    break;
                case 1:
                    {
                        SetConstructSize(ConstructType.VEHICLE_ENEMY_LARGE);
                        uri = ConstructExtensions.GetRandomContentUri(_vehicle_large_uris);
                    }
                    break;
                default:
                    break;
            }

            _content_image.SetSource(uri);

            RandomizeMovementPattern();
            SetScaleTransform(1);
            SetHonkDelay();
        }

        private void RandomizeMovementPattern()
        {
            Speed = _random.Next(Constants.DEFAULT_CONSTRUCT_SPEED, Constants.DEFAULT_CONSTRUCT_SPEED + 2);
            MovementPattern = (VehicleBossMovementPattern)_random.Next(Enum.GetNames(typeof(VehicleBossMovementPattern)).Length);

            _changeMovementPatternDelay = _random.Next(40, 60);
            _movementDirection = MovementDirection.None;
        }

        public void Move(
           double speed,
           double sceneWidth,
           double sceneHeight)
        {
            MoveUpLeftDownRight(
                speed: speed,
                sceneWidth: sceneWidth,
                sceneHeight: sceneHeight);
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

                    if (GetBottom() < 0 || GetRight() < 0)
                    {
                        Reposition();
                        _movementDirection = MovementDirection.DownRight;
                    }
                }
                else
                {
                    if (_movementDirection == MovementDirection.DownRight)
                    {
                        MoveDownRight(speed);

                        if (GetLeft() > sceneWidth || GetTop() > sceneHeight)
                        {
                            _movementDirection = MovementDirection.UpLeft;
                        }
                    }
                }
            }

            return false;
        }

        #endregion
    }

    public enum VehicleBossMovementPattern
    {
        ISOMETRIC_SQUARE,
        UPLEFT_DOWNRIGHT,
    }
}
