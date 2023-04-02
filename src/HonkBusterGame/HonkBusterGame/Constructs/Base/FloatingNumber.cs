using Microsoft.UI;
using Microsoft.UI.Text;

namespace HonkBusterGame
{
    public partial class FloatingNumber : MovableConstruct
    {
        #region Fields

        private readonly Random _random;
        private readonly TextBlock _textBlock;

        private double _messageOnScreenDelay;
        private readonly double _messageOnScreenDelayDefault = 5;

        private MovementDirection _movementDirection;

        #endregion

        #region Ctor

        public FloatingNumber(
            Action<Construct> animateAction,
            Action<Construct> recycleAction)
        {
            ConstructType = ConstructType.FLOATING_NUMBER;

            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            _random = new Random();

            SetConstructSize(ConstructType);

            _textBlock = new TextBlock() { FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Colors.White), FontSize = Constants.DEFAULT_GUI_FONT_SIZE - 3 };

            SetChild(_textBlock);
            IsometricDisplacement = Constants.DEFAULT_ISOMETRIC_DISPLACEMENT;
        }

        #endregion

        #region Properties

        public bool IsDepleted => _messageOnScreenDelay <= 0;

        #endregion

        #region Methods

        public void Reset(int number)
        {
            _textBlock.Text = $"-{number}";
            _messageOnScreenDelay = _messageOnScreenDelayDefault;
            RandomizeMovementDirection();
        }

        public void Reposition(Construct source)
        {
            SetPosition(
                left: (source.GetLeft() + source.Width / 2) - Width / 2,
                top: (source.GetTop() + source.Height / 2) - Height / 2);
        }

        public bool DepleteOnScreenDelay()
        {
            _messageOnScreenDelay -= 0.1;
            return true;
        }

        public void Move()
        {
            var speed = Speed;
            speed /= 3;

            switch (_movementDirection)
            {
                case MovementDirection.Up:
                    MoveUp(speed);
                    break;
                case MovementDirection.UpLeft:
                    MoveUpLeft(speed);
                    break;
                case MovementDirection.UpRight:
                    MoveUpRight(speed);
                    break;
                case MovementDirection.Down:
                    MoveDown(speed);
                    break;
                case MovementDirection.DownLeft:
                    MoveDownLeft(speed);
                    break;
                case MovementDirection.DownRight:
                    MoveDownRight(speed);
                    break;
                case MovementDirection.Right:
                    MoveRight(speed);
                    break;
                case MovementDirection.Left:
                    MoveLeft(speed);
                    break;
                default:
                    break;
            }
        }

        private void RandomizeMovementDirection()
        {
            _movementDirection = (MovementDirection)_random.Next(1, Enum.GetNames(typeof(MovementDirection)).Length);
        }

        #endregion
    }
}
