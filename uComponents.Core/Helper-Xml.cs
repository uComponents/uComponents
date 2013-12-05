using System;
using System.Xml;
using umbraco;

namespace uComponents.Core
{
	/// <summary>
	/// Generic helper methods
	/// </summary>
	internal static partial class Helper
	{
		/// <summary>
		/// Xml helper
		/// </summary>
		public static class Xml
		{
			/// <summary>
			/// Determines whether the specified string appears to be XML.
			/// </summary>
			/// <param name="xml">The XML string.</param>
			/// <returns>
			/// 	<c>true</c> if the specified string appears to be XML; otherwise, <c>false</c>.
			/// </returns>
			public static bool CouldItBeXml(string xml)
			{
				if (!string.IsNullOrEmpty(xml))
				{
					xml = xml.Trim();

					if (xml.StartsWith("<") && xml.EndsWith(">"))
					{
						return true;
					}
				}

				return false;
			}

			/// <summary>
			/// Splits the specified delimited string into an XML document.
			/// </summary>
			/// <param name="data">The data.</param>
			/// <param name="separator">The separator.</param>
			/// <param name="rootName">Name of the root.</param>
			/// <param name="elementName">Name of the element.</param>
			/// <returns>Returns an <c>System.Xml.XmlDocument</c> representation of the delimited string data.</returns>
			public static XmlDocument Split(string data, string[] separator, string rootName, string elementName)
			{
				return Split(new XmlDocument(), data, separator, rootName, elementName);
			}

			/// <summary>
			/// Splits the specified delimited string into an XML document.
			/// </summary>
			/// <param name="xml">The XML document.</param>
			/// <param name="data">The delimited string data.</param>
			/// <param name="separator">The separator.</param>
			/// <param name="rootName">Name of the root node.</param>
			/// <param name="elementName">Name of the element node.</param>
			/// <returns>Returns an <c>System.Xml.XmlDocument</c> representation of the delimited string data.</returns>
			public static XmlDocument Split(XmlDocument xml, string data, string[] separator, string rootName, string elementName)
			{
				// load new XML document.
				xml.LoadXml(string.Concat("<", rootName, "/>"));

				// get the data-value, check it isn't empty.
				if (!string.IsNullOrEmpty(data))
				{
					// explode the values into an array
					var values = data.Split(separator, StringSplitOptions.None);

					// loop through the array items.
					foreach (string value in values)
					{
						// add each value to the XML document.
						var xn = xmlHelper.addTextNode(xml, elementName, value);
						xml.DocumentElement.AppendChild(xn);
					}
				}

				// return the XML node.
				return xml;
			}
		}
	}
}