using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using uComponents.XsltExtensions;

namespace uComponents.Core.UnitTests.XsltExtensions
{
	[TestClass]
	public class StringsTest
	{
		[TestMethod]
		public void CoalesceTests()
		{
			var first = "first";
			var second = "second";
			var third = "third";
			var forth = "forth";
			var fifth = "fifth";

			Assert.AreEqual(Strings.Coalesce(null, null), null, "Should return null if all parameters are null");
			Assert.AreEqual(Strings.Coalesce(first, second), first, "Should return first value if all strings are not null");
			Assert.AreEqual(Strings.Coalesce(null, second), second, "Should return second value if first parameter is null");
			Assert.AreEqual(Strings.Coalesce(string.Empty, second), second, "Should return second value if first parameter is an empty string");
			Assert.AreEqual(Strings.Coalesce(null, null, third), third, "Should return third parameters if previous are null");
			Assert.AreEqual(Strings.Coalesce(null, null, null, forth), forth, "Should return forth parameters if previous are null");
			Assert.AreEqual(Strings.Coalesce(null, null, null, null, fifth), fifth, "Should return fifth parameters if previous are null");
			Assert.AreEqual(Strings.Coalesce(string.Empty, string.Empty, string.Empty, string.Empty, fifth), fifth, "Should return fifth parameters if previous are emptry strings");
			Assert.AreEqual(Strings.Coalesce(null, second, null), second, "Should return second parameters even if followed by nulls");
			Assert.AreEqual(Strings.Coalesce(null, second, "third"), second, "Should return second parameters as its the first non-null even if followed by valid strings");
			Assert.AreEqual(Strings.Coalesce(first, second, third, forth, fifth), first, "Should return first parameter as its the first non-null even if followed by valid strings");
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
			Assert.AreEqual(expected, actual, "Should return the text values from the XML (string) joined together by commas");
		}

		[TestMethod]
		public void FormatTest_With1Argument()
		{
			var format = "{0}";
			var arg1 = "hello";
			var expected = "hello";
			var actual = Strings.Format(format, arg1);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void FormatTest_With2Arguments()
		{
			var format = "{0} {1}";
			var arg1 = "hello";
			var arg2 = "world";
			var expected = "hello world";
			var actual = Strings.Format(format, arg1, arg2);
			Assert.AreEqual(expected, actual, "Should return the strings in the appropriate format - space separated");
		}

		[TestMethod]
		public void FormatTest_With3Arguments()
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
		public void FormatTest_With4Arguments()
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
		public void FormatTest_With5Arguments()
		{
			var format = "{0} {1} {2} {3} {4}";
			var arg1 = "hello";
			var arg2 = "world";
			var arg3 = "foo";
			var arg4 = "bar";
			var arg5 = "nom";
			var expected = "hello world foo bar nom";
			var actual = Strings.Format(format, arg1, arg2, arg3, arg4, arg5);
			Assert.AreEqual(expected, actual, "Should return the strings in the appropriate format - space separated");
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
		public void GetFirstWordsTest_WithCustomAppender()
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

			Assert.AreEqual(expected, actual, "Should return the HTML string with all the HTML tags in lowercase");
		}

		[TestMethod]
		public void MakeEmailHyperlinkTest()
		{
			var email = "leekelleher@gmail.com";
			var expected = "<a href=\"mailto:leekelleher@gmail.com\">leekelleher@gmail.com</a>";
			var actual = Strings.MakeEmailHyperlink(email);

			Assert.AreEqual(expected, actual, "Should return a HTML 'mailto' anchor tag for the specified email address");
		}

		[TestMethod]
		public void MakeEmailHyperlinkTest_WithCustomText()
		{
			var email = "leekelleher@gmail.com";
			var text = string.Empty;
			var expected = "<a href=\"mailto:leekelleher@gmail.com\">leekelleher@gmail.com</a>";
			var actual = Strings.MakeEmailHyperlink(email, text);

			Assert.AreEqual(expected, actual, "Should return a HTML 'mailto' anchor tag for the specified email address");
		}

		[TestMethod]
		public void MakeUrlHyperlinkTest()
		{
			var input = "http://our.umbraco.org/";
			var expected = "<a href=\"http://our.umbraco.org/\" target=\"_blank\">http://our.umbraco.org/</a>";
			var actual = Strings.MakeUrlHyperlink(input);
			Assert.AreEqual(expected, actual, "Should return a HTML anchor tag for the specified URL");
		}

		[TestMethod]
		public void RemoveCharsTest()
		{
			var input = "hello! 'world' #foo ~bar";
			var expected = "helloworldfoobar";
			var actual = Strings.RemoveChars(input);
			Assert.AreEqual(expected, actual, "Should return the specified string with any non-alphanumeric characters removed");
		}

		[TestMethod]
		public void RemoveDuplicateEntriesTest()
		{
			var list = "1111,2222,3333,2222,4444,1111";
			var separator = ",";
			var expected = "1111,2222,3333,4444";

			var actual = Strings.RemoveDuplicateEntries(list);
			Assert.AreEqual(expected, actual, "Should return a unique list of comma-separated values - no separator specified");

			var actual2 = Strings.RemoveDuplicateEntries(list, separator);
			Assert.AreEqual(expected, actual2, "Should return a unique list of comma-separated values - single comma separated specified");

			var actual3 = Strings.RemoveDuplicateEntries(list, separator.ToCharArray());
			Assert.AreEqual(expected, actual3, "Should return a unique list of comma-separated values - single comma separator (in array) specified");

			Assert.IsTrue(actual == actual2 && actual2 == actual3, "All 3 actual values should be equal");
		}

		[TestMethod]
		public void RemoveEmptyEntriesTest_WithCharArraySeparator()
		{
			var list = "1111,,2222,3333,,4444,";
			var separators = new[] { ',' };
			var expected = "1111,2222,3333,4444";
			var actual = Strings.RemoveEmptyEntries(list, separators);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void RemoveEmptyEntriesTest_WithDefaultSeparator()
		{
			var list = "1111,,2222,3333,,4444,";
			var expected = "1111,2222,3333,4444";
			var actual = Strings.RemoveEmptyEntries(list);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void RemoveEmptyEntriesTest_WithStringSeparator()
		{
			var list = "1111,,2222,3333,,4444,";
			var separator = ",";
			var expected = "1111,2222,3333,4444";
			var actual = Strings.RemoveEmptyEntries(list, separator);
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
			var expected = "items";
			var actual = Strings.SingularPlural(count, singular, plural);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void SingularPluralTest_WithPrefixCount()
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
			Assert.AreEqual(expected, actual, "Should return the HTML string with the <font> tags removed");
		}

		[TestMethod]
		public void StripHTMLTest()
		{
			var input = "<FONT><font face='Ariel'>Hello world</font>, how are <U>you</U> today?</FONT>";
			var expected = "Hello world, how are you today?";
			var actual = Strings.StripHTML(input);
			Assert.AreEqual(expected, actual, "Should return a plain-text version of the specified HTML string input");
		}

//		[TestMethod]
//		public void StripLineBreaksTest()
//		{
//			var input = @"hello
//world
//foo
//bar";
//			var expected = "helloworldfoobar";
//			var actual = Strings.StripLineBreaks(input);
//			Assert.AreEqual(expected, actual);
//		}

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
