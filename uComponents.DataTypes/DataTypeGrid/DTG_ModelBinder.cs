using umbraco.MacroEngines;

namespace uComponents.DataTypes.DataTypeGrid
{
    using System.Linq;

    using uComponents.DataTypes.DataTypeGrid.Model;

    /// <summary>
    /// Model binder for the DataTypeGrid data-type.
    /// </summary>
    [RazorDataTypeModel(DataTypeConstants.DataTypeGridId)]
    public class ModelBinder : IRazorDataTypeModel
    {
        /// <summary>
        /// Initializes the model binder.
        /// </summary>
        /// <param name="CurrentNodeId">The current node id.</param>
        /// <param name="PropertyData">The property data.</param>
        /// <param name="instance">The instance.</param>
        /// <returns>True if initialization was successful. Otherwise false.</returns>
        public bool Init(int CurrentNodeId, string PropertyData, out object instance)
        {
            if (!Settings.RazorModelBindingEnabled)
            {
                instance = new DynamicXml(PropertyData);

                return true;
            }

            try
            {
                instance = new GridRowCollection(PropertyData).OrderBy(x => x.SortOrder);
            }
            catch
            {
                instance = new GridRowCollection();

                return false;
            }

            return true;
        }
    }
}