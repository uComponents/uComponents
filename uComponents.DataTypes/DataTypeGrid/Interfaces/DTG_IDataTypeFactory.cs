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
    /// Interface for DTG compatibility factories
    /// </summary>
    /// <typeparam name="TDataType">The <see cref="IDataType">datatype</see> this factory represents.</typeparam>
    public interface IDataTypeFactory<in TDataType> where TDataType : IDataType
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
        /// <remarks>Called when the method <see cref="GridCell.GetObject{T}()" /> method is called on a <see cref="GridCell" />.</remarks>
        object GetObject(TDataType dataType);

        /// <summary>
        /// Method for performing special actions while creating the <typeparamref name="TDataType">datatype</typeparamref> editor.
        /// </summary>
        /// <remarks>Called when the grid creates the editor controls for the specified <typeparamref name="TDataType">datatype</typeparamref>.</remarks>
        /// <param name="dataType">The <typeparamref name="TDataType">datatype</typeparamref> instance.</param>
        /// <param name="container">The editor control container.</param>
        void Configure(TDataType dataType, Control container);

        /// <summary>
        /// Method for executing special actions before saving the editor value to the database.
        /// </summary>
        /// <remarks>Called when the grid is saved for the specified <typeparamref name="TDataType">datatype</typeparamref>.</remarks>
        /// <param name="dataType">The <typeparamref name="TDataType">datatype</typeparamref> instance.</param>
        void Save(TDataType dataType);
    }
}