namespace HonkBusterGame
{
    public partial class AudioTuple
    {
        public HtmlAudioTag[] Sources { get; set; }

        public HtmlAudioTag Instance { get; set; }

        public SoundType SoundType { get; set; }

        public AudioTuple(HtmlAudioTag[] audioSources, HtmlAudioTag audioInstance, SoundType soundType)
        {
            Sources = audioSources;
            Instance = audioInstance;
            SoundType = soundType;
        }
    }
}
