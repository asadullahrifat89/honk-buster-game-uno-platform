namespace HonkBusterGame
{
    public partial class AudioTuple
    {
        public Audio[] AudioSources { get; set; }

        public Audio AudioInstance { get; set; }

        public SoundType SoundType { get; set; }

        public AudioTuple(Audio[] audioSources, Audio audioInstance, SoundType soundType)
        {
            AudioSources = audioSources;
            AudioInstance = audioInstance;
            SoundType = soundType;
        }
    }
}
