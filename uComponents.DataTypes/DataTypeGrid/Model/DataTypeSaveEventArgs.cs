namespace uComponents.DataTypes.DataTypeGrid.Model
{
    using System;
    using System.Web.UI;

    using uComponents.DataTypes.DataTypeGrid.Constants;

    /// <summary>
    /// Event arguments for DataTypeGrid datatype save events
    /// </summary>
    public class DataTypeSaveEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataTypeSaveEventArgs" /> class.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="action">The action.</param>
        public DataTypeSaveEventArgs(DataEditor grid, DataTypeAction action)
        {
            this.Grid = grid;
            this.Action = action;
        }

        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <value>The action.</value>
        public DataTypeAction Action { get; private set; }

        /// <summary>
        /// Gets the grid.
        /// </summary>
        /// <value>The grid.</value>
        public DataEditor Grid { get; private set; }
    }
}