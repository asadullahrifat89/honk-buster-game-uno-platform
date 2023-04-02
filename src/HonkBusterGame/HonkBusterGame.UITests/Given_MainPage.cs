namespace HonkBusterGame.UITests
{
    public class Given_MainPage : TestBase
    {
        [Test]
        public void When_SmokeTest()
        {
            // NOTICE
            // To run UITests, Run the WASM target without debugger. Note
            // the port that is being used and update the Constants.cs file
            // in the UITests project with the correct port number.

            // Query for the MainPage Text Block
            Query textBlock = q => q.All().Marked("Hello Uno Platform");
            App.WaitForElement(textBlock);

            // Take a screenshot and add it to the test results
            TakeScreenshot("After launch");
        }
    }
}