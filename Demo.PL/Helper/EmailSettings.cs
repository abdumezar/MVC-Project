using Demo.DAL.Models;
using System.Net;
using System.Net.Mail;

namespace Demo.PL.Helper
{
	public class EmailSettings
	{
		public static void SendEmail(Email email)
		{
			var client = new SmtpClient("smtp.gmail.com", 587);
			client.EnableSsl = true;
			client.Credentials = new NetworkCredential("abdumezar@gmail.com", "typzttcofoupgvze");
			client.Send("abdumezar@gmail.com", email.To, email.Subject, email.Body);
			// await client.SendAsync(new MailMessage("abdumezar@gmail.com", email.To, email.Subject, email.Body), userToken);
		}
	}
}
