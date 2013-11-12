namespace uComponents.DataTypes.DataTypeGrid.Model
{
    using System;
    using System.Web.UI;

    /// <summary>
    /// Event arguments for DataTypeGrid datatype load events
    /// </summary>
    public class DataTypeLoadEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataTypeLoadEventArgs"/> class.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="container">The container.</param>
        public DataTypeLoadEventArgs(DataEditor grid, Control container)
        {
            this.Grid = grid;
            this.Container = container;
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        public Control Container { get; private set; }

        /// <summary>
        /// Gets the grid.
        /// </summary>
        /// <value>The grid.</value>
        public DataEditor Grid { get; private set; }
    }
}