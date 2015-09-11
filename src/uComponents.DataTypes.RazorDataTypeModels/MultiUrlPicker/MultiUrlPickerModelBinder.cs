using uComponents.Core;
using uComponents.DataTypes.MultiUrlPicker.Dto;
using umbraco.MacroEngines;

namespace uComponents.DataTypes.RazorDataTypeModels.MultiUrlPicker
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
			if (!Settings.RazorModelBindingEnabled)
			{
				if (Helper.Xml.CouldItBeXml(PropertyData))
				{
#pragma warning disable 0618
					instance = new DynamicXml(PropertyData);
#pragma warning restore 0618
					return true;
				}

				instance = PropertyData;
				return true;
			}

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