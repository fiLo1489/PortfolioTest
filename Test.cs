using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Configuration;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;

namespace SemestralnaPracaTest
{
    public class Test
    {
        private Report report;
        private Data data;
        private IWebDriver driver;
        Dictionary<string, string> input;

        #region CONTROLS

        public Test()
        {
            report = new Report();
            data = new Data();
            input = new Dictionary<string, string>();
        }

        public void End()
        {
            report.Save();
            data.Close();
        }

        public IWebDriver GetLoggedDriver(bool? loginRole, bool? passwordRole)
        {
            try
            {
                input.Clear();
                IWebDriver driver = GetUnloggedDriver();
                string _LOGIN = string.Empty;
                string _PASSWORD = string.Empty;

                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'PRIHLÁSENIE')]")).Click();
                if (loginRole == true)
                {
                    _LOGIN = ConfigurationManager.AppSettings["AdminMail"].ToString();
                    _PASSWORD = ConfigurationManager.AppSettings["AdminPassword"].ToString();
                }
                else if (loginRole == false)
                {
                    _LOGIN = ConfigurationManager.AppSettings["UserMail"].ToString();
                    _PASSWORD = ConfigurationManager.AppSettings["UserPassword"].ToString();
                }
                else
                {
                    if (passwordRole == true)
                    {
                        _LOGIN = ConfigurationManager.AppSettings["AdminMail"].ToString();
                    }
                    else if (passwordRole == false)
                    {
                        _LOGIN = ConfigurationManager.AppSettings["UserMail"].ToString();
                    }
                    else
                    {
                        _LOGIN = ConfigurationManager.AppSettings["DummyMail"].ToString();
                    }
                    _PASSWORD = ConfigurationManager.AppSettings["DummyPassword"].ToString();
                }
                driver.FindElement(By.XPath("//input[@id='mailInput']")).SendKeys(_LOGIN);
                driver.FindElement(By.XPath("//input[@id='passwordInput']")).SendKeys(_PASSWORD);
                input.Add(nameof(_LOGIN), _LOGIN);
                input.Add(nameof(_PASSWORD), _PASSWORD);
                driver.FindElement(By.XPath("//button[contains(text(),'PRIHLÁSIŤ')]")).Click();

                return driver;
            }
            catch (Exception exception)
            {
                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, exception, null);

