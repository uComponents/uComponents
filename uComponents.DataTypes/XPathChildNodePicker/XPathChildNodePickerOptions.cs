using System.ComponentModel;
using umbraco.editorControls;

namespace uComponents.DataTypes.XPathChildNodePicker
{
	/// <summary>
	/// Data class, used to store the configuration options for the XPathChildNodePickerPrevalueEditor.
	/// </summary>
	public class XPathChildNodePickerOptions : AbstractOptions
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XPathChildNodePickerOptions"/> class.
		/// </summary>
		public XPathChildNodePickerOptions()
		{
			this.XPath = string.Empty;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XPathChildNodePickerOptions"/> class.
		/// </summary>
		/// <param name="loadDefaults">if set to <c>true</c> [load defaults].</param>
		public XPathChildNodePickerOptions(bool loadDefaults)
			: base(loadDefaults)
		{
		}

		/// <summary>
		/// XPath string used to get Nodes to be used as CheckBox options in a CheckBoxList
		/// </summary>
		[DefaultValue("")]
		public string XPath { get; set; }

		/// <summary>
		/// Gets or sets the ListControl type format.
		/// </summary>
		/// <value>The output format.</value>
		[DefaultValue(ListControlTypes.CheckBoxList)]
		public ListControlTypes ListControlType { get; set; }

		/// <summary>
		/// The types of ListControl for the data-type.
		/// </summary>
		public enum ListControlTypes
		{
			/// <summary>
			/// Renders as a CheckBoxList.
			/// </summary>
			CheckBoxList = 0,

			/// <summary>
			/// Renders as a DropDownList.
			/// </summary>
			DropDownList = 1,

			/// <summary>
			/// Renders as a RadioButtonList.
			/// </summary>
			RadioButtonList = 2
		}
	}
}
