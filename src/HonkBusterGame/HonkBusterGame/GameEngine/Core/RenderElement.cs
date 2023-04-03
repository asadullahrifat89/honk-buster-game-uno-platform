using Windows.Foundation;

namespace HonkBusterGame
{
    public partial class RenderElement
    {
        #region Fields

        private readonly CompositeTransform _transform = new()
        {
            CenterX = 0.5,
            CenterY = 0.5,
        };

        private bool _isPoppingComplete;
        private readonly double _popUpScalingLimit = 1.5;

        #endregion

        #region Ctor

        public RenderElement()
        {
            Content = new FrameworkElement();
            IsAnimating = false;

            ScaleX = 1;
            ScaleY = 1;
            Z = 0;
            Rotation = 0;
            Opacity = 1;
            Tag = new();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Unique id of the construct.
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// The canvas X position of the content.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// The canvas Y position of the content.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// The canvas Z order of the content.
        /// </summary>
        public int Z { get; set; }

        /// <summary>
        /// Scale X factor of the content.
        /// </summary>
        public double ScaleX { get; set; }

        /// <summary>
        /// Scale Y factor of the content.
        /// </summary>
        public double ScaleY { get; set; }

        /// <summary>
        /// Skew X offset of the content.
        /// </summary>
        public double SkewX { get; set; }

        /// <summary>
        /// Skew Y offset of the content.
        /// </summary>
        public double SkewY { get; set; }

        /// <summary>
        /// Rotation of the content.
        /// </summary>
        public double Rotation { get; set; }

        /// <summary>
        /// Opacity of the content.
        /// </summary>
        public double Opacity { get; set; }

        /// <summary>
        /// Width of the content.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Height of the content.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// The content to be rendered in scene.
        /// </summary>
        public FrameworkElement Content { get; set; }

        /// <summary>
        /// An object tag for element.
        /// </summary>
        public object Tag { get; set; }

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

        private bool isAnimating;

        /// <summary>
        /// Only animated by the scene if set to true.
        /// </summary>
        public bool IsAnimating
        {
            get { return isAnimating; }
            set
            {
                isAnimating = value;

                if (!isAnimating && Content is not null)
                    MoveOutOfSight();
            }
        }

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
            ScaleX = scaleXY;
            ScaleY = scaleXY;
        }

        public void SetScaleTransform(double scaleX, double scaleY)
        {
            ScaleX = scaleX;
            ScaleY = scaleY;
        }

        public double GetScaleX()
        {
            return ScaleX;
        }

        public double GetScaleY()
        {
            return ScaleY;
        }

        public void SetScaleX(double scaleX)
        {
            ScaleX = scaleX;
        }

        public void SetScaleY(double scaleY)
        {
            ScaleY = scaleY;
        }

        public double GetRotation()
        {
            return Rotation;
        }

        public void SetRotation(double rotation)
        {
            Rotation = rotation;
        }

        public void SetSkewX(double skewX)
        {
            SkewX = skewX;
        }

        public void SetSkewY(double skewY)
        {
            SkewY = skewY;
        }

        public void SetOpacity(double opacity)
        {
            Opacity = opacity;
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
            ScaleX -= 0.1;
            ScaleY -= 0.1;
        }

        public void Expand()
        {
            ScaleX += 0.1;
            ScaleY += 0.1;
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
                            Rotation += rotationSpeed;
                        }
                        else
                        {
                            if (Rotation <= threadhold)
                                Rotation += rotationSpeed;
                        }
                    }
                    break;
                case RotationDirection.Backward:
                    {
                        if (threadhold == 0)
                        {
                            Rotation -= rotationSpeed;
                        }
                        else
                        {
                            if (Rotation >= threadhold * -1)
                                Rotation -= rotationSpeed;
                        }

                    }
                    break;
            }
        }

        public void UnRotate(double rotationSpeed = 0.1)
        {
            if (Convert.ToInt32(Rotation) != 0)
            {
                if (Rotation < 0)
                {
                    Rotation += rotationSpeed;
                    return;
                }

                if (Rotation > 0)
                {
                    Rotation -= rotationSpeed;
                }
            }
        }

        public void SetSize(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public void SetContent(FrameworkElement content)
        {
            Content = content;

            //if (content is not ImageElement) // only use composite transformation on non html image element
            //{
            Content.RenderTransformOrigin = new Point(0.5, 0.5);
            Content.RenderTransform = _transform;
            //}
        }

        public void Render()
        {
            if (Content is not null)
            {
                Canvas.SetZIndex(Content, Z);
                Canvas.SetLeft(Content, X);
                Canvas.SetTop(Content, Y);

                if (Content.Width != Width)
                    Content.Width = Width;

                if (Content.Height != Height)
                    Content.Height = Height;

                //if (Content is ImageElement imageElement)
                //{
                //    if (imageElement.ImageOpacity != Opacity)
                //        imageElement.ImageOpacity = Opacity;

                //    if (imageElement.ImageScaleX != ScaleX)
                //        imageElement.ImageScaleX = ScaleX;

                //    if (imageElement.ImageScaleY != ScaleY)
                //        imageElement.ImageScaleY = ScaleY;

                //    if (imageElement.ImageRotation != Rotation)
                //        imageElement.ImageRotation = Rotation;

                //    if (imageElement.ImageSkewX != SkewX)
                //        imageElement.ImageSkewX = SkewX;

                //    if (imageElement.ImageSkewY != SkewY)
                //        imageElement.ImageSkewY = SkewY;
                //}
                //else
                //{

                if (Content.Opacity != Opacity)
                    Content.Opacity = Opacity;

                if (_transform.ScaleX != ScaleX)
                    _transform.ScaleX = ScaleX;

                if (_transform.ScaleY != ScaleY)
                    _transform.ScaleY = ScaleY;

                if (_transform.Rotation != Rotation)
                    _transform.Rotation = Rotation;

                if (_transform.SkewX != SkewX)
                    _transform.SkewX = SkewX;

                if (_transform.SkewY != SkewY)
                    _transform.SkewY = SkewY;
                //}
            }
        }

        private void MoveOutOfSight()
        {
            Canvas.SetLeft(Content, -3000);
            Canvas.SetTop(Content, -3000);
        }

        #endregion
    }

    public enum RotationDirection
    {
        Forward,
        Backward,
    }
}
