using System;
using System.Collections.Generic;
using System.Linq;
using uComponents.Core.Shared;
using umbraco.MacroEngines;

namespace uComponents.Core.DataTypes.MultipleDates
{
	/// <summary>
	/// Model binder for the Multiple Dates data-type.
	/// </summary>
	[RazorDataTypeModel(DataTypeConstants.MultipleDatesId)]
	public class MD_ModelBinder : IRazorDataTypeModel
	{
		/// <summary>
		/// Inits the specified current node id.
		/// </summary>
		/// <param name="CurrentNodeId">The current node id.</param>
		/// <param name="PropertyData">The property data.</param>
		/// <param name="instance">The instance.</param>
		/// <returns></returns>
		public bool Init(int CurrentNodeId, string PropertyData, out object instance)
		{
			var dates = new List<DateTime>();

			if (!string.IsNullOrEmpty(PropertyData))
			{
				var values = PropertyData.Split(Settings.COMMA).Select(s => s.Trim()).ToList();
				foreach (var value in values)
				{
					DateTime date;
					if (DateTime.TryParse(value, out date))
					{
						dates.Add(date);
					}
				}
			}

			instance = dates;

			return true;
		}
	}
}