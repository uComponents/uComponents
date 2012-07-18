// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 23.05.2012 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace uComponents.DataTypes.DataTypeGrid.Factories
{
    using uComponents.DataTypes.DataTypeGrid.Interfaces;

    using umbraco.interfaces;

    internal class DataTypeFunctionsFactory<TDataTypeFunctions, TDataType>
        where TDataTypeFunctions : IDataTypeFunctions<TDataType>, new()
        where TDataType : IDataType
    {
        /// <summary>
        /// The instance of T
        /// </summary>
        private static readonly TDataTypeFunctions instance = new TDataTypeFunctions();

        /// <summary>
        /// Prevents a default instance of the <see cref="DataTypeFunctionsFactory&lt;TDataTypeFunctions, TDataType&gt;"/> class from being created.
        /// </summary>
        private DataTypeFunctionsFactory()
        {
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static TDataTypeFunctions Instance
        {
            get
            {
                return instance;
            }
        }
    }
}