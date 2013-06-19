using System;
using uComponents.DataTypes;
using umbraco;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web;

namespace uComponents.PropertyEditors.ValueConverters.Similarity
{
	public class SimilarityPropertyEditorValueConverter : IPropertyEditorValueConverter
	{
		public bool IsConverterFor(Guid propertyEditorId, string docTypeAlias, string propertyTypeAlias)
		{
			return Guid.Parse(DataTypeConstants.SimilarityId).Equals(propertyEditorId);
		}

		public Attempt<object> ConvertPropertyValue(object value)
		{
			if (UmbracoContext.Current != null && value != null && value.ToString() != string.Empty)
			{
				var nodeIds = uQuery.GetCsvIds(value.ToString());
				var helper = new UmbracoHelper(UmbracoContext.Current);
				return new Attempt<object>(true, helper.TypedContent(nodeIds));
			}

			return Attempt<object>.False;
		}
	}
}