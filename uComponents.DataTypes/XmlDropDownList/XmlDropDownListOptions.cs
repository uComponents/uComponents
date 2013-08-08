using System.ComponentModel;
using umbraco.editorControls;

namespace uComponents.DataTypes.XmlDropDownList
{
	/// <summary>
	/// The options for the Multiple Textstring data-type.
	/// </summary>
	public class XmlDropDownListOptions : AbstractOptions
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XmlDropDownListOptions"/> class.
		/// </summary>
		public XmlDropDownListOptions()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlDropDownListOptions"/> class.
		/// </summary>
		/// <param name="loadDefaults">if set to <c>true</c> [load defaults].</param>
		public XmlDropDownListOptions(bool loadDefaults)
			: base(loadDefaults)
		{
		}

		/// <summary>
		/// Gets or sets the XML file path.
		/// </summary>
		/// <value>The XML file path.</value>
		[DefaultValue("")]
		public string XmlFilePath { get; set; }

		/// <summary>
		/// Gets or sets the XPath expression.
		/// </summary>
		/// <value>The XPath expression.</value>
		[DefaultValue("")]
		public string XPathExpression { get; set; }

		/// <summary>
		/// Gets or sets the text column.
		/// </summary>
		/// <value>The text column.</value>
		[DefaultValue("")]
		public string TextColumn { get; set; }

		/// <summary>
		/// Gets or sets the value column.
		/// </summary>
		/// <value>The value column.</value>
		[DefaultValue("")]
		public string ValueColumn { get; set; }
	}
}
