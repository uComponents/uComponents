// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 11.01.2011 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.Script.Serialization;

namespace uComponents.Core.DataTypes.DataTypeGrid.Model
{
	/// <summary>
	/// 
	/// </summary>
	public class BasePreValueRow
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BasePreValueRow"/> class.
		/// </summary>
		public BasePreValueRow()
		{
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
		/// Gets or sets the validation expression.
		/// </summary>
		/// <value>The validation expression.</value>
		public string ValidationExpression { get; set; }

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

		/// <summary>
		/// Gets or sets the content sort priority.
		/// </summary>
		/// <value>
		/// The content sort priority.
		/// </value>
		public string ContentSortPriority { get; set; }

		/// <summary>
		/// Gets or sets the content sort order.
		/// </summary>
		/// <value>
		/// The content sort order.
		/// </value>
		public string ContentSortOrder { get; set; }
	}
}
