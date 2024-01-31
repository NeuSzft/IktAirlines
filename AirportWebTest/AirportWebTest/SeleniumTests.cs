using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace AirportWebTest;

[TestClass]
public class SeleniumTests
{
    private const string Sut = "http://localhost:8080";
    private WebDriver _webDriver = null!;

    [TestInitialize]
    public void InitializeTest()
    {
        _webDriver = new FirefoxDriver(new FirefoxOptions());
        _webDriver.Url = Sut;
    }

    [TestCleanup]
    public void CleanupTest()
    {
        _webDriver.Quit();
    }

    [TestMethod]
    public void TitleOnAllPagesIsCorrectAndNavButtonsAreWorking()
    {
        _webDriver.FindElement(By.CssSelector("a[href='/']")).Click();
        Assert.AreEqual("Airlines | Home", _webDriver.Title);

        _webDriver.FindElement(By.CssSelector("a[href='/booking']")).Click();
        Assert.AreEqual("Airlines | Booking", _webDriver.Title);

        _webDriver.FindElement(By.CssSelector("a[href='/summary']")).Click();
        Assert.AreEqual("Airlines | Summary", _webDriver.Title);
    }

    [TestMethod]
    public void LightDarkSwitchIsWorking()
    {
        Assert.AreEqual(null, _webDriver.FindElement(By.TagName("body")).GetAttribute("data-bs-theme"));
        _webDriver.FindElement(By.Id("flexSwitchCheckDefault")).Click();
        Assert.AreEqual("dark", _webDriver.FindElement(By.TagName("body")).GetAttribute("data-bs-theme"));
    }
}