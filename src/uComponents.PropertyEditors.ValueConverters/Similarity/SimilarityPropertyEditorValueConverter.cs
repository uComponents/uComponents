using System;
using uComponents.DataTypes;
using umbraco;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web;

namespace uComponents.PropertyEditors.ValueConverters.Similarity
{
	/// <summary>
	/// Property-editor value converter for the Similarity data-type.
	/// </summary>
	public class SimilarityPropertyEditorValueConverter : IPropertyEditorValueConverter
	{
		/// <summary>
		/// Returns true if this converter can perform the value conversion for the specified property editor id
		/// </summary>
		/// <param name="propertyEditorId"></param>
		/// <param name="docTypeAlias"></param>
		/// <param name="propertyTypeAlias"></param>
		/// <returns></returns>
		public bool IsConverterFor(Guid propertyEditorId, string docTypeAlias, string propertyTypeAlias)
		{
			return Guid.Parse(DataTypeConstants.SimilarityId).Equals(propertyEditorId);
		}

		/// <summary>
		/// Attempts to convert the value specified into a useable value on the front-end
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Attempt<object> ConvertPropertyValue(object value)
		{
			if (UmbracoContext.Current != null && value != null && value.ToString().Length > 0)
			{
				var nodeIds = uQuery.GetCsvIds(value.ToString());
				var helper = new UmbracoHelper(UmbracoContext.Current);
				return new Attempt<object>(true, helper.TypedContent(nodeIds));
			}

			return Attempt<object>.False;
		}
	}
}