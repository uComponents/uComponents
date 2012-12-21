using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Xml;
using umbraco.cms.businesslogic.datatype;
using Umbraco.Core;

namespace uComponents.DataTypes.TextstringArray
{
	/// <summary>
	/// Overrides the <see cref="umbraco.cms.businesslogic.datatype.DefaultData"/> object to return the value as XML.
	/// </summary>
	public class TextstringArrayData : DefaultData
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TextstringArrayData"/> class.
		/// </summary>
		/// <param name="dataType">Type of the data.</param>
		public TextstringArrayData(BaseDataType dataType)
			: base(dataType)
		{
		}
		
		/// <summary>
		/// Converts the data value to XML.
		/// </summary>
		/// <param name="data">The data to convert to XML.</param>
		/// <returns></returns>
		public override XmlNode ToXMl(XmlDocument data)
		{
			// check that the value isn't null
			if (this.Value != null)
			{
				// load the values into a string array/list.
				var deserializer = new JavaScriptSerializer();
				var values = deserializer.Deserialize<List<string[]>>(this.Value.ToString());

				if (values != null && values.Count>0)
				{
					// load the values into an XML document.
					var xd = new XmlDocument();
					xd.LoadXml("<TextstringArray/>");

					// loop through the list/array items.
					foreach (string[] row in values)
					{
						// add each row to the XML document.
						var xrow = xd.CreateElement("values");

						foreach (string value in row)
						{
							// add each value to the XML document.
							var xvalue = XmlHelper.AddTextNode(xd, "value", value);
							xrow.AppendChild(xvalue);
						}

						xd.DocumentElement.AppendChild(xrow);
					}

					// return the XML node.
					return data.ImportNode(xd.DocumentElement, true);
				}
			}

			// otherwise render the value as default (in CDATA)
			return base.ToXMl(data);
		}
	}
}
