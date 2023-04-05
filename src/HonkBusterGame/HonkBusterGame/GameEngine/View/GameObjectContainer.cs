namespace HonkBusterGame
{
    public partial class GameObjectContainer : DisplayObjectBase
    {
        #region Fields

        private readonly Canvas _canvas;

        #endregion

        #region Ctor

        public GameObjectContainer()
        {
            _canvas = new Canvas();
            SetContent(_canvas);
        }

        #endregion

        #region Properties

        public List<GameObject> Children { get; set; } = new();

        /// <summary>
        /// Animation function.
        /// </summary>
        public Action<GameObjectContainer> AnimateAction { get; set; }

        /// <summary>
        /// Recycling function.
        /// </summary>
        public Action<GameObjectContainer> RecycleAction { get; set; }

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

        public void AddChild(GameObject gameObject)
        {
            Children.Add(gameObject);
            _canvas.Children.Add(gameObject.Content);
        }

        public void RemoveChild(GameObject gameObject)
        {
            Children.Remove(gameObject);
            _canvas.Children.Remove(gameObject.Content);
        }

        public void Clear()
        {
            Children.Clear();
            _canvas.Children.Clear();
        }

        public new void SetSize(double width, double height)
        {
            Width = width;
            Height = height;

            _canvas.Width = width;
            _canvas.Height = height;
        }

        #endregion
    }
}
