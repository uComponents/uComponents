namespace uComponents.Core.DataTypes.DataTypeGrid.Interfaces
{
    using System.Web.UI;

    using umbraco.interfaces;

    /// <summary>
    /// Interface for DTG compatibility functions
    /// </summary>
    /// <typeparam name="T">The DataType</typeparam>
    public interface IDataTypeFunctions<T> where T : IDataType
    {
        /// <summary>
        /// Converts the datatype value to a DTG compatible string
        /// </summary>
        /// <param name="dataType">The DataType.</param>
        /// <returns>A human-readable string</returns>
        string ToDtgString(T dataType);

        /// <summary>
        /// Configures the datatype to be compatible with DTG.
        /// </summary>
        /// <param name="dataType">The DataType.</param>
        /// <param name="container">The container.</param>
        void ConfigureForDtg(T dataType, Control container);

        /// <summary>
        /// Saves the datatype for DTG.
        /// </summary>
        /// <param name="dataType">The DataType.</param>
        void SaveForDtg(T dataType);
    }
}