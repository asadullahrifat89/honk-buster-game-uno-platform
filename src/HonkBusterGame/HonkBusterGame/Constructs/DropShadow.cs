namespace HonkBusterGame
{
    public partial class DropShadow : MovableBase
    {
        #region Fields

        private readonly Border _content;

        #endregion

        #region Properties

        private GameObject ParentConstruct { get; set; }

        private double ParentConstructSpeed { get; set; } = 0;

        #endregion

        #region Ctor

        public DropShadow(
            Action<GameObject> animateAction,
            Action<GameObject> recycleAction)
        {
            ConstructType = ConstructType.DROP_SHADOW;

            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            _content = new Border();

            _content.Background = Application.Current.Resources["DropShadowColor"] as SolidColorBrush;
            _content.CornerRadius = new CornerRadius(100);

            SetContent(_content);

            SetOpacity(0.9);

            SetConstructSize(ConstructType);

            Speed = Constants.DEFAULT_CONSTRUCT_SPEED;
            IsometricDisplacement = Constants.DEFAULT_ISOMETRIC_DISPLACEMENT;
        }

        #endregion

        #region Methods

        public bool IsParentConstructAnimating()
        {
            return ParentConstruct.IsAnimating;
        }

        public void SetParent(GameObject construct)
        {
            Id = construct.Id;
            ParentConstruct = construct;
        }

        public void Reset()
        {
            SetPosition(
                left: (ParentConstruct.GetLeft() + ParentConstruct.Width / 2) - Width / 2,
                top: ParentConstruct.GetBottom() + (ParentConstruct.DropShadowDistance));

            ParentConstructSpeed = ParentConstruct.Speed;

            Height = 25;
            Width = ParentConstruct.Width * 0.5;
        }

        public void Move()
        {
            SetLeft((ParentConstruct.GetLeft() + ParentConstruct.Width / 2) - Width / 2);

            if (ParentConstruct.IsGravitatingDownwards)
            {
                MoveDownRight(ParentConstructSpeed * IsometricDisplacement);

                if (Width < ParentConstruct.Width)
                    Width += 1;
            }
            else if (ParentConstruct.IsGravitatingUpwards)
            {
                MoveDownRight(ParentConstructSpeed);

                if (Width > 0)
                {
                    Width -= 0.5;
                    Height -= 0.3;
                }
            }
            else
            {
                SetTop(ParentConstruct.GetBottom() + ParentConstruct.DropShadowDistance); // in normal circumstances, follow the parent
            }
        }

        #endregion
    }
}
