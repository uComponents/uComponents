using System;
using System.Collections.Generic;
using System.Linq;
using uComponents.Core;
using umbraco.MacroEngines;

namespace uComponents.DataTypes.RazorDataTypeModels.MultipleDates
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
			if (!Settings.RazorModelBindingEnabled)
			{
				instance = PropertyData;
				return true;
			}

			var dates = new List<DateTime>();

			if (!string.IsNullOrEmpty(PropertyData))
			{
				var values = PropertyData.Split(Constants.Common.COMMA).Select(s => s.Trim()).ToList();
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