using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DeletarUsuario
{
    public class Email
    {
        public static void EnviarEmail(string erro)
        {
            string from = "assistente@intelliway.com.br";
            string to = "servicedesk@aguiabranca.com.br,davi.vinter@intelliway.com.br";//servicedesk@aguiabranca.com.br
            string subject = "Erro Assistente Gabi - Automação";
            int port = 587;
            string smtpAddress = "smtp.office365.com";
            int timeout = 10000;
            string userEmail = "assistente@intelliway.com.br";
            string passwordEmail = "Intelliway2017@";

            SmtpClient client = new SmtpClient();
            client.Port = port;
            client.Host = smtpAddress;
            client.EnableSsl = true;
            client.Timeout = timeout;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(userEmail, passwordEmail);

            var body = CriarCorpoEmail(erro);

            MailMessage mm = new MailMessage(from, to, subject, body);
            mm.IsBodyHtml = true;
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            client.Send(mm);
            Console.WriteLine("E-mail enviado para: <" + to + ">");
        }

        private static string CriarCorpoEmail(string erro)
        {
            string corpo = string.Empty;

            using (StreamReader reader = new StreamReader("CorpoEmail.html")) //carregando o arquivo html na string
            {
                corpo = reader.ReadToEnd(); //lendo todos os caracteres da string até o final
            }

            corpo = corpo.Replace("{descricao}", erro); //substituindo o valor do campo na string  
            corpo = corpo.Replace("{data}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

            return corpo;
        }
    }
}
