namespace uComponents.DataTypes.DataTypeGrid.Factories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI.WebControls;

    using Umbraco.Core;

    using uComponents.DataTypes.DataTypeGrid.Interfaces;
    using uComponents.DataTypes.DataTypeGrid.Model;

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

        /// <summary>
        /// Builds the content priority dropdown list.
        /// </summary>
        /// <param name="configurations">The column configurations.</param>
        /// <param name="currentSortPriority">The current sort priority.</param>
        /// <returns>&gt; DropDownList.</returns>
        public DropDownList BuildContentPriorityDropdownList(IList<PreValueRow> configurations, string currentSortPriority)
        {
            var ddl = new DropDownList();

            // Add blank item to beginning
            ddl.Items.Add(new ListItem(string.Empty, string.Empty));

            // Add a number for each stored prevalue
            foreach (var storedConfig in configurations)
            {
                var priority = configurations.IndexOf(storedConfig) + 1;

                ddl.Items.Add(new ListItem(priority.ToString(), priority.ToString()));
            }

            ddl.SelectedValue = currentSortPriority ?? string.Empty;

            return ddl;
        }
    }
}