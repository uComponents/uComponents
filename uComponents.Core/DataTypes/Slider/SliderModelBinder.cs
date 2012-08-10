using System.Linq;
using uComponents.Core.Shared;
using umbraco.MacroEngines;

namespace uComponents.Core.DataTypes.Slider
{
	/// <summary>
	/// Model binder for the Slider data-type.
	/// </summary>
	[RazorDataTypeModel(DataTypeConstants.SliderId)]
	public class SliderModelBinder : IRazorDataTypeModel
	{
		/// <summary>
		/// Gets or sets the value1.
		/// </summary>
		/// <value>The value1.</value>
		public int Value1 { get; set; }

		/// <summary>
		/// Gets or sets the value2.
		/// </summary>
		/// <value>The value2.</value>
		public int Value2 { get; set; }

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

			if (!string.IsNullOrEmpty(PropertyData))
			{
				int value1, value2;
				var values = PropertyData.Split(Constants.Common.COMMA).Select(s => s.Trim()).ToList();

				if (values.Count > 0 && int.TryParse(values[0], out value1))
				{
					this.Value1 = value1;

					if (values.Count > 1 && int.TryParse(values[1], out value2))
					{
						this.Value2 = value2;
					}
				}
			}

			instance = this;

			return true;
		}
	}
}