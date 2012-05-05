using System;
using System.Collections;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace uComponents.XsltExtensions
{
	/// <summary>
	/// XSLT Extension methods for exceptions.
	/// </summary>
	internal static class Exceptions
	{
		/// <summary>
		/// Returns the Exception message as XML.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <returns>An XDocument of the Exception object.</returns>
		/// <remarks>
		/// http://stackoverflow.com/questions/486460/how-to-serialize-an-exception-object-in-c
		/// </remarks>
		internal static XDocument ToXml(this Exception exception)
		{
			// The root element is 'Exception' with the Type in an attribute.
			var root = new XElement("Exception", new XAttribute("Type", exception.GetType().ToString()));

			if (exception.Message != null)
			{
				root.Add(new XElement("Message", exception.Message));
			}

			if (exception.StackTrace != null)
			{
				root.Add
				(
					new XElement("StackTrace",
						from frame in exception.StackTrace.Split('\n')
						let prettierFrame = frame.Substring(6).Trim()
						select new XElement("Frame", prettierFrame))
				);
			}

			// Data is never null; it's empty if there is no data
			if (exception.Data.Count > 0)
			{
				root.Add
				(
					new XElement("Data",
						from entry in
							exception.Data.Cast<DictionaryEntry>()
						let key = entry.Key.ToString()
						let value = (entry.Value == null) ?
							"null" : entry.Value.ToString()
						select new XElement(key, value))
				);
			}

			// Add the InnerException if it exists
			if (exception.InnerException != null)
			{
				root.Add
				(
					exception.InnerException.ToXml()
				);
			}

			var doc = new XDocument();
			doc.Add(root);

			return doc;
		}

		/// <summary>
		/// Returns the Exception message as a XPathNodeIterator object.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <returns>
		/// An XPathNodeIterator instance of the Exception object.
		/// </returns>
		internal static XPathNodeIterator ToXPathNodeIterator(this Exception exception)
		{
			var doc = exception.ToXml();
			return doc.CreateNavigator().Select("/Exception");
		}
	}
}
