using System.Linq;
using uComponents.Core.Shared;
using uComponents.Core.XsltExtensions;
using umbraco.MacroEngines;
using umbraco.MacroEngines.Library;

namespace uComponents.Core.DataTypes.XPathCheckBoxList
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
				instance = PropertyData;
				return true;
			}

			var nodeIds = Xml.CouldItBeXml(PropertyData) ? uQuery.GetXmlIds(PropertyData) : uQuery.ConvertToIntArray(uQuery.GetCsvIds(PropertyData));
			var library = new RazorLibraryCore(null);

			instance = (library.NodesById(nodeIds.ToList()) as DynamicNodeList);

			return true;
		}
	}
}