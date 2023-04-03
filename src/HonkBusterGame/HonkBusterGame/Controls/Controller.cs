using Microsoft.UI;
using Microsoft.UI.Input;
using Windows.Devices.Sensors;
using Windows.Foundation;

namespace HonkBusterGame
{
    public partial class Controller : ControllerElement
    {
        #region Fields

        //private int _move_up_activator;
        //private int _move_down_activator;
        //private int _move_left_activator;
        //private int _move_right_activator;

        //private int _move_x_activator;
        //private int _move_y_activator;

        private Border _thumbstickThumbChild = new Border();

        #endregion

        #region Properties

        private Button PauseButton { get; set; }

        private Button FocusButton { get; set; }

        private Border AttackButton { get; set; }

        private Canvas ThumbstickCanvas { get; set; }

        private Construct ThumbstickThumb { get; set; }

        private Construct ThumbstickUpLeft { get; set; }

        private Construct ThumbstickUp { get; set; }

        private Construct ThumbstickUpRight { get; set; }

        private Construct ThumbstickLeft { get; set; }

        private Construct ThumbstickRight { get; set; }

        private Construct ThumbstickDownLeft { get; set; }

        private Construct ThumbstickDown { get; set; }

        private Construct ThumbstickDownRight { get; set; }

        private Gyrometer Gyrometer { get; set; }

        private bool IsGyrometerReadingsActive { get; set; }

        private double AngularVelocityX { get; set; }

        private double AngularVelocityY { get; set; }

        private double AngularVelocityZ { get; set; }

        private bool IsThumbstickGripActive { get; set; }

        private Grid Keypad { get; set; }

        #endregion

        #region Ctor

        public Controller()
        {
            CanDrag = false;

            SetAttackButton();
            SetPauseButton();
            SetFocusButton();

            KeyUp += Controller_KeyUp;
            KeyDown += Controller_KeyDown;

            SetThumbstick();
        }

        #endregion

        #region Methods

        #region Thumbstick

        public void SetThumbstickThumbColor(SolidColorBrush color)
        {
            _thumbstickThumbChild.Background = color;
        }

        public void SetDefaultThumbstickPosition()
        {
            ThumbstickThumb.SetPosition(
                left: Constants.DEFAULT_CONTROLLER_KEY_SIZE * 1.30,
                top: Constants.DEFAULT_CONTROLLER_KEY_SIZE * 1.30);
            ThumbstickThumb.Update();
        }

        private void SetThumbstickThumbPosition(PointerPoint point)
        {
            ThumbstickThumb.SetPosition(
                left: point.Position.X - ThumbstickThumb.Width / 2,
                top: point.Position.Y - ThumbstickThumb.Height / 2);
            ThumbstickThumb.Update();
        }

