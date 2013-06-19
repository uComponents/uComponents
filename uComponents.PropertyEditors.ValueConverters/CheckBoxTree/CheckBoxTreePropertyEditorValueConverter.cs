using System;
using uComponents.DataTypes;
using umbraco;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web;

namespace uComponents.PropertyEditors.ValueConverters.CheckBoxTree
{
	/// <summary>
	/// Property-editor value converter for the CheckBoxTree data-type.
	/// </summary>
	public class CheckBoxTreePropertyEditorValueConverter : IPropertyEditorValueConverter
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
			return Guid.Parse(DataTypeConstants.CheckBoxTreeId).Equals(propertyEditorId);
		}

		/// <summary>
		/// Attempts to convert the value specified into a useable value on the front-end
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <remarks>
		/// This is used to convert the value stored in the repository into a usable value on the front-end.
		/// For example, if a 0 or 1 is stored for a boolean, we'd want to convert this to a real boolean.
		/// Also note that the value might not come in as a 0 or 1 but as a "0" or "1"
		/// </remarks>
		public Attempt<object> ConvertPropertyValue(object value)
		{
			if (UmbracoContext.Current != null && value != null && value.ToString().Length > 0)
			{
				var data = value.ToString();
				var nodeIds = XmlHelper.CouldItBeXml(data) ? uQuery.GetXmlIds(data) : uQuery.ConvertToIntArray(uQuery.GetCsvIds(data));

				var helper = new UmbracoHelper(UmbracoContext.Current);
				return new Attempt<object>(true, helper.TypedContent(nodeIds));
			}

			return Attempt<object>.False;
		}
	}
}