namespace HonkBusterGame
{
    public partial class PlayerBalloon : HealthyObject
    {
        #region Fields

        private Uri[] _player_uris;
        private Uri[] _player_attack_uris;
        private Uri[] _player_win_uris;
        private Uri[] _player_hit_uris;

        private double _movementStopDelay;
        private readonly double _movementStopDelayDefault = 6;
        private readonly double _movementStopSpeedLoss = 0.5;

        private double _lastSpeed;
        private readonly double _rotationThreadhold = 9;
        private readonly double _unrotationSpeed = 1.1;
        private readonly double _rotationSpeed = 0.5;
        private double _attackStanceDelay;
        private readonly double _attackStanceDelayDefault = 1.5;

        private double _winStanceDelay;
        private readonly double _winStanceDelayDefault = 15;

        private double _hitStanceDelay;
        private readonly double _hitStanceDelayDefault = 1.5;

        private readonly ImageContainer _content_image;
        private readonly AudioStub _audioStub;

        private MovementDirection _movementDirection;

        private double _health_loss_recovery_Delay;
        private int _health_loss_opacity_effect;

        #endregion

        #region Ctor

        public PlayerBalloon(
            Action<GameObject> animateAction,
            Action<GameObject> recycleAction)
        {
            ConstructType = ConstructType.PLAYER_BALLOON;

            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            SetConstructSize(ConstructType);

            _player_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.PLAYER_BALLOON_IDLE).Select(x => x.Uri).ToArray();

            var uri = ConstructExtensions.GetRandomContentUri(_player_uris);
            _content_image = new(uri: uri, width: this.Width, height: this.Height);

            SetContent(_content_image);

            IsometricDisplacement = Constants.DEFAULT_ISOMETRIC_DISPLACEMENT;
            Speed = Constants.DEFAULT_CONSTRUCT_SPEED;
            DropShadowDistance = Constants.DEFAULT_DROP_SHADOW_DISTANCE;

            _audioStub = new AudioStub((SoundType.PLAYER_HEALTH_LOSS, 1, false));
        }

        #endregion

        #region Properties

        public PlayerBalloonStance PlayerBalloonStance { get; set; } = PlayerBalloonStance.Idle;

        #endregion

        #region Methods

        public void Reset()
        {
            Health = 100;

            _movementDirection = MovementDirection.None;
            _movementStopDelay = _movementStopDelayDefault;
            _lastSpeed = 0;

            SetRotation(0);
        }

        public void Reposition()
        {
            var scaling = ScreenExtensions.GetScreenSpaceScaling();

            SetPosition(
               left: ((Constants.DEFAULT_SCENE_WIDTH / 2 - Width / 2) * scaling),
               top: ((Constants.DEFAULT_SCENE_HEIGHT / 2 - Height / 2) * scaling) - 150);
        }

