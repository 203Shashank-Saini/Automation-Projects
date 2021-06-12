using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace ServiceNow_Ticket_Monitoring_System
{

	//*****************************************************************************************************************************
	//
	// DEVELOPER - SHASHANK SAINI, DEVELOPER'S EMAIL- SHASHANKSAINI203@GMAIL.COM
	// LinkedIn - https://in.linkedin.com/in/shashank-saini-9b6ba3166
	// This code is developed in an effort to illustrate the uses and benefits of Selenium for Web Automation. 
	// In addition to Test automation( NUnit is used in this code ), Selenium can also be used for various web automations.
	// ServiceNow - IT Service Management (ITSM), developer program (free) has been used for the development of this code.
	// This code is for educational and learning purpose only.
	// PLESSE MAKE SURE TO EDIT THE Configuration.xml, BEFORE RUNNING THE CODE.
	//	
	//*****************************************************************************************************************************
	class MainProgram
	{
		ChromeDriver _chromeDriver;
		//IWebDriver _chromeDriver;
		string _serviceNowResult;
		bool _eMailNotificatioResult;
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			Console.WriteLine("Welcome to ServiceNow Automation with Selenium");
			MainProgram mainP = new MainProgram();
			mainP.Invoking();
		}


		//[SetUp]
		[Test]
		//[TearDown]
		public void Invoking()
		{
			try
			{
				ChromeOptions options = new ChromeOptions();
				options.AddArgument("ignore-certificate-errors");
				options.AddArgument("enable-logging");
				options.AddAdditionalCapability("useAutomationExtension", false);
				options.AddArguments("--no-sandbox");
				_chromeDriver = new ChromeDriver();
				_chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
				
				ServiceNowMonitoring serviceNow = new ServiceNowMonitoring(); //Creating object of "ServiceNowMonitoring" class
				_serviceNowResult = serviceNow.InvestigateServiceNow(_chromeDriver); //Invoking "InvestigateServiceNow" method + passing ChromeDriver
				
				SMTP_eMail_Notification sendEmail = new SMTP_eMail_Notification();
				_eMailNotificatioResult = sendEmail.SendMail();
			}
			catch(Exception ex)
            {
				if (string.IsNullOrEmpty(_serviceNowResult))
				{
					Console.WriteLine("Error occurred while navigating through ServiceNow. Please check the ServiceNow instance is working and internet connection is good. \n \n Exception Details : " + ex);
				}
				else if(!_eMailNotificatioResult)
                {

                }
				else
				{
					Console.WriteLine("Error occurred while invoking chrome driver, make sure Chrome is installed and working properly. \n \n Exception Details : " + ex);
				}
			}

			//Assert.AreEqual(x.id, 100)

			//ChromeDriver.Close();
			//ChromeDriver.Quit();
		}





	}	
}
