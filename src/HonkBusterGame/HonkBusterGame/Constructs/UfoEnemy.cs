namespace HonkBusterGame
{
    public partial class UfoEnemy : VehicleBase
    {
        #region Fields

        private readonly Random _random;
        private readonly Uri[] _enemy_uris;

        private readonly ImageContainer _content_image;

        private double _attackDelay;

        #endregion

        #region Ctor

        public UfoEnemy(
            Action<GameObject> animateAction,
            Action<GameObject> recycleAction)
        {
            ConstructType = ConstructType.UFO_ENEMY;

            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            _random = new Random();

            _enemy_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.UFO_ENEMY).Select(x => x.Uri).ToArray();

            SetConstructSize(ConstructType);

            var uri = ConstructExtensions.GetRandomContentUri(_enemy_uris);
            _content_image = new(uri: uri, width: this.Width, height: this.Height);
            //_content_image.SetDropShadow(offsetX: 0, offsetY: 0, blurRadius: 6, color: "#956ec4");

            SetContent(_content_image);

            IsometricDisplacement = Constants.DEFAULT_ISOMETRIC_DISPLACEMENT;
            DropShadowDistance = Constants.DEFAULT_DROP_SHADOW_DISTANCE;
        }

        #endregion     

        #region Methods

        public void Reset()
        {
            SetOpacity(1);
            SetScaleTransform(1);

            Health = HitPoint * _random.Next(4);

            WillHonk = Convert.ToBoolean(_random.Next(2));

            if (WillHonk)
                SetHonkDelay();

            var uri = ConstructExtensions.GetRandomContentUri(_enemy_uris);
            _content_image.SetSource(uri);

            Speed = _random.Next(Constants.DEFAULT_CONSTRUCT_SPEED - 2, Constants.DEFAULT_CONSTRUCT_SPEED);
        }

        public bool Attack()
        {
            if (!IsDead)
            {
                _attackDelay--;

                if (_attackDelay < 0)
                {
                    SetAttackDelay();
                    return true;
                }
            }

            return false;
        }

        public void SetAttackDelay()
        {
            _attackDelay = _random.Next(50, 80);
        }

        public void LooseHealth()
        {
            Health -= 5;
        }

        #endregion
    }
}
