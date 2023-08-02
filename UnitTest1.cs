using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace SteamUITests
{
    [TestFixture]
    public class InvalidLoginTests
    {
        private IWebDriver driver;
        private MainPage mainPage;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            new DriverManager().SetUpDriver(new ChromeConfig());
        }

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();

            mainPage = new MainPage(driver);
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }

        [Test]
        public void InvalidLoginTest()
        {
            // Navigate to URL
            driver.Navigate().GoToUrl("https://store.steampowered.com/");

            // Login button 
            LoginPage loginPage = mainPage.ClickLoginButton();

            // Random credentials and click the sign-in button
            string randomUsername = "random" + DateTime.Now.Millisecond;
            string randomPassword = "randompwd" + DateTime.Now.Millisecond;

            LoginPage postLoginPage = loginPage.InputRandomCredentialsAndSignIn(randomUsername, randomPassword);

            // Assert expected results
            Assert.IsTrue(driver.Url.Contains("https://store.steampowered.com/"), "Main page is not displayed.");
            Assert.IsTrue(postLoginPage.IsLoadingElementDisplayed(), "Loading element is not displayed.");
            Assert.IsTrue(postLoginPage.IsErrorTextDisplayed(), "Error text is not displayed after loading element disappears.");
        }
    }

    public class MainPage
    {
        private IWebDriver driver;

        public MainPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        public LoginPage ClickLoginButton()
        {
            IWebElement loginButton = driver.FindElement(By.ClassName("global_action_link"));
            loginButton.Click();

            return new LoginPage(driver);
        }
    }

    public class LoginPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        // Locators
        private By usernameInputLocator = By.Id("input_username");
        private By passwordInputLocator = By.Id("input_password");
        private By signInButtonLocator = By.XPath("//div[@class='login_btn_v6']//button");

        public LoginPage(IWebDriver driver)
        {
            this.driver = driver;
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        public LoginPage InputRandomCredentialsAndSignIn(string randomUsername, string randomPassword)
        {
            IWebElement usernameInput = wait.Until(ExpectedConditions.ElementIsVisible(usernameInputLocator));
            IWebElement passwordInput = wait.Until(ExpectedConditions.ElementIsVisible(passwordInputLocator));

            usernameInput.SendKeys(randomUsername);
            passwordInput.SendKeys(randomPassword);

            IWebElement signInButton = wait.Until(ExpectedConditions.ElementToBeClickable(signInButtonLocator));
            signInButton.Click();

            return this;
        }

        public bool IsLoadingElementDisplayed()
        {
            try
            {
                IWebElement loadingElement = driver.FindElement(By.Id("loading"));
                return loadingElement.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool IsErrorTextDisplayed()
        {
            try
            {
                IWebElement errorText = wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("error")));
                return errorText.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}




