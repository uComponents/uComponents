using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

namespace uComponents.Core.XsltExtensions
{
	/// <summary>
	/// The Dates class exposes XSLT extensions to offer extended date/time functionality.
	/// </summary>
	public class Dates
	{
		/// <summary>
		/// The default DateTime format for uComponents.
		/// </summary>
		internal const string DEFAULT_DATE_FORMAT = "dd MMMM yyyy";

		/// <summary>
		/// Tests if a date is within the last number of specified days.
		/// </summary>
		/// <param name="date">The date.</param>
		/// <param name="days">The number of days.</param>
		/// <returns>
		/// Returns true or false depending on if the date is within the last number of days.
		/// </returns>
		public static bool DateWithinLastDays(string date, int days)
		{
			var lastDays = (double)0 - days;
			var currentDate = DateTime.UtcNow.Date;
			var startDate = currentDate.AddDays(lastDays);

			DateTime.TryParse(date, out currentDate);

			return (currentDate > startDate);
		}

		/// <summary>
		/// Tests if a date is within the specified duration.
		/// </summary>
		/// <param name="date">The date.</param>
		/// <param name="duration">The duration (in XSD Duration format).</param>
		/// <returns>
		/// Returns true or false depending on if the date is within the specified duration.
		/// </returns>
		public static bool DateWithinDuration(string date, string duration)
		{
			var currentDate = DateTime.UtcNow.Date;
			var timespan = XmlConvert.ToTimeSpan(duration);
			var startDate = currentDate.Subtract(timespan);

			DateTime.TryParse(date, out currentDate);

			return (currentDate > startDate);
		}

		/// <summary>
		/// Gets the pretty date.
		/// </summary>
		/// <param name="date">The date.</param>
		/// <returns>Returns a pretty date.</returns>
		public static string GetPrettyDate(string date)
		{
			return GetPrettyDate(date, DEFAULT_DATE_FORMAT);
		}

		/// <summary>
		/// Gets the pretty date.
		/// </summary>
		/// <param name="date">The date.</param>
		/// <param name="format">The format.</param>
		/// <returns>Returns a pretty date.</returns>
		/// <remarks>
		/// http://dotnetperls.com/pretty-date
		/// http://ejohn.org/blog/javascript-pretty-date/
		/// </remarks>
		public static string GetPrettyDate(string date, string format)
		{
			// 0. Convert the String into DateTime
			DateTime d;

			if (DateTime.TryParse(date, out d))
			{
				// 1. Get time span elapsed since the date.
				var s = DateTime.Now.Subtract(d);

				// 2. Get total number of days elapsed.
				var dayDiff = (int)s.TotalDays;

				// 3. Get total number of seconds elapsed.
				var secDiff = (int)s.TotalSeconds;

				// 4. Don't allow out of range values.
				if (dayDiff < 0 || dayDiff >= 31)
				{
					return FormatDateTime(date, format); // d.ToString(format);
				}

				// 5. Handle same-day times.
				if (dayDiff == 0)
				{
					// A. Less than one minute ago.
					if (secDiff < 60)
					{
						return "just now";
					}

					// B. Less than 2 minutes ago.
					if (secDiff < 120)
					{
						return "1 minute ago";
					}

					// C.Less than one hour ago.
					if (secDiff < 3600)
					{
						return string.Format("{0} minutes ago", Math.Floor((double)secDiff / 60));
					}

					// D. Less than 2 hours ago.
					if (secDiff < 7200)
					{
						return "1 hour ago";
					}

					// E. Less than one day ago.
					if (secDiff < 86400)
					{
						return string.Format("{0} hours ago", Math.Floor((double)secDiff / 3600));
					}
				}

				// 6. Handle previous days.
				if (dayDiff == 1)
				{
					return "yesterday";
				}

				if (dayDiff < 7)
				{
					return string.Format("{0} days ago", dayDiff);
				}

				if (dayDiff < 14)
				{
					return "1 week ago";
				}

				if (dayDiff < 31)
				{
					return string.Format("{0} weeks ago", Math.Ceiling((double)dayDiff / 7));
				}
			}

			return date;
		}