                return null;
            }
        }

        private IWebDriver GetUnloggedDriver()
        {
            try
            {
                ChromeOptions options = new ChromeOptions();
                options.AddArgument("--window-size=1920,1080");
                IWebDriver driver = new ChromeDriver(options);

                driver.Manage().Cookies.DeleteCookieNamed("PortfolioSession");
                driver.Navigate().GoToUrl(ConfigurationManager.ConnectionStrings["Image"].ConnectionString);
                input.Clear();

                return driver;
            }
            catch (Exception exception)
            {
                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, exception, null);

                return null;
            }
        }

        #endregion

        #region TESTS

        public void LoginLogoutWithCorrectCredentialsUser()
        {
            IWebDriver driver = GetLoggedDriver(false, false);

            try
            {
                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'ÚDAJE')]")).Click();
                Assert.AreEqual(driver.FindElement(By.XPath("//input[@id='mailInput']")).GetAttribute("value").ToString(), ConfigurationManager.AppSettings["UserMail"].ToString());

                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'ODHLÁSENIE')]")).Click();
                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                Assert.IsTrue(driver.FindElement(By.XPath("//a[contains(text(),'PRIHLÁSENIE')]")).Displayed);
                Assert.IsTrue(driver.FindElement(By.XPath("//a[contains(text(),'REGISTRÁCIA')]")).Displayed);

                driver.Quit();

                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, null, input);
            }
            catch (Exception exception)
            {
                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, exception, input);
            }
        }

        public void ChangeCredentialsUser(bool? role)
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
            string name = (role == null ? ConfigurationManager.AppSettings["DummyName"].ToString() : ConfigurationManager.AppSettings["UserName"].ToString());
            string surname = (role == null ? ConfigurationManager.AppSettings["DummySurname"].ToString() : ConfigurationManager.AppSettings["UserSurname"].ToString());
            string phone = (role == null ? ConfigurationManager.AppSettings["DummyPhone"].ToString() : ConfigurationManager.AppSettings["UserPhone"].ToString());
            string password = (role == null ? ConfigurationManager.AppSettings["DummyPassword"].ToString() : ConfigurationManager.AppSettings["UserPassword"].ToString());
            input.Add(nameof(name), name);
            input.Add(nameof(surname), surname);
            input.Add(nameof(phone), phone);
            input.Add(nameof(password), password);

            try
            {
                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'ÚDAJE')]")).Click();
                driver.FindElement(By.XPath("//input[@id='nameInput']")).Clear();
                driver.FindElement(By.XPath("//input[@id='nameInput']")).SendKeys(name);
                driver.FindElement(By.XPath("//input[@id='surnameInput']")).Clear();
                driver.FindElement(By.XPath("//input[@id='surnameInput']")).SendKeys(surname);
                driver.FindElement(By.XPath("//input[@id='phoneInput']")).Clear();
                driver.FindElement(By.XPath("//input[@id='phoneInput']")).SendKeys(phone);
                driver.FindElement(By.XPath("//input[@id='passwordInput']")).Clear();
                driver.FindElement(By.XPath("//input[@id='passwordInput']")).SendKeys(password);
                driver.FindElement(By.XPath("//input[@id='passwordConfirmationInput']")).Clear();
                driver.FindElement(By.XPath("//input[@id='passwordConfirmationInput']")).SendKeys(password);
                driver.FindElement(By.XPath("//button[contains(text(),'ULOŽIŤ')]")).Click();
                Assert.IsTrue(driver.FindElement(By.XPath("//p[contains(text(),'údaje boli uložené')]")).Displayed);

                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'ÚDAJE')]")).Click();
                Assert.AreEqual(driver.FindElement(By.XPath("//input[@id='nameInput']")).GetAttribute("value").ToString(), name);
                Assert.AreEqual(driver.FindElement(By.XPath("//input[@id='surnameInput']")).GetAttribute("value").ToString(), surname);
                Assert.AreEqual(driver.FindElement(By.XPath("//input[@id='phoneInput']")).GetAttribute("value").ToString(), phone);

                driver.Quit();

                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, null, input);
            }
            catch (Exception exception)
            {
                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, exception, input);
            }
        }

        public void SendRequestUser()
        {
            IWebDriver driver = GetLoggedDriver(false, false);
            int category = data.GetRandomNumber(5);
            string description = data.GetRandomText(data.GetRandomNumber(100));
            DateTime date = data.GetRandomRequestDate();
            input.Add(nameof(category), category.ToString());
            input.Add(nameof(description), description);
            input.Add(nameof(date), date.ToString("MM-dd-yyyy"));

            try
            {
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

                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, null, input);
            }
            catch (Exception exception)
            {
                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, exception, input);
            }
        }

        public void EditRequestUser()
        {
            IWebDriver driver = GetLoggedDriver(false, false);
            string description = data.GetRandomText(data.GetRandomNumber(100));
            DateTime date = data.GetRandomRequestDate();
            input.Add(nameof(description), description);
            input.Add(nameof(date), date.ToString("MM-dd-yyyy"));

            try
            {
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

                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, null, input);
            }
            catch (Exception exception)
            {
                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, exception, input);
            }
        }

        public void DeleteRequestUser()
        {
            IWebDriver driver = GetLoggedDriver(false, false);
            string date;
            int count = data.GetCount("REQUESTS");

            try
            {
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
                Assert.AreEqual(data.GetCount("REQUESTS"), (count - 1));

                driver.Quit();

                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, null, input);
            }
            catch (Exception exception)
            {
                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, exception, input);
            }
        }

        public void RegisterCorrectCredentialsUser()
        {
            IWebDriver driver = GetUnloggedDriver();
            string mail = (ConfigurationManager.AppSettings["DummyMail"].ToString());
            string name = (ConfigurationManager.AppSettings["DummyName"].ToString());
            string surname = (ConfigurationManager.AppSettings["DummySurname"].ToString());
            string phone = (ConfigurationManager.AppSettings["DummyPhone"].ToString());
            string password = (ConfigurationManager.AppSettings["DummyPassword"].ToString());
            input.Add(nameof(mail), mail);
            input.Add(nameof(name), name);
            input.Add(nameof(surname), surname);
            input.Add(nameof(phone), phone);
            input.Add(nameof(password), password);

            try
            {
                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'REGISTRÁCIA')]")).Click();
                driver.FindElement(By.XPath("//input[@id='mailInput']")).SendKeys(mail);
                driver.FindElement(By.XPath("//input[@id='nameInput']")).SendKeys(name);
                driver.FindElement(By.XPath("//input[@id='surnameInput']")).SendKeys(surname);
                driver.FindElement(By.XPath("//input[@id='phoneInput']")).SendKeys(phone);
                driver.FindElement(By.XPath("//input[@id='passwordInput']")).SendKeys(password);
                driver.FindElement(By.XPath("//input[@id='passwordConfirmationInput']")).SendKeys(password);
                driver.FindElement(By.XPath("//button[contains(text(),'REGISTROVAŤ')]")).Click();

                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'ÚDAJE')]")).Click();
                Assert.AreEqual(driver.FindElement(By.XPath("//input[@id='mailInput']")).GetAttribute("value"), mail);

                driver.Quit();

                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, null, input);
            }
            catch (Exception exception)
            {
                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, exception, input);
            }
        }

        public void DeleteUserAdmin()
        {
            IWebDriver driver = GetLoggedDriver(true, true);
            int count = data.GetCount("CREDENTIALS");
            string mail = (ConfigurationManager.AppSettings["DummyMail"].ToString());
            string password = (ConfigurationManager.AppSettings["DummyPassword"].ToString());
            input.Add(nameof(mail), mail);
            input.Add(nameof(password), password);
            
            try
            {
                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'SPRÁVA POUŽIVATEĽOV')]")).Click();
                driver.FindElement(By.XPath("//body/div[2]/div[2]/div[4]/a[1]/i[1]")).Click();
                driver.SwitchTo().Alert().Accept();
                Assert.IsTrue(driver.FindElement(By.XPath("/html[1]/body[1]/div[2]/p[1]")).Displayed);

                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'ODHLÁSENIE')]")).Click();
                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'PRIHLÁSENIE')]")).Click();
                driver.FindElement(By.XPath("//input[@id='mailInput']")).SendKeys(mail);
                driver.FindElement(By.XPath("//input[@id='passwordInput']")).SendKeys(password);
                driver.FindElement(By.XPath("//button[contains(text(),'PRIHLÁSIŤ')]")).Click();
                Assert.IsTrue(driver.FindElement(By.XPath("//a[contains(text(),'daný účet neexistuje')]")).Displayed);
                Assert.AreEqual(data.GetCount("CREDENTIALS"), (count - 1));

                driver.Quit();

                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, null, input);
            }
            catch (Exception exception)
            {
                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, exception, input);
            }
        }

        public void CannotRegisterTwiceUser()
        {
            IWebDriver driver = GetUnloggedDriver();
            string mail = (ConfigurationManager.AppSettings["DummyMail"].ToString());
            string name = (ConfigurationManager.AppSettings["DummyName"].ToString());
            string surname = (ConfigurationManager.AppSettings["DummySurname"].ToString());
            string phone = (ConfigurationManager.AppSettings["DummyPhone"].ToString());
            string password = (ConfigurationManager.AppSettings["DummyPassword"].ToString());
            input.Add(nameof(mail), mail);
            input.Add(nameof(name), name);
            input.Add(nameof(surname), surname);
            input.Add(nameof(phone), phone);
            input.Add(nameof(password), password);

            try
            {
                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'REGISTRÁCIA')]")).Click();
                driver.FindElement(By.XPath("//input[@id='mailInput']")).SendKeys(mail);
                driver.FindElement(By.XPath("//input[@id='nameInput']")).SendKeys(name);
                driver.FindElement(By.XPath("//input[@id='surnameInput']")).SendKeys(surname);
                driver.FindElement(By.XPath("//input[@id='phoneInput']")).SendKeys(phone);
                driver.FindElement(By.XPath("//input[@id='passwordInput']")).SendKeys(password);
                driver.FindElement(By.XPath("//input[@id='passwordConfirmationInput']")).SendKeys(password);
                driver.FindElement(By.XPath("//button[contains(text(),'REGISTROVAŤ')]")).Click();
                Assert.IsTrue(driver.FindElement(By.XPath("//a[contains(text(),'účet so zadaným mailom už existuje')]")).Displayed);

                driver.Quit();

                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, null, input);
            }
            catch (Exception exception)
            {
                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, exception, input);
            }
        }

        public void SetAdminRoleAdmin()
        {
            IWebDriver driver = GetLoggedDriver(true, true);
            int role = 1;
            input.Add(nameof(role), role.ToString());
            
            try
            {
                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'SPRÁVA POUŽIVATEĽOV')]")).Click();
                driver.FindElement(By.XPath("//body/div[2]/div[2]/div[3]/a[1]/i[1]")).Click();
                new SelectElement(driver.FindElement(By.XPath("//select[@id='roleInput']"))).SelectByIndex(role);
                driver.FindElement(By.XPath("//button[contains(text(),'ULOŽIŤ')]")).Click();
                Assert.IsTrue(driver.FindElement(By.XPath("//p[contains(text(),'údaje boli uložené')]")).Displayed);

                driver.Quit();

                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, null, input);
            }
            catch (Exception exception)
            {
                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, exception, input);
            }
        }

        public void CheckStatisticsAdmin()
        {
            IWebDriver driver = GetLoggedDriver(null, null);
            DateTime date = data.GetRandomPastDate();
            int month = data.GetRandomNumber(12);
            input.Add(nameof(date), date.ToString("MM-dd-yyyy"));
            input.Add(nameof(month), month.ToString());

            try
            {
                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'ŠTATISTIKA')]")).Click();
                driver.FindElement(By.XPath("//input[@id='dateInput']")).SendKeys(date.ToString("MM-dd-yyyy"));
                driver.FindElement(By.XPath("//button[@id='refreshDate']")).Click();

                new SelectElement(driver.FindElement(By.XPath("//select[@id='monthInput']"))).SelectByIndex(month);
                driver.FindElement(By.XPath("//button[@id='refreshMonth']")).Click();

                driver.Quit();

                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, null, input);
            }
            catch (Exception exception)
            {
                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, exception, input);
            }
        }

        public void FinishRequestAdmin()
        {
            IWebDriver driver = GetLoggedDriver(true, true);
            int status = 3;
            string result = data.GetRandomText(data.GetRandomNumber(100));
            input.Add(nameof(status), status.ToString());
            input.Add(nameof(result), result);

            try
            {
                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'SPRÁVA ŽIADOSTÍ')]")).Click();
                driver.FindElement(By.XPath("/html[1]/body[1]/div[2]/div[1]/div[3]/a[1]/i[1]")).Click();
                new SelectElement(driver.FindElement(By.XPath("//select[@id='statusInput']"))).SelectByIndex(status);
                driver.FindElement(By.XPath("//input[@id='resultInput']")).SendKeys(result);
                driver.FindElement(By.XPath("//button[contains(text(),'UPRAVIŤ')]")).Click();
                Assert.IsTrue(driver.FindElement(By.XPath("//p[contains(text(),'požiadavka bola úspešne aktualizovaná')]")).Displayed);

                driver.Quit();

                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, null, input);
            }
            catch (Exception exception)
            {
                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, exception, input);
            }
        }

        public void CheckRequestUser()
        {
            IWebDriver driver = GetLoggedDriver(false, false);

            try
            {
                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'SPRÁVA ŽIADOSTÍ')]")).Click();
                driver.FindElement(By.XPath("/html[1]/body[1]/div[2]/div[1]/div[3]/a[1]/i[1]")).Click();
                Assert.IsFalse(driver.FindElement(By.XPath("//button[contains(text(),'UPRAVIŤ')]")).Enabled);
                Assert.IsTrue(driver.FindElement(By.XPath("//input[@id='scheduledInput']")).GetAttribute("readonly").Equals("true"));
                Assert.IsTrue(driver.FindElement(By.XPath("//textarea[@id='descriptionInput']")).GetAttribute("readonly").Equals("true"));
                Assert.IsTrue(driver.FindElement(By.XPath("//input[@id='resultInput']")).GetAttribute("readonly").Equals("true"));

                driver.Quit();

                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, null, input);
            }
            catch (Exception exception)
            {
                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, exception, input);
            }
        }

        public void DeleteRequestAdmin()
        {
            IWebDriver driver = GetLoggedDriver(true, true);
            int count = data.GetCount("REQUESTS");
            int status = 3;
            string result = data.GetRandomText(data.GetRandomNumber(100));
            input.Add(nameof(status), status.ToString());
            input.Add(nameof(result), result);

            try
            {
                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'SPRÁVA ŽIADOSTÍ')]")).Click();
                driver.FindElement(By.XPath("/html[1]/body[1]/div[2]/div[1]/div[4]/a[1]/i[1]")).Click();
                driver.SwitchTo().Alert().Accept();
                Assert.IsTrue(driver.FindElement(By.XPath("/html[1]/body[1]/div[2]/p[1]")).Displayed);
                Assert.AreEqual(data.GetCount("REQUESTS"), (count - 1));

                driver.Quit();

                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, null, input);
            }
            catch (Exception exception)
            {
                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, exception, input);
            }
        }

        //public void TestName()
        //{
        //    IWebDriver driver = GetLoggedDriver(null, null);
        //    input.Add("input", "");

        //    try
        //    {
        //        driver.FindElement(By.XPath("")).Click();

        //        driver.Quit();

        //        report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, null, input);
        //    }
        //    catch (Exception exception)
        //    {
        //        report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, exception, input);
        //    }
        //}

        #endregion
    }
}