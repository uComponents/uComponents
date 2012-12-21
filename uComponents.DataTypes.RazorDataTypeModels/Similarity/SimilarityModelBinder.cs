using umbraco;
using umbraco.MacroEngines;
using umbraco.MacroEngines.Library;

namespace uComponents.DataTypes.RazorDataTypeModels.Similarity
{
	/// <summary>
	/// Model binder for the Similarity data-type.
	/// </summary>
	[RazorDataTypeModel(DataTypeConstants.SimilarityId)]
	public class SimilarityModelBinder : IRazorDataTypeModel
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

			var nodeIds = uQuery.GetCsvIds(PropertyData);
			var library = new RazorLibraryCore(null);

			instance = (library.NodesById(nodeIds) as DynamicNodeList);

			return true;
		}
	}
}