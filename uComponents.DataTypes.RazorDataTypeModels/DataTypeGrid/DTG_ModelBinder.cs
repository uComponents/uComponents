namespace uComponents.DataTypes.RazorDataTypeModels.DataTypeGrid
{
    using System.Linq;

    using uComponents.DataTypes.DataTypeGrid.Model;

    using umbraco.MacroEngines;

    using DynamicXml = Umbraco.Core.Dynamics.DynamicXml;

    /// <summary>
    /// Model binder for the DataTypeGrid data-type.
    /// </summary>
    [RazorDataTypeModel(DataTypeConstants.DataTypeGridId)]
    public class DtgModelBinder : IRazorDataTypeModel
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
                var c = new GridRowCollection(PropertyData).OrderBy(x => x.SortOrder).ToList();

                instance = (GridRowCollection)c;
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