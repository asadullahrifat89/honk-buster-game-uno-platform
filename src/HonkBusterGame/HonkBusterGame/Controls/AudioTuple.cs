namespace HonkBusterGame
{
    public partial class AudioTuple
    {
        public AudioElement[] AudioSources { get; set; }

        public AudioElement AudioInstance { get; set; }

        public SoundType SoundType { get; set; }

        public AudioTuple(AudioElement[] audioSources, AudioElement audioInstance, SoundType soundType)
        {
            AudioSources = audioSources;
            AudioInstance = audioInstance;
            SoundType = soundType;
        }
    }
}