        public void SetPlayerTemplate(PlayerBalloonTemplate playerTemplate)
        {
            switch (playerTemplate)
            {
                case PlayerBalloonTemplate.Blue:
                    {
                        _player_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.PLAYER_BALLOON_IDLE && x.Uri.OriginalString.Contains("1")).Select(x => x.Uri).ToArray();
                        _player_win_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.PLAYER_BALLOON_WIN && x.Uri.OriginalString.Contains("1")).Select(x => x.Uri).ToArray();
                        _player_hit_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.PLAYER_BALLOON_HIT && x.Uri.OriginalString.Contains("1")).Select(x => x.Uri).ToArray();
                        _player_attack_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.PLAYER_BALLOON_ATTACK && x.Uri.OriginalString.Contains("1")).Select(x => x.Uri).ToArray();
                        //_content_image.SetDropShadow(offsetX: 0, offsetY: 0, blurRadius: 5, color: "#5fc4f8");
                    }
                    break;
                case PlayerBalloonTemplate.Red:
                    {
                        _player_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.PLAYER_BALLOON_IDLE && x.Uri.OriginalString.Contains("2")).Select(x => x.Uri).ToArray();
                        _player_win_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.PLAYER_BALLOON_WIN && x.Uri.OriginalString.Contains("2")).Select(x => x.Uri).ToArray();
                        _player_hit_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.PLAYER_BALLOON_HIT && x.Uri.OriginalString.Contains("2")).Select(x => x.Uri).ToArray();
                        _player_attack_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.PLAYER_BALLOON_ATTACK && x.Uri.OriginalString.Contains("2")).Select(x => x.Uri).ToArray();
                        //_content_image.SetDropShadow(offsetX: 0, offsetY: 0, blurRadius: 5, color: "#d75e72");
                    }
                    break;
                default:
                    break;
            }

            var uri = ConstructExtensions.GetRandomContentUri(_player_uris);
            _content_image.SetSource(uri);
        }

        public void SetAttackStance()
        {
            PlayerBalloonStance = PlayerBalloonStance.Attack;

            var uri = ConstructExtensions.GetRandomContentUri(_player_attack_uris);
            _content_image.SetSource(uri);

            _attackStanceDelay = _attackStanceDelayDefault;
        }

        public void SetWinStance()
        {
            PlayerBalloonStance = PlayerBalloonStance.Win;

            var uri = ConstructExtensions.GetRandomContentUri(_player_win_uris);
            _content_image.SetSource(uri);

            _winStanceDelay = _winStanceDelayDefault;
        }

        public void SetHitStance()
        {
            if (PlayerBalloonStance != PlayerBalloonStance.Win)
            {
                PlayerBalloonStance = PlayerBalloonStance.Hit;

                var uri = ConstructExtensions.GetRandomContentUri(_player_hit_uris);
                _content_image.SetSource(uri);

                _hitStanceDelay = _hitStanceDelayDefault;
            }
        }

        private void SetIdleStance()
        {
            PlayerBalloonStance = PlayerBalloonStance.Idle;

            var uri = ConstructExtensions.GetRandomContentUri(_player_uris);
            _content_image.SetSource(uri);
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

        public void DepleteAttackStance()
        {
            if (_attackStanceDelay > 0)
            {
                _attackStanceDelay -= 0.1;

                if (_attackStanceDelay <= 0)
                {
                    SetIdleStance();
                }
            }
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

        public new void MoveUp(double speed)
        {
            _movementDirection = MovementDirection.Up;

            base.MoveUp(speed);

            _movementStopDelay = _movementStopDelayDefault;
            _lastSpeed = speed;

            Rotate(
                rotationDirection: RotationDirection.Backward,
                threadhold: _rotationThreadhold,
                rotationSpeed: _rotationSpeed);
        }

        public new void MoveDown(double speed)
        {
            _movementDirection = MovementDirection.Down;

            base.MoveDown(speed);

            _movementStopDelay = _movementStopDelayDefault;
            _lastSpeed = speed;

            Rotate(
                rotationDirection: RotationDirection.Forward,
                threadhold: _rotationThreadhold,
                rotationSpeed: _rotationSpeed);
        }

        public new void MoveLeft(double speed)
        {
            _movementDirection = MovementDirection.Left;

            base.MoveLeft(speed);

            _movementStopDelay = _movementStopDelayDefault;
            _lastSpeed = speed;

            Rotate(
                rotationDirection: RotationDirection.Backward,
                threadhold: _rotationThreadhold,
                rotationSpeed: _rotationSpeed);
        }

        public new void MoveRight(double speed)
        {
            _movementDirection = MovementDirection.Right;

            base.MoveRight(speed);

            _movementStopDelay = _movementStopDelayDefault;
            _lastSpeed = speed;

            Rotate(
                rotationDirection: RotationDirection.Forward,
                threadhold: _rotationThreadhold,
                rotationSpeed: _rotationSpeed);
        }

        public new void MoveUpRight(double speed)
        {
            _movementDirection = MovementDirection.UpRight;

            base.MoveUpRight(speed);

            _movementStopDelay = _movementStopDelayDefault;
            _lastSpeed = speed;

            Rotate(
                rotationDirection: RotationDirection.Forward,
                threadhold: _rotationThreadhold,
                rotationSpeed: _rotationSpeed);
        }

        public new void MoveUpLeft(double speed)
        {
            _movementDirection = MovementDirection.UpLeft;

            base.MoveUpLeft(speed);

            _movementStopDelay = _movementStopDelayDefault;
            _lastSpeed = speed;

            Rotate(
                rotationDirection: RotationDirection.Backward,
                threadhold: _rotationThreadhold,
                rotationSpeed: _rotationSpeed);
        }

        public new void MoveDownRight(double speed)
        {
            _movementDirection = MovementDirection.DownRight;

            base.MoveDownRight(speed);

            _movementStopDelay = _movementStopDelayDefault;
            _lastSpeed = speed;

            Rotate(
                rotationDirection: RotationDirection.Forward,
                threadhold: _rotationThreadhold,
                rotationSpeed: _rotationSpeed);
        }

        public new void MoveDownLeft(double speed)
        {
            _movementDirection = MovementDirection.DownLeft;

            base.MoveDownLeft(speed);

            _movementStopDelay = _movementStopDelayDefault;
            _lastSpeed = speed;

            Rotate(
                rotationDirection: RotationDirection.Backward,
                threadhold: _rotationThreadhold,
                rotationSpeed: _rotationSpeed);
        }

        public void Move(
            double speed,
            double sceneWidth,
            double sceneHeight,
            GameController controller)
        {
            var halfHeight = Height / 2;
            var halfWidth = Width / 2;

            if (controller.IsMoveUp && controller.IsMoveLeft)
            {
                if (GetTop() + halfHeight > 0 && GetLeft() + halfWidth > 0)
                    MoveUpLeft(speed);
            }
            else if (controller.IsMoveUp && controller.IsMoveRight)
            {
                if (GetLeft() - halfWidth < sceneWidth && GetTop() + halfHeight > 0)
                    MoveUpRight(speed);
            }
            else if (controller.IsMoveUp)
            {
                if (GetTop() + halfHeight > 0)
                    MoveUp(speed);
            }
            else if (controller.IsMoveDown && controller.IsMoveRight)
            {
                if (GetBottom() - halfHeight < sceneHeight && GetLeft() - halfWidth < sceneWidth)
                    MoveDownRight(speed);
            }
            else if (controller.IsMoveDown && controller.IsMoveLeft)
            {
                if (GetLeft() + halfWidth > 0 && GetBottom() - halfHeight < sceneHeight)
                    MoveDownLeft(speed);
            }
            else if (controller.IsMoveDown)
            {
                if (GetBottom() - halfHeight < sceneHeight)
                    MoveDown(speed);
            }
            else if (controller.IsMoveRight)
            {
                if (GetLeft() - halfWidth < sceneWidth)
                    MoveRight(speed);
            }
            else if (controller.IsMoveLeft)
            {
                if (GetLeft() + halfWidth > 0)
                    MoveLeft(speed);
            }
            else
            {
                StopMovement();
            }
        }

        public void StopMovement()
        {
            if (_movementStopDelay > 0)
            {
                _movementStopDelay--;

                double movementSpeedLoss = _movementStopSpeedLoss;

                if (_lastSpeed > 0)
                {
                    switch (_movementDirection)
                    {
                        case MovementDirection.None:
                            break;
                        case MovementDirection.Up:
                            MoveUp(_lastSpeed - movementSpeedLoss);
                            break;
                        case MovementDirection.UpLeft:
                            MoveUpLeft(_lastSpeed - movementSpeedLoss);
                            break;
                        case MovementDirection.UpRight:
                            MoveUpRight(_lastSpeed - movementSpeedLoss);
                            break;
                        case MovementDirection.Down:
                            MoveDown(_lastSpeed - movementSpeedLoss);
                            break;
                        case MovementDirection.DownLeft:
                            MoveDownLeft(_lastSpeed - movementSpeedLoss);
                            break;
                        case MovementDirection.DownRight:
                            MoveDownRight(_lastSpeed - movementSpeedLoss);
                            break;
                        case MovementDirection.Right:
                            MoveRight(_lastSpeed - movementSpeedLoss);
                            break;
                        case MovementDirection.Left:
                            MoveLeft(_lastSpeed - movementSpeedLoss);
                            break;
                        default:
                            break;
                    }
                }

                UnRotate(rotationSpeed: _unrotationSpeed);
            }
            else
            {
                _movementDirection = MovementDirection.None;
            }
        }

        public void LooseHealth()
        {
            if (_health_loss_recovery_Delay <= 0) // only loose health if recovery delay is less that 0 as upon taking damage this is set to 10
            {
                //TODO: set default  Health -= HitPoint;
                Health -= HitPoint;
                SetOpacity(0.7);

                _audioStub.Play(SoundType.PLAYER_HEALTH_LOSS);

                _health_loss_recovery_Delay = 10;
            }
        }

        public void RecoverFromHealthLoss()
        {
            if (_health_loss_recovery_Delay > 0)
            {
                _health_loss_recovery_Delay -= 0.1;

                _health_loss_opacity_effect++; // blinking effect

                if (_health_loss_opacity_effect > 2)
                {
                    if (Opacity != 1)
                    {
                        SetOpacity(1);
                    }
                    else
                    {
                        SetOpacity(0.7);  
                    }

                    _health_loss_opacity_effect = 0;
                }
            }

            if (_health_loss_recovery_Delay <= 0 && Opacity != 1)
                SetOpacity(1);
        }

        public void GainHealth()
        {
            Health += HitPoint * 3;
        }

        #endregion        
    }

    public enum MovementDirection
    {
        None,

        Up,
        UpLeft,
        UpRight,

        Down,
        DownLeft,
        DownRight,

        Right,
        Left,
    }

    public enum PlayerBalloonStance
    {
        Idle,
        Attack,
        Hit,
        Win,
    }

    public enum PlayerBalloonTemplate
    {
        Blue,
        Red,
    }
}
