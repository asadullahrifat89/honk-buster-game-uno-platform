namespace HonkBusterGame
{
    public static class LoggingExtensions
    {
        public static void Log(string message)
        {
#if DEBUG
            Console.WriteLine(message);
#endif
        }
    }
}
