using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uComponents.DataTypes.DataTypeGrid.Model
{
    /// <summary>
    /// Represents a single stored value
    /// </summary>
    /// <remarks>
    /// We use the <c>uComponents.RazorModels.DataTypeGrid.StoredValueForModel</c> object 
    /// instead of the <c>uComponents.DataTypes.DataTypeGrid.Model.StoredValue</c> object 
    /// to override the <c>uComponents.DataTypes.DataTypeGrid.Model.StoredValue.Value</c> 
    /// property, as the type is <c>umbraco.interfaces.IDataType</c>, which we do not need to 
    /// access at the front-end.
    /// </remarks>
    public class StoredValueForModel : StoredValue
    {
        /// <summary>
        /// Gets or sets the type of the node.
        /// </summary>
        /// <value>The type of the node.</value>
        public int NodeType { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public new string Value { get; set; }
    }
}
