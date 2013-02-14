using uComponents.XsltExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using uComponents.Core.UnitTests.Abstractions;
using System.Collections.Specialized;

namespace uComponents.Core.UnitTests.XsltExtensions
{
	[TestClass]
	public class UrlsTest
	{
		[TestMethod]
		public void ConcatAsUrlTest()
		{
			// Various tests with slashes all over the place
			var actual = Urls.ConcatAsUrl("http://www.microsoft.com", "test.htm");
			Assert.AreEqual("http://www.microsoft.com/test.htm", actual);

			actual = Urls.ConcatAsUrl("http://www.microsoft.com/", "test.htm");
			Assert.AreEqual("http://www.microsoft.com/test.htm", actual);

			actual = Urls.ConcatAsUrl("http://www.microsoft.com", "/test.htm");
			Assert.AreEqual("http://www.microsoft.com/test.htm", actual);

			actual = Urls.ConcatAsUrl("http://www.microsoft.com/", "/test.htm");
			Assert.AreEqual("http://www.microsoft.com/test.htm", actual);

			actual = Urls.ConcatAsUrl("http://www.microsoft.com/", "/test/");
			Assert.AreEqual("http://www.microsoft.com/test/", actual);

			// Different divider anyone?
			actual = Urls.ConcatAsUrl("http://www.microsoft.com|", "|test|", '|');
			Assert.AreEqual("http://www.microsoft.com|test|", actual);
		}

		[TestMethod]
		[DeploymentItem("uComponents.Core.dll")]
		public void ConstructQueryStringTest()
		{
			var parameters = new NameValueCollection();

			// Empty collection first
			Assert.AreEqual("", Urls_Accessor.ConstructQueryString(parameters));

			// Various names/values
			parameters["test1"] = "value1";
			Assert.AreEqual("test1=value1", Urls_Accessor.ConstructQueryString(parameters));

			parameters["test2"] = "value2";
			Assert.AreEqual("test1=value1&test2=value2", Urls_Accessor.ConstructQueryString(parameters));

			parameters["test3"] = "value3";
			Assert.AreEqual("test1=value1&test2=value2&test3=value3", Urls_Accessor.ConstructQueryString(parameters));

			// Different delimiter
			Assert.AreEqual("test1=value1~test2=value2~test3=value3", Urls_Accessor.ConstructQueryString(parameters, "~", true));

			// Empty values
			parameters["test3"] = null;
			Assert.AreEqual("test1=value1&test2=value2", Urls_Accessor.ConstructQueryString(parameters));
			Assert.AreEqual("test1=value1&test2=value2&test3=", Urls_Accessor.ConstructQueryString(parameters, false));   // Include empty values

			parameters["test1"] = null;
			Assert.AreEqual("test2=value2", Urls_Accessor.ConstructQueryString(parameters));
			Assert.AreEqual("test1=&test2=value2&test3=", Urls_Accessor.ConstructQueryString(parameters, false));   // Include empty values
		}

		[TestMethod]
		public void AddAltTemplateTest()
		{
			// A little help to make the tests read a bit better
			var baseUrl = "http://www.test.com/page.aspx";
			var useDirectories = true;
			var doNotUseDirectories = false;
			var useQueryString = true;
			var doNotUseQueryString = false;

			string actual = Urls.AddAltTemplate(baseUrl, "altTemp", useQueryString, doNotUseDirectories);
			Assert.AreEqual("http://www.test.com/page.aspx?altTemplate=alttemp", actual, "Added as a querystring with directory names switched off failed");

			// Note the alttemp is converted to lower case...
			actual = Urls.AddAltTemplate(baseUrl, "altTemp", doNotUseQueryString, doNotUseDirectories);
			Assert.AreEqual("http://www.test.com/page/alttemp.aspx", actual, "Added as a folder name with directory names switched off");

			actual = Urls.AddAltTemplate(baseUrl, "altTemp", doNotUseQueryString, useDirectories);
			Assert.AreEqual("http://www.test.com/page/alttemp/", actual, "Added as a folder name with directory names switched on");

			actual = Urls.AddAltTemplate(baseUrl, "altTemp", useQueryString, useDirectories);
			Assert.AreEqual("http://www.test.com/page.aspx?altTemplate=alttemp", actual, "Added as a QS with directory names switched on");
		}

		[TestMethod]
		public void AppendOrUpdateQueryStringTest()
		{
			// No existing querystring
			var actual = Urls.AppendOrUpdateQueryString("http://www.test.com", "testKey", "testValue");
			Assert.AreEqual("http://www.test.com?testKey=testValue", actual);

			actual = Urls.AppendOrUpdateQueryString("http://www.test.com/", "testKey", "testValue");
			Assert.AreEqual("http://www.test.com/?testKey=testValue", actual);

			// Existing QS but differing keys
			actual = Urls.AppendOrUpdateQueryString("http://www.test.com?existingKey=existingValue", "testKey", "testValue");
			Assert.AreEqual("http://www.test.com?existingKey=existingValue&testKey=testValue", actual);

			actual = Urls.AppendOrUpdateQueryString("http://www.test.com?existingKey=existingValue&existingKey1=existingValue1", "testKey", "testValue");
			Assert.AreEqual("http://www.test.com?existingKey=existingValue&existingKey1=existingValue1&testKey=testValue", actual);

			// Existing QS with same key
			actual = Urls.AppendOrUpdateQueryString("http://www.test.com/?testKey=oldValue", "testKey", "testValue");
			Assert.AreEqual("http://www.test.com/?testKey=testValue", actual);

			// Same key mixed with other keys
			actual = Urls.AppendOrUpdateQueryString("http://www.test.com/?existingKey=existingValue&testKey=oldValue", "testKey", "testValue");
			Assert.AreEqual("http://www.test.com/?existingKey=existingValue&testKey=testValue", actual);

			actual = Urls.AppendOrUpdateQueryString("http://www.test.com/?testKey=oldValue&existingKey=existingValue", "testKey", "testValue");
			Assert.AreEqual("http://www.test.com/?testKey=testValue&existingKey=existingValue", actual);
		}

		[TestMethod]
		public void NiceUrlTest()
		{
			var cannedUrl = "/my/test/url/1234.aspx";
			Urls_Accessor.Library = new MockUmbracoLibrary(cannedUrl);

			var nodeId = 1234;

			var actual = Urls.NiceUrl(nodeId);
			Assert.AreEqual(cannedUrl, actual, "On its own it should just return our dummy url as is");

			actual = Urls.NiceUrl(nodeId, null);
			Assert.AreEqual(cannedUrl, actual, "Invalid alt temp so should just return our dummy url as is");

			actual = Urls.NiceUrl(nodeId, "");
			Assert.AreEqual(cannedUrl, actual, "Invalid alt temp so should just return our dummy url as is");

			actual = Urls.NiceUrl(nodeId, "altTemp");
			Assert.AreEqual("/my/test/url/1234/alttemp.aspx", actual, "Dummy url with altTemp added, defaulted as a folder name, assumes directory naming is OFF");

			actual = Urls.NiceUrl(nodeId, "altTemp", false);
			Assert.AreEqual("/my/test/url/1234/alttemp.aspx", actual, "Dummy url with altTemp added as a folder name, assumes directory naming is OFF");

			actual = Urls.NiceUrl(nodeId, "altTemp", true);
			Assert.AreEqual(cannedUrl + "?altTemplate=alttemp", actual, "Dummy url with altTemp added as a QS");
		}
	}
}