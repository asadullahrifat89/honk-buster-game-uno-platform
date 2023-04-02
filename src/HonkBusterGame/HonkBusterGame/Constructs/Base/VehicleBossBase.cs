namespace HonkBusterGame
{
    public partial class VehicleBossBase : VehicleBase
    {
        #region Properties

        public bool IsAttacking { get; set; }

        #endregion

        #region Methods

        public void Reset()
        {
            Opacity = 1;
            Health = 100;

            IsAttacking = false;
            WillHonk = true;
        }

        public void LooseHealth()
        {
            Health -= HitPoint;

            if (IsDead)
            {
                Speed = Constants.DEFAULT_CONSTRUCT_SPEED - 1;
                IsAttacking = false;
            }
        }

        #endregion
    }
}
