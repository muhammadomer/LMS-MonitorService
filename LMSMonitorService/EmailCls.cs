using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogApp;
using System.Net.Mail;
using System.Net;
using System.Threading;
using System.Configuration;

namespace LMSMonitorService
{
    class EmailCls
    {

      //  public bool sentemail = false;
        public static bool SendEmail(string MailServer, int SMTP_port, bool ssl, string MailUsername, string MailPassword, string EmailTo, string EmailFromName, string EmailSubject, string EmailBody, string AttachmentPath,int remindertype)
        {
            try
            {
              
               
                
                EmailSubject = EmailSubject.ToUpper();
                string EmailFrom = MailUsername;
              
                Log4Net.WriteLog("Mail Server [ " + MailServer + " ] with Username [ " + MailUsername + " ].", LogType.GENERALLOG);
                if (MailServer.Trim() == string.Empty)
                {
                    Log4Net.WriteLog("Mail server not found cannot send emails", LogType.GENERALLOG);
                    return false;
                }
                else if (EmailTo.Trim() == string.Empty)
                {
                    Log4Net.WriteLog("Email address to send not found", LogType.ERRORLOG);
                    return false;
                }

                //  Log4Net.WriteLog("Going to send email to " + EmailTo + ", email cc " + EmailToCC + ", email bcc " + EmailToBCC + ", email from " + EmailFrom + " with subject " + EmailSubject + ", body " + EmailBody, LogType.GENERALLOG);
                Log4Net.WriteLog("Going to send email to " + EmailTo + ", email from " + EmailFrom , LogType.GENERALLOG);


                string maildomain = clsGetAppConfig.GetValueForTheStringKey("MailDomain").Trim();



                if (maildomain!="" && !maildomain.Contains(EmailTo))
                {
                    Log4Net.WriteLog("Email not sent to :"+EmailTo+" beacuse email not match: "+maildomain, LogType.ERRORLOG);
                    return false;

                }

                SmtpClient objsmtp = new SmtpClient(MailServer);
              

                if (MailUsername == "")
                {
                    objsmtp.UseDefaultCredentials = true;
                }
                else
                {
                    objsmtp.Port = SMTP_port;
                    objsmtp.EnableSsl = ssl;

                    objsmtp.UseDefaultCredentials = false;
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;


                    objsmtp.Credentials = new NetworkCredential(MailUsername, MailPassword);
                }
                objsmtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                mail.From = new MailAddress(EmailFrom, EmailFromName);


                if (remindertype == 3)
                {
                    string appemail = ConfigurationManager.AppSettings["ApplicationEmails"];
                    foreach (var address in appemail.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        mail.To.Add(address);
                    }
                }
               


                string[] emailAddresses = EmailTo.Split(';');
                foreach (string Email in emailAddresses)
                {
                    if (Email.Trim() != "")
                    {
                        mail.To.Add(Email);
                    }
                }

              
                //set the content
                mail.Subject = EmailSubject;
                mail.Body = EmailBody;

               

               

                if (!string.IsNullOrWhiteSpace(AttachmentPath))
                {
                    mail.Attachments.Add(new Attachment(AttachmentPath));
                }

                object[] parameters = new object[] { objsmtp, mail };
                new Thread(new ParameterizedThreadStart(SendEmail)).Start(parameters);
              
                Log4Net.WriteLog("Mail sent successfully", LogType.GENERALLOG);
                return true;
            }
            catch (Exception ex)
            {
                Log4Net.WriteException(ex);
                Log4Net.WriteLog("Failed to sent email", LogType.ERRORLOG);
                return false;
            }
        }



        private static void SendEmail(object parameters)
        {
            try
            {
                //  EmailCls em = new EmailCls();

                object[] ObjectParameters = (object[])parameters;
                SmtpClient objsmtp = (SmtpClient)ObjectParameters[0];
                MailMessage mail = (MailMessage)ObjectParameters[1];

                objsmtp.Send(mail);
                // em.sentemail= true;
            }
            catch (Exception ex)
            {
                Log4Net.WriteException(ex);// em.sentemail = true; }
            }
        }
    }
}
