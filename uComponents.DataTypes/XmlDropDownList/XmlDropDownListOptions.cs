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

		[DefaultValue("")]
		public string XmlFilePath { get; set; }

		[DefaultValue("")]
		public string XPathExpression { get; set; }

		[DefaultValue("")]
		public string TextColumn { get; set; }

		[DefaultValue("")]
		public string ValueColumn { get; set; }
	}
}
