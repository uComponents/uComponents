// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 30.01.2013 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace uComponents.DataTypes.DataTypeGrid.Model
{
    using System.Collections.ObjectModel;
    using System.Dynamic;
    using System.Linq;
    using System.Linq.Expressions;

    using Umbraco.Core.Dynamics;

    /// <summary>
    /// Represents a DataTypeGrid Row
    /// </summary>
    public class GridRow : KeyedCollection<string, GridCell>, IDynamicMetaObjectProvider
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>The sort order.</value>
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets the cell with the specified key.
        /// </summary>
        /// <param name="cellKey">The cell alias or name. Will only get by name if no cells exist with the specified alias.</param>
        /// <returns>The cell.</returns>
        public GridCell GetCell(string cellKey)
        {
            // Get cell by alias or name
            var cell = this.FirstOrDefault(x => x.Alias == cellKey) ?? this.FirstOrDefault(x => x.Name == cellKey);

            return cell ?? new GridCell();
        }

        /// <summary>
        /// Converts the collection to a <see cref="DynamicXml"/> object.
        /// </summary>
        /// <returns>The dynamic xml.</returns>
        public DynamicXml AsDynamicXml()
        {
            var xml = string.Format(@"<item id=""{0}"" sortOrder=""{1}"">", this.Id, this.SortOrder);

            // Convert all cells
            foreach (var item in this)
            {
                xml += item.AsDynamicXml().ToXml();
            }

            xml += "</item>";

            return new DynamicXml(xml);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return this.AsDynamicXml().ToXml();
        }

        /// <summary>
        /// Returns the <see cref="T:System.Dynamic.DynamicMetaObject" /> responsible for binding operations performed on this object.
        /// </summary>
        /// <param name="parameter">The expression tree representation of the runtime value.</param>
        /// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> to bind this object.</returns>
        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new DynamicGridRowMetaObject(parameter, this);
        }

        /// <summary>
        /// Gets the key for item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The item key.</returns>
        protected override string GetKeyForItem(GridCell item)
        {
            return item.Alias;
        }

        /// <summary>
        /// The dynamic meta object for the <see cref="GridRow"/> class.
        /// </summary>
        private class DynamicGridRowMetaObject : DynamicMetaObject
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DynamicGridRowMetaObject" /> class.
            /// </summary>
            /// <param name="parameter">The parameter.</param>
            /// <param name="value">The value.</param>
            internal DynamicGridRowMetaObject(Expression parameter, GridRow value)
                : base(parameter, BindingRestrictions.Empty, value)
            {
            }

            /// <summary>
            /// Performs the binding of the dynamic get member operation.
            /// </summary>
            /// <param name="binder">An instance of the <see cref="T:System.Dynamic.GetMemberBinder" /> that represents the details of the dynamic operation.</param>
            /// <returns>The new <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
            public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
            {
                // If the binding is a built-in property, call that instead
                if (typeof(GridRow).GetProperties().Any(x => x.Name == binder.Name))
                {
                    // Convert expression to GridRow
                    var i = Expression.Convert(this.Expression, this.LimitType);

                    // Get property
                    var m = Expression.Property(i, binder.Name);

                    // Convert back to object
                    var v = Expression.Convert(m, typeof(object));

                    return new DynamicMetaObject(v, BindingRestrictions.GetTypeRestriction(this.Expression, this.LimitType)); 
                }

                // Create method parameters
                var parameters = new Expression[] { Expression.Constant(binder.Name) };

                var expression = Expression.Call(
                    Expression.Convert(this.Expression, this.LimitType),
                    typeof(GridRow).GetMethod("GetCell"),
                    parameters);

                return
                    new DynamicMetaObject(
                        expression,
                        BindingRestrictions.GetTypeRestriction(this.Expression, this.LimitType));
            }
        }
    }
}