		///<summary>
		/// Converts the value of the date time string to its equivalent string representation using the specified format.
		///</summary>
		///<param name="date">The date string</param>
		///<param name="format">The date format</param>
		///<returns>The formated date string</returns>
		public static string FormatDateTime(string date, string format)
		{
			DateTime result;
			if (DateTime.TryParse(date, out result) && !string.IsNullOrEmpty(format))
			{
				return FormatDateTime(result, format);
			}

			return date;
		}

		/// <summary>
		/// Parses the exact value of the date time string to its equivalent string representation using the specified format.
		/// </summary>
		/// <param name="date">A string that contains a date to convert.</param>
		/// <param name="inputFormat">A string that defines the required format of the date.</param>
		/// <returns>
		/// Returns a sortable date/time formatted to the specified pattern.
		/// </returns>
		public static string ParseExact(string date, string inputFormat)
		{
			return ParseExact(date, inputFormat, "s");
		}

		/// <summary>
		/// Parses the exact value of the date time string to its equivalent string representation using the specified format.
		/// </summary>
		/// <param name="date">A string that contains a date to convert.</param>
		/// <param name="inputFormat">A string that defines the required format of the date.</param>
		/// <param name="outputFormat">The date format for the output.</param>
		/// <returns>
		/// Returns a sortable date/time formatted to the specified pattern.
		/// </returns>
		public static string ParseExact(string date, string inputFormat, string outputFormat)
		{
			DateTime result;
			if (!string.IsNullOrEmpty(date) && 
				!string.IsNullOrEmpty(inputFormat) && 
				!string.IsNullOrEmpty(outputFormat) && 
				DateTime.TryParseExact(date, inputFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
			{
				return result.ToString(outputFormat);
			}

			return date;
		}

		/// <summary>
		/// Gets the elapsed seconds since the input DateTime.
		/// </summary>
		/// <param name="input">The input DateTime (as a string).</param>
		/// <returns>
		/// Returns the elapsed seconds since the input DateTime.
		/// </returns>
		public static double ElapsedSeconds(string input)
		{
			DateTime datetime;

			if (DateTime.TryParse(input, out datetime))
			{
				return DateTime.Now.Subtract(datetime).TotalSeconds;
			}

			return 0;
		}

		/// <summary>
		/// Get the current age, from the specified date of birth.
		/// </summary>
		/// <param name="dateOfBirth">The date of birth.</param>
		/// <returns>
		/// Returns the age based on the specified date of birth.
		/// </returns>
		public static int Age(string dateOfBirth)
		{
			DateTime date;
			var today = DateTime.Today;

			if (DateTime.TryParse(dateOfBirth, out date))
			{
				// if month is less, or if month is equal, and day less
				if (today.Month < date.Month || today.Month == date.Month && today.Day < date.Day)
				{
					// then they haven't had is year's birthday yet!
					return today.Year - date.Year - 1;
				}
				else
				{
					// otherwise, substract the current year from date-of-birth.
					return today.Year - date.Year;
				}
			}

			// unable to parse date-of-birth.
			return -1;
		}

		/// <summary>
		/// Determines whether [is leap year] [the specified date].
		/// </summary>
		/// <param name="date">The date.</param>
		/// <returns>
		/// 	<c>true</c> if [is leap year] [the specified date]; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsLeapYear(string date)
		{
			DateTime result;

			// parse the date
			if (DateTime.TryParse(date, out result))
			{
				// test if number of days in Feburary is 29
				return (DateTime.DaysInMonth(result.Year, 2).Equals(29));
			}

			// otherwise return false
			return false;
		}

		/// <summary>
		/// Determines whether the specified date is weekday.
		/// </summary>
		/// <param name="date">The date string.</param>
		/// <returns>
		/// 	<c>true</c> if the specified date is weekday; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsWeekday(string date)
		{
			DateTime result;

			if (DateTime.TryParse(date, out result))
			{
				return IsWeekday(result.DayOfWeek);
			}

			return false;
		}

		/// <summary>
		/// Determines whether the specified date is weekend.
		/// </summary>
		/// <param name="date">The date string.</param>
		/// <returns>
		/// 	<c>true</c> if the specified date is weekend; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsWeekend(string date)
		{
			return !IsWeekday(date);
		}

		/// <summary>
		/// Adds the workdays, excluding weekends.
		/// </summary>
		/// <param name="date">The date.</param>
		/// <param name="days">The days.</param>
		/// <returns></returns>
		public static string AddWorkdays(string date, int days)
		{
			return AddWorkdays(date, days, DEFAULT_DATE_FORMAT);
		}

		/// <summary>
		/// Adds the workdays, excluding weekends.
		/// </summary>
		/// <param name="date">The date.</param>
		/// <param name="days">The days.</param>
		/// <param name="format">The format.</param>
		/// <returns></returns>
		public static string AddWorkdays(string date, int days, string format)
		{
			DateTime result;

			// parse the date
			if (DateTime.TryParse(date, out result))
			{
				// start from a weekday
				while (!IsWeekday(result.DayOfWeek))
				{
					result = result.AddDays(1);
				}

				// increment through the days
				for (int i = 0; i < days; ++i)
				{
					// add a day
					result = result.AddDays(1);

					// check if weekend, add day until its a weekday
					while (!IsWeekday(result.DayOfWeek))
					{
						result = result.AddDays(1);
					}
				}

				return FormatDateTime(result, format);
			}

			// return with date formatting
			return FormatDateTime(date, format);
		}

		/// <summary>
		/// Gets the first day of month.
		/// </summary>
		/// <param name="date">The date.</param>
		/// <returns></returns>
		public static string GetFirstDayOfMonth(string date)
		{
			return GetFirstDayOfMonth(date, DEFAULT_DATE_FORMAT);
		}

		/// <summary>
		/// Gets the first day of month.
		/// </summary>
		/// <param name="date">The date.</param>
		/// <param name="format">The format.</param>
		/// <returns></returns>
		public static string GetFirstDayOfMonth(string date, string format)
		{
			DateTime result;

			// parse the date
			if (DateTime.TryParse(date, out result))
			{
				// get the first day of the month.
				var firstDay = new DateTime(result.Year, result.Month, 1);
				return FormatDateTime(firstDay, format);
			}

			// return with date formatting
			return FormatDateTime(date, format);
		}

		/// <summary>
		/// Gets the last day of month.
		/// </summary>
		/// <param name="date">The date  string.</param>
		/// <returns></returns>
		public static string GetLastDayOfMonth(string date)
		{
			return GetLastDayOfMonth(date, DEFAULT_DATE_FORMAT);
		}

		/// <summary>
		/// Gets the last day of month.
		/// </summary>
		/// <param name="date">The date string.</param>
		/// <param name="format">The format.</param>
		/// <returns></returns>
		public static string GetLastDayOfMonth(string date, string format)
		{
			DateTime result;

			// parse the date
			if (DateTime.TryParse(date, out result))
			{
				// get the last day of the month.
				var lastDay = new DateTime(result.Year, result.Month, 1).AddMonths(1).AddDays(-1);
				return FormatDateTime(lastDay, format);
			}

			// return with date formatting
			return FormatDateTime(date, format);
		}

		/// <summary>
		/// Lists all the dates between the start and end dates.
		/// </summary>
		/// <param name="startDate">The start date.</param>
		/// <param name="endDate">The end date.</param>
		/// <returns>Returns a nodeset of all the dates between the start and end date.</returns>
		public static XPathNodeIterator ListDates(string startDate, string endDate)
		{
			DateTime start, end;
			string separator = "|";
			var dates = new List<string>();

			// parse the dates
			if (DateTime.TryParse(startDate, out start) && DateTime.TryParse(endDate, out end))
			{
				for (var date = start; date <= end; date = date.AddDays(1))
				{
					dates.Add(date.ToString("s"));
				}
			}

			return umbraco.library.Split(string.Join(separator, dates.ToArray()), separator);
		}

		/// <summary>
		/// Converts a date to Unix time.
		/// </summary>
		/// <param name="date">The date string.</param>
		/// <returns>
		/// Return the total number of seconds between Unix epoch and the specified date/time.
		/// </returns>
		public static double ToUnixTime(string date)
		{
			// set the Unix epoch
			var unixEpoch = new DateTime(1970, 1, 1);

			// set the current date/time
			var result = DateTime.UtcNow;

			// parse the date
			DateTime.TryParse(date, out result);

			// return the total seconds (from either specified date or current date/time).
			return (result - unixEpoch).TotalSeconds;
		}

		/// <summary>
		/// Gets the number of workdays between two dates.
		/// </summary>
		/// <param name="startDate">The start date.</param>
		/// <param name="endDate">The end date.</param>
		/// <returns>Returns the number of workdays between two dates.</returns>
		public static int WorkdaysDiff(string startDate, string endDate)
		{
			DateTime start, end;

			// parse the dates
			if (DateTime.TryParse(startDate, out start) && DateTime.TryParse(endDate, out end))
			{
				return WorkdaysDiff(start, end);
			}

			return -1;
		}

		/// <summary>
		/// Formats the date time.
		/// </summary>
		/// <param name="date">The date.</param>
		/// <param name="format">The format.</param>
		/// <returns></returns>
		private static string FormatDateTime(DateTime date, string format)
		{
			// if the format string is empty...
			if (string.IsNullOrEmpty(format))
			{
				// ... set it to the default date format.
				format = DEFAULT_DATE_FORMAT;
			}
			else
			{
				// otherwise, replace the ordinal token
				format = Regex.Replace(format, @"(?<!\\)((\\\\)*)(S)", "$1" + GetDayNumberSuffix(date));
			}

			// return with formatting the remaining tokens.
			return date.ToString(format);
		}

		///<summary>
		/// Gets the ordinal suffix for a given date
		///</summary>
		///<param name="date">The date</param>
		///<returns>The ordinal suffix</returns>
		private static string GetDayNumberSuffix(DateTime date)
		{
			switch (date.Day)
			{
				case 1:
				case 21:
				case 31:
					return @"\s\t";
				case 2:
				case 22:
					return @"\n\d";
				case 3:
				case 23:
					return @"\r\d";
				default:
					return @"\t\h";
			}
		}

		/// <summary>
		/// Determines whether the specified day is weekday.
		/// </summary>
		/// <param name="day">The day.</param>
		/// <returns>
		/// 	<c>true</c> if the specified day is weekday; otherwise, <c>false</c>.
		/// </returns>
		private static bool IsWeekday(DayOfWeek day)
		{
			switch (day)
			{
				// is a weekend?
				case DayOfWeek.Saturday:
				case DayOfWeek.Sunday:
					return false;

				// otherwise its a weekday?
				default:
					return true;
			}
		}

		/// <summary>
		/// Gets the number of workdays between two dates.
		/// </summary>
		/// <param name="startDate">The start date.</param>
		/// <param name="endDate">The end date.</param>
		/// <returns>Returns the number of workdays between two dates.</returns>
		/// <remarks>
		/// http://www.eggheadcafe.com/community/aspnet/2/44982/how-to-calculate-num-of-weekdays-between-2-dates.aspx#Post45102
		/// </remarks>
		private static int WorkdaysDiff(DateTime startDate, DateTime endDate)
		{
			// This function includes the start and end date in the count if they fall on a weekday
			var start = ((int)startDate.DayOfWeek == 0 ? 7 : (int)startDate.DayOfWeek);
			var end = ((int)endDate.DayOfWeek == 0 ? 7 : (int)endDate.DayOfWeek);
			var diff = endDate - startDate;

			if (start <= end)
			{
				return (((diff.Days / 7) * 5) + Math.Max((Math.Min((end + 1), 6) - start), 0));
			}

			return (((diff.Days / 7) * 5) + Math.Min((end + 6) - Math.Min(start, 6), 5));
		}
	}
}
