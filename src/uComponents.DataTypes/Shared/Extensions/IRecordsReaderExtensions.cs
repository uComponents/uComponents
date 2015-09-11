using umbraco.DataLayer;

namespace uComponents.DataTypes.Shared.Extensions
{
	/// <summary>
	/// Extensions for the IRecordsReader class
	/// </summary>
	public static class IRecordsReaderExtensions
	{
		/// <summary>
		/// Tries the get column value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="reader">The recordsreader.</param>
		/// <param name="columnName">Name of the column.</param>
		/// <param name="value">The value.</param>
		/// <returns>Returns the value of the specified column.</returns>
		public static bool TryGetColumnValue<T>(this IRecordsReader reader, string columnName, out T value)
		{
			if (reader.ContainsField(columnName) && !reader.IsNull(columnName))
			{
				value = reader.Get<T>(columnName);

				return true;
			}

			value = default(T);

			return false;
		}
	}
}