        private void SetThumbstick()
        {
            var sizeXY = 3.5;

            ThumbstickCanvas = new Canvas()
            {
                Height = Constants.DEFAULT_CONTROLLER_KEY_SIZE * sizeXY,
                Width = Constants.DEFAULT_CONTROLLER_KEY_SIZE * sizeXY,
                Background = new SolidColorBrush(Colors.Transparent),
                RenderTransformOrigin = new Point(1, 1),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
            };

            Border neutralZoneChild = new()
            {
                Background = new SolidColorBrush(Colors.Goldenrod),
                CornerRadius = new CornerRadius(Constants.DEFAULT_CONTROLLER_KEY_CORNER_RADIUS * sizeXY),
                BorderBrush = Application.Current.Resources["BorderColor"] as SolidColorBrush,
                BorderThickness = new Thickness(Constants.DEFAULT_CONTROLLER_KEY_BORDER_THICKNESS),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
            };

            var neutralZone = new Construct()
            {
                Height = Constants.DEFAULT_CONTROLLER_KEY_SIZE * 1.55,
                Width = Constants.DEFAULT_CONTROLLER_KEY_SIZE * 1.55,
                Opacity = 0.4,
            };

            neutralZone.SetContent(neutralZoneChild);

            ThumbstickUpLeft = new()
            {
                Tag = MovementDirection.UpLeft,
                Height = Constants.DEFAULT_CONTROLLER_KEY_SIZE,
                Width = Constants.DEFAULT_CONTROLLER_KEY_SIZE,
                Rotation = -45,
                //RenderTransformOrigin = new Point(0.5, 0.5),
                //RenderTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = -45 },
            };

            ThumbstickUp = new()
            {
                Tag = MovementDirection.Up,
                Height = Constants.DEFAULT_CONTROLLER_KEY_SIZE,
                Width = Constants.DEFAULT_CONTROLLER_KEY_SIZE * sizeXY,
                //RenderTransformOrigin = new Point(0.5, 0.5),
            };

            ThumbstickUpRight = new()
            {
                Tag = MovementDirection.UpRight,
                Height = Constants.DEFAULT_CONTROLLER_KEY_SIZE,
                Width = Constants.DEFAULT_CONTROLLER_KEY_SIZE,
                Rotation = -45,
                //RenderTransformOrigin = new Point(0.5, 0.5),
                //RenderTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = -45 },
            };

            ThumbstickLeft = new()
            {
                Tag = MovementDirection.Left,
                Height = Constants.DEFAULT_CONTROLLER_KEY_SIZE * sizeXY,
                Width = Constants.DEFAULT_CONTROLLER_KEY_SIZE,
                //RenderTransformOrigin = new Point(0.5, 0.5),
            };

            ThumbstickRight = new()
            {
                Tag = MovementDirection.Right,
                Height = Constants.DEFAULT_CONTROLLER_KEY_SIZE * sizeXY,
                Width = Constants.DEFAULT_CONTROLLER_KEY_SIZE,
                //RenderTransformOrigin = new Point(0.5, 0.5),
            };

            ThumbstickDownLeft = new()
            {
                Tag = MovementDirection.DownLeft,
                Height = Constants.DEFAULT_CONTROLLER_KEY_SIZE,
                Width = Constants.DEFAULT_CONTROLLER_KEY_SIZE,
                Rotation = -45,
                //RenderTransformOrigin = new Point(0.5, 0.5),
                //RenderTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = -45 },
            };

            ThumbstickDown = new()
            {
                Tag = MovementDirection.Down,
                Height = Constants.DEFAULT_CONTROLLER_KEY_SIZE,
                Width = Constants.DEFAULT_CONTROLLER_KEY_SIZE * sizeXY,
                Rotation = 180,
                //RenderTransformOrigin = new Point(0.5, 0.5),
                //RenderTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = 180 },
            };

            ThumbstickDownRight = new()
            {
                Tag = MovementDirection.DownRight,
                Height = Constants.DEFAULT_CONTROLLER_KEY_SIZE,
                Width = Constants.DEFAULT_CONTROLLER_KEY_SIZE,
                Rotation = 45,
                //RenderTransformOrigin = new Point(0.5, 0.5),
                //RenderTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = 45 },
            };

            _thumbstickThumbChild = new Border()
            {
                CornerRadius = new CornerRadius(Constants.DEFAULT_CONTROLLER_KEY_CORNER_RADIUS * 2),
                Background = new SolidColorBrush(Colors.Crimson),
                BorderBrush = Application.Current.Resources["BorderColor"] as SolidColorBrush,
                BorderThickness = new Thickness(Constants.DEFAULT_CONTROLLER_KEY_BORDER_THICKNESS),
            };

            ThumbstickThumb = new()
            {
                Tag = MovementDirection.None,
                Height = Constants.DEFAULT_CONTROLLER_KEY_SIZE * 0.90,
                Width = Constants.DEFAULT_CONTROLLER_KEY_SIZE * 0.90,
            };

            ThumbstickThumb.SetContent(_thumbstickThumbChild);

            neutralZone.SetPosition(left: ThumbstickCanvas.Width / 2 - neutralZone.Width / 2, top: ThumbstickCanvas.Height / 2 - neutralZone.Height / 2);
            ThumbstickCanvas.Children.Add(neutralZone.Content);
            neutralZone.Update();

            ThumbstickUpLeft.SetPosition(left: 0, top: 0);
            ThumbstickCanvas.Children.Add(ThumbstickUpLeft.Content);
            ThumbstickUpLeft.Update();

            ThumbstickUp.SetPosition(left: 0 * 1.25, top: 0);
            ThumbstickCanvas.Children.Add(ThumbstickUp.Content);
            ThumbstickUp.Update();

            ThumbstickUpRight.SetPosition(left: Constants.DEFAULT_CONTROLLER_KEY_SIZE * 2 * 1.25, top: 0);
            ThumbstickCanvas.Children.Add(ThumbstickUpRight.Content);
            ThumbstickUpRight.Update();

            ThumbstickLeft.SetPosition(left: 0, top: 0);
            ThumbstickCanvas.Children.Add(ThumbstickLeft.Content);
            ThumbstickLeft.Update();

            ThumbstickRight.SetPosition(left: Constants.DEFAULT_CONTROLLER_KEY_SIZE * 2 * 1.25, top: 0);
            ThumbstickCanvas.Children.Add(ThumbstickRight.Content);
            ThumbstickRight.Update();

            ThumbstickDownLeft.SetPosition(left: 0, top: Constants.DEFAULT_CONTROLLER_KEY_SIZE * 2 * 1.25);
            ThumbstickCanvas.Children.Add(ThumbstickDownLeft.Content);
            ThumbstickDownLeft.Update();

            ThumbstickDown.SetPosition(left: 0 * 1.25, top: Constants.DEFAULT_CONTROLLER_KEY_SIZE * 2 * 1.25);
            ThumbstickCanvas.Children.Add(ThumbstickDown.Content);
            ThumbstickDown.Update();

            ThumbstickDownRight.SetPosition(left: Constants.DEFAULT_CONTROLLER_KEY_SIZE * 2 * 1.25, top: Constants.DEFAULT_CONTROLLER_KEY_SIZE * 2 * 1.25);
            ThumbstickCanvas.Children.Add(ThumbstickDownRight.Content);
            ThumbstickDownRight.Update();

            SetDefaultThumbstickPosition();
            ThumbstickCanvas.Children.Add(ThumbstickThumb.Content);

            ThumbstickCanvas.PointerPressed += (s, e) =>
            {
                var point = e.GetCurrentPoint(ThumbstickCanvas);
                SetThumbstickThumbPosition(point);

                DeactivateMoveUp();
                DeactivateMoveDown();
                DeactivateMoveLeft();
                DeactivateMoveRight();

                IsThumbstickGripActive = true;
                ActivateThumbstick();
            };
            ThumbstickCanvas.PointerMoved += (s, e) =>
            {
                if (IsThumbstickGripActive)
                {
                    var point = e.GetCurrentPoint(ThumbstickCanvas);
                    SetThumbstickThumbPosition(point);
                    ActivateThumbstick();
                }
            };
            ThumbstickCanvas.PointerReleased += (s, e) =>
            {
                DeactivateMoveUp();
                DeactivateMoveDown();
                DeactivateMoveLeft();
                DeactivateMoveRight();

                IsThumbstickGripActive = false;
                SetDefaultThumbstickPosition();
            };

            this.Children.Add(ThumbstickCanvas);
        }

