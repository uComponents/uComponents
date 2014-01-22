using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.XPath;
using uComponents.Core;
using uComponents.DataTypes.Shared.Enums;
using umbraco;
using Umbraco.Core;

namespace uComponents.XsltExtensions
{
	/// <summary>
	/// The Enums class exposes XSLT extensions to access data from EnumCheckBoxListAttributes and EnumDropDownListAttributes
	/// </summary>
	[XsltExtension("ucomponents.enums")]
	public class Enums
	{

		/// <summary>
		/// Gets the Enum of the supplied type and returns its attribute values as a ListItem
		/// </summary>
		/// <param name="enumFullTypeName">The full type name of the enum to parse</param>
		/// <returns>List of ListItems representing the Enum's attribute values</returns>
		public static XPathNodeIterator GetEnumListAttributeValues(string assemblyName, string typeName)
		{
			if (!assemblyName.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
				assemblyName += ".dll";

			var assembly = Helper.IO.GetAssembly(assemblyName);
			var type = assembly.GetType(typeName);
			var enumListItems= EnumHelper.GetEnumListAttributeValues(type);

			// create the XML document
			var xd = new XmlDocument();
			xd.LoadXml("<ListItems/>");

			// loop through each of the directories
			foreach (var enumListItem in enumListItems)
			{
				var enumNode = Umbraco.Core.XmlHelper.AddTextNode(xd, "ListItem", "");
				enumNode.Attributes.Append(XmlHelper.AddAttribute(xd, "Text", enumListItem.Text));
				enumNode.Attributes.Append(XmlHelper.AddAttribute(xd, "Value", enumListItem.Value));
				enumNode.Attributes.Append(XmlHelper.AddAttribute(xd, "Enabled", enumListItem.Enabled.ToString()));

				// add the node to the XML document
				xd.DocumentElement.AppendChild(enumNode);
			}

			// return the XML document
			return xd.CreateNavigator().Select("/ListItems");
		}

	}
}
