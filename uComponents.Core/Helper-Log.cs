namespace uComponents.Core
{
	using System;
	using System.Web;

	using umbraco.BusinessLogic;

	using Umbraco.Core.Logging;

	/// <summary>
	/// Generic helper methods
	/// </summary>
	internal static partial class Helper
	{
		/// <summary>
		/// Log helper
		/// </summary>
		public static class Log
		{
			/// <summary>
			/// Adds an error log entry.
			/// </summary>
			/// <typeparam name="T">The datatype.</typeparam>
			/// <param name="message">The message.</param>
			/// <param name="ex">The exception.</param>
			public static void Error<T>(string message, Exception ex)
			{
				int pageId;
				int.TryParse(HttpContext.Current.Request.QueryString["id"], out pageId);

				LogHelper.Error<T>(string.Format("[User {0}] [Page {1}] {2}", User.GetCurrent().Id, pageId, message), ex);
			}

			/// <summary>
			/// Adds a warning log entry.
			/// </summary>
			/// <typeparam name="T">The datatype.</typeparam>
			/// <param name="message">The message.</param>
			public static void Warn<T>(string message)
			{
				int pageId;
				int.TryParse(HttpContext.Current.Request.QueryString["id"], out pageId);

				LogHelper.Warn<T>(string.Format("[User {0}] [Page {1}] {2}", User.GetCurrent().Id, pageId, message));
			}

			/// <summary>
			/// Adds a debug log entry.
			/// </summary>
			/// <typeparam name="T">The datatype.</typeparam>
			/// <param name="message">The message.</param>
			public static void Debug<T>(string message)
			{
				int pageId;
				int.TryParse(HttpContext.Current.Request.QueryString["id"], out pageId);

				LogHelper.Debug<T>(string.Format("[User {0}] [Page {1}] {2}", User.GetCurrent().Id, pageId, message));
			}
		}
	}
}