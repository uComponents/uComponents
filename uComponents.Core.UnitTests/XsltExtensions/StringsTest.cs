using uComponents.Core.XsltExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.XPath;
using System.Xml;

namespace uComponents.Core.UnitTests.XsltExtensions
{
	[TestClass()]
	public class StringsTest
	{
		private TestContext testContextInstance;

		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		[TestMethod]
		public void CoalesceTest()
		{
			var arg1 = string.Empty;
			var arg2 = "hello";
			var arg3 = "world";
			var expected = "hello";
			var actual = Strings.Coalesce(arg1, arg2, arg3);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void CoalesceTest1()
		{
			var arg1 = string.Empty;
			var arg2 = "hello";
			var arg3 = "world";
			var arg4 = "foo";
			var expected = "hello";
			var actual = Strings.Coalesce(arg1, arg2, arg3, arg4);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void CoalesceTest2()
		{
			var arg1 = string.Empty;
			var arg2 = "hello";
			var arg3 = "world";
			var arg4 = "foo";
			var arg5 = string.Empty;
			var expected = "hello";
			var actual = Strings.Coalesce(arg1, arg2, arg3, arg4, arg5);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void CoalesceTest3()
		{
			var arg1 = string.Empty;
			var arg2 = "hello";
			var expected = "hello";
			var actual = Strings.Coalesce(arg1, arg2);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ConcatTest()
		{
			var xml = new XmlDocument();
			xml.LoadXml("<values><value>hello</value><value>world</value><value>foo</value><value>bar</value></values>");

			var nodeset = xml.CreateNavigator().Select("/values/value");
			var separator = ",";
			var expected = "hello,world,foo,bar";
			var actual = Strings.Concat(nodeset, separator);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void FormatTest()
		{
			var format = "{0} {1} {2} {3} {4}";
			var arg1 = "hello";
			var arg2 = "world";
			var arg3 = "foo";
			var arg4 = "bar";
			var arg5 = "nom";
			var expected = "hello world foo bar nom";
			var actual = Strings.Format(format, arg1, arg2, arg3, arg4, arg5);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void FormatTest1()
		{
			var format = "{0} {1}";
			var arg1 = "hello";
			var arg2 = "world";
			var expected = "hello world";
			var actual = Strings.Format(format, arg1, arg2);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void FormatTest2()
		{
			var format = "{0} {1} {2} {3}";
			var arg1 = "hello";
			var arg2 = "world";
			var arg3 = "foo";
			var arg4 = "bar";
			var expected = "hello world foo bar";
			var actual = Strings.Format(format, arg1, arg2, arg3, arg4);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void FormatTest3()
		{
			var format = "{0}";
			var arg1 = "hello";
			var expected = "hello";
			var actual = Strings.Format(format, arg1);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void FormatTest4()
		{
			var format = "{0} {1} {2}";
			var arg1 = "hello";
			var arg2 = "world";
			var arg3 = "foo";
			var expected = "hello world foo";
			var actual = Strings.Format(format, arg1, arg2, arg3);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void FromBase64StringTest()
		{
			var input = "aGVsbG8gd29ybGQgZm9vIGJhcg==";
			var expected = "hello world foo bar";
			var actual = Strings.FromBase64String(input);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void GetFirstWordsTest()
		{
			var text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque luctus urna a lorem consectetur eget cursus sapien porttitor.";
			var count = 10;
			var expected = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque luctus&#8230;";
			var actual = Strings.GetFirstWords(text, count);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void GetFirstWordsTest1()
		{
			var text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque luctus urna a lorem consectetur eget cursus sapien porttitor.";
			var count = 10;
			var appender = "...";
			var expected = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque luctus...";
			var actual = Strings.GetFirstWords(text, count, appender);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void LowerCaseTagsTest()
		{
			var input = "<FONT>Hello world, how are <U>you</U> today?</FONT>";
			var expected = "<font>Hello world, how are <u>you</u> today?</font>";
			var actual = Strings.LowerCaseTags(input);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void MakeEmailHyperlinkTest()
		{
			var email = "leekelleher@gmail.com";
			var expected = "<a href=\"mailto:leekelleher@gmail.com\">leekelleher@gmail.com</a>";
			var actual = Strings.MakeEmailHyperlink(email);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void MakeEmailHyperlinkTest1()
		{
			var email = "leekelleher@gmail.com";
			var text = string.Empty;
			var expected = "<a href=\"mailto:leekelleher@gmail.com\">leekelleher@gmail.com</a>";
			var actual = Strings.MakeEmailHyperlink(email, text);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void MakeUrlHyperlinkTest()
		{
			var input = "http://our.umbraco.org/";
			var expected = "<a href=\"http://our.umbraco.org/\" target=\"_blank\">http://our.umbraco.org/</a>";
			var actual = Strings.MakeUrlHyperlink(input);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void RemoveCharsTest()
		{
			var input = "hello! 'world' #foo ~bar";
			var expected = "helloworldfoobar";
			var actual = Strings.RemoveChars(input);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void RemoveDuplicateEntriesTest()
		{
			var list = "1111,2222,3333,2222,4444,1111";
			var separators = new[] { ',' };
			var expected = "1111,2222,3333,4444";
			var actual = Strings.RemoveDuplicateEntries(list, separators);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void RemoveDuplicateEntriesTest1()
		{
			var list = "1111,2222,3333,2222,4444,1111";
			var separator = ",";
			var expected = "1111,2222,3333,4444";
			var actual = Strings.RemoveDuplicateEntries(list, separator);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void RemoveDuplicateEntriesTest2()
		{
			var list = "1111,2222,3333,2222,4444,1111";
			var expected = "1111,2222,3333,4444";
			var actual = Strings.RemoveDuplicateEntries(list);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void RemoveEmptyEntriesTest()
		{
			var list = "1111,,2222,3333,,4444,";
			var separators = new[] { ',' };
			var expected = "1111,2222,3333,4444";
			var actual = Strings.RemoveEmptyEntries(list, separators);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void RemoveEmptyEntriesTest1()
		{
			var list = "1111,,2222,3333,,4444,";
			var separator = ",";
			var expected = "1111,2222,3333,4444";
			var actual = Strings.RemoveEmptyEntries(list, separator);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void RemoveEmptyEntriesTest2()
		{
			var list = "1111,,2222,3333,,4444,";
			var expected = "1111,2222,3333,4444";
			var actual = Strings.RemoveEmptyEntries(list);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ReverseTest()
		{
			var input = "hello world";
			var expected = "dlrow olleh";
			var actual = Strings.Reverse(input);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void SafeAliasTest()
		{
			var input = "Hello World Foo Bar";
			var expected = "HelloWorldFooBar";
			var actual = Strings.SafeAlias(input);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void SingularPluralTest()
		{
			var count = 5;
			var singular = "item";
			var plural = "items";
			var prefixCount = true;
			var expected = "5 items";
			var actual = Strings.SingularPlural(count, singular, plural, prefixCount);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void SingularPluralTest1()
		{
			var count = 5;
			var singular = "item";
			var plural = "items";
			var expected = "items";
			var actual = Strings.SingularPlural(count, singular, plural);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void SpaceOutCamelCaseTest()
		{
			var input = "HelloWorldFooBar";
			var expected = "Hello World Foo Bar";
			var actual = Strings.SpaceOutCamelCase(input);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void StripFontTagsTest()
		{
			var input = "<FONT><font face='Ariel'>Hello world</font>, how are <U>you</U> today?</FONT>";
			var expected = "Hello world, how are <U>you</U> today?";
			var actual = Strings.StripFontTags(input);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void StripHTMLTest()
		{
			var input = "<FONT><font face='Ariel'>Hello world</font>, how are <U>you</U> today?</FONT>";
			var expected = "Hello world, how are you today?";
			var actual = Strings.StripHTML(input);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void StripLineBreaksTest()
		{
			var input = @"hello
world
foo
bar";
			var expected = "helloworldfoobar";
			var actual = Strings.StripLineBreaks(input);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void StripNonAlphaNumericTest()
		{
			var input = "hello !£$%^&*()_+= world 12";
			var expected = "helloworld12";
			var actual = Strings.StripNonAlphaNumeric(input);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void StripWhitespaceTest()
		{
			var input = "hello	world   foo		bar";
			var expected = "helloworldfoobar";
			var actual = Strings.StripWhitespace(input);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ToBase64StringTest()
		{
			var input = "hello world foo bar";
			var expected = "aGVsbG8gd29ybGQgZm9vIGJhcg==";
			var actual = Strings.ToBase64String(input);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ToCamelCaseTest()
		{
			var input = "Hello World Foo Bar";
			var expected = "helloWorldFooBar";
			var actual = Strings.ToCamelCase(input);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ToLowerCaseTest()
		{
			var input = "HeLlO wOrLd";
			var expected = "hello world";
			var actual = Strings.ToLowerCase(input);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ToProperCaseTest()
		{
			var input = "HeLlO wOrLd";
			var expected = "Hello world";
			var actual = Strings.ToProperCase(input);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ToTitleCaseTest()
		{
			var input = "HeLlO wOrLd";
			var expected = "Hello World";
			var actual = Strings.ToTitleCase(input);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ToUpperCaseTest()
		{
			var input = "HeLlO wOrLd";
			var expected = "HELLO WORLD";
			var actual = Strings.ToUpperCase(input);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void TrimStringFromEndTest()
		{
			var input = ".....hello world.....";
			var textToTrim = ".";
			var expected = ".....hello world";
			var actual = Strings.TrimStringFromEnd(input, textToTrim);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void TrimStringFromStartTest()
		{
			var input = ".....hello world.....";
			var textToTrim = ".";
			var expected = "hello world.....";
			var actual = Strings.TrimStringFromStart(input, textToTrim);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void TruncateInnerTest()
		{
			var input = "Hello world, how are you today?";
			var maxLength = 15;
			var expected = "Hello...today?";
			var actual = Strings.TruncateInner(input, maxLength);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void WordCountTest()
		{
			var input = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque luctus urna a lorem consectetur eget cursus sapien porttitor.";
			var expected = 18;
			var actual = Strings.WordCount(input);
			Assert.AreEqual(expected, actual);
		}
	}
}
