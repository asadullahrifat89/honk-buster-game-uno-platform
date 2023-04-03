namespace HonkBusterGame
{
    /// <summary>
    /// A game object that can be added in a game view and be rendered.
    /// </summary>
    public partial class GameObject : RenderableBase
    {
        public GameObject()
        {
            Speed = Constants.DEFAULT_CONSTRUCT_SPEED;
            AnimateAction = (s) => { };
            RecycleAction = (s) => { };
            GameView = new();            
        }

        #region Properties

        /// <summary>
        /// Type of the construct.
        /// </summary>
        public ConstructType ConstructType { get; set; }

        /// <summary>
        /// Animation function.
        /// </summary>
        public Action<GameObject> AnimateAction { get; set; }

        /// <summary>
        /// Recycling function.
        /// </summary>
        public Action<GameObject> RecycleAction { get; set; }

        /// <summary>
        /// Sets the movement speed.
        /// </summary>
        public double Speed { get; set; }

        /// <summary>
        /// Displacement value that determines isometric movement.
        /// </summary>
        public double IsometricDisplacement { get; set; }

        /// <summary>
        /// The scene in which this construct exists.
        /// </summary>
        public GameView GameView { get; set; }

        #endregion

        #region Methods      

        public void Animate()
        {
            AnimateAction(this);
        }

        public void Recycle()
        {
            RecycleAction(this);
        }

        public void SetConstructSize(ConstructType constructType)
        {
            if (ConstructType != ConstructType.NONE)
            {
                var size = Constants.CONSTRUCT_SIZES.FirstOrDefault(x => x.ConstructType == constructType);

                var width = size.Width;
                var height = size.Height;

                SetSize(width: width, height: height);
            }
        }

        #endregion
    }
}
