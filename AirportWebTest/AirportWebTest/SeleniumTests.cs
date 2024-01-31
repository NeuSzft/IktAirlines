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

    [TestMethod]
    [DataRow("Bangkok", "Tehran")]
    [DataRow("Beijing", "Istanbul")]
    [DataRow("Beijing", "Los Angeles")]
    public void TryBooking(string origin, string destination)
    {
        WebDriverWait wait = new(_webDriver, TimeSpan.FromMilliseconds(250));

        _webDriver.FindElement(By.CssSelector("a[href='/booking']")).Click();

        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#origin-city option")));
        Assert.AreEqual(false, _webDriver.FindElement(By.Id("destination-city")).Enabled);

        _webDriver.FindElement(By.Id("origin-city")).Click();
        _webDriver.FindElement(By.CssSelector($"#origin-city option[value='{origin}']")).Click();
        Assert.AreEqual(origin, _webDriver.FindElement(By.Id("origin-city")).GetAttribute("value"));
        Assert.AreEqual(true, _webDriver.FindElement(By.Id("destination-city")).Enabled);

        _webDriver.FindElement(By.Id("destination-city")).Click();
        _webDriver.FindElement(By.CssSelector($"#destination-city option[value='{destination}']")).Click();
        Assert.AreEqual(destination, _webDriver.FindElement(By.Id("destination-city")).GetAttribute("value"));

        wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("tickets")));
        foreach (var flight in _webDriver.FindElements(By.ClassName("tickets")))
        {
            var tickets = flight.FindElements(By.CssSelector(".card-header"));
            Assert.IsTrue(tickets.FirstOrDefault()!.Text.Contains(origin));
            Assert.IsTrue(tickets.LastOrDefault()!.Text.Contains(destination));
        }

        Assert.AreEqual(false, _webDriver.FindElement(By.CssSelector(".btn-outline-success")).Enabled);

        _webDriver.FindElement(By.Id("passengers")).SendKeys("1");
        Assert.AreEqual(true, _webDriver.FindElement(By.TagName("button")).Enabled);
        _webDriver.FindElement(By.Id("children")).SendKeys("1");

        _webDriver.FindElement(By.CssSelector(".btn-outline-success")).Click();
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".alert-success")));
        Assert.AreEqual(true, _webDriver.FindElement(By.CssSelector(".alert-success")).Displayed);

        string ticketUrl = _webDriver.FindElement(By.CssSelector(".alert-success a")).GetAttribute("href");
        string ticketId = ticketUrl.Split('#')[^1];
        _webDriver.FindElement(By.CssSelector(".alert-success a")).Click();

        Assert.AreEqual("Airlines | Summary", _webDriver.Title);
        Assert.AreEqual(true, _webDriver.Url.Contains(ticketUrl));
        wait.Until(ExpectedConditions.ElementIsVisible(By.Id(ticketId)));

        Assert.AreEqual(true, _webDriver.FindElement(By.CssSelector($"#{ticketId} .origin-destination")).Text.Contains(origin));
        Assert.AreEqual(true, _webDriver.FindElement(By.CssSelector($"#{ticketId} .origin-destination")).Text.Contains(destination));

        //TODO: Get from the api every ticket, then sum up and check the: Total distance, Total flight time and Total cost
    }
}