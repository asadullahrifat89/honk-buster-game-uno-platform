using Windows.Foundation;

namespace HonkBusterGame
{
    public partial class CanvasElement : Border
    {
        #region Fields

        private readonly CompositeTransform _compositeTransform = new()
        {
            CenterX = 0.5,
            CenterY = 0.5,
            Rotation = 0,
            ScaleX = 1,
            ScaleY = 1,
        };

        private bool _isPoppingComplete;
        private readonly double _popUpScalingLimit = 1.5;

        #endregion

        #region Ctor

        public CanvasElement()
        {
            RenderTransformOrigin = new Point(0.5, 0.5);
            RenderTransform = _compositeTransform;
            CanDrag = false;
            IsAnimating = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Unique id of the construct.
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Returns true if faded.
        /// </summary>
        public bool IsFadingComplete => Opacity <= 0;

        /// <summary>
        /// Returns true if shrinked.
        /// </summary>
        public bool IsShrinkingComplete => GetScaleX() <= 0 || GetScaleY() <= 0;

        /// <summary>
        /// Returns true of expanded.
        /// </summary>
        public bool IsExpandingComplete => GetScaleX() >= 2 || GetScaleY() >= 2;

        /// <summary>
        /// Only animated by the scene if set to true.
        /// </summary>
        //public bool IsAnimating { get; set; }

        private bool _IsAnimating;

        public bool IsAnimating
        {
            get { return _IsAnimating; }
            set
            {
                _IsAnimating = value;

                if (!_IsAnimating)
                {
                    Canvas.SetLeft(this,-3000);
                    Canvas.SetTop(this, -3000);
                }
            }
        }

        /// <summary>
        /// The distance from this construct at which a shadow should appear.
        /// </summary>
        public double DropShadowDistance { get; set; }

        /// <summary>
        /// Determines gravitating effect so that it can reach it's drop shadow.
        /// </summary>
        public bool IsGravitatingDownwards { get; set; }

        /// <summary>
        /// Determines flying up effect.
        /// </summary>
        public bool IsGravitatingUpwards { get; set; }

        /// <summary>
        /// Determines if pop effect should execute for this construct.
        /// </summary>
        private bool IsAwaitingPop { get; set; }

        /// <summary>
        /// The canvas X position of the element.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// The canvas Y position of the element.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// The canvas X order of the element.
        /// </summary>
        public int Z { get; set; }

        #endregion

        #region Methods

        public double GetTop()
        {
            return Y;
        }

        public double GetBottom()
        {
            return Y + Height;
        }

        public double GetLeft()
        {
            return X;
        }

        public double GetRight()
        {
            return X + Width;
        }

        public int GetZ()
        {
            return Z;
        }

        public void SetTop(double top)
        {
            Y = top;
        }

        public void SetLeft(double left)
        {
            X = left;
        }

        public void SetZ(int z)
        {
            Z = z;
        }

        public void SetPosition(double left, double top)
        {
            Y = top;
            X = left;
        }

        public void SetPosition(double left, double top, int z)
        {
            Y = top;
            X = left;
            Z = z;
        }

        public void SetScaleTransform(double scaleXY)
        {
            _compositeTransform.ScaleX = scaleXY;
            _compositeTransform.ScaleY = scaleXY;
        }

        public void SetScaleTransform(double scaleX, double scaleY)
        {
            _compositeTransform.ScaleX = scaleX;
            _compositeTransform.ScaleY = scaleY;
        }

        public double GetScaleX()
        {
            return _compositeTransform.ScaleX;
        }

        public double GetScaleY()
        {
            return _compositeTransform.ScaleY;
        }

        public void SetScaleX(double scaleX)
        {
            _compositeTransform.ScaleX = scaleX;
        }

        public void SetScaleY(double scaleY)
        {
            _compositeTransform.ScaleY = scaleY;
        }

        public double GetRotation()
        {
            return _compositeTransform.Rotation;
        }

        public void SetRotation(double rotation)
        {
            _compositeTransform.Rotation = rotation;
        }

        public void SetSkewX(double skewX)
        {
            _compositeTransform.SkewX = skewX;
        }

        public void SetSkewY(double skewY)
        {
            _compositeTransform.SkewY = skewY;
        }

        public void Fade()
        {
            Opacity -= 0.005;
        }

        public void Fade(double fade)
        {
            Opacity -= fade;
        }

        public void Shrink()
        {
            _compositeTransform.ScaleX -= 0.1;
            _compositeTransform.ScaleY -= 0.1;
        }

        public void Expand()
        {
            _compositeTransform.ScaleX += 0.1;
            _compositeTransform.ScaleY += 0.1;
        }

        public void SetPopping()
        {
            if (!IsAwaitingPop)
            {
                SetScaleTransform(1);
                _isPoppingComplete = false;
                IsAwaitingPop = true;
            }
        }

        public void Pop()
        {
            if (IsAwaitingPop)
            {
                if (!_isPoppingComplete && GetScaleX() < _popUpScalingLimit)
                    Expand();

                if (GetScaleX() >= _popUpScalingLimit)
                    _isPoppingComplete = true;

                if (_isPoppingComplete)
                {
                    Shrink();

                    if (GetScaleX() <= 1)
                    {
                        _isPoppingComplete = false;
                        IsAwaitingPop = false; // stop popping effect                        
                    }
                }
            }
        }

        public void Rotate(
            RotationDirection rotationDirection = RotationDirection.Forward,
            double threadhold = 0,
            double rotationSpeed = 0.1)
        {
            switch (rotationDirection)
            {
                case RotationDirection.Forward:
                    {
                        if (threadhold == 0)
                        {
                            _compositeTransform.Rotation += rotationSpeed;
                        }
                        else
                        {
                            if (_compositeTransform.Rotation <= threadhold)
                                _compositeTransform.Rotation += rotationSpeed;
                        }
                    }
                    break;
                case RotationDirection.Backward:
                    {
                        if (threadhold == 0)
                        {
                            _compositeTransform.Rotation -= rotationSpeed;
                        }
                        else
                        {
                            if (_compositeTransform.Rotation >= threadhold * -1)
                                _compositeTransform.Rotation -= rotationSpeed;
                        }

                    }
                    break;
            }
        }

        public void UnRotate(double rotationSpeed = 0.1)
        {
            if (Convert.ToInt32(_compositeTransform.Rotation) != 0)
            {
                if (_compositeTransform.Rotation < 0)
                {
                    _compositeTransform.Rotation += rotationSpeed;
                    return;
                }

                if (_compositeTransform.Rotation > 0)
                {
                    _compositeTransform.Rotation -= rotationSpeed;
                }
            }
        }

        public void SetSize(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public void Update()
        {
            Canvas.SetTop(this, Y);
            Canvas.SetLeft(this, X);
            Canvas.SetZIndex(this, Z);
        }

        #endregion
    }

    public enum RotationDirection
    {
        Forward,
        Backward,
    }
}
