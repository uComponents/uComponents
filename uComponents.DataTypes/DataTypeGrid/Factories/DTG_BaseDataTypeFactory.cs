namespace uComponents.DataTypes.DataTypeGrid.Factories
{
    using System.Web.UI;

    using uComponents.DataTypes.DataTypeGrid.Interfaces;
    using uComponents.DataTypes.DataTypeGrid.Model;

    using umbraco.interfaces;

    /// <summary>
    /// Base DataTypeFactory
    /// </summary>
    /// <typeparam name="T">The <see cref="IDataType"/></typeparam>
    public abstract class BaseDataTypeFactory<T> : IDataTypeFactory<T>
        where T : IDataType
    {
        /// <summary>
        /// Method for customizing the way the <see cref="IDataType">datatype</see> value is displayed in the grid.
        /// </summary>
        /// <remarks>Called when the grid displays the cell value for the specified <see cref="IDataType">datatype</see>.</remarks>
        /// <param name="dataType">The <see cref="IDataType">datatype</see> instance.</param>
        /// <returns>The display value.</returns>
        public virtual string GetDisplayValue(T dataType)
        {
            return dataType.Data.Value != null ? dataType.Data.Value.ToString() : string.Empty;
        }

        /// <summary>
        /// Method for getting the backing object for the specified <see cref="IDataType">datatype</see>.
        /// </summary>
        /// <param name="dataType">The <see cref="IDataType">datatype</see> instance.</param>
        /// <returns>The backing object.</returns>
        /// <remarks>Called when the method <see cref="GridCell.GetObject{T}()" /> method is called on a <see cref="GridCell" />.</remarks>
        public virtual object GetObject(T dataType)
        {
            if (dataType.Data == null)
            {
                return null;
            }

            try
            {
                return dataType.Data;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Method for performing special actions while creating the <see cref="IDataType">datatype</see> editor.
        /// </summary>
        /// <remarks>Called when the grid creates the editor controls for the specified <see cref="IDataType">datatype</see>.</remarks>
        /// <param name="dataType">The <see cref="IDataType">datatype</see> instance.</param>
        /// <param name="container">The editor control container.</param>
        public virtual void Configure(T dataType, Control container)
        {
        }

        /// <summary>
        /// Method for executing special actions before saving the editor value to the database.
        /// </summary>
        /// <remarks>Called when the grid is saved for the specified <see cref="IDataType">datatype</see>.</remarks>
        /// <param name="dataType">The <see cref="IDataType">datatype</see> instance.</param>
        public virtual void Save(T dataType)
        {
            dataType.DataEditor.Save();
        }
    }
}
