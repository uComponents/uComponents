using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using uComponents.Core;
using uComponents.Core.Abstractions;
using uComponents.Core.Interfaces;
using umbraco;

namespace uComponents.XsltExtensions
{
	/// <summary>
	/// The Random class exposes XSLT extensions to offer extended randomizing functionality.
	/// </summary>
	[XsltExtension("ucomponents.random")]
	public class Random
	{
		/// <summary>
		/// Instance of the wrapper for <c>umbraco.library</c>, so we can 'mock' this for unit-testing.
		/// </summary>
		private static IUmbracoLibrary Library = new UmbracoLibraryWrapper();

		/// <summary>
		/// Gets the random double.
		/// </summary>
		/// <returns>Returns a random dobule.</returns>
		public static double GetRandomDouble()
		{
			return Library.GetRandom().NextDouble();
		}

		/// <summary>
		/// Gets the random GUID.
		/// </summary>
		/// <returns>Returns a random GUID.</returns>
		public static string GetRandomGuid()
		{
			return GetRandomGuid("D");
		}

		/// <summary>
		/// Gets the random GUID, with a specified format.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <returns>Returns a random GUID, with a specified format.</returns>
		public static string GetRandomGuid(string format)
		{
			return Guid.NewGuid().ToString(format);
		}

		/// <summary>
		/// Gets the random items from CSV.
		/// </summary>
		/// <param name="csv">The CSV.</param>
		/// <param name="count">The count.</param>
		/// <returns>Returns a random selection of items from the original comma-separated values.</returns>
		public static string GetRandomItemsFromCsv(string csv, int count)
		{
			// if empty, return empty
			if (string.IsNullOrWhiteSpace(csv))
				return string.Empty;

			// split CSV, load into array
			var list = csv.Split(new[] { Constants.Common.COMMA }, StringSplitOptions.RemoveEmptyEntries);

			// randomly sort the order of the list (based on GUIDs)
			var items = list.OrderBy(s => Guid.NewGuid()).ToList();

			// if the number of items is larger than the count...
			if (items.Count >= count)
			{
				// ... then take the first [x] items
				items = items.Take(count).ToList();
			}

			// rebuild the CSV
			return string.Join(new string(Constants.Common.COMMA, 1), items.ToArray());
		}

		/// <summary>
		/// Gets the random number.
		/// </summary>
		/// <returns>Returns a random integer.</returns>
		public static int GetRandomNumber()
		{
			return Library.GetRandom().Next();
		}

		/// <summary>
		/// Gets the random number.
		/// </summary>
		/// <param name="maximum">The maximum.</param>
		/// <returns>Returns a random integer, less than specified maximum.</returns>
		public static int GetRandomNumber(int maximum)
		{
			return Library.GetRandom().Next(maximum);
		}

		/// <summary>
		/// Gets the random number.
		/// </summary>
		/// <param name="minimum">The minimum.</param>
		/// <param name="maximum">The maximum.</param>
		/// <returns>Returns a random integer, between the specified minimum and maximum.</returns>
		public static int GetRandomNumber(int minimum, int maximum)
		{
			return Library.GetRandom().Next(minimum, maximum);
		}

		/// <summary>
		/// Gets the random numbers.
		/// </summary>
		/// <param name="count">The count.</param>
		/// <returns>Returns a sequence of random numbers.</returns>
		public static string GetRandomNumbers(int count)
		{
			return GetRandomNumbers(count, new string(Constants.Common.COMMA, 1));
		}

		/// <summary>
		/// Gets the random numbers.
		/// </summary>
		/// <param name="count">The count.</param>
		/// <param name="delimiter">The delimiter.</param>
		/// <returns>Returns a sequence of random numbers.</returns>
		public static string GetRandomNumbers(int count, string delimiter)
		{
			var random = Library.GetRandom();
			var sequence = Enumerable.Range(1, count).OrderBy(n => n * n * random.Next()).Select(i => i.ToString());

			return string.Join(delimiter, sequence.ToArray());
		}

		/// <summary>
		/// Gets the random numbers as XML.
		/// </summary>
		/// <param name="count">The count.</param>
		/// <returns>Returns a sequence of random numbers as an XML node-set.</returns>
		public static XPathNodeIterator GetRandomNumbersAsXml(int count)
		{
			var numbers = GetRandomNumbers(count);
			return Xml.Split(numbers);
		}

		/// <summary>
		/// Gets the random string.
		/// </summary>
		/// <param name="count">The count.</param>
		/// <returns>Returns a sequence of random characters in a string.</returns>
		public static string GetRandomString(int count)
		{
			return GenerateRandomString(new string('X', count), "X");
		}

		/// <summary>
		/// Generates the random string.
		/// </summary>
		/// <param name="pattern">The pattern.</param>
		/// <param name="replacer">The replacer.</param>
		/// <returns>Returns the specified pattern with characters replaced with random characters.</returns>
		internal static string GenerateRandomString(string pattern, string replacer)
		{
			var characters = "AzByCxDwEvFuGtHsIrJqKpLoMnNmOlPkQjRiShTgUfVeWdXcYbZa1234567890";
			var random = Library.GetRandom();
			var evaluator = new MatchEvaluator(delegate(Match m) { return characters[random.Next(characters.Length)].ToString(); });

			return Regex.Replace(pattern, replacer, evaluator);
		}
	}
}