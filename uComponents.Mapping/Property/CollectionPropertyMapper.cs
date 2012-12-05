using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using umbraco.NodeFactory;
using umbraco;
using uComponents.Mapping;

namespace uComponents.Mapping.Property
{
    internal class CollectionPropertyMapper : PropertyMapperBase
    {
        private readonly Func<object, IEnumerable<int>> _mapping;
        private readonly Type _sourcePropertyType;
        private readonly Type _elementType;
        private readonly bool _canAssignDirectly;

        /// <summary>
        /// Maps a collection relationship.
        /// </summary>
        /// <param name="mapping">
        /// Mapping from <paramref name="sourcePropertyType"/> to a collection
        /// of node IDs.  If <c>null</c>, the mapping will be deduced from 
        /// the other parameters.
        /// </param>
        /// <param name="sourcePropertyType">
        /// The type of the first parameter being supplied to <paramref name="mapping"/>.
        /// Cannot be <c>null</c> if <paramref name="mapping"/> is not <c>null</c>.
        /// </param>
        /// <param name="sourcePropertyAlias">
        /// The alias of the node property to map from.  If null, descendants of
        /// the node which are compatible with <paramref name="destinationProperty"/>
        /// will be mapped instead.
        /// </param>
        public CollectionPropertyMapper(
            Func<object, IEnumerable<int>> mapping,
            Type sourcePropertyType,
            NodeMapper nodeMapper,
            PropertyInfo destinationProperty,
            string sourcePropertyAlias
            )
            : base(nodeMapper, destinationProperty, sourcePropertyAlias)
        {
            if (sourcePropertyType == null && mapping != null)
            {
                throw new ArgumentException("Source property type must be specified when setting a mapping");
            }
            else if (sourcePropertyAlias == null && mapping != null)
            {
                throw new ArgumentException("Source property alias must be specified when mapping is specified.");
            }

            _elementType = destinationProperty.PropertyType.GetElementType();
            Type rawCollectionType = null;

            if (_elementType == null)
            {
                throw new CollectionTypeNotSupportedException(destinationProperty.PropertyType);
            }

            if (_elementType == typeof(int))
            {
                // Collection of IDs
                RequiresInclude = false;
                rawCollectionType = typeof(IEnumerable<int>);
            }
            else
            {
                // Collection of models
                RequiresInclude = true;
            }

            // See if the collection can be assigned, or must be instantiated
            _canAssignDirectly = CheckCollectionCanBeAssigned(DestinationInfo.PropertyType, rawCollectionType);

            AllowCaching = true;
            _mapping = mapping;
            _sourcePropertyType = sourcePropertyType;
        }

        public override object MapProperty(NodeMappingContext context)
        {
            IEnumerable<int> ids = null;

            // Get IDs
            if (AllowCaching
                && Engine.CacheProvider != null
                && Engine.CacheProvider.ContainsPropertyValue(context.Id, DestinationInfo.Name))
            {
                ids = Engine.CacheProvider.GetPropertyValue(context.Id, DestinationInfo.Name) as IEnumerable<int>;
            }
            else
            {
                var node = context.GetNode();

                if (node == null || string.IsNullOrEmpty(node.Name))
                {
                    throw new InvalidOperationException("Node cannot be null or empty");
                }

                if (_mapping != null)
                {
                    // Custom mapping
                    ids = _mapping(GetSourcePropertyValue(node, _sourcePropertyType));
                }
                else if (!string.IsNullOrEmpty(SourcePropertyAlias))
                {
                    // Maps IDs from node property
                    ids = GetSourcePropertyValue<IEnumerable<int>>(node);
                }
                else
                {
                    // Get compatible descendants
                    var aliases = Engine.GetCompatibleNodeTypeAliases(_elementType);

                    var nodes = aliases.SelectMany(alias => node.GetDescendantNodesByType(alias));

                    // Might as well store the nodes if we're creating them
                    foreach (var n in nodes)
                    {
                        context.AddNodeToContextCache(n);
                    }

                    ids = nodes.Select(n => n.Id);
                }

                if (AllowCaching
                    && Engine.CacheProvider != null)
                {
                    Engine.CacheProvider.InsertPropertyValue(context.Id, DestinationInfo.Name, ids);
                }
            }

            if (_elementType == typeof(int))
            {
                // Map ID collection
                return _canAssignDirectly
                    ? ids
                    : Activator.CreateInstance(DestinationInfo.PropertyType, ids);
            }

            // Map model collection
            var childPaths = GetNextLevelPaths(context.Paths.ToArray());
            var sourceListType = typeof(List<>).MakeGenericType(_elementType);
            var mappedCollection = Activator.CreateInstance(sourceListType);

            foreach (var id in ids)
            {
                var childContext = new NodeMappingContext(id, childPaths, context);
                var mappedElement = Engine.Map(childContext, _elementType);

                // Like "items.Add(item)" but for generic list
                sourceListType.InvokeMember("Add", BindingFlags.InvokeMethod, null, mappedCollection, new object[] { mappedElement });
            }

            return _canAssignDirectly
                ? mappedCollection
                : Activator.CreateInstance(DestinationInfo.PropertyType, mappedCollection);
        }

        /// <summary>
        /// Checks collection assignment and instantation
        /// </summary>
        /// <param name="destinationCollectionType">The type of the collection to populate</param>
        /// <param name="sourceCollectionType">The type of collection the items are coming from</param>
        /// <returns>True if the collection can be directly assigned,
        /// False if the collection needs to be instatiated.</returns>
        /// <exception cref="CollectionTypeNotSupportedException">The collection type cannot be 
        /// instatiated or assigned.</exception>
        private static bool CheckCollectionCanBeAssigned(Type destinationCollectionType, Type sourceCollectionType)
        {
            bool assignCollectionDirectly;

            // Determine how we go about creating the list
            if (destinationCollectionType.IsAssignableFrom(sourceCollectionType))
            {
                // We can just use the list
                assignCollectionDirectly = true;
            }
            else
            {
                // Look for a constructor on the collection type which 
                // takes a List/IEnumerable
                var hasSuitableConstructor = destinationCollectionType
                    .GetConstructors()
                    .Any(x =>
                    {
                        var parameters = x.GetParameters();
                        return parameters.Count() == 1
                            && parameters.Any(p => p.ParameterType.IsAssignableFrom(sourceCollectionType));
                    });

                if (hasSuitableConstructor)
                {
                    // Instantiate the collection
                    assignCollectionDirectly = false;
                }
                else
                {
                    throw new CollectionTypeNotSupportedException(destinationCollectionType);
                }
            }

            return assignCollectionDirectly;
        }
    }
}
