namespace uComponents.DataTypes.DataTypeGrid.Factories
{
    using System.Linq;
    using System.Web.UI.WebControls;

    using uComponents.DataTypes.DataTypeGrid.Interfaces;

    using umbraco;

    /// <summary>
    /// A prevalue editor control factory
    /// </summary>
    public class PrevalueEditorControlFactory : IPrevalueEditorControlFactory
    {
        /// <summary>
        /// The prevalue editor settings factory
        /// </summary>
        private readonly IConfigurationFactory configurationFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrevalueEditorControlFactory"/> class.
        /// </summary>
        public PrevalueEditorControlFactory()
        {
            this.configurationFactory = new ConfigurationFactory();
        }

        /// <summary>
        /// Builds the data type dropdown list.
        /// </summary>
        /// <returns>A DropDownList.</returns>
        public DropDownList BuildDataTypeDropDownList()
        {
            var d = new DropDownList() { CssClass = "propertyItemContent" };

            foreach (var item in uQuery.GetAllDataTypes())
            {
                var guid = uQuery.GetBaseDataTypeGuid(item.Key);
                if (this.configurationFactory.GetCompatibleDataTypes().Contains(guid))
                {
                    d.Items.Add(new ListItem(item.Value, item.Key.ToString()));
                }
            }

            return d;
        }
    }
}