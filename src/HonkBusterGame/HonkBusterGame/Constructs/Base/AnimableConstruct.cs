namespace HonkBusterGame
{
    public partial class AnimableConstruct : MovableConstruct
    {
        #region Fields

        private double _hoverDelay;
        private readonly double _hoverDelayDefault = 15;
        private readonly double _hoverSpeed = 0.5;

        private double _dillyDallyDelay;
        private readonly double _dillyDallyDelayDefault = 15;
        private readonly double _dillyDallySpeed = 0.7;


        private double _vibrateDelay;
        private readonly double _vibrateDelayDefault = 3;
        private readonly double _vibrateSpeed = 0.5;

        #endregion

        #region Properties

        public bool AwaitMoveUp { get; set; }

        public bool AwaitMoveDown { get; set; }

        public bool AwaitMoveLeft { get; set; }

        public bool AwaitMoveRight { get; set; }

        public bool AwaitMoveUpRight { get; set; }

        public bool AwaitMoveDownLeft { get; set; }

        public bool AwaitMoveUpLeft { get; set; }

        public bool AwaitMoveDownRight { get; set; }

        public bool IsBlasting { get; set; }

        #endregion

        #region Methods

        public void Hover()
        {
            _hoverDelay--;

            if (_hoverDelay >= 0)
            {
                SetTop(GetTop() + _hoverSpeed);
            }
            else
            {
                SetTop(GetTop() - _hoverSpeed);

                if (_hoverDelay <= _hoverDelayDefault * -1)
                    _hoverDelay = _hoverDelayDefault;
            }
        }

        public void DillyDally()
        {
            _dillyDallyDelay--;

            if (_dillyDallyDelay >= 0)
            {
                SetLeft(GetLeft() + _dillyDallySpeed);
            }
            else
            {
                SetLeft(GetLeft() - _dillyDallySpeed);

                if (_dillyDallyDelay <= _dillyDallyDelayDefault * -1)
                    _dillyDallyDelay = _dillyDallyDelayDefault;
            }
        }

        public void Vibrate()
        {
            _vibrateDelay--;

            if (_vibrateDelay >= 0)
            {
                SetTop(GetTop() + _vibrateSpeed);
            }
            else
            {
                SetTop(GetTop() - _vibrateSpeed);

                if (_vibrateDelay <= _vibrateDelayDefault * -1)
                    _vibrateDelay = _vibrateDelayDefault;
            }
        }

        #endregion
    }
}
