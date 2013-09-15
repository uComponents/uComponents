using System;
using uComponents.DataTypes;
using umbraco;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web;

namespace uComponents.PropertyEditors.ValueConverters.ImagePoint
{
	/// <summary>
	/// Property-editor value converter for the Image Point data-type.
	/// </summary>
	public class ImagePointPropertyEditorValueConverter : IPropertyEditorValueConverter
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
			return Guid.Parse(DataTypeConstants.ImagePointId).Equals(propertyEditorId);
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
				var imagePoint = new DataTypes.ImagePoint.ImagePoint();

				// use the uQuery.IGetProperty interface to do the work
				((uQuery.IGetProperty)imagePoint).LoadPropertyValue(value.ToString());

				return new Attempt<object>(true, imagePoint);
			}

			return Attempt<object>.False;
		}
	}
}
