using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Xml;
using uComponents.Core;
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
		/// Field for the options.
		/// </summary>
		private TextstringArrayOptions options;

		/// <summary>
		/// Initializes a new instance of the <see cref="TextstringArrayData"/> class.
		/// </summary>
		/// <param name="dataType">Type of the data.</param>
		public TextstringArrayData(BaseDataType dataType, TextstringArrayOptions options)
			: base(dataType)
		{
			this.options = options;
		}

		/// <summary>
		/// Converts the data value to XML.
		/// </summary>
		/// <param name="data">The data to convert to XML.</param>
		/// <returns>Returns the XML node for the data-type's value.</returns>
		public override XmlNode ToXMl(XmlDocument data)
		{
			// check that the value isn't null
			if (this.Value != null)
			{
				var value = this.Value.ToString();
				var xd = new XmlDocument();

				// if the value is coming from a translation task, it will always be XML
				if (Helper.Xml.CouldItBeXml(value))
				{
					xd.LoadXml(value);
					return data.ImportNode(xd.DocumentElement, true);
				}

				// load the values into a string array/list.
				var deserializer = new JavaScriptSerializer();
				var values = deserializer.Deserialize<List<string[]>>(value);

				if (values != null && values.Count > 0)
				{
					// load the values into an XML document.
					xd.LoadXml("<TextstringArray/>");

					// load the config options
					if (this.options.ShowColumnLabels && !string.IsNullOrWhiteSpace(this.options.ColumnLabels))
					{
						var xlabels = xd.CreateElement("labels");

						// loop through the labels.
						foreach (var label in this.options.ColumnLabels.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
						{
							// add each label to the XML document.
							var xlabel = XmlHelper.AddTextNode(xd, "label", label);
							xlabels.AppendChild(xlabel);
						}

						xd.DocumentElement.AppendChild(xlabels);
					}

					// loop through the list/array items.
					foreach (var row in values)
					{
						// add each row to the XML document.
						var xrow = xd.CreateElement("values");

						foreach (var item in row)
						{
							// add each value to the XML document.
							var xvalue = XmlHelper.AddTextNode(xd, "value", item);
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