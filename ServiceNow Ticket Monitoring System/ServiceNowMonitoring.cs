using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ServiceNow_Ticket_Monitoring_System
{
    //*****************************************************************************************************************************
    // DEVELOPER - SHASHANK SAINI, DEVELOPER'S EMAIL- SHASHANKSAINI203@GMAIL.COM
    //*****************************************************************************************************************************
    class ServiceNowMonitoring : MainProgram
    {
        public string InvestigateServiceNow(ChromeDriver chromeDriver)
        {
            string website = "";
            string username = "";
            string password = "";
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
            }

            Console.WriteLine(website + " " + username + " " + password);

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

            IWebElement btnFilterAND = chromeDriver.FindElement(By.XPath("//*[contains(@id, 'filterdiv')]/list_filter/div[1]/button[3]"));
            btnFilterAND.Click();









            Console.WriteLine("Done");
            return "Done";
        }

    }
}


/*
DYNAMIC ID, XPATH NOT WORKING - At this situation, We have lots of options-

Option 1: Look for any other attribute which Is not changing every time In that div node like name, class etc. So If this div node has class attribute then we can write xpath as bellow.
//div[@class='post-body entry-content']/div[1]/form[1]/input[1] 

Option 2: We can use absolute xpath (full xpath) where you do not need to give any attribute names In xpath.
/html/body/div[3]/ div[2] / div[2] / div[2] / div[2] / div[2] / div[2] / div / div[4] / div[1] / div / div / div / div[1] / div / div / div / div[1] / div[2] / div[1] / form[1] / input[1]

Option 3: We can use starts-with function. In this xpath's ID attribute, "post-body-" part remains same every time.
 //div[starts-with(@id,'post-body-')]/div[1]/form[1]/input[1] 

Option 4: We can use contains function. Same way you can use contains function as bellow.
 div[contains(@id, 'post-body-')]/ div[1] / form[1] / input[1] 



//_____________________________________________________________________________________________________


 You've got:

//*[contains(@id, 'ctl00_btnAircraftMapCell')]//*[contains(@title, 'Select Seat')]
Which translates into:

Get me all the elements that have an ID that contains ctl00_btnAircraftMapCell. Out of these elements, get any child elements that have a title that contains Select Seat.

What you actually want is:

//a[contains(@id, 'ctl00_btnAircraftMapCell') and contains(@title, 'Select Seat')]
Which translates into:

Get me all the anchor elements that have both: an id that contains ctl00_btnAircraftMapCell and a title that contains Select Seat.

 
 
 
 
 
 
 
 */