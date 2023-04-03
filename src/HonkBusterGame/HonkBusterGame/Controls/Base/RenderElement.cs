﻿using Windows.Foundation;

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
            Content = new UIElement();
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
        /// The canvas X position of the element.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// The canvas Y position of the element.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// The canvas Z order of the element.
        /// </summary>
        public int Z { get; set; }

        public double ScaleX { get; set; }

        public double ScaleY { get; set; }

        public double SkewX { get; set; }

        public double SkewY { get; set; }

        public double Rotation { get; set; }

        public double Opacity { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public UIElement Content { get; set; }

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

        private bool _IsAnimating;

        /// <summary>
        /// Only animated by the scene if set to true.
        /// </summary>
        public bool IsAnimating
        {
            get { return _IsAnimating; }
            set
            {
                _IsAnimating = value;

                if (!_IsAnimating && Content is not null)
                {
                    Canvas.SetLeft(Content, -3000);
                    Canvas.SetTop(Content, -3000);
                }
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

        public void SetContent(UIElement content)
        {
            Content = content;

            Content.RenderTransformOrigin = new Point(0.5, 0.5);
            Content.RenderTransform = _transform;
        }

        public void Update()
        {
            if (Content is not null)
            {
                Canvas.SetZIndex(Content, Z);
                Canvas.SetLeft(Content, X);
                Canvas.SetTop(Content, Y);                

                if (Content is FrameworkElement element)
                {
                    element.Width = Width;
                    element.Height = Height;
                }

                Content.Opacity = Opacity;

                _transform.ScaleX = ScaleX;
                _transform.ScaleY = ScaleY;

                _transform.Rotation = Rotation;
                _transform.SkewX = SkewX;
                _transform.SkewY = SkewY;

                //TODO: draw function to render element in HTML Canvas
            }
        }

        #endregion
    }

    public enum RotationDirection
    {
        Forward,
        Backward,
    }
}