        private void ActivateThumbstick()
        {
            if (ThumbstickThumb.GetHitBox().IntersectsWith(ThumbstickUpLeft.GetCloseHitBox())) // up left
            {
                ActivateMoveUp();
                ActivateMoveLeft();
            }
            else
            {
                DeactivateMoveUp();
                DeactivateMoveLeft();
            }

            if (ThumbstickThumb.GetHitBox().IntersectsWith(ThumbstickUpRight.GetCloseHitBox())) // up right
            {
                ActivateMoveUp();
                ActivateMoveRight();
            }
            else
            {
                DeactivateMoveUp();
                DeactivateMoveRight();
            }

            if (ThumbstickThumb.GetHitBox().IntersectsWith(ThumbstickDownLeft.GetCloseHitBox())) // down left
            {
                ActivateMoveDown();
                ActivateMoveLeft();
            }
            else
            {
                DeactivateMoveDown();
                DeactivateMoveLeft();
            }

            if (ThumbstickThumb.GetHitBox().IntersectsWith(ThumbstickDownRight.GetCloseHitBox())) // down right
            {
                ActivateMoveDown();
                ActivateMoveRight();
            }
            else
            {
                DeactivateMoveDown();
                DeactivateMoveRight();
            }

            if (ThumbstickThumb.GetHitBox().IntersectsWith(ThumbstickUp.GetCloseHitBox())) // up
            {
                ActivateMoveUp();
            }
            else
            {
                DeactivateMoveUp();
            }

            if (ThumbstickThumb.GetHitBox().IntersectsWith(ThumbstickLeft.GetCloseHitBox())) // left
            {
                ActivateMoveLeft();
            }
            else
            {
                DeactivateMoveLeft();
            }

            if (ThumbstickThumb.GetHitBox().IntersectsWith(ThumbstickRight.GetHitBox())) // right
            {
                ActivateMoveRight();
            }
            else
            {
                DeactivateMoveRight();
            }

            if (ThumbstickThumb.GetHitBox().IntersectsWith(ThumbstickDown.GetHitBox())) // down
            {
                ActivateMoveDown();
            }
            else
            {
                DeactivateMoveDown();
            }
        }

