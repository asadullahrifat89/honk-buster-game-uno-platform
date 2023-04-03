namespace HonkBusterGame
{
    public partial class AudioTuple
    {
        public AudioElement[] Sources { get; set; }

        public AudioElement Instance { get; set; }

        public SoundType SoundType { get; set; }

        public AudioTuple(AudioElement[] audioSources, AudioElement audioInstance, SoundType soundType)
        {
            Sources = audioSources;
            Instance = audioInstance;
            SoundType = soundType;
        }
    }
}
