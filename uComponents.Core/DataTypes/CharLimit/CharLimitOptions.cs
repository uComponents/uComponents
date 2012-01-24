using System;
using System.ComponentModel;
using System.Web.UI.WebControls;

using uComponents.Core.Shared.PrevalueEditors;

namespace uComponents.Core.DataTypes.CharLimit
{
	/// <summary>
	/// The options for the CharLimit data-type.
	/// </summary>
	public class CharLimitOptions : AbstractOptions
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CharLimitOptions"/> class.
		/// </summary>
		public CharLimitOptions()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CharLimitOptions"/> class.
		/// </summary>
		/// <param name="loadDefaults">if set to <c>true</c> [load defaults].</param>
		public CharLimitOptions(bool loadDefaults)
			: base(loadDefaults)
		{
		}

		/// <summary>
		/// Gets or sets the limit.
		/// </summary>
		/// <value>The limit.</value>
		[DefaultValue(100)]
		public int Limit { get; set; }

		/// <summary>
		/// Gets or sets the text box mode.
		/// </summary>
		/// <value>The text box mode.</value>
		[DefaultValue(TextBoxMode.SingleLine)]
		public TextBoxMode TextBoxMode { get; set; }
	}
}