        #endregion

        #region Keypad

        public void SetKeypad()
        {
            Keypad = new()
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(20),
            };

            Keypad.RowDefinitions.Add(new RowDefinition());
            Keypad.RowDefinitions.Add(new RowDefinition());
            Keypad.RowDefinitions.Add(new RowDefinition());

            Keypad.ColumnDefinitions.Add(new ColumnDefinition());
            Keypad.ColumnDefinitions.Add(new ColumnDefinition());
            Keypad.ColumnDefinitions.Add(new ColumnDefinition());

            #region UpLeft

            Border upLeft = new()
            {
                Background = new SolidColorBrush(Colors.Goldenrod),
                Height = Constants.DEFAULT_CONTROLLER_KEY_SIZE,
                Width = Constants.DEFAULT_CONTROLLER_KEY_SIZE,
                CornerRadius = new CornerRadius(Constants.DEFAULT_CONTROLLER_KEY_CORNER_RADIUS),
                Child = new SymbolIcon()
                {
                    Symbol = Symbol.Up,
                },
                BorderBrush = new SolidColorBrush(Colors.White),
                BorderThickness = new Thickness(Constants.DEFAULT_CONTROLLER_KEY_BORDER_THICKNESS),
                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = -45 },
                Margin = new Thickness(Constants.DEFAULT_CONTROLLER_DIRECTION_KEYS_MARGIN),
            };

            upLeft.PointerEntered += (s, e) =>
            {
                ActivateMoveUp();
                ActivateMoveLeft();
            };
            upLeft.PointerExited += (s, e) =>
            {
                DeactivateMoveUp();
                DeactivateMoveLeft();
            };

            SetRow(upLeft, 0);
            SetColumn(upLeft, 0);

            #endregion

            #region Up

            Border up = new()
            {
                Background = new SolidColorBrush(Colors.Goldenrod),
                Height = Constants.DEFAULT_CONTROLLER_KEY_SIZE,
                Width = Constants.DEFAULT_CONTROLLER_KEY_SIZE,
                CornerRadius = new CornerRadius(Constants.DEFAULT_CONTROLLER_KEY_CORNER_RADIUS),
                Child = new SymbolIcon()
                {
                    Symbol = Symbol.Up,
                },
                BorderBrush = new SolidColorBrush(Colors.White),
                BorderThickness = new Thickness(Constants.DEFAULT_CONTROLLER_KEY_BORDER_THICKNESS),
                RenderTransformOrigin = new Point(0.5, 0.5),
                //RenderTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = -45 },
                Margin = new Thickness(Constants.DEFAULT_CONTROLLER_DIRECTION_KEYS_MARGIN),
            };

            up.PointerEntered += (s, e) => { ActivateMoveUp(); };
            up.PointerExited += (s, e) => { DeactivateMoveUp(); };

            SetRow(up, 0);
            SetColumn(up, 1);

            #endregion

            #region UpRight

