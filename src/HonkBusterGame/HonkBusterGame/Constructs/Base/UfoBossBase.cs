namespace HonkBusterGame
{
    public partial class UfoBossBase : HealthyConstruct
    {
        #region Fields

        private readonly AudioStub _audioStub;

        #endregion

        public UfoBossBase()
        {
            _audioStub = new AudioStub(
               (SoundType.UFO_HOVERING, 0.8, true),
               (SoundType.UFO_BOSS_ENTRY, 0.8, false),
               (SoundType.UFO_BOSS_DEAD, 1, false));
        }

        #region Properties

        public bool IsAttacking { get; set; }

        #endregion

        #region Methods

        public void Reset()
        {
            _audioStub.Play(SoundType.UFO_BOSS_ENTRY);

            PlaySoundLoop();

            Opacity = 1;
            Health = 100;
            IsAttacking = false;
        }

        public void PlaySoundLoop()
        {
            _audioStub.Play(SoundType.UFO_HOVERING);
        }

        public void LooseHealth()
        {
            Health -= HitPoint;

            if (IsDead)
            {
                IsAttacking = false;
                StopSoundLoop();
                _audioStub.Play(SoundType.UFO_BOSS_DEAD);
            }
        }

        public void StopSoundLoop()
        {
            _audioStub.Stop(SoundType.UFO_HOVERING);
        }

        #endregion
    }
}
