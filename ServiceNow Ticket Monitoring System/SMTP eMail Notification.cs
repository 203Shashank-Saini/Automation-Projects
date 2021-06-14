using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ServiceNow_Ticket_Monitoring_System
{
    class SMTP_eMail_Notification
    {
		//*****************************************************************************************************************************
		// DEVELOPER - SHASHANK SAINI, DEVELOPER'S EMAIL- SHASHANKSAINI203@GMAIL.COM
		//*****************************************************************************************************************************
		public bool SendMail(List<INCdetails> ServiceNowResult)
        {
            try
            {
                string fullName="";
                string fromEmail = "";
                string toEmail = "";
                string CCemail = "";
                string searchValue1 = "";

                XmlDocument Xmlconfig = new XmlDocument();
                Xmlconfig.Load("Configuration.xml");
                XmlNodeList nodeList = Xmlconfig.DocumentElement.SelectNodes("/Configurations");
                foreach (XmlNode node in nodeList)
                {
                    fullName = node.SelectSingleNode("FullName").InnerText;
                    fromEmail = node.SelectSingleNode("FromEmail").InnerText;
                    toEmail = node.SelectSingleNode("ToEmail").InnerText;
                    CCemail = node.SelectSingleNode("CCemail").InnerText;
                    searchValue1 = node.SelectSingleNode("SearchValue1").InnerText;
                }
                SmtpClient smtpClient = new SmtpClient("smtp.na.jnj.com", 25);

                smtpClient.Credentials = new System.Net.NetworkCredential("shashanksaini203@gmail.com", "_");
                // smtpClient.UseDefaultCredentials = true; // uncomment if you don't want to use the network credentials
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.EnableSsl = true;
                MailMessage mail = new MailMessage();

                //Setting From , To and CC
                mail.From = new MailAddress(fromEmail, fullName);
                mail.To.Add(new MailAddress(toEmail));
                mail.CC.Add(new MailAddress(CCemail));
                mail.Subject = "Service Now Ticket Monitoring Notification";
                mail.IsBodyHtml = true;

                string eMailBody = "Hi all, <br><br> Please take care of the below Incidents assigned to the group - <b>"+ searchValue1 +"</b> : <br><br><br>";
                    foreach(var result in ServiceNowResult)
                {
                    eMailBody = eMailBody + "Incident Number: " + result.IncidentNumber + "<br> Incident Link : <a href= " + result.IncidentLink + ">"+ result.IncidentLink + "</a>" + "<br><br>";
                }
                eMailBody = eMailBody + "Thanks & Regards, <br>Automation tool by Shashank Saini <br>shashanksaini203@gmail.com";

                mail.Body = eMailBody;
                smtpClient.Send(mail);
                Console.WriteLine("Email Sent");
                return true;
            }
            catch(Exception)
            {
                return false;
            }


            
        }
    }
}
