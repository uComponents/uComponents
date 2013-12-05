using System;
using System.Net.Mail;
using System.Text.RegularExpressions;
using umbraco;
using umbraco.BusinessLogic;

namespace uComponents.XsltExtensions
{
	/// <summary>
	/// The Email class exposes XSLT extensions to offer extended email functionality.
	/// </summary>
	public class Email
	{
		/// <summary>
		/// Determines whether [is valid email] [the specified input].
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns>
		/// 	<c>true</c> if [is valid email] [the specified input]; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>http://haacked.com/archive/2007/08/21/i-knew-how-to-validate-an-email-address-until-i.aspx</remarks>
		public static bool IsValidEmail(string input)
		{
			var pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
			var regex = new Regex(pattern, RegexOptions.IgnoreCase);
			return regex.IsMatch(input);
		}

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
				Log.Add(LogTypes.Error, -1, string.Format("uComponents.XsltExtensions.Email.SendMail: Error sending mail. Exception: {0}", ex));
			}
		}
	}
}