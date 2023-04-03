namespace HonkBusterGame
{
    public partial class MovableBase : GameObject
    {
        #region Methods

        public void MoveUp(double speed)
        {
            if (GameView.IsSlowMotionActivated)
                speed /= 2;

            SetTop(GetTop() - speed * 2);
        }

        public void MoveDown(double speed)
        {
            if (GameView.IsSlowMotionActivated)
                speed /= 2;

            SetTop(GetTop() + speed * 2);
        }

        public void MoveLeft(double speed)
        {
            if (GameView.IsSlowMotionActivated)
                speed /= 2;

            SetLeft(GetLeft() - speed * 2);
        }

        public void MoveRight(double speed)
        {
            if (GameView.IsSlowMotionActivated)
                speed /= 2;

            SetLeft(GetLeft() + speed * 2);
        }

        public void MoveUpRight(double speed)
        {
            if (GameView.IsSlowMotionActivated)
                speed /= 2;

            SetLeft(GetLeft() + speed * 2);
            SetTop(GetTop() - speed);
        }

        public void MoveUpLeft(double speed)
        {
            if (GameView.IsSlowMotionActivated)
                speed /= 2;

            SetLeft(GetLeft() - speed * 2);
            SetTop(GetTop() - speed);
        }

        public void MoveDownRight(double speed)
        {
            if (GameView.IsSlowMotionActivated)
                speed /= 2;

            SetLeft(GetLeft() + speed * 2);
            SetTop(GetTop() + speed);
        }

        public void MoveDownLeft(double speed)
        {
            if (GameView.IsSlowMotionActivated)
                speed /= 2;

            SetLeft(GetLeft() - speed * 2);
            SetTop(GetTop() + speed);
        }

        #endregion
    }
}
