using uComponents.Core.Shared;
using uComponents.Core.Shared.Extensions;
using umbraco.cms.businesslogic.datatype;

namespace uComponents.Core.uQueryExtensions
{
	/// <summary>
	/// uQuery extensions for the PreValue object.
	/// </summary>
	public static class PreValueExtensions
	{
		/// <summary>
		/// Gets the alias of the specified PreValue
		/// </summary>
		/// <param name="preValue">The PreValue.</param>
		/// <returns>The alias</returns>
		public static string GetAlias(this PreValue preValue)
		{
			using (var reader = uQuery.SqlHelper.ExecuteReader(
				"SELECT alias FROM cmsDataTypePreValues WHERE id = @id",
				uQuery.SqlHelper.CreateParameter("@id", preValue.Id)))
			{
				var hasRows = reader.Read();

				if (!hasRows)
				{
					return null;
				}

				var alias = string.Empty;

				while (hasRows)
				{
					string tmpStr;

					if (reader.TryGetColumnValue("alias", out tmpStr))
					{
						alias = tmpStr;
					}

					hasRows = reader.Read();
				}

				return alias;
			}
		}
	}
}