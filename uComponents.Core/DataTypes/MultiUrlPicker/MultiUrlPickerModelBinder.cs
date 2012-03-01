using uComponents.Core.DataTypes.MultiUrlPicker.Dto;
using umbraco.MacroEngines;

namespace uComponents.Core.DataTypes.MultiUrlPicker
{
	/// <summary>
	/// Model binder for the MultiUrlPicker data-type.
	/// </summary>
	[RazorDataTypeModel(DataTypeConstants.MultiUrlPickerId)]
	public class MultiUrlPickerModelBinder : IRazorDataTypeModel
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
			MultiUrlPickerState state = null;

			if (!string.IsNullOrEmpty(PropertyData))
			{
				state = MultiUrlPickerState.Deserialize(PropertyData);
			}

			instance = state;

			return true;
		}
	}
}