﻿namespace HonkBusterGame
{
    public partial class VehicleBossRocket : AnimableBase
    {
        #region Fields

        private readonly Uri[] _bombUris;
        private readonly Uri[] _bombBlastUris;

        private readonly ImageContainer _imageContainer;

        private double _autoBlastDelay;
        private readonly double _autoBlastDelayDefault = 9;

        private readonly AudioStub _audioStub;

        #endregion

        #region Ctor

        public VehicleBossRocket(
           Action<GameObject> animateAction,
           Action<GameObject> recycleAction)
        {
            ConstructType = ConstructType.VEHICLE_BOSS_ROCKET;

            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            _bombUris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.VEHICLE_BOSS_ROCKET).Select(x => x.Uri).ToArray();
            _bombBlastUris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.BLAST).Select(x => x.Uri).ToArray();

            SetConstructSize(ConstructType);

            var uri = ConstructExtensions.GetRandomContentUri(_bombUris);
            _imageContainer = new(uri: uri, width: this.Width, height: this.Height);
            _imageContainer.SetDropShadow(offsetX: 0, offsetY: 0, blurRadius: 6, color: "#ffbe40");

            SetContent(_imageContainer);

            //BorderThickness = new Microsoft.UI.Xaml.Thickness(Constants.DEFAULT_BLAST_RING_BORDER_THICKNESS);
            //CornerRadius = new Microsoft.UI.Xaml.CornerRadius(Constants.DEFAULT_BLAST_RING_CORNER_RADIUS);

            IsometricDisplacement = Constants.DEFAULT_ISOMETRIC_DISPLACEMENT;
            Speed = Constants.DEFAULT_CONSTRUCT_SPEED - 3;
            DropShadowDistance = Constants.DEFAULT_DROP_SHADOW_DISTANCE;

            _audioStub = new AudioStub((SoundType.ROCKET_LAUNCH, 0.3, false), (SoundType.ROCKET_BLAST, 1, false));
        }

        #endregion

        #region Methods

        public void Reset()
        {
            _audioStub.Play(SoundType.ROCKET_LAUNCH);

            SetOpacity(1);
            SetScaleTransform(1);

            //BorderBrush = new SolidColorBrush(Colors.Transparent);

            IsBlasting = false;

            var uri = ConstructExtensions.GetRandomContentUri(_bombUris);
            _imageContainer.SetSource(uri);

            AwaitMoveDownLeft = false;
            AwaitMoveUpRight = false;

            AwaitMoveUpLeft = false;
            AwaitMoveDownRight = false;

            _autoBlastDelay = _autoBlastDelayDefault;
        }

        public void Reposition(VehicleBoss vehicleBoss)
        {
            SetPosition(
                left: (vehicleBoss.GetLeft() + vehicleBoss.Width / 2) - Width / 2,
                top: vehicleBoss.GetTop() + vehicleBoss.Height / 2);
        }

        public void SetBlast()
        {
            _audioStub.Play(SoundType.ROCKET_BLAST);

            SetScaleTransform(Constants.DEFAULT_BLAST_SHRINK_SCALE);

            //BorderBrush = new SolidColorBrush(Colors.Purple);

            var uri = ConstructExtensions.GetRandomContentUri(_bombBlastUris);
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
