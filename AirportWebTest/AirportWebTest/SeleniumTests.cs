using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace AirportWebTest;

[TestClass]
public class SeleniumTests
{
    private const string SutHub = "http://selenium-hub:4444";
    private const string SutAirlines = "http://web-selenium:80";
    private IWebDriver _webDriver = null!;
    private WebDriverWait _wait = null!;

    [TestInitialize]
    public void InitializeTest()
    {
        var firefoxOptions = new FirefoxOptions();
        _webDriver = new RemoteWebDriver(new Uri(SutHub), firefoxOptions.ToCapabilities());
        _webDriver.Navigate().GoToUrl(SutAirlines);
        _wait = new(_webDriver, TimeSpan.FromMilliseconds(3000));
    }

    [TestCleanup]
    public void CleanupTest()
    {
        _webDriver.Quit();
    }

    [TestMethod]
    public void TestTitleOnAllPagesIsCorrectAndNavButtonsAreWorking()
    {
        _webDriver.FindElement(By.CssSelector("a[href='/']")).Click();
        Assert.AreEqual("Airlines | Home", _webDriver.Title);

        _webDriver.FindElement(By.CssSelector("a[href='/booking']")).Click();
        Assert.AreEqual("Airlines | Booking", _webDriver.Title);

        _webDriver.FindElement(By.CssSelector("a[href='/summary']")).Click();
        Assert.AreEqual("Airlines | Summary", _webDriver.Title);
    }

    [TestMethod]
    public void TestLightDarkSwitchIsWorking()
    {
        Assert.AreEqual(null, _webDriver.FindElement(By.TagName("body")).GetAttribute("data-bs-theme"));
        _webDriver.FindElement(By.Id("flexSwitchCheckDefault")).Click();
        Assert.AreEqual("dark", _webDriver.FindElement(By.TagName("body")).GetAttribute("data-bs-theme"));
    }

    [TestMethod]
    public void TestThereAreNoTicketsBought()
    {
        _webDriver.FindElement(By.CssSelector("a[href='/summary']")).Click();
        Assert.AreEqual(true, _webDriver.FindElement(By.TagName("h2")).Displayed);
        Assert.AreEqual(true, _webDriver.FindElement(By.CssSelector(".btn.btn-outline-primary")).Displayed);
    }

    [TestMethod]
    public void TestThereAreNoTicketsBoughtButtonRedirectsToBooking()
    {
        _webDriver.FindElement(By.CssSelector("a[href='/summary']")).Click();
        _webDriver.FindElement(By.CssSelector(".btn.btn-outline-primary")).Click();
        Assert.AreEqual(true, _webDriver.Url.Contains("/booking"));
    }

    [TestMethod]
    public void TestDestinationSelectIsDisabledByDefault()
    {
        _webDriver.FindElement(By.CssSelector("a[href='/booking']")).Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#origin-city option")));
        Assert.AreEqual(false, _webDriver.FindElement(By.Id("destination-city")).Enabled);
    }

    [TestMethod]
    [DataRow("Bangkok", "Tehran")]
    public void TestDestinationSelectIsEnabledAfterSelectingOrigin(string origin, string destination)
    {
        _webDriver.FindElement(By.CssSelector("a[href='/booking']")).Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#origin-city option")));

        _webDriver.FindElement(By.Id("origin-city")).Click();
        _webDriver.FindElement(By.CssSelector($"#origin-city option[value='{origin}']")).Click();

        Assert.AreEqual(true, _webDriver.FindElement(By.Id("destination-city")).Enabled);
    }

    [TestMethod]
    [DataRow("Bangkok", "Tehran")]
    public void TestDestinationAndOriginValue(string origin, string destination)
    {
        _webDriver.FindElement(By.CssSelector("a[href='/booking']")).Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#origin-city option")));

        _webDriver.FindElement(By.Id("origin-city")).Click();
        _webDriver.FindElement(By.CssSelector($"#origin-city option[value='{origin}']")).Click();

        _webDriver.FindElement(By.Id("destination-city")).Click();
        _webDriver.FindElement(By.CssSelector($"#destination-city option[value='{destination}']")).Click();

        Assert.AreEqual(origin, _webDriver.FindElement(By.Id("origin-city")).GetAttribute("value"));
        Assert.AreEqual(destination, _webDriver.FindElement(By.Id("destination-city")).GetAttribute("value"));
    }

