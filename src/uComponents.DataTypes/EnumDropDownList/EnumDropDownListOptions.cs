using System.ComponentModel;
using umbraco.editorControls;

namespace uComponents.DataTypes.EnumDropDownList
{
	/// <summary>
	/// The options for the EnumDropDownList data-type.
	/// </summary>
	public class EnumDropDownListOptions : AbstractOptions
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EnumDropDownListOptions"/> class.
		/// </summary>
		public EnumDropDownListOptions()
		{
			this.Assembly = string.Empty;
			this.Enum = string.Empty;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EnumDropDownListOptions"/> class.
		/// </summary>
		/// <param name="loadDefaults">if set to <c>true</c> [load defaults].</param>
		public EnumDropDownListOptions(bool loadDefaults)
			: base(loadDefaults)
		{
		}

		/// <summary>
		/// Assembly in which to look for an enum
		/// </summary>
		[DefaultValue("")]
		public string Assembly { get; set; }

		/// <summary>
		/// Enum to use as the values for the drop down list
		/// </summary>
		[DefaultValue("")]
		public string Enum { get; set; }

		/// <summary>
		/// If true, then the first item in the enum will be selected
		/// </summary>
		[DefaultValue(false)]
		public bool DefaultToFirstItem { get; set; }
	}
}