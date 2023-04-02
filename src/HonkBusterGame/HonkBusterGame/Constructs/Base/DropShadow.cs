using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;

namespace HonkBusterGame
{
    public partial class DropShadow : MovableConstruct
    {
        #region Fields


        #endregion

        #region Properties

        private Construct ParentConstruct { get; set; }

        private double ParentConstructSpeed { get; set; } = 0;

        #endregion

        #region Ctor

        public DropShadow(
            Action<Construct> animateAction,
            Action<Construct> recycleAction)
        {
            ConstructType = ConstructType.DROP_SHADOW;

            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            Background = new SolidColorBrush(Colors.Black);
            CornerRadius = new CornerRadius(100);
            Opacity = 0.3;

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

        public void SetParent(Construct construct)
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