    [TestMethod]
    [DataRow("Bangkok", "Tehran")]
    public void TestRouteOriginAndDestinationValueOfTheFirstAndLastTicket(string origin, string destination)
    {
        _webDriver.FindElement(By.CssSelector("a[href='/booking']")).Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#origin-city option")));

        _webDriver.FindElement(By.Id("origin-city")).Click();
        _webDriver.FindElement(By.CssSelector($"#origin-city option[value='{origin}']")).Click();

        _webDriver.FindElement(By.Id("destination-city")).Click();
        _webDriver.FindElement(By.CssSelector($"#destination-city option[value='{destination}']")).Click();

        _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("tickets")));
        foreach (var flight in _webDriver.FindElements(By.ClassName("tickets")))
        {
            var tickets = flight.FindElements(By.CssSelector(".card-header"));
            Assert.IsTrue(tickets.FirstOrDefault()!.Text.Contains(origin));
            Assert.IsTrue(tickets.LastOrDefault()!.Text.Contains(destination));
        }
    }

    [TestMethod]
    [DataRow("Bangkok", "Tehran")]
    public void TestBuyButtonIsDisabledWhenPassengerCountIsZero(string origin, string destination)
    {
        _webDriver.FindElement(By.CssSelector("a[href='/booking']")).Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#origin-city option")));

        _webDriver.FindElement(By.Id("origin-city")).Click();
        _webDriver.FindElement(By.CssSelector($"#origin-city option[value='{origin}']")).Click();

        _webDriver.FindElement(By.Id("destination-city")).Click();
        _webDriver.FindElement(By.CssSelector($"#destination-city option[value='{destination}']")).Click();

        _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("tickets")));
        Assert.AreEqual(false, _webDriver.FindElement(By.CssSelector(".btn-outline-success")).Enabled);
    }

    [TestMethod]
    [DataRow("Bangkok", "Tehran")]
    public void TestBookingWithInvalidPassengerCount(string origin, string destination)
    {
        _webDriver.FindElement(By.CssSelector("a[href='/booking']")).Click();

        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#origin-city option")));

        _webDriver.FindElement(By.Id("origin-city")).Click();
        _webDriver.FindElement(By.CssSelector($"#origin-city option[value='{origin}']")).Click();

        _webDriver.FindElement(By.Id("destination-city")).Click();
        _webDriver.FindElement(By.CssSelector($"#destination-city option[value='{destination}']")).Click();

        _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("tickets")));

        Assert.AreEqual(false, _webDriver.FindElement(By.CssSelector(".btn-outline-success")).Enabled);

        _webDriver.FindElement(By.Id("passengers")).SendKeys("-1");
        Assert.AreEqual(false, _webDriver.FindElement(By.CssSelector(".btn-outline-success")).Enabled);

        _webDriver.FindElement(By.Id("children")).SendKeys("-1");
        Assert.AreEqual(false, _webDriver.FindElement(By.CssSelector(".btn-outline-success")).Enabled);
    }

    [TestMethod]
    [DataRow("Bangkok", "Tehran")]
    public void TestBuyButtonIsEnabledWhenPassengerCountIsGreaterThenZero(string origin, string destination)
    {
        _webDriver.FindElement(By.CssSelector("a[href='/booking']")).Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#origin-city option")));

        _webDriver.FindElement(By.Id("origin-city")).Click();
        _webDriver.FindElement(By.CssSelector($"#origin-city option[value='{origin}']")).Click();

        _webDriver.FindElement(By.Id("destination-city")).Click();
        _webDriver.FindElement(By.CssSelector($"#destination-city option[value='{destination}']")).Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("tickets")));

        _webDriver.FindElement(By.Id("passengers")).SendKeys("1");
        Assert.AreEqual(true, _webDriver.FindElement(By.TagName("button")).Enabled);

        _webDriver.FindElement(By.Id("children")).SendKeys("1");
        Assert.AreEqual(true, _webDriver.FindElement(By.TagName("button")).Enabled);
    }

    [TestMethod]
    [DataRow("Bangkok", "Tehran")]
    public void TestConfirmationAlertBoxIsDisplayedWhenTheBuyButtonIsPressed(string origin, string destination)
    {
        _webDriver.FindElement(By.CssSelector("a[href='/booking']")).Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#origin-city option")));

        _webDriver.FindElement(By.Id("origin-city")).Click();
        _webDriver.FindElement(By.CssSelector($"#origin-city option[value='{origin}']")).Click();

        _webDriver.FindElement(By.Id("destination-city")).Click();
        _webDriver.FindElement(By.CssSelector($"#destination-city option[value='{destination}']")).Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("tickets")));

        _webDriver.FindElement(By.Id("passengers")).SendKeys("1");
        _webDriver.FindElement(By.Id("children")).SendKeys("1");

        _webDriver.FindElement(By.CssSelector(".btn-outline-success")).Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".alert-success")));
        Assert.AreEqual(true, _webDriver.FindElement(By.CssSelector(".alert-success")).Displayed);
    }

    [TestMethod]
    [DataRow("Bangkok", "Tehran")]
    public void TestConfirmationAlertBoxLinkIsRoutingCorrectly(string origin, string destination)
    {
        _webDriver.FindElement(By.CssSelector("a[href='/booking']")).Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#origin-city option")));

        _webDriver.FindElement(By.Id("origin-city")).Click();
        _webDriver.FindElement(By.CssSelector($"#origin-city option[value='{origin}']")).Click();

        _webDriver.FindElement(By.Id("destination-city")).Click();
        _webDriver.FindElement(By.CssSelector($"#destination-city option[value='{destination}']")).Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("tickets")));

        _webDriver.FindElement(By.Id("passengers")).SendKeys("1");
        _webDriver.FindElement(By.Id("children")).SendKeys("1");

        _webDriver.FindElement(By.CssSelector(".btn-outline-success")).Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".alert-success")));

        string ticketUrl = _webDriver.FindElement(By.CssSelector(".alert-success a")).GetAttribute("href");
        _webDriver.FindElement(By.CssSelector(".alert-success a")).Click();
        Assert.AreEqual(true, _webDriver.Url.Contains(ticketUrl));
        Assert.AreEqual(true, _webDriver.Url.Contains($"/summary"));
    }

    [TestMethod]
    [DataRow("Bangkok", "Tehran")]
    public void TestSummaryPageContainsBoughtTicket(string origin, string destination)
    {
        _webDriver.FindElement(By.CssSelector("a[href='/booking']")).Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#origin-city option")));

        _webDriver.FindElement(By.Id("origin-city")).Click();
        _webDriver.FindElement(By.CssSelector($"#origin-city option[value='{origin}']")).Click();

        _webDriver.FindElement(By.Id("destination-city")).Click();
        _webDriver.FindElement(By.CssSelector($"#destination-city option[value='{destination}']")).Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("tickets")));

        _webDriver.FindElement(By.Id("passengers")).SendKeys("1");
        _webDriver.FindElement(By.Id("children")).SendKeys("1");

        _webDriver.FindElement(By.CssSelector(".btn-outline-success")).Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".alert-success")));

        string ticketUrl = _webDriver.FindElement(By.CssSelector(".alert-success a")).GetAttribute("href");
        string ticketId = ticketUrl.Split('#')[^1];

        _webDriver.FindElement(By.CssSelector(".alert-success a")).Click();

        Assert.AreEqual(true,
            _webDriver.FindElement(By.CssSelector($"#{ticketId} .origin-destination")).Text.Contains(origin));
        Assert.AreEqual(true,
            _webDriver.FindElement(By.CssSelector($"#{ticketId} .origin-destination")).Text.Contains(destination));
    }

    [TestMethod]
    [DataRow("Bangkok", "Tehran")]
    [DataRow("Beijing", "Istanbul")]
    [DataRow("Beijing", "Los Angeles")]
    public void TestBookingWithValidOriginAndDestinationWithOneAdult(string origin, string destination)
    {
        _webDriver.FindElement(By.CssSelector("a[href='/booking']")).Click();

        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#origin-city option")));
        _webDriver.FindElement(By.Id("origin-city")).Click();
        _webDriver.FindElement(By.CssSelector($"#origin-city option[value='{origin}']")).Click();

        _webDriver.FindElement(By.Id("destination-city")).Click();
        _webDriver.FindElement(By.CssSelector($"#destination-city option[value='{destination}']")).Click();

        _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("tickets")));

        _webDriver.FindElement(By.Id("passengers")).SendKeys("1");

        _webDriver.FindElement(By.CssSelector(".btn-outline-success")).Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".alert-success")));

        string ticketUrl = _webDriver.FindElement(By.CssSelector(".alert-success a")).GetAttribute("href");
        string ticketId = ticketUrl.Split('#')[^1];

        _webDriver.FindElement(By.CssSelector(".alert-success a")).Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.Id(ticketId)));

        Assert.IsTrue(true);
        //TODO: Get from the api every ticket, then sum up and check the: Total distance, Total flight time and Total cost
    }


    [TestMethod]
    [DataRow("Bangkok", "Tehran")]
    public void TestBookingConfirmation(string origin, string destination)
    {
        // Your test code for booking confirmation and API validation
        _webDriver.FindElement(By.CssSelector("a[href='/booking']")).Click();

        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#origin-city option")));

        _webDriver.FindElement(By.Id("origin-city")).Click();
        _webDriver.FindElement(By.CssSelector($"#origin-city option[value='{origin}']")).Click();

        _webDriver.FindElement(By.Id("destination-city")).Click();
        _webDriver.FindElement(By.CssSelector($"#destination-city option[value='{destination}']")).Click();

        _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("tickets")));

        _webDriver.FindElement(By.Id("passengers")).SendKeys("1");

        _webDriver.FindElement(By.CssSelector(".btn-outline-success")).Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".alert-success")));
        Assert.AreEqual(true, _webDriver.FindElement(By.CssSelector(".alert-success")).Displayed);

        string ticketUrl = _webDriver.FindElement(By.CssSelector(".alert-success a")).GetAttribute("href");
        string ticketId = ticketUrl.Split('#')[^1];

        _webDriver.FindElement(By.CssSelector(".alert-success a")).Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.Id(ticketId)));

        var ticket = _webDriver.FindElement(By.Id($"{ticketId}"));
        ticket.FindElement(By.TagName("button")).Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector($"#{ticketId} .tickets")));

        var tickets = ticket.FindElements(By.CssSelector(".tickets .card")).ToList();
        var ticketsIds = tickets.Select(x => x.GetAttribute("id"));

        // var ticketsDistance = tickets.Select(x => x.GetAttribute("id"));
        // var ticketAdult = tickets.Select(x => x.GetAttribute("id"));
        // var ticketChildren = tickets.Select(x => x.GetAttribute("id"));
    }
}