using Windows.Foundation;

namespace HonkBusterGame
{
    public partial class MafiaBossRocketBullsEye : SeekingRocketBase
    {
        #region Fields

        private readonly Uri[] _bombUris;
        private readonly Uri[] _bombBlastUris;

        private readonly ImageContainer _imageContainer;

        private double _autoBlastDelay;
        private readonly double _autoBlastDelayDefault = 15;

        private readonly AudioStub _audioStub;

        private Rect _targetHitbox;

        #endregion

        #region Ctor

        public MafiaBossRocketBullsEye(
            Action<GameObject> animateAction,
            Action<GameObject> recycleAction)
        {
            ConstructType = ConstructType.MAFIA_BOSS_ROCKET_BULLS_EYE;

            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            _bombUris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.MAFIA_BOSS_ROCKET_BULLS_EYE).Select(x => x.Uri).ToArray();
            _bombBlastUris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.BLAST).Select(x => x.Uri).ToArray();

            SetConstructSize(ConstructType);

            var uri = ConstructExtensions.GetRandomContentUri(_bombUris);
            _imageContainer = new(uri: uri, width: this.Width, height: this.Height);
            _imageContainer.SetDropShadow(offsetX: 0, offsetY: 0, blurRadius: 6, color: "#4b6ca5");

            SetContent(_imageContainer);

            //BorderThickness = new Microsoft.UI.Xaml.Thickness(Constants.DEFAULT_BLAST_RING_BORDER_THICKNESS);
            //CornerRadius = new Microsoft.UI.Xaml.CornerRadius(Constants.DEFAULT_BLAST_RING_CORNER_RADIUS);

            Speed = Constants.DEFAULT_CONSTRUCT_SPEED;
            IsometricDisplacement = Constants.DEFAULT_ISOMETRIC_DISPLACEMENT;
            DropShadowDistance = Constants.DEFAULT_DROP_SHADOW_DISTANCE;

            _audioStub = new AudioStub((SoundType.BULLS_EYE_ROCKET_LAUNCH, 0.5, false), (SoundType.ROCKET_BLAST, 1, false));
        }

        #endregion

        #region Properties

        public bool IsBlasting { get; set; }

        #endregion

        #region Methods

        public void Reset()
        {
            _audioStub.Play(SoundType.BULLS_EYE_ROCKET_LAUNCH);

            SetOpacity(1);

            var uri = ConstructExtensions.GetRandomContentUri(_bombUris);
            _imageContainer.SetSource(uri);

            //BorderBrush = new SolidColorBrush(Colors.Transparent);

            SetScaleTransform(1);
            SetRotation(0);

            IsBlasting = false;

            _targetHitbox = new Rect(0, 0, 0, 0);
            _autoBlastDelay = _autoBlastDelayDefault;
        }

        public void Reposition(MafiaBoss mafiaBoss)
        {
            SetPosition(
                left: (mafiaBoss.GetLeft() + mafiaBoss.Width / 2) - Width / 2,
                top: mafiaBoss.GetTop() + Height / 2);
        }

        public void SetTarget(Rect target)
        {
            double left = GetLeft();
            double top = GetTop();

            double rocketMiddleX = left + Width / 2;
            double rocketMiddleY = top + Height / 2;

            var scaling = ScreenExtensions.GetScreenSpaceScaling();

            // move up
            if (target.Y < rocketMiddleY)
            {
                var distance = Math.Abs(target.Y - rocketMiddleY);
                _targetHitbox.Y = target.Y - distance;

                if (_targetHitbox.Y > 0)
                    _targetHitbox.Y -= distance;
            }

            // move left
            if (target.X < rocketMiddleX)
            {
                var distance = Math.Abs(target.X - rocketMiddleX);
                _targetHitbox.X = target.X - distance;

                if (_targetHitbox.X > 0)
                    _targetHitbox.X -= distance;
            }

            // move down
            if (target.Y > rocketMiddleY)
            {
                var distance = Math.Abs(target.Y - rocketMiddleY);
                _targetHitbox.Y = target.Y + distance;

                if (_targetHitbox.Y < Constants.DEFAULT_SCENE_HEIGHT * scaling)
                    _targetHitbox.Y += distance;

            }

            // move right
            if (target.X > rocketMiddleX)
            {
                var distance = Math.Abs(target.X - rocketMiddleX);
                _targetHitbox.X = target.X + distance;

                if (_targetHitbox.X < Constants.DEFAULT_SCENE_WIDTH * scaling)
                    _targetHitbox.X += distance;
            }
        }

        public void Move()
        {
            Seek(target: _targetHitbox); // seek at normal speed so that the player can evade
        }

        public bool AutoBlast()
        {
            if (_targetHitbox.IntersectsWith(this.GetCloseHitBox())) // if target reached
                return true;

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

            //BorderBrush = new SolidColorBrush(Colors.Chocolate);

            var uri = ConstructExtensions.GetRandomContentUri(_bombBlastUris);
            _imageContainer.SetSource(uri);
            IsBlasting = true;
        }

        #endregion
    }
}
