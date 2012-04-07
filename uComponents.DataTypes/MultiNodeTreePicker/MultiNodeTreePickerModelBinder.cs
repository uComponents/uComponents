using System.Linq;
using umbraco.MacroEngines;
using umbraco.MacroEngines.Library;

namespace uComponents.DataTypes.MultiNodeTreePicker
{
	/// <summary>
	/// Model binder for the DataTypeGrid data-type.
	/// </summary>
	[RazorDataTypeModel(DataTypeConstants.MultiNodeTreePickerId)]
	public class MultiNodeTreePickerModelBinder : IRazorDataTypeModel
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
			var nodeIds = uQuery.Helper.CouldItBeXml(PropertyData) ? uQuery.GetXmlIds(PropertyData) : uQuery.ConvertToIntArray(uQuery.GetCsvIds(PropertyData));
			var library = new RazorLibraryCore(null);
			
			instance = (library.NodesById(nodeIds.ToList()) as DynamicNodeList);

			return true;
		}
	}
}
