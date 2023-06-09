﻿namespace HonkBusterGame
{
    public partial class VehicleEnemy : VehicleBase
    {
        #region Fields

        private readonly Random _random;

        private readonly Uri[] _vehicleSmallUris;
        private readonly Uri[] _vehicleLargeUris;

        private readonly ImageContainer _imageContainer;
        private readonly AudioStub _audioStub;

        #endregion

        #region Ctor

        public VehicleEnemy(
            Action<GameObject> animateAction,
            Action<GameObject> recycleAction)
        {
            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            _random = new Random();

            _vehicleSmallUris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.VEHICLE_ENEMY_SMALL).Select(x => x.Uri).ToArray();
            _vehicleLargeUris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.VEHICLE_ENEMY_LARGE).Select(x => x.Uri).ToArray();

            WillHonk = Convert.ToBoolean(_random.Next(2));

            var vehicleType = _random.Next(2);

            Uri uri = null;

            switch (vehicleType)
            {
                case 0:
                    {
                        ConstructType = ConstructType.VEHICLE_ENEMY_SMALL;
                        SetConstructSize(ConstructType.VEHICLE_ENEMY_SMALL);
                        uri = ConstructExtensions.GetRandomContentUri(_vehicleSmallUris);

                    }
                    break;
                case 1:
                    {
                        ConstructType = ConstructType.VEHICLE_ENEMY_LARGE;
                        SetConstructSize(ConstructType.VEHICLE_ENEMY_LARGE);
                        uri = ConstructExtensions.GetRandomContentUri(_vehicleLargeUris);
                    }
                    break;
                default:
                    break;
            }

            _imageContainer = new(uri: uri, width: this.Width, height: this.Height);
            _imageContainer.SetDropShadow(offsetX: 0, offsetY: 10, blurRadius: 2);
            SetContent(_imageContainer);

            IsometricDisplacement = Constants.DEFAULT_ISOMETRIC_DISPLACEMENT;

            if (WillHonk)
                SetHonkDelay();

            _audioStub = new AudioStub((SoundType.HONK_BUST_REACTION, 1, false));
        }

        #endregion     

        #region Methods

        public void Reset()
        {
            _imageContainer.SetGrayscale(0);
            SetScaleTransform(1);

            Speed = _random.Next(Constants.DEFAULT_CONSTRUCT_SPEED - 4, Constants.DEFAULT_CONSTRUCT_SPEED - 2);

            WillHonk = Convert.ToBoolean(_random.Next(2));

            if (WillHonk)
            {
                Health = HitPoint * _random.Next(3);
                SetHonkDelay();
            }
        }

        public void LooseHealth()
        {
            Health -= HitPoint;
        }

        public void SetBlast()
        {
            WillHonk = false;

            Speed = Constants.DEFAULT_CONSTRUCT_SPEED - 1;
            _imageContainer.SetGrayscale(100);
            
            _audioStub.Play(SoundType.HONK_BUST_REACTION);
        }

        #endregion
    }
}
