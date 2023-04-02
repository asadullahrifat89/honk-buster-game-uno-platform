namespace HonkBusterGame
{
    public partial class Generator
    {
        #region Fields

        private readonly bool _randomizeGenerationDelay = false;
        private double _generationDelay;
        private double _generationDelayInCount;

        private readonly Random _random = new Random();

        #endregion

        #region Properties

        private Action GenerationAction { get; set; }

        public Scene Scene { get; set; }

        #endregion

        public Generator(
            int delay,
            Action elaspedAction,
            bool scramble = false)
        {
            _randomizeGenerationDelay = scramble;
            _generationDelay = delay;

            _generationDelayInCount = _generationDelay;

            GenerationAction = elaspedAction;
        }

        #region Methods

        public void Generate()
        {
            if (_generationDelay > 0)
            {
                _generationDelayInCount -= Scene.IsSlowMotionActivated ? 0.5 : 1;

                if (_generationDelayInCount <= 0)
                {
                    GenerationAction();
                    _generationDelayInCount = _randomizeGenerationDelay ? _random.Next((int)(_generationDelay / 2), (int)_generationDelay) : _generationDelay;
                }
            }
        }

        public void SetGenerationDelay(int deplay)
        {
            _generationDelay = deplay;
        }

        #endregion
    }
}
