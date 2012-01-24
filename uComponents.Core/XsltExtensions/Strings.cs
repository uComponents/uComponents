using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.XPath;

using uComponents.Core.Shared;
using umbraco.cms.helpers;

namespace uComponents.Core.XsltExtensions
{
	/// <summary>
	/// The Strings class exposes XSLT extensions to offer extended string functionality.
	/// </summary>
	public class Strings
	{
		/// <summary>
		/// A delegate for the character condition.
		/// </summary>
		/// <param name="c">The character.</param>
		/// <returns>A boolean of the condition of the character.</returns>
		private delegate bool CharCondition(char c);

		/// <summary>
		/// Return the first amount of words defined by 'count' contained in 'text'.
		/// </summary>
		/// <param name="text">String to work on</param>
		/// <param name="count">Amount of words to look for</param>
		/// <returns>
		/// String containing only the first x words.
		/// </returns>
		/// <remarks>Word delimiters are: linefeed, carrige return, tab and space</remarks>
		public static string GetFirstWords(string text, int count)
		{
			return GetFirstWords(text, count, "&#8230;");
		}

		/// <summary>
		/// Return the first amount of words defined by 'count' contained in 'text'.
		/// </summary>
		/// <param name="text">String to work on</param>
		/// <param name="count">Amount of words to look for</param>
		/// <param name="appender">The string to append to the text</param>
		/// <returns>
		/// String containing only the first x words.
		/// </returns>
		/// <remarks>Word delimiters are: linefeed, carrige return, tab and space</remarks>
		public static string GetFirstWords(string text, int count, string appender)
		{
			if (string.IsNullOrEmpty(text))
			{
				return string.Empty;
			}

			var chars = new char[] { '\n', '\r', '\t', ' ' };
			var words = text.Split(chars, StringSplitOptions.RemoveEmptyEntries);

			if ((words == null) || (words.Length <= 0))
			{
				return string.Empty;
			}

			if (words.Length <= count)
			{
				return text;
			}

			var sb = new StringBuilder();

			for (int i = 0; i < Math.Min(words.Length, count); i++)
			{
				sb.AppendFormat("{0}{1}", words[i], (i < (count - 1)) ? " " : appender);
			}

			return sb.ToString();
		}

		/// <summary>
		/// Converts an email address into a hyperlink.
		/// </summary>
		/// <param name="email">The email address.</param>
		/// <returns></returns>
		public static string MakeEmailHyperlink(string email)
		{
			return MakeEmailHyperlink(email, email);
		}

		/// <summary>
		/// Converts an email address into a hyperlink.
		/// </summary>
		/// <param name="email">The email address.</param>
		/// <param name="text">The text.</param>
		/// <returns></returns>
		public static string MakeEmailHyperlink(string email, string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				text = email;
			}

			return string.Concat("<a href=\"mailto:", email, "\">", text, "</a>");
		}

		/// <summary>
		/// Converts all URLs into hyperlinks within a string.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <returns>Returns a string with all URLs turned into hyperlinks.</returns>
		public static string MakeUrlHyperlink(string input)
		{
			var pattern = @"(?<Protocol>\w+):\/\/(?<Domain>[\w.]+\/?)\S*(?x)";
			var replacement = "<a href=\"$&\" target=\"_blank\">$&</a>";
			return Regex.Replace(input, pattern, replacement, RegexOptions.Compiled);
		}

		/// <summary>
		/// Truncates the middle section of a string, this is ideal for long filepaths or URLs.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <returns>Returns a shortened path of the string.</returns>
		[Obsolete("Please use uComponents.Core.XsltExtensions.IO.PathShortener")]
		public static string PathShortener(string input)
		{
			return IO.PathShortener(input);
		}

		/// <summary>
		/// Truncates the inner-string.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <param name="maxLength">Maximum lenth of the truncated string.</param>
		/// <returns>Returns a string with the mid-section truncated.</returns>
		public static string TruncateInner(string input, int maxLength)
		{
			// if input length is less than the max length, return it.
			if (string.IsNullOrEmpty(input) || input.Length <= maxLength)
			{
				return input;
			}

			// get the number of characters in each half.
			var half = (maxLength - 3) / 2;

			// get the right part of the string.
			var right = input.Substring(input.Length - half, half).TrimStart();

			// get the left part of the string.
			var left = input.Substring(0, (maxLength - 3) - right.Length).TrimEnd();

			// return the concatenate string.
			return string.Concat(left, "...", right);
		}

