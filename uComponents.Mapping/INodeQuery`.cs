using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace uComponents.Mapping
{
    /// <summary>
    /// Represents a collection of <c>Node</c>s which can be queried.
    /// </summary>
    /// <typeparam name="TDestination">The type which queried <c>Node</c>s will
    /// be mapped to.</typeparam>
    public interface INodeQuery<TDestination>
        where TDestination : class, new()
    {
        /// <summary>
        /// Includes the selected relationship path in the node query.
        /// </summary>
        /// <param name="path">
        /// The path of relationships to include, 
        /// e.g. "Employers.Employees.GoldStars"
        /// </param>
        /// <example>
        /// <code>
        /// // If querying on a set of companies,
        /// var myQuery = uMapper.Set&lt;Company&gt;()
        ///     .Include("Employers.Employees.GoldStars")
        ///     .Include("Employers.Employees.FavouriteMeals");
        /// </code>
        /// </example>
        /// <returns>
        /// The current query with the <paramref name="path"/> 
        /// included.
        /// </returns>
        INodeQuery<TDestination> Include(string path);

        INodeQuery<TDestination> Include<TProperty>(Expression<Func<TDestination, TProperty>> path);

        IEnumerable<TDestination> All();

        TDestination Single(int nodeId);

        TDestination Current();
    }
}
