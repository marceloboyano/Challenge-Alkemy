
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;


namespace challenge.Services
{
    public class EmailService:IEmailService
    {
        public async Task SendEmail(string email, string userName)
        {

            //se deberra correr papercut con docker para que funcione el envio de emails
            //docker run --name = papercut - p 25:25 - p 37408:37408 jijiechen / papercut:latest
            var mail = new MimeMessage();
            mail.From.Add(MailboxAddress.Parse("challenge@papercut.com"));
            mail.To.Add(MailboxAddress.Parse(email));
            mail.Subject = "Registro Exitoso!!";
            mail.Body = new TextPart(TextFormat.Html) { Text = "Hola, Gracias por Registrarte!!!" };

            using var smtp = new SmtpClient();
            smtp.Connect("localhost", 25);
            var response = await smtp.SendAsync(mail);

            smtp.Disconnect(true);

        }
    }
}
