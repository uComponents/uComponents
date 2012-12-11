using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using umbraco.NodeFactory;
using umbraco;
using System.Collections;
using uComponents.Mapping.Property;

namespace uComponents.Mapping
{
    /// <summary>
    /// Represents a query for mapped Umbraco nodes.  Enumerating the
    /// query gets mapped instances of every node which can be mapped to 
    /// <typeparamref name="TDestination"/>. 
    /// </summary>
    /// <typeparam name="TDestination">
    /// The type which queried nodes will be mapped to.
    /// </typeparam>
    internal class NodeQuery<TDestination> : INodeQuery<TDestination>
        where TDestination : class, new()
    {
        // Cache keys
        private const string _explicitCacheFormat = "Explicit_{0}";
        private const string _allCacheFormat = "All_{0}";

        // The paths included in the query
        private readonly List<string> _paths;
        private readonly Dictionary<string, Func<object, bool>> _propertyFilters;
        private bool _isExplicit = false;

        // The engine which will execute the query
        private readonly NodeMappingEngine _engine;

        public NodeQuery(NodeMappingEngine engine)
        {
            var destinationType = typeof(TDestination);

            if (engine == null)
            {
                throw new ArgumentNullException("engine");
            }
            else if (!engine.NodeMappers.ContainsKey(destinationType))
            {
                throw new MapNotFoundException(destinationType);
            }

            _engine = engine;
            _paths = new List<string>();
            _propertyFilters = new Dictionary<string, Func<object, bool>>();
        }

        public INodeMappingEngine Engine
        {
            get
            {
                return _engine;
            }
        }

        #region Include

        public INodeQuery<TDestination> Include(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("The path cannot be null or empty", "path");
            }

            if (!_paths.Contains(path))
            {
                _paths.Add(path);
            }

            return this;
        }

        public INodeQuery<TDestination> Include(Expression<Func<TDestination, object>> path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            string parsedPath;
            if (!path.Body.TryParsePath(out parsedPath)
                || parsedPath == null)
            {
                throw new ArgumentException(
                    string.Format("Path could not be parsed (got this far: '{0}')", parsedPath),
                    "path"
                    );
            }

            return Include(parsedPath);
        }

        public INodeQuery<TDestination> IncludeMany(string[] paths)
        {
            if (paths == null)
            {
                throw new ArgumentNullException("paths");
            }

            foreach (var path in paths)
            {
                this.Include(path);
            }

            return this;
        }

        public INodeQuery<TDestination> IncludeMany(Expression<Func<TDestination, object>>[] paths)
        {
            if (paths == null)
            {
                throw new ArgumentNullException("paths");
            }

            foreach (var path in paths)
            {
                this.Include(path);
            }

            return this;
        }

        #endregion

        #region Filtering

        public INodeQuery<TDestination> Explicit()
        {
            _isExplicit = true;

            return this;
        }

        public INodeQuery<TDestination> WhereProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> property,
            Func<TProperty, bool> predicate
            )
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }
            else if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            var destinationType = typeof(TDestination);
            var destinationInfo = property.GetPropertyInfo();

            if (!_engine.NodeMappers[destinationType].PropertyMappers
                .Any(x => x.DestinationInfo.Name == destinationInfo.Name))
            {
                throw new PropertyNotMappedException(destinationType, destinationInfo.Name);
            }

            _propertyFilters.Add(destinationInfo.Name, x => predicate((TProperty)x));

            return this;
        }

        /// <summary>
        /// Filters a collection of node IDs based on the predicates in <see cref="_propertyFilters"/>.
        /// </summary>
        /// <param name="ids">The IDs to filter</param>
        /// <param name="cachedNodes">
        /// Nodes which have already been retrieved which might as well be used for filtering.
        /// </param>
        /// <returns>The filtered subset of <paramref name="ids"/>.</returns>
        private int[] FilterCollection(int[] ids, IEnumerable<Node> cachedNodes)
        {
            if (ids == null)
            {
                throw new ArgumentNullException("ids");
            }

            // Include cached nodes in parent context
            var parentContext = new NodeMappingContext(0, null, null);
            if (cachedNodes != null)
            {
                parentContext.AddNodesToContextCache(cachedNodes);
            }

            var filteredIds = new List<int>(ids);

            foreach (var filter in _propertyFilters)
            {
                var propertyMapper = _engine.NodeMappers[typeof(TDestination)]
                    .PropertyMappers
                    .Single(x => x.DestinationInfo.Name == filter.Key);

                filteredIds.RemoveAll(id =>
                    {
                        var context = new NodeMappingContext(id, new string[0], parentContext);
                        var property = propertyMapper.MapProperty(context);
                        return !filter.Value(property);
                    });
            }

            return filteredIds.ToArray();
        }

        #endregion

        #region Execute

        public TDestination Map(Node node)
        {
            if (node == null
                || string.IsNullOrEmpty(node.Name))
            {
                return null;
            }

            var context = new NodeMappingContext(node, _paths.ToArray(), null);

            return (TDestination)_engine.Map(
                context,
                typeof(TDestination)
                );
        }

        [Obsolete("Use Find() instead")]
        public TDestination Single(int id)
        {
            return Find(id);
        }

        public TDestination Find(int id)
        {
            var context = new NodeMappingContext(id, _paths.ToArray(), null);

            return (TDestination)_engine.Map(
                context,
                typeof(TDestination)
                );
        }

        public TDestination Current()
        {
            return Map(Node.GetCurrent());
        }

        public IEnumerable<TDestination> Many(IEnumerable<int> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException("nodeIds");
            }

            var filteredIds = FilterCollection(ids.ToArray(), null);

            return filteredIds.Select(id => Find(id));
        }

        public IEnumerable<TDestination> Many(IEnumerable<Node> nodes)
        {
            if (nodes == null)
            {
                throw new ArgumentNullException("nodes");
            }

            var filteredIds = FilterCollection(nodes.Select(x => x.Id).ToArray(), nodes);

            return nodes
                .Where(n => filteredIds.Contains(n.Id))
                .Select(n => Map(n));
        }

        public IEnumerable<TProperty> SelectProperty<TProperty>(
            Expression<Func<TDestination, TProperty>> property
            )
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable

        /// <summary>
        /// Gets and enumerator of mapped instances of every node which 
        /// can be mapped to <typeparamref name="TDestination"/>. 
        /// </summary>
        public IEnumerator<TDestination> GetEnumerator()
        {
            var destinationType = typeof(TDestination);

            if (!_engine.NodeMappers.ContainsKey(destinationType))
            {
                throw new MapNotFoundException(destinationType);
            }

            string cacheKey = string.Format(
                _isExplicit ? _explicitCacheFormat : _allCacheFormat,
                destinationType.FullName
                );

            if (_engine.CacheProvider != null
                && _engine.CacheProvider.ContainsKey(cacheKey))
            {
                var ids = _engine.CacheProvider.Get(cacheKey) as int[];
                return Many(ids).GetEnumerator();
            }

            // Check whether to include derived maps
            var sourceNodeTypeAliases = _isExplicit
                ? new[] { _engine.NodeMappers[destinationType].SourceDocumentType.Alias }
                : _engine.GetCompatibleNodeTypeAliases(destinationType);

            var nodes = sourceNodeTypeAliases.SelectMany(alias => uQuery.GetNodesByType(alias));

            if (_engine.CacheProvider != null
                && !_engine.CacheProvider.ContainsKey(cacheKey))
            {
                // Cache the node IDs
                _engine.CacheProvider.Insert(cacheKey, nodes.Select(n => n.Id).ToArray());
            }

            return Many(nodes).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
