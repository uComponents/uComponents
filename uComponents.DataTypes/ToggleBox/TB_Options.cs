using System.ComponentModel;

using uComponents.DataTypes.Shared.PrevalueEditors;

namespace uComponents.DataTypes.ToggleBox
{
	/// <summary>
	/// The options for the ToggleBox data-type.
	/// </summary>
	public class TB_Options : AbstractOptions
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TB_Options"/> class.
		/// </summary>
		public TB_Options()
			: base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TB_Options"/> class.
		/// </summary>
		/// <param name="loadDefaults">if set to <c>true</c> [loads defaults].</param>
		public TB_Options(bool loadDefaults)
			: base(loadDefaults)
		{
		}

		/// <summary>
		/// Gets or sets a value indicating whether [default value].
		/// </summary>
		/// <value><c>true</c> if [default value]; otherwise, <c>false</c>.</value>
		[DefaultValue(false)]
		public bool DefaultValue { get; set; }

		/// <summary>
		/// Gets or sets the background-color for the 'off' state.
		/// </summary>
		/// <value>The background-color for the 'off' state.</value>
		[DefaultValue("#F8837C")]
		public string OffBackgroundColor { get; set; }

		/// <summary>
		/// Gets or sets the label text for the 'off' state.
		/// </summary>
		/// <value>The label text for the 'off' state.</value>
		[DefaultValue("Off")]
		public string OffText { get; set; }

		/// <summary>
		/// Gets or sets the background-color for the 'on' state.
		/// </summary>
		/// <value>The background-color for the 'on' state</value>
		[DefaultValue("#8FE38D")]
		public string OnBackgroundColor { get; set; }

		/// <summary>
		/// Gets or sets the label text for the 'on' state.
		/// </summary>
		/// <value>The label text for the 'on' state.</value>
		[DefaultValue("On")]
		public string OnText { get; set; }
	}
}
