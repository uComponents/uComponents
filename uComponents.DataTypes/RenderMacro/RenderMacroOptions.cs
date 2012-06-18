using System.ComponentModel;
using umbraco.editorControls;

namespace uComponents.DataTypes.RenderMacro
{
	/// <summary>
	/// The options for the RenderMacro data-type.
	/// </summary>
	public class RenderMacroOptions : AbstractOptions
	{
		/// <summary>
		/// Value for the default macro tag.
		/// </summary>
		public const string DEFAULT_MACRO_TAG = "<?UMBRACO_MACRO macroAlias=\"\" />";

		/// <summary>
		/// Initializes a new instance of the <see cref="RenderMacroOptions"/> class.
		/// </summary>
		public RenderMacroOptions()
			: base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RenderMacroOptions"/> class.
		/// </summary>
		/// <param name="loadDefaults">if set to <c>true</c> [load defaults].</param>
		public RenderMacroOptions(bool loadDefaults)
			: base(loadDefaults)
		{
		}

		/// <summary>
		/// Gets or sets a value indicating whether [show label].
		/// </summary>
		/// <value><c>true</c> if [show label]; otherwise, <c>false</c>.</value>
		[DefaultValue(true)]
		public bool ShowLabel { get; set; }

		/// <summary>
		/// Gets or sets the macro tag.
		/// </summary>
		/// <value>The macro tag.</value>
		[DefaultValue(DEFAULT_MACRO_TAG)]
		public string MacroTag { get; set; }
	}
}
