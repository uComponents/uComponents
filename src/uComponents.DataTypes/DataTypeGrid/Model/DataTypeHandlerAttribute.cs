namespace uComponents.DataTypes.DataTypeGrid.Model
{
    using System;

    using uComponents.DataTypes.DataTypeGrid.Interfaces;

    /// <summary>
    /// Attribute for configuring <see cref="IDataTypeHandler{TDataType}"/> classes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DataTypeHandlerAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataTypeHandlerAttribute" /> class.
        /// </summary>
        public DataTypeHandlerAttribute()
        {
            this.Priority = 0;
        }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <remarks>Lower number means lower priority.</remarks>
        /// <remarks>The default value is 0.</remarks>
        /// <value>The priority.</value>
        public int Priority { get; set; }
    }
}