		/// <summary>
		/// Removes non alpha-numeric characters from a string.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <returns>Returns the string with non alpha-numeric characters removed.</returns>
		public static string RemoveChars(string input)
		{
			return CharLoop(
				input,
				delegate(char c)
				{
					return (!char.IsControl(c)) && (!char.IsPunctuation(c)) && (!char.IsSymbol(c)) && (!char.IsWhiteSpace(c));
				}
			);
		}

		/// <summary>
		/// Reverses the specified input.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns></returns>
		public static string Reverse(string input)
		{
			var c = input.ToCharArray();

			Array.Reverse(c);

			return new string(c);
		}

		/// <summary>
		/// Makes an alias name safe to use as an XML element name.
		/// Removes all spaces and non-alphanumeric characters.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <returns>Returns a safe alias string.</returns>
		/// <remarks>This is a wrapper method for <c>umbraco.cms.helpers.Casing.SafeAlias</c>.</remarks>
		public static string SafeAlias(string input)
		{
			return Casing.SafeAlias(input);
		}

		/// <summary>
		/// Spaces out a string on capitals or numbers.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <returns>
		/// The string with spaces before each capital letter or number
		/// </returns>
		public static string SpaceOutCamelCase(string input)
		{
			var space = ' ';
			var sb = new StringBuilder();
			for (var i = 0; i < input.Length; i++)
			{
				if (char.IsUpper(input, i) || char.IsNumber(input, i))
				{
					sb.Append(space);
				}

				sb.Append(input[i]);
			}

			return sb.ToString();
		}

		/// <summary>
		/// Strips the &lt;font&gt; tags from a string.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <returns>Returns the string stripped of all &lt;font&gt; tags.</returns>
		public static string StripFontTags(string input)
		{
			var pattern = @"<\/?font[^>]*>";
			return Regex.Replace(input, pattern, string.Empty);
		}

