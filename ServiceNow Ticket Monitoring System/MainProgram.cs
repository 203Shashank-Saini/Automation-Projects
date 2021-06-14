using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;

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
		List<INCdetails> _serviceNowResult = new List<INCdetails>();
		bool NormalStart = true;
		bool _eMailNotificatioResult;
		bool ClosedFlag = true;
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			Console.WriteLine("Welcome to ServiceNow Automation with Selenium");
			MainProgram mainP = new MainProgram();
			mainP.Start();
		}


		[SetUp]
		public void NUnitSetup()
        {
			NormalStart = false;

		}
		[Test, Order(1)]
		public void Start()
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
				if (NormalStart) 
				{
					SendEmailNotification();
				}
				close();
				//Assert.IsNotEmpty(_serviceNowResult);
			}

			catch (Exception ex)
            {
				if (_serviceNowResult.Count == 0)
				{
					Console.WriteLine("Error occurred while navigating through ServiceNow. Please check the ServiceNow instance is working and internet connection is good. \n \n Exception Details : " + ex);
				}
				else
				{
					Console.WriteLine("Error occurred while invoking chrome driver, make sure Chrome is installed and working properly. \n \n Exception Details : " + ex);
				}
			}

		}

		[Test, Order(2)]
		public void SendEmailNotification()
		{
			SMTP_eMail_Notification sendEmail = new SMTP_eMail_Notification();
			if (_serviceNowResult.Count > 0)
			{
				_eMailNotificatioResult = sendEmail.SendMail(_serviceNowResult);
			}

			if (_eMailNotificatioResult == false)
			{
				Console.WriteLine("Error occurred while navigating sending Email Notification. Please check the 'SMTP email notification' class");
			}

			//if (NormalStart)
   //         {
			//	Assert.IsTrue(_eMailNotificatioResult);
			//	close();
			//}
			//else
			//Assert.IsFalse(_eMailNotificatioResult);

		}


		[TearDown]
		public void close()
        {
			if (ClosedFlag)
			{
				_chromeDriver.Close();
				_chromeDriver.Quit();
				ClosedFlag = false;
			}
		}


	}	
}
