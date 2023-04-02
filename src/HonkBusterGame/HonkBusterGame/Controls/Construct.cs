namespace HonkBusterGame
{
    public partial class Construct : CanvasElement
    {
        public Construct()
        {
            Speed = Constants.DEFAULT_CONSTRUCT_SPEED;
        }

        #region Properties

        /// <summary>
        /// Type of the construct.
        /// </summary>
        public ConstructType ConstructType { get; set; }

        /// <summary>
        /// Animation function.
        /// </summary>
        public Action<Construct> AnimateAction { get; set; }

        /// <summary>
        /// Recycling function.
        /// </summary>
        public Action<Construct> RecycleAction { get; set; }

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
        public Scene Scene { get; set; }

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

        public void SetChild(UIElement content)
        {
            Child = content;
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
