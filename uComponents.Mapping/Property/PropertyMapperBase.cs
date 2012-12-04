using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using umbraco.NodeFactory;
using umbraco;

namespace uComponents.Mapping.Property
{
    /// <summary>
    /// An immutable mapper for Umbraco Node properties to strongly typed model properties
    /// </summary>
    internal abstract class PropertyMapperBase
    {
        public NodeMappingEngine Engine { get; private set; }
        public PropertyInfo DestinationInfo { get; private set; }
        public string SourcePropertyAlias { get; private set; }
        public bool RequiresInclude { get; protected set; }
        public bool AllowCaching { get; protected set; }

        public PropertyMapperBase(
            NodeMapper nodeMapper,
            PropertyInfo destinationProperty,
            string sourcePropertyAlias
            )
        {
            if (nodeMapper == null)
            {
                throw new ArgumentNullException("nodeMapper");
            }
            else if (destinationProperty == null)
            {
                throw new ArgumentNullException("destinationProperty");
            }
            else if (string.IsNullOrEmpty(sourcePropertyAlias)
                && !(destinationProperty.PropertyType.IsModelCollection()
                    || destinationProperty.PropertyType.IsModel()))
            {
                throw new ArgumentException(string.Format(@"Invalid destination property type '{0}' for a null 
source property alias: A source property alias must be specified when the destination type is not a collection",
                    destinationProperty.PropertyType.FullName));
            }

            Engine = nodeMapper.Engine;
            SourcePropertyAlias = sourcePropertyAlias;
            DestinationInfo = destinationProperty;
        }

        /// <summary>
        /// Infer a mapping based on the type of the destination property.
        /// </summary>
        //        public NodePropertyMapper(
        //            NodeMapper nodeMapper, 
        //            PropertyInfo destinationProperty, 
        //            string sourcePropertyAlias
        //            )
        //        {
        //            if (nodeMapper == null)
        //            {
        //                throw new ArgumentNullException("nodeMapper");
        //            }
        //            else if (destinationProperty == null)
        //            {
        //                throw new ArgumentNullException("destinationProperty");
        //            }
        //            else if (string.IsNullOrEmpty(sourcePropertyAlias)
        //                && !(destinationProperty.PropertyType.IsModelCollection()
        //                    || destinationProperty.PropertyType.IsModel()))
        //            {
        //                throw new ArgumentException(string.Format(@"Invalid destination property type '{0}' for a null 
        //source property alias: A source property alias must be specified when the destination type is not a collection",
        //                    destinationProperty.PropertyType.FullName));
        //            }

        //            Engine = nodeMapper.Engine;
        //            SourcePropertyAlias = sourcePropertyAlias;
        //            DestinationInfo = destinationProperty;

        //            // Mappings
        //            if (destinationProperty.PropertyType.IsModelCollection())
        //            {
        //                // A collection
        //                _itemType = destinationProperty.PropertyType.GetGenericArguments().FirstOrDefault();

        //                if (_itemType == null || _itemType.IsAssignableFrom(typeof(int)))
        //                {
        //                    // Map IDs
        //                    _canAssignCollectionDirectly = CheckCollectionCanBeAssigned(destinationProperty.PropertyType, typeof(IEnumerable<int>));

        //                    _mapping = MapCollectionAsIds;
        //                    RequiresInclude = false;
        //                    AllowCaching = true;
        //                }
        //                else
        //                {
        //                    // Map model collection
        //                    var sourceCollectionType = typeof(IEnumerable<>).MakeGenericType(_itemType);
        //                    _canAssignCollectionDirectly = CheckCollectionCanBeAssigned(destinationProperty.PropertyType, sourceCollectionType);

        //                    _mapping = MapCollectionAsModels;
        //                    RequiresInclude = true;
        //                    AllowCaching = true;
        //                }
        //            }
        //            else if (destinationProperty.PropertyType.IsSystem()
        //                || destinationProperty.PropertyType.IsEnum)
        //            {
        //                // Basic system types
        //                var method = NodeMappingEngine.GetNodePropertyMethod.MakeGenericMethod(destinationProperty.PropertyType);

        //                _mapping = (node, paths) => method.Invoke(null, new object[] { node, SourcePropertyAlias });
        //                RequiresInclude = false;
        //                AllowCaching = true;
        //            }
        //            else if (destinationProperty.PropertyType.IsModel())
        //            {
        //                // Try to map to model
        //                _mapping = MapModel;
        //                RequiresInclude = true;
        //                AllowCaching = true;
        //            }
        //            else
        //            {
        //                throw new NotImplementedException("Cannot map to a property that is not a collection, system type, enum or class");
        //            }
        //        }

        /// <summary>
        /// Maps a node property.
        /// </summary>
        /// <param name="context">The context describing the mapping.</param>
        /// <returns>The strongly typed, mapped property.</returns>
        public abstract object MapProperty(NodeMappingContext context);

