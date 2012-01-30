using System;
using System.Net.Mail;
using umbraco;
using umbraco.BusinessLogic;

namespace uComponents.Core.XsltExtensions
{
	/// <summary>
	/// The Email class exposes XSLT extensions to offer extended email functionality.
	/// </summary>
	public class Email
	{
		/// <summary>
		/// Sends an email. Performs the same function an <c>umbraco.library.SendMail</c>, with the option of sending via SSL.
		/// </summary>
		/// <param name="from">The 'from' email address.</param>
		/// <param name="to">The 'to' email address.</param>
		/// <param name="subject">The subject of the email.</param>
		/// <param name="body">The content body of the email.</param>
		/// <param name="isHtml">If set to <c>true</c>, then content body is HTML, otherwise plain-text.</param>
		/// <param name="useSSL">If set to <c>true</c>, then use SSL, otherwise use non-secure protocol.</param>
		public static void SendMail(string from, string to, string subject, string body, bool isHtml, bool useSSL)
		{
			if (!useSSL)
			{
				library.SendMail(from, to, subject, body, isHtml);
				return;
			}

			try
			{
				var message = new MailMessage(from, to, subject, body) { IsBodyHtml = isHtml };
				var client = new SmtpClient();
				client.EnableSsl = true;
				client.Send(message);
			}
			catch (Exception ex)
			{
				Log.Add(LogTypes.Error, -1, string.Format("uComponents.Core.XsltExtensions.Email.SendMail: Error sending mail. Exception: {0}", ex));
			}
		}
	}
}