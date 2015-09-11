using System.ComponentModel;
using umbraco.editorControls;

namespace uComponents.DataTypes.ElasticTextBox
{
	/// <summary>
	/// The options for the Elastic TextBox data-type.
	/// </summary>
	public class ETB_Options : AbstractOptions
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ETB_Options"/> class.
		/// </summary>
		public ETB_Options()
			: base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ETB_Options"/> class.
		/// </summary>
		/// <param name="loadDefaults">if set to <c>true</c> [load defaults].</param>
		public ETB_Options(bool loadDefaults)
			: base(loadDefaults)
		{
		}

		/// <summary>
		/// Gets or sets the CSS styles.
		/// </summary>
		/// <value>The CSS styles.</value>
		[DefaultValue("color:#000;font-size:16px;font-weight:bold;font-family:sans-serif;padding:5px;")]
		public string Css { get; set; }

		/// <summary>
		/// Gets or sets the height.
		/// </summary>
		/// <value>The height.</value>
		[DefaultValue(400)]
		public int Height { get; set; }

		/// <summary>
		/// Gets or sets the width.
		/// </summary>
		/// <value>The width.</value>
		[DefaultValue(490)]
		public int Width { get; set; }
	}
}