        /// <summary>
        /// Gets value of the node property, cached or otherwise.
        /// </summary>
        protected TProperty GetPropertyValue<TProperty>(NodeMappingContext context)
        {
            object value = null;

            if (Engine.CacheProvider != null)
            {
                value = (TProperty)Engine.CacheProvider.GetPropertyValue(context.Id, DestinationInfo.Name);
            }

            if (value == null)
            {
                var node = context.GetNode();

                if (node == null || string.IsNullOrEmpty(node.Name))
                {
                    throw new InvalidOperationException("Node does not exist");
                }

                value = node.GetProperty<TProperty>(SourcePropertyAlias);
            }

            return (TProperty)value;
        }

        /// <summary>
        /// Gets the paths relative to the property being mapped.
        /// </summary>
        protected string[] GetNextLevelPaths(string[] paths)
        {
            if (paths == null)
            {
                // No paths
                return new string[0];
            }

            var pathsForProperty = new List<string>();

            foreach (var path in paths)
            {
                if (string.IsNullOrEmpty(path))
                {
                    throw new ArgumentException("No path can be null or empty", "paths");
                }

                var segments = path.Split('.');

                if (segments.First() == DestinationInfo.Name
                    && segments.Length > 1)
                {
                    pathsForProperty.Add(string.Join(".", segments.Skip(1)));
                }
            }

            return pathsForProperty.ToArray();
        }

        #region Default property mappings

        private object MapCollectionAsIds(Node node, string[] paths)
        {
            var ids = GetRelatedNodeIds(node);

            return _canAssignCollectionDirectly
                ? ids
                : Activator.CreateInstance(DestinationInfo.PropertyType, ids);
        }

        private object MapCollectionAsModels(Node node, string[] paths)
        {
            var relatedNodes = GetRelatedNodes(node, _itemType);

            if (relatedNodes != null)
            {
                var sourceListType = typeof(List<>).MakeGenericType(_itemType);
                var items = Activator.CreateInstance(sourceListType);

                foreach (var relatedNode in relatedNodes)
                {
                    var item = Engine.Map(relatedNode, _itemType, paths);
                    // items.Add(item) but for generic list
                    sourceListType.InvokeMember("Add", BindingFlags.InvokeMethod, null, items, new object[] { item });
                }

                return _canAssignCollectionDirectly
                    ? items
                    : Activator.CreateInstance(DestinationInfo.PropertyType, items);
            }
            else
            {
                return null;
            }
        }

        private object MapModel(Node node, string[] paths)
        {
            if (SourcePropertyAlias != null)
            {
                // Ensure map exists
                if (!Engine.NodeMappers.ContainsKey(DestinationInfo.PropertyType))
                {
                    throw new MapNotFoundException(DestinationInfo.PropertyType);
                }

                // Map to single property relationship
                var id = node.GetProperty<int?>(SourcePropertyAlias);

                if (id.HasValue)
                {
                    var relatedNode = new Node(id.Value);

                    if (!string.IsNullOrEmpty(relatedNode.Name))
                    {
                        return Engine.Map(relatedNode, DestinationInfo.PropertyType, paths);
                    }
                }
            }
            else if (Engine.NodeMappers.ContainsKey(DestinationInfo.PropertyType))
            {
                // Map to ancestor (if possible)
                var aliases = Engine
                    .GetCompatibleNodeTypeAliases(DestinationInfo.PropertyType)
                    .ToArray();
                var ancestorNode = node.GetAncestorNodes()
                    .FirstOrDefault(x => aliases.Contains(x.NodeTypeAlias));

                return Engine.Map(ancestorNode, DestinationInfo.PropertyType, paths);
            }

            return null;
        }

        #endregion
    }

    /// <summary>
    /// The value of a node property which is being mapped by the mapping engine
    /// cannot be parsed to IDs.
    /// </summary>
    public class RelationPropertyFormatNotSupported : Exception
    {
        /// <param name="propertyValue">The unsupported value of the property.</param>
        /// <param name="destinationPropertyType">The destination property type.</param>
        public RelationPropertyFormatNotSupported(string propertyValue, Type destinationPropertyType)
            : base(string.Format(@"Could not parse '{0}' into integer IDs for destination type '{1}'.  
Trying storing your relation properties as CSV (e.g. '1234,2345,4576')", propertyValue, destinationPropertyType.FullName))
        {
        }
    }

    internal static class TypeExtensions
    {
        /// <summary>
        /// Checks if a type is a relationship collection (for our purposes).
        /// </summary>
        public static bool IsModelCollection(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException();
            }

            var isString = (type == typeof(string));
            var isEnumerable = typeof(IEnumerable).IsAssignableFrom(type);
            var isDictionary = type.FullName.StartsWith(typeof(IDictionary).FullName)
                || type.FullName.StartsWith(typeof(IDictionary<,>).FullName)
                || type.FullName.StartsWith(typeof(Dictionary<,>).FullName);

            return !isString && !isDictionary && isEnumerable;
        }

        /// <summary>
        /// Checks if a type comes from the CLR
        /// </summary>
        public static bool IsSystem(this Type type)
        {
            return type.Module.ScopeName == "CommonLanguageRuntimeLibrary";
        }

        /// <summary>
        /// Checks if a type is a model
        /// </summary>
        public static bool IsModel(this Type type)
        {
            return !type.IsModelCollection()
                && !type.IsSystem()
                && !type.IsEnum;
        }
    }
}
