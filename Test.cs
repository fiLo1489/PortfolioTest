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
            data.CloseConnection();
        }

        public IWebDriver GetLoggedDriver(bool? login, bool? password)
        {
            IWebDriver driver = GetUnloggedDriver();
            input.Clear();

            driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
            driver.FindElement(By.XPath("//a[contains(text(),'PRIHLÁSENIE')]")).Click();
            if (login == true)
            {
                driver.FindElement(By.XPath("//input[@id='mailInput']")).SendKeys(ConfigurationManager.AppSettings["AdminMail"].ToString());
                driver.FindElement(By.XPath("//input[@id='passwordInput']")).SendKeys(ConfigurationManager.AppSettings["AdminPassword"].ToString());
                input.Add("login", ConfigurationManager.AppSettings["AdminMail"].ToString());
                input.Add("password", ConfigurationManager.AppSettings["AdminPassword"].ToString());
            }
            else if (login == false)
            {
                driver.FindElement(By.XPath("//input[@id='mailInput']")).SendKeys(ConfigurationManager.AppSettings["UserMail"].ToString());
                driver.FindElement(By.XPath("//input[@id='passwordInput']")).SendKeys(ConfigurationManager.AppSettings["UserPassword"].ToString());
                input.Add("login", ConfigurationManager.AppSettings["UserMail"].ToString());
                input.Add("password", ConfigurationManager.AppSettings["UserPassword"].ToString());
            }
            else
            {
                if (password == true)
                {
                    driver.FindElement(By.XPath("//input[@id='mailInput']")).SendKeys(ConfigurationManager.AppSettings["AdminMail"].ToString());
                    input.Add("login", ConfigurationManager.AppSettings["AdminMail"].ToString());
                }
                else if (password == false)
                {
                    driver.FindElement(By.XPath("//input[@id='mailInput']")).SendKeys(ConfigurationManager.AppSettings["UserMail"].ToString());
                    input.Add("login", ConfigurationManager.AppSettings["UserMail"].ToString());
                }
                else
                {
                    driver.FindElement(By.XPath("//input[@id='mailInput']")).SendKeys(ConfigurationManager.AppSettings["DummyMail"].ToString());
                    input.Add("login", ConfigurationManager.AppSettings["DummyMail"].ToString());
                }
                driver.FindElement(By.XPath("//input[@id='passwordInput']")).SendKeys(ConfigurationManager.AppSettings["DummyPassword"].ToString());
                input.Add("password", ConfigurationManager.AppSettings["DummyPassword"].ToString());
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
            input.Clear();

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

                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, null, input);
            }
            catch (Exception exception)
            {
                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, exception, input);
            }
        }

        public void ChangeCredentialsUser(bool? role)
        {
            string name = (role == null ? ConfigurationManager.AppSettings["DummyName"].ToString() : ConfigurationManager.AppSettings["UserName"].ToString());
            string surname = (role == null ? ConfigurationManager.AppSettings["DummySurname"].ToString() : ConfigurationManager.AppSettings["UserSurname"].ToString());
            string phone = (role == null ? ConfigurationManager.AppSettings["DummyPhone"].ToString() : ConfigurationManager.AppSettings["UserPhone"].ToString());
            string password = (role == null ? ConfigurationManager.AppSettings["DummyPassword"].ToString() : ConfigurationManager.AppSettings["UserPassword"].ToString());
            input.Add("name", name);
            input.Add("surname", surname);
            input.Add("phone", phone);
            input.Add("password", password);

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

        public void SendRequestUser(bool role)
        {
            int category = data.GetRandomNumber(5);
            string description = data.GetRandomText(data.GetRandomNumber(100));
            DateTime date = data.GetRandomRequestDate();
            input.Add("role", (role == true ? "Admin" : "User"));
            input.Add("category", category.ToString());
            input.Add("description", description);
            input.Add("date", date.ToString());

            try
            {
                IWebDriver driver = GetLoggedDriver(false, false);
                
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
            string description = data.GetRandomText(data.GetRandomNumber(100));
            DateTime date = data.GetRandomRequestDate();
            input.Add("description", description);
            input.Add("date", date.ToString());

            try
            {
                IWebDriver driver = GetLoggedDriver(false, false);

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

                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, null, input);
            }
            catch (Exception exception)
            {
                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, exception, input);
            }
        }

        public void RegisterCorrectCredentialstUser()
        {
            string mail = (ConfigurationManager.AppSettings["DummyMail"].ToString());
            string name = (ConfigurationManager.AppSettings["DummyName"].ToString());
            string surname = (ConfigurationManager.AppSettings["DummySurname"].ToString());
            string phone = (ConfigurationManager.AppSettings["DummyPhone"].ToString());
            string password = (ConfigurationManager.AppSettings["DummyPassword"].ToString());
            input.Add("mail", mail);
            input.Add("name", name);
            input.Add("surname", surname);
            input.Add("phone", phone);
            input.Add("password", password);

            try
            {
                IWebDriver driver = GetUnloggedDriver();

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
            string mail = (ConfigurationManager.AppSettings["DummyMail"].ToString());
            string password = (ConfigurationManager.AppSettings["DummyPassword"].ToString());
            
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
                driver.FindElement(By.XPath("//input[@id='mailInput']")).SendKeys(mail);
                driver.FindElement(By.XPath("//input[@id='passwordInput']")).SendKeys(password);
                driver.FindElement(By.XPath("//button[contains(text(),'PRIHLÁSIŤ')]")).Click();
                Assert.IsTrue(driver.FindElement(By.XPath("//a[contains(text(),'daný účet neexistuje')]")).Displayed);

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
            string mail = (ConfigurationManager.AppSettings["DummyMail"].ToString());
            string name = (ConfigurationManager.AppSettings["DummyName"].ToString());
            string surname = (ConfigurationManager.AppSettings["DummySurname"].ToString());
            string phone = (ConfigurationManager.AppSettings["DummyPhone"].ToString());
            string password = (ConfigurationManager.AppSettings["DummyPassword"].ToString());
            input.Add("mail", mail);
            input.Add("name", name);
            input.Add("surname", surname);
            input.Add("phone", phone);
            input.Add("password", password);

            try
            {
                IWebDriver driver = GetUnloggedDriver();

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
            int role = 1;
            input.Add("role", role.ToString());
            
            try
            {
                IWebDriver driver = GetLoggedDriver(true, true);

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
            DateTime date = data.GetRandomPastDate();
            input.Add("input", date.ToString());

            try
            {
                IWebDriver driver = GetLoggedDriver(null, null);
                

                driver.FindElement(By.XPath("//a[@id='navbarDropdownMenuLink']")).Click();
                driver.FindElement(By.XPath("//a[contains(text(),'ŠTATISTIKA')]")).Click();
                driver.FindElement(By.XPath("//input[@id='dateInput']")).SendKeys(date.ToString("MM-dd-yyyy"));
                driver.FindElement(By.XPath("//button[@id='refreshDate']")).Click();

                new SelectElement(driver.FindElement(By.XPath("//select[@id='monthInput']"))).SelectByIndex(data.GetRandomNumber(12));
                driver.FindElement(By.XPath("//button[@id='refreshMonth']")).Click();

                driver.Quit();

                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, null, input);
            }
            catch (Exception exception)
            {
                report.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, exception, input);
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