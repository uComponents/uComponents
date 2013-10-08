// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 11.01.2011 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.Script.Serialization;

namespace uComponents.DataTypes.DataTypeGrid.Model
{
    using System.ComponentModel;

    /// <summary>
	/// Base class for a Prevalue Row.
	/// </summary>
	public class BasePreValueRow
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BasePreValueRow"/> class.
		/// </summary>
		public BasePreValueRow()
		{
		    this.Visible = true;
		}

		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		[ScriptIgnore]
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the alias.
		/// </summary>
		/// <value>The alias.</value>
		public string Alias { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this column is mandatory.
		/// </summary>
		/// <value><c>true</c> if mandatory; otherwise, <c>false</c>.</value>
		public bool Mandatory { get; set; }

		/// <summary>
		/// Gets or sets the validation expression.
		/// </summary>
		/// <value>The validation expression.</value>
		public string ValidationExpression { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this column is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        public bool Visible { get; set; }

		/// <summary>
		/// Gets or sets the data type id.
		/// </summary>
		/// <value>The data type id.</value>
		public int DataTypeId { get; set; }

		/// <summary>
		/// Gets or sets the sort order.
		/// </summary>
		/// <value>The sort order.</value>
		[ScriptIgnore]
		public int SortOrder { get; set; }
	}
}
