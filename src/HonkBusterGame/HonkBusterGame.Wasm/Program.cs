namespace HonkBusterGame.Wasm
{
    public class Program
    {
        private static App? _app;

        public static int Main(string[] args)
        {
            Uno.UI.Xaml.Media.FontFamilyHelper.PreloadAsync("ms-appx:///HonkBusterGame/Assets/Fonts/Gameplay.ttf#Gameplay");

            Microsoft.UI.Xaml.Application.Start(_ => _app = new AppHead());

            return 0;
        }
    }
}