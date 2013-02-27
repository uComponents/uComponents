namespace uComponents.DataTypes.DataTypeGrid.Factories
{
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
        /// Method for performing special actions <b>before</b> creating the <see cref="IDataType" /> editor.
        /// </summary>
        /// <param name="dataType">The <see cref="IDataType" /> instance.</param>
        /// <param name="eventArgs">The <see cref="DataTypeLoadEventArgs"/> instance containing the event data.</param>
        /// <remarks>Called <b>before</b> the grid creates the editor controls for the specified <see cref="IDataType" />.</remarks>
        public virtual void Initialize(T dataType, DataTypeLoadEventArgs eventArgs)
        {
        }

        /// <summary>
        /// Method for performing special actions <b>after</b> the <see cref="IDataType" />
        /// <see cref="IDataEditor">editor</see> has been loaded.
        /// </summary>
        /// <param name="dataType">The <see cref="IDataType" /> instance.</param>
        /// <param name="eventArgs">The <see cref="DataTypeLoadEventArgs"/> instance containing the event data.</param>
        /// <remarks>Called <b>after</b> the grid creates the editor controls for the specified <see cref="IDataType" />.</remarks>
        public virtual void Configure(T dataType, DataTypeLoadEventArgs eventArgs)
        {
        }

        /// <summary>
        /// Method for executing special actions before saving the editor value to the database.
        /// </summary>
        /// <param name="dataType">The <see cref="IDataType">datatype</see> instance.</param>
        /// <param name="eventArgs">The <see cref="DataTypeSaveEventArgs"/> instance containing the event data.</param>
        /// <remarks>Called when the grid is saved for the specified <see cref="IDataType">datatype</see>.</remarks>
        public virtual void Save(T dataType, DataTypeSaveEventArgs eventArgs)
        {
            dataType.DataEditor.Save();
        }
    }
}
