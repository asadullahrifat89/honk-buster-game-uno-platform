using Microsoft.UI;
using Windows.Foundation;

namespace HonkBusterGame
{
    public partial class GameObjectContainer : MovableBase
    {
        #region Fields


        private readonly CompositeTransform _transform = new()
        {
            CenterX = 0.5,
            CenterY = 0.5,
            Rotation = 0,
            ScaleX = 1,
            ScaleY = 1,
        };


        private readonly Canvas _canvas = new()
        {
            RenderTransformOrigin = new Point(0.5, 0.5),
            Background = new SolidColorBrush(Colors.Transparent),
            Width = 512,
            Height = 512,
        };

        #endregion

        #region Ctor

        public GameObjectContainer(
            Action<GameObjectContainer> animateAction,
            Action<GameObjectContainer> recycleAction)
        {
            Width = 512;
            Height = 512;

            _canvas.RenderTransform = _transform;
            SetContent(_canvas);

            AnimateAction = animateAction;
            RecycleAction = recycleAction;
        }

        #endregion

        #region Properties       

        /// <summary>
        /// Animation function.
        /// </summary>
        public new Action<GameObjectContainer> AnimateAction { get; set; }

        /// <summary>
        /// Recycling function.
        /// </summary>
        public new Action<GameObjectContainer> RecycleAction { get; set; }

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
            //Children.Add(gameObject);
            _canvas.Children.Add(gameObject.Content);
        }

        public void RemoveChild(GameObject gameObject)
        {
            //Children.Remove(gameObject);
            _canvas.Children.Remove(gameObject.Content);
        }

        public void Clear()
        {
            //Children.Clear();
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
