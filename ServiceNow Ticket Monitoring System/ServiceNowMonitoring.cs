using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ServiceNow_Ticket_Monitoring_System
{
    //*****************************************************************************************************************************
    // DEVELOPER - SHASHANK SAINI, DEVELOPER'S EMAIL- SHASHANKSAINI203@GMAIL.COM
    //*****************************************************************************************************************************
    class ServiceNowMonitoring : MainProgram
    {
        public List<INCdetails> InvestigateServiceNow(ChromeDriver chromeDriver)
        {
            string website = "";
            string username = "";
            string password = "";
            string searchParameter1 = "";
            string searchValue1 = "";

            List<INCdetails> INCdetailsList = new List<INCdetails>();

            bool checkLoginPage = false;
            chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            XmlDocument Xmlconfig = new XmlDocument();
            Xmlconfig.Load("Configuration.xml");
            XmlNodeList nodeList = Xmlconfig.DocumentElement.SelectNodes("/Configurations");
            foreach (XmlNode node in nodeList)
            {
                website = node.SelectSingleNode("WebsiteURL").InnerText;
                username = node.SelectSingleNode("Username").InnerText;
                password = node.SelectSingleNode("Password").InnerText;
                searchParameter1 = node.SelectSingleNode("SearchParameter1").InnerText;
                searchValue1 = node.SelectSingleNode("SearchValue1").InnerText;

            }

            //Console.WriteLine(website + " " + username + " " + password);
            
            chromeDriver.Navigate().GoToUrl(website);   //Navigating to the website, please edit this value in configration.xml

            //NOTE : XPath generally have the id(s) enclosed by double inverted commas "" , Change them to single inverted commas '' , e.g //*[@id="user_name"]"   -->  //*[@id='user_name']"
            chromeDriver.SwitchTo().Frame("gsft_main");//switching the iFrame
            try
            {
                checkLoginPage = chromeDriver.FindElement(By.XPath("//*[@id='sysverb_login']")).Displayed; //Checking if Login button is present, it may be possible that session is already logged in.
            }
            catch(NoSuchElementException)
            {
                checkLoginPage = false;
            }
            
            
            if (checkLoginPage)     //Only if the login page is displayed, meaning the navigation did not take us to the ServiceNow ITSM Home Page Dashboard.
            {
                //chromeDriver.SwitchTo().Frame("gsft_main");     
                IWebElement txtUsername = chromeDriver.FindElement(By.XPath("//*[@id='user_name']"));   //Finding Username textbox
                txtUsername.SendKeys(username);     // I realised that its better to immediate send keys to type text rather than finding all elements first and then send keys, because sometimes entering some text may bring different results like in Filter Searches.
                IWebElement txtPassword = chromeDriver.FindElement(By.XPath("//*[@id='user_password']"));   //Finding Password textbox
                txtPassword.SendKeys(password);
                IWebElement btnLogIn = chromeDriver.FindElement(By.XPath("//*[@id='sysverb_login']"));   //Clicking "LogIn" button
                btnLogIn.Click();
            }


            try
            {
                checkLoginPage = chromeDriver.FindElement(By.XPath("//*[@id='filter']")).Displayed; //Checking if Login button is present, it may be possible that session is already logged in.
            }
            catch (NoSuchElementException)
            {
                chromeDriver.SwitchTo().ParentFrame();
            }
            


            IWebElement txtFilterNavigator = chromeDriver.FindElement(By.XPath("//*[@id='filter']"));
            txtFilterNavigator.SendKeys("Service Desk");    //Incidents under Service Desk category is taken into account for this Automation
            IWebElement btnIncidents = chromeDriver.FindElement(By.XPath("//*[@id='4fed4395c0a8016400fcf06c27b1e6c6']/div/div"));
            btnIncidents.Click();


            chromeDriver.SwitchTo().Frame("gsft_main");     //switching the iFrame
            IWebElement btnFilterIncidents = chromeDriver.FindElement(By.XPath("//*[@id='incident_filter_toggle_image']"));
            btnFilterIncidents.Click();
            Thread.Sleep(2000);
            IWebElement btnFilterAND = chromeDriver.FindElement(By.XPath("//*[contains(@id, 'filterdiv')]/list_filter/div[1]/button[3]"));
            btnFilterAND.Click();


            

            IWebElement FilterOne = chromeDriver.FindElement(By.XPath("//*[@id='select2-chosen-4']"));       
            FilterOne.Click();
            IWebElement AssignmentGroupSearch = chromeDriver.FindElement(By.XPath("//*[@id='s2id_autogen4_search']"));       //.FindElement(By.XPath("//*[contains(@id, 'QUERYPART')]/tr[4]//*[contains(@id,'select2-chosen']"));
            AssignmentGroupSearch.Click();
            AssignmentGroupSearch.SendKeys(searchParameter1);
            AssignmentGroupSearch.SendKeys(Keys.Enter);
            
            IWebElement AssignmentGroupName = chromeDriver.FindElement(By.XPath("//*[contains(@id, 'sys_display.incident.assignment_group')]"));       //.FindElement(By.XPath("//*[contains(@id, 'QUERYPART')]/tr[4]//*[contains(@id,'select2-chosen']"));
            AssignmentGroupName.SendKeys(searchValue1);
            Thread.Sleep(3000);
            AssignmentGroupName.SendKeys(Keys.Down);
            AssignmentGroupName.SendKeys(Keys.Tab);


            IWebElement btnFilterRUN = chromeDriver.FindElement(By.XPath("//*[@id='test_filter_action_toolbar_run']"));
            btnFilterRUN.Click();

            IWebElement btnIncidentRowControl = chromeDriver.FindElement(By.XPath("//*[@id='incident_control_button']"));
            btnIncidentRowControl.Click();
            IWebElement btnIncidentRowShow= chromeDriver.FindElement(By.XPath("//*[@id='context_list_titleincident']/div[5]"));
            btnIncidentRowShow.Click();
            IWebElement btnIncidentRowShow100 = chromeDriver.FindElement(By.XPath("//*[contains(@id,'_incident')]/div[6]"));
            btnIncidentRowShow100.Click();

            //I was getting StaleElementReferenceException because the chrome driver was forgetting the element, this was happening because the code was
            //running faster, and the table was refreshing after the element was found once..
            //Let the elements load properly first otherwise a refresh of elements or change in elements will cause StaleElementReferenceException

            Thread.Sleep(3000); //Explicit wait is used to pause for a moment and let the table refresh properly after the previous command

            TakeScreenshot(chromeDriver);

            IWebElement IncidentTable = chromeDriver.FindElement(By.XPath("//*[@id='incident_table']/tbody"));
            List<IWebElement> IncidentTableRows = IncidentTable.FindElements(By.TagName("tr")).ToList();
            if (IncidentTableRows.Count > 0)
            {
                int count = 1;
                foreach (IWebElement row in IncidentTableRows)
                {
                    
                    List<IWebElement> columns = row.FindElements(By.TagName("td")).ToList();
                    
                    Console.WriteLine(columns.ElementAt(2).Text);

                    string INClink = "";
                    IWebElement Column3 = row.FindElement(By.XPath("//table / tbody / tr[" + count + "] / td[3]"));
                    List<IWebElement> Links11 = Column3.FindElements(By.TagName("a")).ToList();
                    foreach (IWebElement links in Links11)
                    {
                        Console.WriteLine(links.GetAttribute("href").ToString());
                        INClink = links.GetAttribute("href").ToString();
                    }
                    INCdetailsList.Add( new INCdetails { IncidentNumber = columns.ElementAt(2).Text, IncidentLink = INClink } );


                    count++;

                }


            }

            Console.WriteLine("Done");
            return INCdetailsList;
        }

        private void TakeScreenshot(ChromeDriver chromeDriver)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string subPath = path + "//Screenshots";

            bool exists = System.IO.Directory.Exists(subPath);

            if (!exists)
                System.IO.Directory.CreateDirectory(subPath);

            DateTime dateTime = DateTime.UtcNow;
            string screenshot_name = "Image" + dateTime.ToString("dd-MM-yyyy-hh-mm");

            Screenshot ss = ((ITakesScreenshot)chromeDriver).GetScreenshot();
            ss.SaveAsFile(path + "//Screenshots//" + screenshot_name + ".png",
            ScreenshotImageFormat.Png);
        }
    }
}
