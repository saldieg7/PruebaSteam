using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

public class MainPage
{
    private IWebDriver driver;

    public MainPage(IWebDriver driver)
    {
        this.driver = driver;
    }

    public void ClickLoginButton()
    {
        IWebElement loginButton = driver.FindElement(By.ClassName("global_action_link"));
        loginButton.Click();
    }
}

public class LoginPage
{
    private IWebDriver driver;

    public LoginPage(IWebDriver driver)
    {
        this.driver = driver;
    }

    public void InputRandomCredentialsAndSignIn(string randomUsername, string randomPassword)
    {
        IList<IWebElement> textInputs = driver.FindElements(By.ClassName("newlogindialog_TextInput_2eKVn"));
        IWebElement usernameInput = textInputs[0];
        IWebElement passwordInput = textInputs[1];

        usernameInput.SendKeys(randomUsername);
        passwordInput.SendKeys(randomPassword);

        IWebElement signInButton = driver.FindElement(By.ClassName("newlogindialog_SubmitButton_2QgFE"));
        signInButton.Click();
    }
}

public static class CustomExpectedConditions
{
    public static Func<IWebDriver, bool> UrlContains(string partialUrl)
    {
        return driver => driver.Url.Contains(partialUrl);
    }
}

namespace SteamUITests
{
    [TestFixture]
    public class Task2Tests
    {
        private IWebDriver driver;
        private MainPage mainPage;
        private LoginPage loginPage;

        [SetUp]
        public void Setup()
        {
            new DriverManager().SetUpDriver(new ChromeConfig());
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();

            mainPage = new MainPage(driver);
            loginPage = new LoginPage(driver);
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }

        [Test]
        public void InvalidLoginTest()
        {
            // Go to the URL
            driver.Navigate().GoToUrl("https://store.steampowered.com/");

            // Click login button 
            mainPage.ClickLoginButton();

            //  Random user and password 
            string randomUsername = "random" + DateTime.Now.Millisecond;
            string randomPassword = "randompwd" + DateTime.Now.Millisecond;

            loginPage.InputRandomCredentialsAndSignIn(randomUsername, randomPassword);

            // Explicit waits for the main page and login page URLs
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(CustomExpectedConditions.UrlContains("https://store.steampowered.com/"));
            wait.Until(CustomExpectedConditions.UrlContains("https://store.steampowered.com/login/"));

            // Assert expected results
            Assert.IsTrue(driver.Url.Contains("https://store.steampowered.com/"), "Main page is not displayed.");
            Assert.IsTrue(driver.Url.Contains("https://store.steampowered.com/login/"), "Login page is not displayed.");
        }
    }
}




