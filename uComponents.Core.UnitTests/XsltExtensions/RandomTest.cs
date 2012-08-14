using uComponents.Core.XsltExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.XPath;
using Xslt = uComponents.Core.XsltExtensions;

namespace uComponents.Core.UnitTests.XsltExtensions
{
	[TestClass]
	public class RandomTest
	{
		//[TestMethod]
		//public void GetRandomDoubleTest()
		//{
		//    var actual = Xslt.Random.GetRandomDouble();
		//    Assert.IsTrue(actual >= 0F, "Should be greater than or equal to zero");
		//    Assert.IsTrue(actual <= 1F, "Should be less than or equal to one");
		//}

		[TestMethod]
		public void GetRandomGuidTests()
		{
			var format = "D";
			var newGuid = Guid.NewGuid().ToString(format);
			var randomGuid = Xslt.Random.GetRandomGuid();
			var randomGuidFormatted = Xslt.Random.GetRandomGuid(format);

			Assert.AreEqual(newGuid.Length , randomGuid.Length, "The length of the GUIDs should be the same; 36 characters");
			Assert.AreEqual(newGuid.Length, randomGuidFormatted.Length, "The length of the GUIDs should be the same; 36 characters");
			Assert.AreEqual(randomGuid.Length, randomGuidFormatted.Length, "The length of the GUIDs should be the same; 36 characters");

			Assert.AreNotEqual(newGuid, randomGuid, "The GUID values should be unique");
			Assert.AreNotEqual(newGuid, randomGuidFormatted, "The GUID values should be unique");
			Assert.AreNotEqual(randomGuid, randomGuidFormatted, "The GUID values should be unique");
		}

		//[TestMethod]
		//public void GetRandomNumberTests()
		//{
		//    var minimum = 1;
		//    var maximum = 100;
		//    var actual = Xslt.Random.GetRandomNumber(minimum, maximum);

		//    Assert.IsTrue(actual >= minimum, "Should be greater than or equal to the minimum");
		//    Assert.IsTrue(actual <= maximum, "Should be less than or equal to the maximum");
		//}

		//[TestMethod]
		//public void GetRandomNumbersTest()
		//{
		//    var count = 10;
		//    var actual = Xslt.Random.GetRandomNumbers(count);

		//    Assert.IsFalse(string.IsNullOrWhiteSpace(actual), "Check if the string is empty");

		//    var items = actual.Split(',');

		//    Assert.AreEqual(count, items.Length, "The length of the array should be the same as the original count");
		//}

		//[TestMethod]
		//public void GetRandomStringTest()
		//{
		//    int count = 0; // TODO: Initialize to an appropriate value
		//    string expected = string.Empty; // TODO: Initialize to an appropriate value
		//    string actual;
		//    actual = Xslt.Random.GetRandomString(count);
		//    Assert.AreEqual(expected, actual);
		//    Assert.Inconclusive("Verify the correctness of this test method.");
		//}

		//[TestMethod]
		//public void GetRandomNumbersAsXmlTest()
		//{
		//    int count = 0; // TODO: Initialize to an appropriate value
		//    XPathNodeIterator expected = null; // TODO: Initialize to an appropriate value
		//    XPathNodeIterator actual;
		//    actual = Xslt.Random.GetRandomNumbersAsXml(count);
		//    Assert.AreEqual(expected, actual);
		//    Assert.Inconclusive("Verify the correctness of this test method.");
		//}
	}
}
