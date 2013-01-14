using uComponents.XsltExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.XPath;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;

namespace uComponents.Core.UnitTests.XsltExtensions
{
	[TestClass()]
	public class IOTest
	{
		//[TestMethod()]
		//public void DirectoryExistsTest()
		//{
		//    string path = string.Empty; // TODO: Initialize to an appropriate value
		//    bool expected = false; // TODO: Initialize to an appropriate value
		//    bool actual;
		//    actual = IO.DirectoryExists(path);
		//    Assert.AreEqual(expected, actual);
		//    Assert.Inconclusive("Verify the correctness of this test method.");
		//}

		//[TestMethod()]
		//public void FileExistsTest()
		//{
		//    string path = string.Empty; // TODO: Initialize to an appropriate value
		//    bool expected = false; // TODO: Initialize to an appropriate value
		//    bool actual;
		//    actual = IO.FileExists(path);
		//    Assert.AreEqual(expected, actual);
		//    Assert.Inconclusive("Verify the correctness of this test method.");
		//}

		[TestMethod]
		public void FormatFileSizeTest()
		{
			// "bytes", "KB", "MB", "GB", "TB", "PB", "EB"

			var kilobyte = 1024L;
			var megabyte = kilobyte * kilobyte;
			var gigabyte = kilobyte * megabyte;
			var terabyte = kilobyte * gigabyte;
			var petabyte = kilobyte * terabyte;
			var exabyte = kilobyte * petabyte;

			var items = new Dictionary<long, string>()
			{
				{ 128L, "128 bytes" },
				{ 512L, "512 bytes" },
				{ kilobyte, "1 KB" },
				{ kilobyte * 512, "512 KB" },
				{ megabyte, "1 MB" },
				{ (long)(megabyte * 1.5), "1.5 MB" },
				{ megabyte * 10, "10 MB" },
				{ gigabyte, "1 GB" },
				{ terabyte, "1 TB" },
				{ petabyte, "1 PB" },
				{ exabyte, "1 EB" }
			};

			foreach (var item in items)
			{
				Assert.AreEqual(item.Value, IO.FormatFileSize(item.Key), string.Format("Problem with '{0}'. Expected {1} but was not that.", item.Key, item.Value));
			}
		}

		//[TestMethod()]
		//public void GetDirectoriesTest()
		//{
		//    string path = string.Empty; // TODO: Initialize to an appropriate value
		//    string searchPattern = string.Empty; // TODO: Initialize to an appropriate value
		//    bool allDirectories = false; // TODO: Initialize to an appropriate value
		//    XPathNodeIterator expected = null; // TODO: Initialize to an appropriate value
		//    XPathNodeIterator actual;
		//    actual = IO.GetDirectories(path, searchPattern, allDirectories);
		//    Assert.AreEqual(expected, actual);
		//    Assert.Inconclusive("Verify the correctness of this test method.");
		//}

		//[TestMethod()]
		//public void GetDirectoryNameTest()
		//{
		//    string path = string.Empty; // TODO: Initialize to an appropriate value
		//    string expected = string.Empty; // TODO: Initialize to an appropriate value
		//    string actual;
		//    actual = IO.GetDirectoryName(path);
		//    Assert.AreEqual(expected, actual);
		//    Assert.Inconclusive("Verify the correctness of this test method.");
		//}

		//[TestMethod()]
		//public void GetExtensionTest()
		//{
		//    string path = string.Empty; // TODO: Initialize to an appropriate value
		//    string expected = string.Empty; // TODO: Initialize to an appropriate value
		//    string actual;
		//    actual = IO.GetExtension(path);
		//    Assert.AreEqual(expected, actual);
		//    Assert.Inconclusive("Verify the correctness of this test method.");
		//}

		//[TestMethod()]
		//public void GetFileNameTest()
		//{
		//    string path = string.Empty; // TODO: Initialize to an appropriate value
		//    string expected = string.Empty; // TODO: Initialize to an appropriate value
		//    string actual;
		//    actual = IO.GetFileName(path);
		//    Assert.AreEqual(expected, actual);
		//    Assert.Inconclusive("Verify the correctness of this test method.");
		//}

		//[TestMethod()]
		//public void GetFileNameWithoutExtensionTest()
		//{
		//    string path = string.Empty; // TODO: Initialize to an appropriate value
		//    string expected = string.Empty; // TODO: Initialize to an appropriate value
		//    string actual;
		//    actual = IO.GetFileNameWithoutExtension(path);
		//    Assert.AreEqual(expected, actual);
		//    Assert.Inconclusive("Verify the correctness of this test method.");
		//}

		//[TestMethod()]
		//public void GetFileSizeTest()
		//{
		//    string path = string.Empty; // TODO: Initialize to an appropriate value
		//    long expected = 0; // TODO: Initialize to an appropriate value
		//    long actual;
		//    actual = IO.GetFileSize(path);
		//    Assert.AreEqual(expected, actual);
		//    Assert.Inconclusive("Verify the correctness of this test method.");
		//}

		[TestMethod()]
		public void GetFilesTest()
		{
			var path = string.Concat(Environment.CurrentDirectory, @"\..\..\..");
			var searchPattern = "*.txt";
			var allDirectories = false;

			var xml = new XmlDocument();
			xml.LoadXml(string.Format(@"<Files><File>{0}\MIT-LICENSE.txt</File></Files>", path));

			var expected = xml.CreateNavigator().Select("/Files");
			var actual = IO.GetFiles(path, searchPattern, allDirectories);

			// move to first XML node - <Files>
			if (actual.MoveNext() && expected.MoveNext())
			{
				// select child nodes
				var actualValues = actual.Current.SelectChildren(XPathNodeType.Element);
				var expectedValues = expected.Current.SelectChildren(XPathNodeType.Element);

				// iterate over each of the child nodes - <value>
				while (actualValues.MoveNext() && expectedValues.MoveNext())
				{
					Assert.AreEqual(expectedValues.Current.Name, actualValues.Current.Name, "Should return true if the XML element names are the same");
					Assert.AreEqual(expectedValues.Current.Value, actualValues.Current.Value, "Should return true if the XML element values are the same");
				}
			}
		}

		//[TestMethod()]
		//public void LoadFileTest()
		//{
		//    string filepath = string.Empty; // TODO: Initialize to an appropriate value
		//    string expected = string.Empty; // TODO: Initialize to an appropriate value
		//    string actual;
		//    actual = IO.LoadFile(filepath);
		//    Assert.AreEqual(expected, actual);
		//    Assert.Inconclusive("Inconclusive. Underlying method/test relies on HttpContext.");
		//}

		//[TestMethod()]
		//public void MapPathTest()
		//{
		//    var useHttpContext = false;
		//    var path = @"~/uComponents.Core/Resources/Images/favicon.ico";
		//    var expected = @"C:\SVN\our.umbraco.org\uComponents\uComponents.Core\Resources\Images\favicon.ico";
		//    var actual = IO.MapPath(path, useHttpContext);
		//    Assert.AreEqual(expected, actual);
		//    Assert.Inconclusive("Inconclusive. Underlying method/test relies on HttpContext.");
		//}

		[TestMethod()]
		public void PathShortenerTest()
		{
			var input = @"C:\SVN\our.umbraco.org\uComponents\uComponents.Core\Properties\AssemblyInfo.cs";
			var expected = @"C:\SVN\our.umbraco.org\...\Properties\AssemblyInfo.cs";
			var actual = IO.PathShortener(input);
			Assert.AreEqual(expected, actual);
		}
	}
}
