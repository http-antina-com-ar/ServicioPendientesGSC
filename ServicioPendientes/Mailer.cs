using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ServicioPendientes
{
    public class Mailer
    {
        private string from = "GABD@antina.com.ar";
        private List<string> toList = SettingsPendings.Default.toList.Split(';').ToList();
        private string displayName = "Servicio Pendientes GSC";
        private string smtp_server = SettingsPendings.Default.smtp_server;
        private int port = SettingsPendings.Default.smtp_port;
        private Logger logger = new Logger();


        public void SendMail(string asunto, string body, string attach_path)
        {
            MailMessage mail = new MailMessage();
            Attachment adjunto;
            SmtpClient client = new SmtpClient(smtp_server, port);

            mail.From = new MailAddress(from, displayName);
            foreach (string to in toList)
            {
                mail.To.Add(to);
            }

            mail.Subject = asunto;
            mail.Body = body;
            try
            {
                if (attach_path != "")
                {
                    adjunto = new Attachment(attach_path);
                    mail.Attachments.Add(adjunto);
                }

                mail.IsBodyHtml = true;
                client.Send(mail);
            }
            catch (Exception e)
            {
                
                Console.WriteLine(e);
                throw e;
            }
        }

        public void SendMail(string asunto, string body)
        {
            SendMail(asunto, body, "");

        }
    }
}
