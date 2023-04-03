namespace HonkBusterGame
{
    public partial class AudioPlayerElement : FrameworkElement
    {
        #region Properties

        public Action TrackEnded { get; set; }

        public bool IsPlaying { get; set; }

        public bool IsPaused { get; set; }

        public bool IsStopped { get; set; }

        public double Volume { get; set; }

        #endregion

        #region Methods

        public void Play()
        {
            IsPlaying = true;
            IsStopped = false;
        }

        public void Stop()
        {
            IsPlaying = false;
            IsStopped = true;
        }

        public void Pause()
        {
            IsPlaying = false;
            IsPaused = true;
        }

        public void Resume()
        {
            IsPlaying = true;
            IsPaused = true;
        }

        public void SetVolume(double volume)
        {
            Volume = volume;
        }

        public void EndedEvent(object sender, EventArgs e)
        {
            TrackEnded?.Invoke();
        }

        #endregion
    }
}
