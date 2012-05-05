using System.ComponentModel;
using umbraco.cms.businesslogic.datatype;

namespace uComponents.DataTypes.JsonDropdown
{
	/// <summary>
	/// The options for the JsonDropDown data-type.
	/// </summary>
	public class JD_Options : AbstractOptions
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="JD_Options"/> class.
		/// </summary>
		/// <param name="loadDefaults">if set to <c>true</c> [loads defaults].</param>
		public JD_Options(bool loadDefaults)
			: base(loadDefaults)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JD_Options"/> class.
		/// </summary>
		public JD_Options()
			: base()
		{
		}

		/// <summary>
		/// Gets or sets the background-color for the 'off' state.
		/// </summary>
		/// <value>The background-color for the 'off' state.</value>
		[DefaultValue("https://raw.github.com/gist/1401077/e52262286efcb14b43d88017e8fba81a5eb78e6b/cakes.json")]
		public string UrlToJson { get; set; }

		/// <summary>
		/// Gets or sets the label text for the 'Jquery Expression' state.
		/// </summary>
		/// <value>The label text for the 'jquery expression' state.</value>
		[DefaultValue("[0].topping")]
		public string Expression { get; set; }

		/// <summary>
		/// Gets or sets the key to retrieve and set to the dropdownlist value.
		/// </summary>
		/// <value>The label text for the 'value' of the option in dropdown list.</value>
		[DefaultValue("type")]
		public string Value { get; set; }

		/// <summary>
		/// Gets or sets the key to retrieve and set to the dropdownlist key.
		/// </summary>
		/// <value>The label text for the 'key' of the option in dropdown list.</value>
		[DefaultValue("id")]
		public string Key { get; set; }
	}
}