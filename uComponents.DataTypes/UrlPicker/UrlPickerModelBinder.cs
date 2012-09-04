using uComponents.Core;
using uComponents.DataTypes.UrlPicker.Dto;
using umbraco.MacroEngines;

namespace uComponents.DataTypes.UrlPicker
{
	/// <summary>
	/// Model binder for the UrlPicker data-type.
	/// </summary>
	[RazorDataTypeModel(DataTypeConstants.UrlPickerId)]
	public class UrlPickerModelBinder : IRazorDataTypeModel
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
				if (Helper.Xml.CouldItBeXml(PropertyData))
				{
					instance = new DynamicXml(PropertyData);
					return true;
				}

				instance = PropertyData;
				return true;
			}

			UrlPickerState state = null;

			if (!string.IsNullOrEmpty(PropertyData))
			{
				state = UrlPickerState.Deserialize(PropertyData);
			}

			instance = state;

			return true;
		}
	}
}