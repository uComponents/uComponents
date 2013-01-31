// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 30.01.2013 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace uComponents.DataTypes.DataTypeGrid.Model
{
    using System;
    using System.Collections.ObjectModel;

    using umbraco.MacroEngines;

    /// <summary>
    /// Represents a DataTypeGrid Row
    /// </summary>
    public class GridRow : KeyedCollection<string, GridCell>
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>The sort order.</value>
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets the value of the cell with the specified key.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="cellKey">The cell alias or name. Will only get by name if no cells exist with the specified alias.</param>
        /// <returns>The cell value.</returns>
        public T GetObject<T>(string cellKey)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the value of the cell with the specified key.
        /// </summary>
        /// <param name="cellKey">The cell alias or name. Will only get by name if no cells exist with the specified alias.</param>
        /// <returns>The cell value.</returns>
        public string GetValue(string cellKey)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the collection to a <see cref="DynamicXml"/> object.
        /// </summary>
        /// <returns>The dynamic xml.</returns>
        public DynamicXml AsDynamicXml()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the key for item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The item key.</returns>
        protected override string GetKeyForItem(GridCell item)
        {
            return item.Alias;
        }
    }
}