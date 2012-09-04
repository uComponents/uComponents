using System.Linq;
using uComponents.Core;
using umbraco.MacroEngines;
using umbraco.MacroEngines.Library;

namespace uComponents.DataTypes.XPathCheckBoxList
{
	/// <summary>
	/// Model binder for the XPathCheckBoxList data-type.
	/// </summary>
	[RazorDataTypeModel(DataTypeConstants.XPathCheckBoxListId)]
	public class XPathCheckBoxListModelBinder : IRazorDataTypeModel
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

			var nodeIds = Helper.Xml.CouldItBeXml(PropertyData) ? umbraco.uQuery.GetXmlIds(PropertyData) : umbraco.uQuery.ConvertToIntArray(umbraco.uQuery.GetCsvIds(PropertyData));
			var library = new RazorLibraryCore(null);

			instance = (library.NodesById(nodeIds.ToList()) as DynamicNodeList);

			return true;
		}
	}
}