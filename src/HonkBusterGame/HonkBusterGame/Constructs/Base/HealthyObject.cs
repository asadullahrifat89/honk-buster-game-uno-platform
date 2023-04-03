namespace HonkBusterGame
{
    public partial class HealthyObject : AnimableObject
    {
        public double Health { get; set; }

        public int HitPoint { get; set; } = 5;

        public bool IsDead => Health <= 0;
    }
}
