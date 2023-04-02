using System;
using System.Linq;

namespace HonkBusterGame
{
    public partial class PlayerRocket : AnimableConstruct
    {
        #region Fields

        private readonly Uri[] _bomb_uris;
        private readonly Uri[] _bomb_blast_uris;

        private readonly ImageElement _content_image;

        private double _autoBlastDelay;
        private readonly double _autoBlastDelayDefault = 8;

        private readonly AudioStub _audioStub;

        #endregion

        #region Ctor

        public PlayerRocket(
           Action<Construct> animateAction,
           Action<Construct> recycleAction)
        {
            ConstructType = ConstructType.PLAYER_ROCKET;

            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            _bomb_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.PLAYER_ROCKET).Select(x => x.Uri).ToArray();
            _bomb_blast_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.BLAST).Select(x => x.Uri).ToArray();

            SetConstructSize(ConstructType);

            var uri = ConstructExtensions.GetRandomContentUri(_bomb_uris);
            _content_image = new(uri: uri, width: this.Width, height: this.Height);

            SetChild(_content_image);

            //BorderThickness = new Microsoft.UI.Xaml.Thickness(Constants.DEFAULT_BLAST_RING_BORDER_THICKNESS);
            //CornerRadius = new Microsoft.UI.Xaml.CornerRadius(Constants.DEFAULT_BLAST_RING_CORNER_RADIUS);

            IsometricDisplacement = Constants.DEFAULT_ISOMETRIC_DISPLACEMENT;
            DropShadowDistance = Constants.DEFAULT_DROP_SHADOW_DISTANCE + 10;

            _audioStub = new AudioStub((SoundType.ROCKET_LAUNCH, 0.3, false), (SoundType.ROCKET_BLAST, 1, false));
        }

        #endregion

        #region Methods

        public void Reset()
        {
            _audioStub.Play(SoundType.ROCKET_LAUNCH);

            Opacity = 1;

            var uri = ConstructExtensions.GetRandomContentUri(_bomb_uris);
            _content_image.SetSource(uri);

            SetScaleTransform(1);

            //BorderBrush = new SolidColorBrush(Colors.Transparent);

            Speed = Constants.DEFAULT_CONSTRUCT_SPEED + 2;
            IsBlasting = false;

            AwaitMoveDownLeft = false;
            AwaitMoveUpRight = false;

            AwaitMoveUpLeft = false;
            AwaitMoveDownRight = false;

            _autoBlastDelay = _autoBlastDelayDefault;
        }

        public void Reposition(PlayerBalloon player)
        {
            SetPosition(
                left: (player.GetLeft() + player.Width / 2) - Width / 2,
                top: player.GetBottom() - (30));
        }

        public void SetBlast()
        {
            _audioStub.Play(SoundType.ROCKET_BLAST);

            SetScaleTransform(Constants.DEFAULT_BLAST_SHRINK_SCALE);
            SetRotation(0);

            //BorderBrush = new SolidColorBrush(Colors.Crimson);

            Speed = Constants.DEFAULT_CONSTRUCT_SPEED - 1;

            var uri = ConstructExtensions.GetRandomContentUri(_bomb_blast_uris);
            _content_image.SetSource(uri);

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
