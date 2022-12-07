using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V106.Debugger;
using System;
using System.Configuration;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using DocumentFormat.OpenXml.Spreadsheet;

namespace SemestralnaPracaTest
{
    public class Test
    {
        private Report report;
        private Data data;
        private IWebDriver driver;

        #region CONTROLS

        public Test()
        {
            report = new Report();
            data = new Data();
        }

        public void End()
        {
            report.Save();
            data.CloseConnection();
        }

        public IWebDriver GetLoggedDriver(bool? role, bool admin)
        {
            IWebDriver driver = GetUnloggedDriver();

            driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
            driver.FindElement(By.XPath("//a[contains(text(),'PRIHLÁSENIE')]")).Click();
            if (role == true)
            {
                driver.FindElement(By.XPath("//input[@id='mailInput']")).SendKeys(ConfigurationManager.AppSettings["AdminMail"].ToString());
                driver.FindElement(By.XPath("//input[@id='passwordInput']")).SendKeys(ConfigurationManager.AppSettings["AdminPassword"].ToString());
            }
            else if (role == false)
            {
                driver.FindElement(By.XPath("//input[@id='mailInput']")).SendKeys(ConfigurationManager.AppSettings["UserMail"].ToString());
                driver.FindElement(By.XPath("//input[@id='passwordInput']")).SendKeys(ConfigurationManager.AppSettings["UserPassword"].ToString());
            }
            else
            {
                if (admin)
                {
                    driver.FindElement(By.XPath("//input[@id='mailInput']")).SendKeys(ConfigurationManager.AppSettings["AdminMail"].ToString());
                }
                else
                {
                    driver.FindElement(By.XPath("//input[@id='mailInput']")).SendKeys(ConfigurationManager.AppSettings["UserMail"].ToString());
                }
                driver.FindElement(By.XPath("//input[@id='passwordInput']")).SendKeys(ConfigurationManager.AppSettings["DummyPassword"].ToString());
            }
            driver.FindElement(By.XPath("//button[contains(text(),'PRIHLÁSIŤ')]")).Click();

            return driver;
        }

        private IWebDriver GetUnloggedDriver()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--window-size=1920,1080");
            IWebDriver driver = new ChromeDriver(options);

            driver.Manage().Cookies.DeleteCookieNamed("PortfolioSession");
            driver.Navigate().GoToUrl(ConfigurationManager.ConnectionStrings["Image"].ConnectionString);

            return driver;
        }

        #endregion

        #region TESTS

