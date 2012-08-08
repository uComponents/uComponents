using uComponents.Core.DataTypes.UrlPicker.Dto;
using umbraco.MacroEngines;
using uComponents.Core.Shared;

namespace uComponents.Core.DataTypes.UrlPicker
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