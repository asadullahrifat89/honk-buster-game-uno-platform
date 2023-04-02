using Uno.UI.Runtime.WebAssembly;

namespace HonkBusterGame
{
    [HtmlElement("audio")]
    public partial class Audio : AudioElement
    {
        #region Fields

        private string _baseUrl;

        #endregion

        #region Ctor

        public Audio(
           Uri uri,
           double volume = 1.0,
           bool loop = false,
           Action playback = null)
        {
            Initialize(
                uri: uri,
                volume: volume,
                loop: loop,
                trackEnded: playback);
        }

        #endregion

        #region Methods

        public void Initialize(
            Uri uri,
            double volume = 1.0,
            bool loop = false,
            Action trackEnded = null)
        {
            var indexUrl = "./"; //Uno.Foundation.WebAssemblyRuntime.InvokeJS("window.location.href;");
            var appPackageId = Environment.GetEnvironmentVariable("UNO_BOOTSTRAP_APP_BASE");

            _baseUrl = $"{indexUrl}{appPackageId}";

            var audio = "element.style.display = \"none\"; element.controls = false; element.hidden= \"hidden\"";

            LoggingExtensions.Log($"audio source: {uri} volume: {volume:0.0} loop: {loop.ToString().ToLower()}");

            this.ExecuteJavascript(audio);

            SetSource(uri);
            SetVolume(volume);
            SetLoop(loop);

            if (trackEnded is not null)
            {
                TrackEnded = trackEnded;
                this.RegisterHtmlEventHandler("ended", EndedEvent);
            }
        }

        public void SetSource(Uri uri)
        {
            var source = $"{_baseUrl}/{uri.AbsoluteUri.Replace("ms-appx:///", "")}";

            this.ExecuteJavascript($"element.src = \"{source}\"; ");

            //LoggingExtensions.Log("audio source: " + source);
        }

        public void SetLoop(bool loop)
        {
            this.ExecuteJavascript($"element.loop = {loop.ToString().ToLower()};");
        }

        public new void Play()
        {
            this.ExecuteJavascript("element.currentTime = 0; element.play();");
            base.Play();
        }

        public new void Stop()
        {
            this.ExecuteJavascript("element.pause(); element.currentTime = 0;");
            base.Stop();
        }

        public new void Pause()
        {
            this.ExecuteJavascript("element.pause();");
            base.Pause();
        }

        public new void Resume()
        {
            this.ExecuteJavascript("element.play();");
            base.Resume();
        }

        public new void SetVolume(double volume)
        {
            this.ExecuteJavascript($"element.volume = {volume.ToString("0.0")};");
            base.SetVolume(volume);
        }

        #endregion
    }
}