        public void LoginLogoutWithCorrectCredentialsUser()
        {
            try
            {
                IWebDriver driver = GetLoggedDriver(false, false);

                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'ÚDAJE')]")).Click();
                Assert.AreEqual(driver.FindElement(By.XPath("//input[@id='mailInput']")).GetAttribute("value").ToString(), ConfigurationManager.AppSettings["UserMail"].ToString());

                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'ODHLÁSENIE')]")).Click();
                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                Assert.IsTrue(driver.FindElement(By.XPath("//a[contains(text(),'PRIHLÁSENIE')]")).Displayed);
                Assert.IsTrue(driver.FindElement(By.XPath("//a[contains(text(),'REGISTRÁCIA')]")).Displayed);

                driver.Quit();

                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, true, null);
            }
            catch (Exception exception)
            {
                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, false, exception);
            }
        }

        public void ChangeCredentialsUser(bool? role)
        {
            try
            {
                IWebDriver driver;
                if (role == null)
                {
                    driver = GetLoggedDriver(false, false);
                }
                else
                {
                    driver = GetLoggedDriver(null, false);
                }

                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'ÚDAJE')]")).Click();
                driver.FindElement(By.XPath("//input[@id='nameInput']")).Clear();
                driver.FindElement(By.XPath("//input[@id='nameInput']")).SendKeys(role == null ? ConfigurationManager.AppSettings["DummyName"].ToString() : ConfigurationManager.AppSettings["UserName"].ToString());
                driver.FindElement(By.XPath("//input[@id='surnameInput']")).Clear();
                driver.FindElement(By.XPath("//input[@id='surnameInput']")).SendKeys(role == null ? ConfigurationManager.AppSettings["DummySurname"].ToString() : ConfigurationManager.AppSettings["UserSurname"].ToString());
                driver.FindElement(By.XPath("//input[@id='phoneInput']")).Clear();
                driver.FindElement(By.XPath("//input[@id='phoneInput']")).SendKeys(role == null ? ConfigurationManager.AppSettings["DummyPhone"].ToString() : ConfigurationManager.AppSettings["UserPhone"].ToString());
                driver.FindElement(By.XPath("//input[@id='passwordInput']")).Clear();
                driver.FindElement(By.XPath("//input[@id='passwordInput']")).SendKeys(role == null ? ConfigurationManager.AppSettings["DummyPassword"].ToString() : ConfigurationManager.AppSettings["UserPassword"].ToString());
                driver.FindElement(By.XPath("//input[@id='passwordConfirmationInput']")).Clear();
                driver.FindElement(By.XPath("//input[@id='passwordConfirmationInput']")).SendKeys(role == null ? ConfigurationManager.AppSettings["DummyPassword"].ToString() : ConfigurationManager.AppSettings["UserPassword"].ToString());
                driver.FindElement(By.XPath("//button[contains(text(),'ULOŽIŤ')]")).Click();
                Assert.IsTrue(driver.FindElement(By.XPath("//p[contains(text(),'údaje boli uložené')]")).Displayed);

                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'ÚDAJE')]")).Click();
                Assert.AreEqual(driver.FindElement(By.XPath("//input[@id='nameInput']")).GetAttribute("value").ToString(), role == null ? ConfigurationManager.AppSettings["DummyName"].ToString() : ConfigurationManager.AppSettings["UserName"].ToString());
                Assert.AreEqual(driver.FindElement(By.XPath("//input[@id='surnameInput']")).GetAttribute("value").ToString(), role == null ? ConfigurationManager.AppSettings["DummySurname"].ToString() : ConfigurationManager.AppSettings["UserSurname"].ToString());
                Assert.AreEqual(driver.FindElement(By.XPath("//input[@id='phoneInput']")).GetAttribute("value").ToString(), role == null ? ConfigurationManager.AppSettings["DummyPhone"].ToString() : ConfigurationManager.AppSettings["UserPhone"].ToString());

                driver.Quit();

                report.Write((System.Reflection.MethodBase.GetCurrentMethod().Name + role), true, null);
            }
            catch (Exception exception)
            {
                report.Write((System.Reflection.MethodBase.GetCurrentMethod().Name + role), false, exception);
            }
        }

        public void SendRequestUser(bool role)
        {
            try
            {
                IWebDriver driver = GetLoggedDriver(false, false);
                int category = data.GetRandomNumber(5);
                string description = data.GetRandomText(data.GetRandomNumber(100));
                DateTime date = data.GetRandomDate("REQUESTS");

                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'ODOSLANIE ŽIADOSTI')]")).Click();
                new SelectElement(driver.FindElement(By.XPath("//select[@id='categoryInput']"))).SelectByIndex(category);
                driver.FindElement(By.XPath("//textarea[@id='descriptionInput']")).SendKeys(description);
                driver.FindElement(By.XPath("//input[@id='dateInput']")).SendKeys(date.ToString("MM-dd-yyyy"));
                driver.FindElement(By.XPath("//button[contains(text(),'ODOSLAŤ')]")).Click();
                Assert.IsTrue(driver.FindElement(By.XPath("//p[contains(text(),'požiadavka bola úspešne zaregistrovaná')]")).Displayed);

                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'SPRÁVA ŽIADOSTÍ')]")).Click();
                driver.FindElement(By.XPath("//body/div[2]/div[1]/div[3]/a[1]/i[1]")).Click();
                Assert.AreEqual(driver.FindElement(By.XPath("//textarea[@id='descriptionInput']")).GetAttribute("value").ToString(), description);
                Assert.AreEqual(driver.FindElement(By.XPath("//input[@id='statusInput']")).GetAttribute("value").ToString(), "čakajúce");
                Assert.AreEqual(driver.FindElement(By.XPath("//input[@id='createdInput']")).GetAttribute("value").ToString(), DateTime.Now.ToString("yyyy-MM-dd"));
                Assert.AreEqual(driver.FindElement(By.XPath("//input[@id='scheduledInput']")).GetAttribute("value").ToString(), date.ToString("yyyy-MM-dd"));

                driver.Quit();

                report.Write((System.Reflection.MethodBase.GetCurrentMethod().Name + (role ? "Admin" : "User")), true, null);
            }
            catch (Exception exception)
            {
                report.Write((System.Reflection.MethodBase.GetCurrentMethod().Name + (role ? "Admin" : "User")), false, exception);
            }
        }

        public void EditRequestUser()
        {
            try
            {
                IWebDriver driver = GetLoggedDriver(false, false);
                string description = data.GetRandomText(data.GetRandomNumber(100));
                DateTime date = data.GetRandomDate("REQUESTS");

                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'SPRÁVA ŽIADOSTÍ')]")).Click();
                driver.FindElement(By.XPath("//body/div[2]/div[1]/div[3]/a[1]/i[1]")).Click();
                driver.FindElement(By.XPath("//input[@id='scheduledInput']")).Clear();
                driver.FindElement(By.XPath("//input[@id='scheduledInput']")).SendKeys(date.ToString("MM-dd-yyyy"));
                driver.FindElement(By.XPath("//textarea[@id='descriptionInput']")).Clear();
                driver.FindElement(By.XPath("//textarea[@id='descriptionInput']")).SendKeys(description);
                driver.FindElement(By.XPath("//button[contains(text(),'UPRAVIŤ')]")).Click();
                Assert.IsTrue(driver.FindElement(By.XPath("//p[contains(text(),'požiadavka bola úspešne aktualizovaná')]")).Displayed);

                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'SPRÁVA ŽIADOSTÍ')]")).Click();
                driver.FindElement(By.XPath("//body/div[2]/div[1]/div[3]/a[1]/i[1]")).Click();
                Assert.AreEqual(driver.FindElement(By.XPath("//textarea[@id='descriptionInput']")).GetAttribute("value").ToString(), description);
                Assert.AreEqual(driver.FindElement(By.XPath("//input[@id='scheduledInput']")).GetAttribute("value").ToString(), date.ToString("yyyy-MM-dd"));

                driver.Quit();

                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, true, null);
            }
            catch (Exception exception)
            {
                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, false, exception);
            }
        }

        public void DeleteRequestUser()
        {
            try
            {
                IWebDriver driver = GetLoggedDriver(false, false);
                string date;

                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'SPRÁVA ŽIADOSTÍ')]")).Click();
                driver.FindElement(By.XPath("//body/div[2]/div[1]/div[3]/a[1]/i[1]")).Click();
                date = driver.FindElement(By.XPath("//input[@id='scheduledInput']")).GetAttribute("value").ToString();
                Assert.IsFalse(data.CheckRequestDate(DateTime.Parse(date)));

                driver.Navigate().Back();
                driver.FindElement(By.XPath("//body/div[2]/div[1]/div[4]/a[1]/i[1]")).Click();
                driver.SwitchTo().Alert().Accept();
                Assert.IsTrue(driver.FindElement(By.XPath("/html[1]/body[1]/div[2]/p[1]")).Displayed);
                Assert.IsTrue(data.CheckRequestDate(DateTime.Parse(date)));

                driver.Quit();

                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, true, null);
            }
            catch (Exception exception)
            {
                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, false, exception);
            }
        }

        public void RegisterCorrectCredentialstUser()
        {
            try
            {
                IWebDriver driver = GetUnloggedDriver();

                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'REGISTRÁCIA')]")).Click();
                driver.FindElement(By.XPath("//input[@id='mailInput']")).SendKeys(ConfigurationManager.AppSettings["DummyMail"].ToString());
                driver.FindElement(By.XPath("//input[@id='nameInput']")).SendKeys(ConfigurationManager.AppSettings["DummyName"].ToString());
                driver.FindElement(By.XPath("//input[@id='surnameInput']")).SendKeys(ConfigurationManager.AppSettings["DummySurname"].ToString());
                driver.FindElement(By.XPath("//input[@id='phoneInput']")).SendKeys(ConfigurationManager.AppSettings["DummyPhone"].ToString());
                driver.FindElement(By.XPath("//input[@id='passwordInput']")).SendKeys(ConfigurationManager.AppSettings["DummyPassword"].ToString());
                driver.FindElement(By.XPath("//input[@id='passwordConfirmationInput']")).SendKeys(ConfigurationManager.AppSettings["DummyPassword"].ToString());
                driver.FindElement(By.XPath("//button[contains(text(),'REGISTROVAŤ')]")).Click();

                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'ÚDAJE')]")).Click();
                Assert.AreEqual(driver.FindElement(By.XPath("//input[@id='mailInput']")).GetAttribute("value"), ConfigurationManager.AppSettings["DummyMail"].ToString());

                driver.Quit();

                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, true, null);
            }
            catch (Exception exception)
            {
                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, false, exception);
            }
        }

        public void DeleteUserAdmin()
        {
            try
            {
                IWebDriver driver = GetLoggedDriver(true, true);

                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'SPRÁVA POUŽIVATEĽOV')]")).Click();
                driver.FindElement(By.XPath("//body/div[2]/div[2]/div[4]/a[1]/i[1]")).Click();
                driver.SwitchTo().Alert().Accept();
                Assert.IsTrue(driver.FindElement(By.XPath("/html[1]/body[1]/div[2]/p[1]")).Displayed);

                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'ODHLÁSENIE')]")).Click();
                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'PRIHLÁSENIE')]")).Click();
                driver.FindElement(By.XPath("//input[@id='mailInput']")).SendKeys(ConfigurationManager.AppSettings["DummyMail"].ToString());
                driver.FindElement(By.XPath("//input[@id='passwordInput']")).SendKeys(ConfigurationManager.AppSettings["AdminPassword"].ToString());
                driver.FindElement(By.XPath("//button[contains(text(),'PRIHLÁSIŤ')]")).Click();
                Assert.IsTrue(driver.FindElement(By.XPath("//a[contains(text(),'daný účet neexistuje')]")).Displayed);

                driver.Quit();

                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, true, null);
            }
            catch (Exception exception)
            {
                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, false, exception);
            }
        }

        public void Name(bool role)
        {
            try
            {
                IWebDriver driver = GetLoggedDriver(false, false);

                driver.FindElement(By.XPath("")).Click();

                driver.Quit();

                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, true, null);
            }
            catch (Exception exception)
            {
                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, false, exception);
            }
        }

        //public void Name()
        //{
        //    try
        //    {
        //        IWebDriver driver = GetLoggedDriver(false, false);
        //
        //        driver.FindElement(By.XPath("")).Click();   
        //
        //        driver.Quit();
        //
        //        report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, true, null);
        //    }
        //    catch (Exception exception)
        //    {
        //        report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, false, exception);
        //    }
        //}

        #endregion
    }
}