namespace uComponents.DataTypes.DataTypeGrid.Factories
{
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    using uComponents.Core;
    using uComponents.DataTypes.DataTypeGrid.Model;

    public class GridFactory : IGridFactory
    {
        /// <summary>
        /// Builds the grid header row.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="values">The values.</param>
        /// <returns>The grid header row.</returns>
        public TableHeaderRow BuildGridHeaderRow(Table table, IList<PreValueRow> values)
        {
            var tr = new TableRow { TableSection = TableRowSection.TableHeader };

            // Add ID header cell
            tr.Cells.Add(new TableHeaderCell { Text = Helper.Dictionary.GetDictionaryItem("ID", "ID") });

            tr.Cells.Add(new TableHeaderCell { Text = Helper.Dictionary.GetDictionaryItem("Actions", "Actions") });

            // Add prevalue cells
            foreach (var s in values)
            {
                var th = new TableHeaderCell { Text = s.Name };
                tr.Cells.Add(th);
            }

            return tr;
        }
    }
}
