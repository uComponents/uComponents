// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 11.01.2011 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace uComponents.DataTypes.DataTypeGrid.Model
{
	using System;

	using umbraco.interfaces;

	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class StoredValue
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StoredValue"/> class.
		/// </summary>
		public StoredValue()
		{
		}

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
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public IDataType Value { get; set; }
	}
}
