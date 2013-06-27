// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 23.05.2012 - Created [Ove Andersen]
// 09.02.2013 - Rewritten [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace uComponents.DataTypes.DataTypeGrid.Interfaces
{
    using System.Web.UI;

    using uComponents.DataTypes.DataTypeGrid.Model;

    using umbraco.interfaces;

    /// <summary>
    /// Interface for DTG compatibility handlers
    /// </summary>
    /// <typeparam name="TDataType">The <see cref="IDataType">datatype</see> this factory represents.</typeparam>
    public interface IDataTypeHandler<in TDataType> where TDataType : IDataType
    {
        /// <summary>
        /// Method for customizing the way the <typeparamref name="TDataType">datatype</typeparamref> value is displayed in the grid.
        /// </summary>
        /// <remarks>Called when the grid displays the cell value for the specified <typeparamref name="TDataType">datatype</typeparamref>.</remarks>
        /// <param name="dataType">The <typeparamref name="TDataType">datatype</typeparamref> instance.</param>
        /// <returns>The display value.</returns>
        string GetDisplayValue(TDataType dataType);

        /// <summary>
        /// Method for getting the backing object for the specified <typeparamref name="TDataType">datatype</typeparamref>.
        /// </summary>
        /// <param name="dataType">The <typeparamref name="TDataType">datatype</typeparamref> instance.</param>
        /// <returns>The backing object.</returns>
        /// <remarks>Called when the method <see cref="GridCell.GetPropertyValue()" /> method is called on a <see cref="GridCell" />.</remarks>
        object GetPropertyValue(TDataType dataType);

        /// <summary>
        /// Method for getting the control to use when validating the specified <see cref="IDataType" />.
        /// </summary>
        /// <param name="dataType">The <see cref="IDataType" /> instance.</param>
        /// <param name="editorControl">The <see cref="IDataType" /> editor control.</param>
        /// <returns>The control to validate.</returns>
        Control GetControlToValidate(TDataType dataType, Control editorControl);

        /// <summary>
        /// Method for performing special actions <b>before</b> creating the <see cref="IDataType" /> editor.
        /// </summary>
        /// <param name="dataType">The <see cref="IDataType" /> instance.</param>
        /// <param name="eventArgs">The <see cref="DataTypeLoadEventArgs"/> instance containing the event data.</param>
        /// <remarks>Called <b>before</b> the grid creates the editor controls for the specified <see cref="IDataType" />.</remarks>
        void Initialize(TDataType dataType, DataTypeLoadEventArgs eventArgs);

        /// <summary>
        /// Method for performing special actions <b>after</b> the <see cref="IDataType" /> <see cref="IDataEditor">editor</see> has been loaded.
        /// </summary>
        /// <param name="dataType">The <see cref="IDataType" /> instance.</param>
        /// <param name="eventArgs">The <see cref="DataTypeLoadEventArgs"/> instance containing the event data.</param>
        /// <remarks>Called <b>after</b> the grid creates the editor controls for the specified <see cref="IDataType" />.</remarks>
        void Configure(TDataType dataType, DataTypeLoadEventArgs eventArgs);

        /// <summary>
        /// Method for executing special actions before saving the editor value to the database.
        /// </summary>
        /// <param name="dataType">The <typeparamref name="TDataType">datatype</typeparamref> instance.</param>
        /// <param name="eventArgs">The <see cref="DataTypeSaveEventArgs"/> instance containing the event data.</param>
        /// <remarks>Called when the grid is saved for the specified <typeparamref name="TDataType">datatype</typeparamref>.</remarks>
        void Save(TDataType dataType, DataTypeSaveEventArgs eventArgs);
    }
}