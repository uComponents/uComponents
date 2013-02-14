using System;
using System.Xml.XPath;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xslt = uComponents.XsltExtensions;
using uComponents.Core.UnitTests.Abstractions;
using System.Xml;

namespace uComponents.Core.UnitTests.XsltExtensions
{
	[TestClass]
	public class RandomTest
	{
		[TestMethod]
		[DeploymentItem("uComponents.XsltExtensions.dll")]
		public void GenerateRandomStringTest()
		{
			Xslt.Random_Accessor.Library = new MockUmbracoLibrary();

			var pattern = new string('X', 10);
			var replacer = "X";
			var actual = Xslt.Random_Accessor.GenerateRandomString(pattern, replacer);

			Assert.AreEqual(pattern.Length, actual.Length, "Making sure that the randomly generated string is the same length.");
			Assert.AreNotEqual(pattern, actual, "Make sure that the strings are different.");
		}

		[TestMethod]
		public void GetRandomDoubleTest()
		{
			Xslt.Random_Accessor.Library = new MockUmbracoLibrary();

			var actual = Xslt.Random.GetRandomDouble();

			Assert.IsTrue(actual > 0.0F, "The random number should be greater than zero.");
			Assert.IsTrue(actual < 1.0F, "The random number should be less than zero.");
		}

		[TestMethod]
		public void GetRandomGuidTest()
		{
			Xslt.Random_Accessor.Library = new MockUmbracoLibrary();

			var format = "D";
			var guid = Guid.Empty;
			var actual = Xslt.Random.GetRandomGuid();
			Assert.IsTrue(Guid.TryParseExact(actual, format, out guid), "Tries to parse the Guid in the exact format.");

			actual = Xslt.Random.GetRandomGuid(format);
			Assert.IsTrue(Guid.TryParseExact(actual, format, out guid), "Tries to parse the Guid in the exact format.");
		}

		[TestMethod]
		public void GetRandomItemsFromCsvTest()
		{
			Xslt.Random_Accessor.Library = new MockUmbracoLibrary();

			var csv = "1111,2222,3333,4444,5555";
			var count = 5;
			var actual = Xslt.Random.GetRandomItemsFromCsv(csv, count);

			Assert.AreNotEqual(csv, actual, "The randomly selected items from the CSV should not be equal to the original.");
		}

		[TestMethod]
		public void GetRandomNumberTest()
		{
			Xslt.Random_Accessor.Library = new MockUmbracoLibrary();

			var minimum = 10;
			var maximum = 99;
			var actual = Xslt.Random.GetRandomNumber(minimum, maximum);

			Assert.IsTrue(actual > minimum, "The random number should be greater than minimum.");
			Assert.IsTrue(actual < maximum, "The random number should be less than maximum.");
		}

		[TestMethod]
		public void GetRandomNumbersTest()
		{
			Xslt.Random_Accessor.Library = new MockUmbracoLibrary();

			var count = 10;
			var actual = Xslt.Random.GetRandomNumbers(count);
			var items = actual.Split(Constants.Common.COMMA);

			Assert.AreEqual(count, items.Length, "Making sure that the randomly generated numbers (CSV) is the correct length");
		}

		[TestMethod]
		public void GetRandomNumbersAsXmlTest()
		{
			Xslt.Random_Accessor.Library = new MockUmbracoLibrary();

			var count = 10;
			var actual = Xslt.Random.GetRandomNumbersAsXml(count);

			// move to first XML node - <values>
			if (actual.MoveNext())
			{
				// select child nodes
				var actualValues = actual.Current.SelectChildren(XPathNodeType.Element);

				Assert.AreEqual(count, actualValues.Count, "Making sure that the randomly generated numbers (XML nodes) is the correct count");
			}
		}

		[TestMethod]
		public void GetRandomStringTest()
		{
			Xslt.Random_Accessor.Library = new MockUmbracoLibrary();

			var count = 10;
			var actual = Xslt.Random.GetRandomString(count);

			Assert.AreEqual(count, actual.Length, "Making sure that the randomly generated string is the correct length");
		}
	}
}