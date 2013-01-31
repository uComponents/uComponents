// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 11.01.2011 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.UI;

namespace uComponents.DataTypes.DataTypeGrid.Model
{
	using System.Collections.Generic;

	public class PreValueRow : BasePreValueRow
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PreValueRow"/> class.
		/// </summary>
		public PreValueRow()
		{
			this.Controls = new List<Control>();
		}

		/// <summary>
		/// Gets or sets the controls.
		/// </summary>
		/// <value>The controls.</value>
		public IList<Control> Controls { get; set; }
	}
}