		/// <summary>
		/// Strips the HTML.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <returns>Returns the string stripped of all HTML tags.</returns>
		/// <remarks>
		/// http://dotnetperls.com/remove-html-tags
		/// </remarks>
		public static string StripHTML(string input)
		{
			var sb = new StringBuilder(input.Length);
			var inside = false;
			for (var i = 0; i < input.Length; i++)
			{
				char let = input[i];
				if (let == '<')
				{
					inside = true;
					continue;
				}

				if (let == '>')
				{
					inside = false;
					continue;
				}

				if (inside == false)
				{
					sb.Append(let);
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Strips the whitespace characters from a string.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <returns>Returns the string stripped of any whitespace characters.</returns>
		public static string StripWhitespace(string input)
		{
			return CharLoop(
				input,
				delegate(char c)
				{
					return !char.IsWhiteSpace(c);
				}
			);
		}

		/// <summary>
		/// Strips the non alpha-numeric characters.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <returns>Returns the string with only alpha-numeric characters.</returns>
		public static string StripNonAlphaNumeric(string input)
		{
			return CharLoop(input, char.IsLetterOrDigit);
		}

		/// <summary>
		/// Strips out all the line-breaks.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <returns>Returns the string with all the line-breaks stripped out.</returns>
		public static string StripLineBreaks(string input)
		{
			return input.Replace(Environment.NewLine, string.Empty);
		}

		/// <summary>
		/// Changes the case of the string to lowercase.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <returns>Returns the string as lowercase.</returns>
		public static string ToLowerCase(string input)
		{
			return input.ToLower();
		}

		/// <summary>
		/// Changes the case of the string to uppercase.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <returns>Returns the string as uppercase.</returns>
		public static string ToUpperCase(string input)
		{
			return input.ToUpper();
		}

		/// <summary>
		/// Changes a string to camelCase
		/// </summary>
		/// <param name="input">
		/// The input string.
		/// </param>
		/// <returns>
		/// The camelCased string
		/// </returns>
		public static string ConvertToCamelCase(string input)
		{
			var sb = new StringBuilder();

			// Look for spaces
			var str = input.Split(new[] { ' ' });
			var firstTime = true;

			foreach (string temp in str)
			{
				if (firstTime)
				{
					sb.Append(temp.ToLower());
					firstTime = false;
				}
				else
				{
					sb.Append(Char.ToUpper(temp.ToCharArray()[0]));
					sb.Append(temp.Substring(1).ToLower());
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Changes the case of the string to proper case.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <returns>Returns the string in proper case.</returns>
		public static string ToProperCase(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return string.Empty;
			}

			var chars = input.ToCharArray();
			chars[0] = char.ToUpper(chars[0]);

			return new string(chars);
		}

		/// <summary>
		/// Changes the case of the string to title case.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <returns>Returns the string in title case.</returns>
		/// <remarks>This method makes use of the <c>System.Globalization.CultureInfo.InvariantCulture</c> class.</remarks>
		public static string ToTitleCase(string input)
		{
			return System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(input);
		}

		/// <summary>
		/// Encodes a string as Base64.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <returns>Returns the string encoded as Base64.</returns>
		public static string ToBase64String(string input)
		{
			if (!string.IsNullOrEmpty(input))
			{
				var encoding = new UTF8Encoding();
				return Convert.ToBase64String(encoding.GetBytes(input));
			}

			return string.Empty;
		}

		/// <summary>
		/// Decodes a string from Base64.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <returns>Returns the decoded Base64 string.</returns>
		public static string FromBase64String(string input)
		{
			if (!string.IsNullOrEmpty(input))
			{
				var encoding = new UTF8Encoding();
				return encoding.GetString(Convert.FromBase64String(input));
			}

			return string.Empty;
		}

		/// <summary>
		/// Counts the number of words in a string.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <returns>Returns the number of words in the string.</returns>
		public static int WordCount(string input)
		{
			var pattern = @"\b\w+\b";
			input = StripHTML(input);

			return Regex.Split(input, pattern, RegexOptions.Multiline).Length - 1;
		}

		/// <summary>
		/// Lowers the case of the HTML tags.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <returns>Returns the string with all HTML tags in lowercase.</returns>
		public static string LowerCaseTags(string input)
		{
			var evaluator = new MatchEvaluator(delegate(Match m)
				{
					return m.Value.ToLower();
				}
			);

			return Regex.Replace(input, @"<[^<>]+>", evaluator, RegexOptions.Multiline | RegexOptions.Singleline);
		}

		/// <summary>
		/// Performs a Coalesce among the supplied arguments.
		/// </summary>
		/// <param name="arg1">Argument 1.</param>
		/// <param name="arg2">Argument 2.</param>
		/// <returns>Returns the first nonnull or empty expression among the supplied arguments.</returns>
		public static string Coalesce(string arg1, string arg2)
		{
			return !string.IsNullOrEmpty(arg1) ? arg1 : arg2;
		}

		/// <summary>
		/// Performs a Coalesce among the supplied arguments.
		/// </summary>
		/// <param name="arg1">Argument 1.</param>
		/// <param name="arg2">Argument 2.</param>
		/// <param name="arg3">Argument 3.</param>
		/// <returns>Returns the first nonnull or empty expression among the supplied arguments.</returns>
		public static string Coalesce(string arg1, string arg2, string arg3)
		{
			return Coalesce(Coalesce(arg1, arg2), arg3);
		}

		/// <summary>
		/// Performs a Coalesce among the supplied arguments.
		/// </summary>
		/// <param name="arg1">Argument 1.</param>
		/// <param name="arg2">Argument 2.</param>
		/// <param name="arg3">Argument 3.</param>
		/// <param name="arg4">Argument 4.</param>
		/// <returns>Returns the first nonnull or empty expression among the supplied arguments.</returns>
		public static string Coalesce(string arg1, string arg2, string arg3, string arg4)
		{
			return Coalesce(Coalesce(arg1, arg2, arg3), arg4);
		}

		/// <summary>
		/// Performs a Coalesce among the supplied arguments.
		/// </summary>
		/// <param name="arg1">Argument 1.</param>
		/// <param name="arg2">Argument 2.</param>
		/// <param name="arg3">Argument 3.</param>
		/// <param name="arg4">Argument 4.</param>
		/// <param name="arg5">Argument 5.</param>
		/// <returns>Returns the first nonnull or empty expression among the supplied arguments.</returns>
		public static string Coalesce(string arg1, string arg2, string arg3, string arg4, string arg5)
		{
			return Coalesce(Coalesce(arg1, arg2, arg3, arg4), arg5);
		}

		/// <summary>
		/// Formats the specified string.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="arg1">Argument 1.</param>
		/// <returns>Returns a formatted string.</returns>
		public static string Format(string format, string arg1)
		{
			return Format(format, arg1);
		}

		/// <summary>
		/// Formats the specified string.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="arg1">Argument 1.</param>
		/// <param name="arg2">Argument 2.</param>
		/// <returns>Returns a formatted string.</returns>
		public static string Format(string format, string arg1, string arg2)
		{
			return Format(format, arg1, arg2);
		}

		/// <summary>
		/// Formats the specified string.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="arg1">Argument 1.</param>
		/// <param name="arg2">Argument 2.</param>
		/// <param name="arg3">Argument 3.</param>
		/// <returns>Returns a formatted string.</returns>
		public static string Format(string format, string arg1, string arg2, string arg3)
		{
			return Format(format, arg1, arg2, arg3);
		}

		/// <summary>
		/// Formats the specified string.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="arg1">Argument 1.</param>
		/// <param name="arg2">Argument 2.</param>
		/// <param name="arg3">Argument 3.</param>
		/// <param name="arg4">Argument 4.</param>
		/// <returns>Returns a formatted string.</returns>
		public static string Format(string format, string arg1, string arg2, string arg3, string arg4)
		{
			return Format(format, arg1, arg2, arg3, arg4);
		}

		/// <summary>
		/// Formats the specified string.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="arg1">Argument 1.</param>
		/// <param name="arg2">Argument 2.</param>
		/// <param name="arg3">Argument 3.</param>
		/// <param name="arg4">Argument 4.</param>
		/// <param name="arg5">Argument 5.</param>
		/// <returns>Returns a formatted string.</returns>
		public static string Format(string format, string arg1, string arg2, string arg3, string arg4, string arg5)
		{
			return Format(format, arg1, arg2, arg3, arg4, arg5);
		}

		/// <summary>
		/// Concats the specified nodeset.
		/// </summary>
		/// <param name="nodeset">The nodeset.</param>
		/// <param name="separator">The separator.</param>
		/// <returns></returns>
		public static string Concat(XPathNodeIterator nodeset, string separator)
		{
			// initalise the list (for output)
			var list = new List<string>(nodeset.Count);

			// iterate through the node-set.
			while (nodeset.MoveNext())
			{
				// add the node value to the list.
				list.Add(nodeset.Current.Value);
			}

			// implode the list into a concatenated string.
			return string.Join(separator, list.ToArray());
		}

		/// <summary>
		/// Removes the duplicate entries from a comma-separated list.
		/// </summary>
		/// <param name="list">The comma-separated list.</param>
		/// <returns>
		/// Returns a comma-separated list with duplicate entries removed.
		/// </returns>
		public static string RemoveDuplicateEntries(string list)
		{
			return RemoveDuplicateEntries(list, new[] { Settings.COMMA });
		}

		/// <summary>
		/// Removes the duplicate entries from a comma-separated list.
		/// </summary>
		/// <param name="list">The comma-separated list.</param>
		/// <param name="separator">The separator.</param>
		/// <returns>
		/// Returns a comma-separated list with duplicate entries removed.
		/// </returns>
		public static string RemoveDuplicateEntries(string list, string separator)
		{
			return RemoveDuplicateEntries(list, separator.ToCharArray());
		}

		/// <summary>
		/// Removes the duplicate entries from a delimited list.
		/// </summary>
		/// <param name="list">The delimited list.</param>
		/// <param name="separators">The separators.</param>
		/// <returns>Returns a delimited list with duplicate entries removed.</returns>
		public static string RemoveDuplicateEntries(string list, params char[] separators)
		{
			var items = list.Split(separators, StringSplitOptions.RemoveEmptyEntries);
			var distinct = new List<string>(items).Distinct().ToList();
			return string.Join(new string(separators), distinct.ToArray());
		}

		/// <summary>
		/// Removes the empty entries from a comma-separated list.
		/// </summary>
		/// <param name="list">The comma-separated list.</param>
		/// <returns>
		/// Returns a comma-separated list with empty entries removed.
		/// </returns>
		public static string RemoveEmptyEntries(string list)
		{
			return RemoveEmptyEntries(list, new[] { Settings.COMMA });
		}

		/// <summary>
		/// Removes the empty entries from a comma-separated list.
		/// </summary>
		/// <param name="list">The comma-separated list.</param>
		/// <param name="separator">The separator.</param>
		/// <returns>
		/// Returns a comma-separated list with empty entries removed.
		/// </returns>
		public static string RemoveEmptyEntries(string list, string separator)
		{
			return RemoveEmptyEntries(list, separator.ToCharArray());
		}

		/// <summary>
		/// Removes the empty entries from a delimited list.
		/// </summary>
		/// <param name="list">The delimited list.</param>
		/// <param name="separators">The separators.</param>
		/// <returns>Returns a the delimited list with the empty entries removed.</returns>
		public static string RemoveEmptyEntries(string list, params char[] separators)
		{
			return string.Join(new string(separators), list.Split(separators, StringSplitOptions.RemoveEmptyEntries));
		}

		/// <summary>
		/// Selects a singular or plural word based on the value of the count.
		/// </summary>
		/// <param name="count">The count.</param>
		/// <param name="singular">The singular word.</param>
		/// <param name="plural">The plural word.</param>
		/// <returns>
		/// Returns the singular or plural word based on the count's value.
		/// </returns>
		/// <remarks>
		/// If the count value is one, then the singluar word is returned, otherwise the plural is returned.
		/// </remarks>
		public static string SingularPlural(int count, string singular, string plural)
		{
			return SingularPlural(count, singular, plural, false);
		}

		/// <summary>
		/// Selects a singular or plural word based on the value of the count.
		/// </summary>
		/// <param name="count">The count.</param>
		/// <param name="singular">The singular word.</param>
		/// <param name="plural">The plural word.</param>
		/// <param name="prefixCount">if set to <c>true</c> [prefix count].</param>
		/// <remarks>
		/// If the count value is one, then the singluar word is returned, otherwise the plural is returned.
		/// </remarks>
		public static string SingularPlural(int count, string singular, string plural, bool prefixCount)
		{
			if (prefixCount)
			{
				return string.Concat(count, " ", count == 1 ? singular : plural);
			}

			return count == 1 ? singular : plural;
		}

		/// <summary>
		/// Removes the specified string from the beginning of the target, if it exists.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <param name="textToTrim">The text to trim.</param>
		/// <returns>
		/// Returns the input string with the specified text trimmed from the start.
		/// </returns>
		/// <remarks>
		/// Thanks to Tom Fulton http://our.umbraco.org/member/7606
		/// </remarks>
		public static string TrimStringFromStart(string input, string textToTrim)
		{
			while (input.StartsWith(textToTrim))
			{
				input = input.Substring(textToTrim.Length);
			}

			return input;
		}

		/// <summary>
		/// Removes the specified string from the end of the target, if it exists.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <param name="textToTrim">The text to trim.</param>
		/// <returns>
		/// Returns the input string with the specified text trimmed from the end.
		/// </returns>
		/// <remarks>
		/// Thanks to Tom Fulton http://our.umbraco.org/member/7606
		/// </remarks>
		public static string TrimStringFromEnd(string input, string textToTrim)
		{
			while (input.EndsWith(textToTrim))
			{
				input = input.Substring(0, input.Length - textToTrim.Length);
			}

			return input;
		}

		/// <summary>
		/// Loops through each of the characters in the string.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <param name="condition">The character condition.</param>
		/// <returns>Returns a string that has been passed through the character condition filtered.</returns>
		private static string CharLoop(string input, CharCondition condition)
		{
			var sb = new StringBuilder();
			for (var i = 0; i < input.Length; i++)
			{
				if (condition(input[i]))
				{
					sb.Append(input[i]);
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Performs a Coalesce among the supplied arguments.
		/// </summary>
		/// <param name="args">The arguments.</param>
		/// <returns>Returns the first nonnull or empty expression among the supplied arguments.</returns>
		private static string Coalesce(params string[] args)
		{
			foreach (var arg in args)
			{
				if (!string.IsNullOrEmpty(arg))
				{
					return arg;
				}
			}

			return string.Empty;
		}

		/// <summary>
		/// Formats the specified string.
		/// </summary>
		/// <param name="format">The format string.</param>
		/// <param name="args">The arguments.</param>
		/// <returns>Returns a formatted string.</returns>
		private static string Format(string format, params string[] args)
		{
			return string.Format(format, args);
		}
	}
}