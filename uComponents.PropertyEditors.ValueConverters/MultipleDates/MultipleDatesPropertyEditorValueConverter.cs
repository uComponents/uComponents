using System;
using System.Collections.Generic;
using System.Linq;
using uComponents.DataTypes;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web;

namespace uComponents.PropertyEditors.ValueConverters.MultipleDates
{
	public class MultipleDatesPropertyEditorValueConverter : IPropertyEditorValueConverter
	{
		public bool IsConverterFor(Guid propertyEditorId, string docTypeAlias, string propertyTypeAlias)
		{
			return Guid.Parse(DataTypeConstants.MultipleDatesId).Equals(propertyEditorId);
		}

		public Attempt<object> ConvertPropertyValue(object value)
		{
			if (value != null && value.ToString().Length > 0)
			{
				var dates = new List<DateTime>();
				var items = value.ToString().Split(uComponents.Core.Constants.Common.COMMA).Select(s => s.Trim());
				foreach (var item in items)
				{
					DateTime date;
					if (DateTime.TryParse(item, out date))
					{
						dates.Add(date);
					}
				}

				return new Attempt<object>(true, dates);
			}

			return Attempt<object>.False;
		}
	}
}