            Border upRight = new()
            {
                Background = new SolidColorBrush(Colors.Goldenrod),
                Height = Constants.DEFAULT_CONTROLLER_KEY_SIZE,
                Width = Constants.DEFAULT_CONTROLLER_KEY_SIZE,
                CornerRadius = new CornerRadius(Constants.DEFAULT_CONTROLLER_KEY_CORNER_RADIUS),
                Child = new SymbolIcon()
                {
                    Symbol = Symbol.Forward,
                },
                BorderBrush = new SolidColorBrush(Colors.White),
                BorderThickness = new Thickness(Constants.DEFAULT_CONTROLLER_KEY_BORDER_THICKNESS),
                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = -45 },
                Margin = new Thickness(Constants.DEFAULT_CONTROLLER_DIRECTION_KEYS_MARGIN),
            };

            upRight.PointerEntered += (s, e) =>
            {
                ActivateMoveUp();
                ActivateMoveRight();
            };
            upRight.PointerExited += (s, e) =>
            {
                DeactivateMoveUp();
                DeactivateMoveRight();
            };

            SetRow(upRight, 0);
            SetColumn(upRight, 2);

            #endregion

            #region Left

            Border left = new()
            {
                Background = new SolidColorBrush(Colors.Goldenrod),
                Height = Constants.DEFAULT_CONTROLLER_KEY_SIZE,
                Width = Constants.DEFAULT_CONTROLLER_KEY_SIZE,
                CornerRadius = new CornerRadius(Constants.DEFAULT_CONTROLLER_KEY_CORNER_RADIUS),
                Child = new SymbolIcon()
                {
                    Symbol = Symbol.Back,
                },
                BorderBrush = new SolidColorBrush(Colors.White),
                BorderThickness = new Thickness(Constants.DEFAULT_CONTROLLER_KEY_BORDER_THICKNESS),
                RenderTransformOrigin = new Point(0.5, 0.5),
                //RenderTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = -45 },
                Margin = new Thickness(Constants.DEFAULT_CONTROLLER_DIRECTION_KEYS_MARGIN),
            };

            left.PointerEntered += (s, e) => { ActivateMoveLeft(); };
            left.PointerExited += (s, e) => { DeactivateMoveLeft(); };

            SetRow(left, 1);
            SetColumn(left, 0);

            #endregion

            #region Right

            Border right = new()
            {
                Background = new SolidColorBrush(Colors.Goldenrod),
                Height = Constants.DEFAULT_CONTROLLER_KEY_SIZE,
                Width = Constants.DEFAULT_CONTROLLER_KEY_SIZE,
                CornerRadius = new CornerRadius(Constants.DEFAULT_CONTROLLER_KEY_CORNER_RADIUS),
                Child = new SymbolIcon()
                {
                    Symbol = Symbol.Forward,
                },
                BorderBrush = new SolidColorBrush(Colors.White),
                BorderThickness = new Thickness(Constants.DEFAULT_CONTROLLER_KEY_BORDER_THICKNESS),
                RenderTransformOrigin = new Point(0.5, 0.5),
                //RenderTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = -45 },
                Margin = new Thickness(Constants.DEFAULT_CONTROLLER_DIRECTION_KEYS_MARGIN),
            };

            right.PointerEntered += (s, e) => { ActivateMoveRight(); };
            right.PointerExited += (s, e) => { DeactivateMoveRight(); };

            SetRow(right, 1);
            SetColumn(right, 2);

            #endregion

            #region DownLeft

            Border downLeft = new()
            {
                Background = new SolidColorBrush(Colors.Goldenrod),
                Height = Constants.DEFAULT_CONTROLLER_KEY_SIZE,
                Width = Constants.DEFAULT_CONTROLLER_KEY_SIZE,
                CornerRadius = new CornerRadius(Constants.DEFAULT_CONTROLLER_KEY_CORNER_RADIUS),
                Child = new SymbolIcon()
                {
                    Symbol = Symbol.Back,
                },
                BorderBrush = new SolidColorBrush(Colors.White),
                BorderThickness = new Thickness(Constants.DEFAULT_CONTROLLER_KEY_BORDER_THICKNESS),
                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = -45 },
                Margin = new Thickness(Constants.DEFAULT_CONTROLLER_DIRECTION_KEYS_MARGIN),
            };

            downLeft.PointerEntered += (s, e) =>
            {
                ActivateMoveDown();
                ActivateMoveLeft();
            };
            downLeft.PointerExited += (s, e) =>
            {
                DeactivateMoveDown();
                DeactivateMoveLeft();
            };

            SetRow(downLeft, 2);
            SetColumn(downLeft, 0);

            #endregion

            #region Down

            Border down = new()
            {
                Background = new SolidColorBrush(Colors.Goldenrod),
                Height = Constants.DEFAULT_CONTROLLER_KEY_SIZE,
                Width = Constants.DEFAULT_CONTROLLER_KEY_SIZE,
                CornerRadius = new CornerRadius(Constants.DEFAULT_CONTROLLER_KEY_CORNER_RADIUS),
                Child = new SymbolIcon()
                {
                    Symbol = Symbol.Up
                },
                BorderBrush = new SolidColorBrush(Colors.White),
                BorderThickness = new Thickness(Constants.DEFAULT_CONTROLLER_KEY_BORDER_THICKNESS),
                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = 180 },
                Margin = new Thickness(Constants.DEFAULT_CONTROLLER_DIRECTION_KEYS_MARGIN),
            };

            down.PointerEntered += (s, e) => { ActivateMoveDown(); };
            down.PointerExited += (s, e) => { DeactivateMoveDown(); };

            SetRow(down, 2);
            SetColumn(down, 1);

            #endregion

            #region DownRight

            Border downRight = new()
            {
                Background = new SolidColorBrush(Colors.Goldenrod),
                Height = Constants.DEFAULT_CONTROLLER_KEY_SIZE,
                Width = Constants.DEFAULT_CONTROLLER_KEY_SIZE,
                CornerRadius = new CornerRadius(Constants.DEFAULT_CONTROLLER_KEY_CORNER_RADIUS),
                Child = new SymbolIcon()
                {
                    Symbol = Symbol.Forward
                },
                BorderBrush = new SolidColorBrush(Colors.White),
                BorderThickness = new Thickness(Constants.DEFAULT_CONTROLLER_KEY_BORDER_THICKNESS),
                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = 45 },
                Margin = new Thickness(Constants.DEFAULT_CONTROLLER_DIRECTION_KEYS_MARGIN),
            };

            downRight.PointerEntered += (s, e) =>
            {
                ActivateMoveDown();
                ActivateMoveRight();
            };
            downRight.PointerExited += (s, e) =>
            {
                DeactivateMoveDown();
                DeactivateMoveRight();
            };

            SetRow(downRight, 2);
            SetColumn(downRight, 2);

            #endregion

            Keypad.Children.Add(upLeft);
            Keypad.Children.Add(up);
            Keypad.Children.Add(upRight);

            Keypad.Children.Add(left);
            Keypad.Children.Add(right);

            Keypad.Children.Add(downLeft);
            Keypad.Children.Add(down);
            Keypad.Children.Add(downRight);

            this.Children.Add(Keypad);
        }

        #endregion

        #region Gyrometer

        public void SetGyrometer()
        {

#if __ANDROID__ || __IOS__

            Gyrometer = Gyrometer.GetDefault();

            if (Gyrometer is not null)
            {
                IsGyrometerReadingsActive = false;
                LoggingExtensions.Log($"Gyrometer detected.");
            }
#endif
        }

        public void UnsetGyrometer()
        {

#if __ANDROID__ || __IOS__

            if (Gyrometer is not null)
            {
                LoggingExtensions.Log($"Gyrometer detected.");
                DeactivateGyrometerReading();
            }
#endif
        }

        public void ActivateGyrometerReading()
        {

#if __ANDROID__ || __IOS__

            if (!IsGyrometerReadingsActive && Gyrometer is not null)
            {
                Gyrometer.ReportInterval = 25;
                IsGyrometerReadingsActive = true;
                Gyrometer.ReadingChanged += Gyrometer_ReadingChanged;
            }
#endif
        }

        public void DeactivateGyrometerReading()
        {

#if __ANDROID__ || __IOS__

            if (IsGyrometerReadingsActive && Gyrometer is not null)
            {
                Gyrometer.ReportInterval = 0;
                IsGyrometerReadingsActive = false;
                Gyrometer.ReadingChanged -= Gyrometer_ReadingChanged;
            }
#endif
        }

        private void MoveThumbstickThumbWithGyrometer(double speedX, double speedY)
        {
            if (ThumbstickThumb.GetLeft() + speedX > 0 && ThumbstickThumb.GetRight() + speedX < ThumbstickCanvas.Width)
                ThumbstickThumb.SetLeft(ThumbstickThumb.GetLeft() + speedX);

            if (ThumbstickThumb.GetTop() + speedY > 0 && ThumbstickThumb.GetBottom() + speedY < ThumbstickCanvas.Height)
                ThumbstickThumb.SetTop(ThumbstickThumb.GetTop() + speedY);

            ActivateThumbstick();
        }

        private void Gyrometer_ReadingChanged(Gyrometer sender, GyrometerReadingChangedEventArgs args)
        {
            if (IsGyrometerReadingsActive && !IsThumbstickGripActive) // only take readings when thunmbstick is not active with touch
            {
                AngularVelocityX = args.Reading.AngularVelocityX;
                AngularVelocityY = args.Reading.AngularVelocityY;
                AngularVelocityZ = args.Reading.AngularVelocityZ;

                LoggingExtensions.Log($"AngularVelocityX: {AngularVelocityX}");
                LoggingExtensions.Log($"AngularVelocityY: {AngularVelocityY}");
                LoggingExtensions.Log($"AngularVelocityZ: {AngularVelocityZ}");

#if __ANDROID__ || __IOS__
                MoveThumbstickThumbWithGyrometer(AngularVelocityX / 1.6, AngularVelocityY * -1 / 1.6); // less sensitive on mobile
#else
                //MoveThumbstickThumbWithGyrometer(AngularVelocityX / 1.1, AngularVelocityY * -1 / 1.1); // more sensitive on web
#endif
            }
        }

        #endregion

        #region Attack

        public void SetAttackButtonColor(SolidColorBrush color)
        {
            AttackButton.Background = color;
        }

        public void FocusController()
        {
            FocusButton.Focus(FocusState.Programmatic);
        }

        private void SetAttackButton()
        {
            AttackButton = new()
            {
                Background = new SolidColorBrush(Colors.Crimson),
                Height = Constants.DEFAULT_CONTROLLER_KEY_SIZE * 1.2,
                Width = Constants.DEFAULT_CONTROLLER_KEY_SIZE * 1.2,
                CornerRadius = new CornerRadius(Constants.DEFAULT_CONTROLLER_KEY_CORNER_RADIUS * 2),
                Child = new SymbolIcon()
                {
                    Symbol = Symbol.Placeholder,
                },
                BorderBrush = Application.Current.Resources["BorderColor"] as SolidColorBrush,
                BorderThickness = new Thickness(Constants.DEFAULT_CONTROLLER_KEY_BORDER_THICKNESS),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(55, 55),
                //Opacity = 0.6
            };

            AttackButton.PointerPressed += (s, e) =>
            {
                ActivateAttack();
            };
            this.Children.Add(AttackButton);
        }

        private void SetFocusButton()
        {
            FocusButton = new()
            {
                Height = Constants.DEFAULT_CONTROLLER_KEY_SIZE * 1.2,
                Width = Constants.DEFAULT_CONTROLLER_KEY_SIZE * 1.2,
                Margin = new Thickness(300, 300),
                Opacity = 0.0
            };
            FocusButton.Click += (s, e) =>
            {
                ActivateAttack();
            };
            this.Children.Add(FocusButton);
        }

        #endregion

        #region Pause


        private void SetPauseButton()
        {
            PauseButton = new()
            {
                //Background = new SolidColorBrush(Colors.Goldenrod),
                Height = Constants.DEFAULT_CONTROLLER_KEY_SIZE * 0.75,
                Width = Constants.DEFAULT_CONTROLLER_KEY_SIZE * 0.75,
                CornerRadius = new CornerRadius(Constants.DEFAULT_CONTROLLER_KEY_CORNER_RADIUS * 2),
                Content = new SymbolIcon()
                {
                    Symbol = Symbol.Pause,
                },
                BorderBrush = new SolidColorBrush(Colors.White),
                BorderThickness = new Thickness(Constants.DEFAULT_CONTROLLER_KEY_BORDER_THICKNESS),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(20),
            };

            PauseButton.Click += (s, e) => { ActivatePause(); };
            this.Children.Add(PauseButton);
        }

        #endregion

        #endregion
    }
}
