using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using umbraco.NodeFactory;
using umbraco;

namespace uComponents.Mapping.Property
{
    internal class CollectionPropertyMapper : PropertyMapperBase
    {
        private CollectionPropertyMapping _mapping;
        private Type _itemType;

        public CollectionPropertyMapper(
            CollectionPropertyMapping mapping,
            NodeMapper nodeMapper,
            PropertyInfo destinationProperty,
            string sourcePropertyAlias
            )
            :base(nodeMapper, destinationProperty, sourcePropertyAlias)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException("mapping");
            }

            RequiresInclude = true;
            AllowCaching = true;
            _mapping = mapping;
        }

        public override object MapProperty(NodeMappingContext context)
        {
            // Implement this in a derived class.
            throw new NotImplementedException();
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

        /// <summary>
        /// If SourcePropertyAlias is specified, parses the CSV string of node IDs (or null) and gets the
        /// existing nodes.
        /// 
        /// If SourcePropertyAlias is null, it gets the descendent nodes which are of the correct node type.
        /// </summary>
        /// <returns>The collection of nodes, or null if a map does not exist for relationDestinationType</returns>
        /// <exception cref="RelationPropertyFormatNotSupported">
        /// If SourcePropertyAlias is not null and propertyValue is not a valid CSV list of IDs.
        /// </exception>
        /// <exception cref="MapNotFoundException">If SourcePropertyAlias is not null and
        /// no map exists for relationDestinationType</exception>
        private IEnumerable<Node> GetRelatedNodes(Node node, Type relationDestinationType)
        {
            throw new NotImplementedException();

            if (Engine.NodeMappers.ContainsKey(relationDestinationType))
            {
                if (SourcePropertyAlias != null)
                {
                    // Relation defined by property
                    var csv = node.GetProperty<string>(SourcePropertyAlias);
                    var nodes = new List<Node>();

                    if (!string.IsNullOrWhiteSpace(csv))
                    {
                        foreach (var idString in csv.Split(','))
                        {
                            // Ensure this is actually a list of node IDs
                            int id;
                            if (!int.TryParse(idString.Trim(), out id))
                            {
                                throw new RelationPropertyFormatNotSupported(csv, DestinationInfo.DeclaringType);
                            }

                            var relatedNode = new Node(id);

                            if (!string.IsNullOrEmpty(relatedNode.Name))
                            {
                                nodes.Add(relatedNode);
                            }
                        }
                    }

                    return nodes;
                }
                else
                {
                    // Relation defined by heirarchy
                    var aliases = Engine.GetCompatibleNodeTypeAliases(relationDestinationType);

                    return aliases.SelectMany(alias => node.GetDescendantNodesByType(alias));
                }
            }
            else if (SourcePropertyAlias != null)
            {
                throw new MapNotFoundException(relationDestinationType);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Parses the CSV string of node IDs (or null) and returns the IDs.
        /// Does not ensure the IDs exist.
        /// </summary>
        /// <exception cref="RelationPropertyFormatNotSupported">
        /// If SourcePropertyAlias is not null and propertyValue is not a valid CSV list of IDs.
        /// </exception>
        private IEnumerable<int> GetRelatedNodeIds(Node node)
        {
            throw new NotImplementedException();

            // Relation defined by property
            var csv = node.GetProperty<string>(SourcePropertyAlias);
            var ids = new List<int>();

            if (!string.IsNullOrWhiteSpace(csv))
            {
                foreach (var idString in csv.Split(','))
                {
                    // Ensure this is actually a list of node IDs
                    int id;
                    if (!int.TryParse(idString.Trim(), out id))
                    {
                        throw new RelationPropertyFormatNotSupported(csv, DestinationInfo.DeclaringType);
                    }

                    ids.Add(id);
                }
            }

            return ids;
        }
    }
}
