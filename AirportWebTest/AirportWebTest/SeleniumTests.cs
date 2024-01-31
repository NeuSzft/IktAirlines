using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

namespace AirportWebTest;

[TestClass]
public class SeleniumTests
{
    private const string SutHub = "http://localhost:4444";
    private const string SutAirlines = "http://web-selenium:80";
    private IWebDriver _webDriver = null!;
    
    [TestInitialize]
    public void InitializeTest()
    {
        var firefoxOptions = new FirefoxOptions();
        _webDriver = new RemoteWebDriver(new Uri(SutHub), firefoxOptions.ToCapabilities());
        _webDriver.Url = SutAirlines;
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