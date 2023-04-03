namespace HonkBusterGame
{
    public partial class GameCheckpoint
    {
        #region Ctor

        public GameCheckpoint(double threasholdLimit)
        {
            ThreasholdLimit = threasholdLimit;
        }

        #endregion

        #region Properties

        private double LastReleasePoint { get; set; } = 0;

        private double ThreasholdLimit { get; set; }

        #endregion

        #region Methods

        public double GetReleasePointDifference()
        {
            return ThreasholdLimit;
        }

        public bool ShouldRelease(double currentPoint)
        {
            var release = currentPoint - LastReleasePoint > ThreasholdLimit;

            return release;
        }

        public void IncreaseThreasholdLimit(double increment, double currentPoint)
        {
            LastReleasePoint = currentPoint;
            ThreasholdLimit += increment;
        }

        public void Reset(double value)
        {
            ThreasholdLimit = value;
            LastReleasePoint = 0;
        }

        #endregion
    }
}
