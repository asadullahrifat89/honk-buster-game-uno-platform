namespace HonkBusterGame
{
    public partial class VehicleBase : HealthyObject
    {
        #region Fields

        private readonly Random _random;
        private double _honkDelay;

        #endregion

        #region Ctor

        public VehicleBase()
        {
            _random = new Random();
        }

        #endregion

        #region Properties

        public bool WillHonk { get; set; }

        #endregion

        #region Methods

        public void Reposition()
        {
            var topOrLeft = _random.Next(2); // generate top and left corner lane wise vehicles
            var lane = _random.Next(2); // generate number of lanes based of screen height
            var randomY = _random.Next(-5, 5);

            switch (topOrLeft)
            {
                case 0:
                    {
                        var xLaneWidth = Constants.DEFAULT_SCENE_WIDTH / 4;

                        switch (lane)
                        {
                            case 0:
                                {
                                    SetPosition(
                                        left: 0 - Width / 2,
                                        top: (Height * -1) + randomY);
                                }
                                break;
                            case 1:
                                {
                                    SetPosition(
                                        left: (xLaneWidth - Width / 1.5),
                                        top: (Height * -1) + randomY);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case 1:
                    {
                        var yLaneHeight = Constants.DEFAULT_SCENE_HEIGHT / 6;

                        switch (lane)
                        {
                            case 0:
                                {
                                    SetPosition(
                                        left: Width * -1,
                                        top: (0 - Height / 2) + randomY);
                                }
                                break;
                            case 1:
                                {
                                    SetPosition(
                                        left: Width * -1,
                                        top: (yLaneHeight - Height / 3) + randomY);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public bool Honk()
        {
            if (WillHonk)
            {
                _honkDelay--;

                if (_honkDelay < 0)
                {
                    SetHonkDelay();
                    return true;
                }
            }

            return false;
        }

        public void SetHonkDelay()
        {
            _honkDelay = _random.Next(30, 80);
        }

        #endregion
    }
}
