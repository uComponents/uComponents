using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using umbraco.NodeFactory;
using umbraco;
using System.Collections;

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
        private readonly List<string> _paths = new List<string>();

        // The engine which will execute the query
        private readonly NodeMappingEngine _engine;

        public NodeQuery(NodeMappingEngine engine)
        {
            if (engine == null)
            {
                throw new ArgumentNullException("engine");
            }

            _engine = engine;
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

        public INodeQuery<TDestination> Include<TProperty>(Expression<Func<TDestination, TProperty>> path)
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

        #region Execute

        public TDestination Map(Node node)
        {
            if (node == null || string.IsNullOrEmpty(node.Name))
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

        public IEnumerable<TDestination> Many(IEnumerable<int> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException("nodeIds");
            }

            return ids.Select(id => Find(id));
        }

        public IEnumerable<TDestination> Many(IEnumerable<Node> nodes)
        {
            if (nodes == null)
            {
                throw new ArgumentNullException("nodes");
            }

            return nodes.Select(n => Map(n));
        }

        public TDestination Current()
        {
            return Map(Node.GetCurrent());
        }

        public IEnumerable<TDestination> Explicit()
        {
            var destinationType = typeof(TDestination);

            if (!_engine.NodeMappers.ContainsKey(destinationType))
            {
                throw new MapNotFoundException(destinationType);
            }

            var cacheKey = string.Format(_explicitCacheFormat, destinationType.FullName);

            if (_engine.CacheProvider != null
                && _engine.CacheProvider.ContainsKey(cacheKey))
            {
                var ids = _engine.CacheProvider.Get(cacheKey) as int[];
                return Many(ids);
            }

            var nodeMapper = _engine.NodeMappers[destinationType];
            var nodes = uQuery.GetNodesByType(nodeMapper.SourceDocumentType.Alias);

            if (_engine.CacheProvider != null)
            {
                // Cache the node IDs
                _engine.CacheProvider.Insert(cacheKey, nodes.Select(n => n.Id).ToArray());
            }

            return Many(nodes);
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

            var cacheKey = string.Format(_allCacheFormat, destinationType.FullName);

            if (_engine.CacheProvider != null
                && _engine.CacheProvider.ContainsKey(cacheKey))
            {
                var ids = _engine.CacheProvider.Get(cacheKey) as int[];
                return Many(ids).GetEnumerator();
            }

            var sourceNodeTypeAliases = _engine.GetCompatibleNodeTypeAliases(destinationType);
            var nodes = sourceNodeTypeAliases.SelectMany(alias => uQuery.GetNodesByType(alias));

            if (_engine.CacheProvider != null)
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
