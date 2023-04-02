namespace HonkBusterGame
{
    public static class AssetsPreCache
    {
        public static async Task PreloadImageAssets(Action progressAction)
        {
            try
            {
#if __ANDROID__ || __IOS__
                foreach (var template in Constants.CONSTRUCT_TEMPLATES)
                {
                    progressAction();
                }
#else
                using HttpClient httpClient = new();

                foreach (var template in Constants.CONSTRUCT_TEMPLATES)
                {

                    var indexUrl = Uno.Foundation.WebAssemblyRuntime.InvokeJS("window.location.href;");
                    var appPackageId = Environment.GetEnvironmentVariable("UNO_BOOTSTRAP_APP_BASE");

                    var baseUrl = $"{indexUrl}{appPackageId}";

                    var source = $"{baseUrl}/{template.Uri.AbsoluteUri.Replace("ms-appx:///", "")}";

                    var response = await httpClient.GetAsync(source);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsByteArrayAsync();
                        if (content is not null && content.Length > 0)
                        {
                            progressAction();
                        }

                        LoggingExtensions.Log("image source: " + source);
                    }
                }
#endif
            }
            catch (Exception ex)
            {
                LoggingExtensions.Log(ex.Message);
            }
        }
    }
}
