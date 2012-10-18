using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using umbraco.NodeFactory;
using umbraco;
using System.Collections;

namespace uComponents.Mapping
{
    public class NodePropertyMap
    {
        public NodeMappingEngine Engine { get; set; }
        public PropertyInfo DestinationInfo { get; set; }
        public string SourceAlias { get; set; }

        /// <summary>
        /// A function taking the node and property alias which returns
        /// the strongly typed property value.
        /// </summary>
        public Func<Node, string, object> Mapping { get; set; }
        public bool IsRelationship { get; set; }

        public NodePropertyMap(NodeMappingEngine engine)
        {
            if (engine == null)
            {
                throw new ArgumentNullException("engine");
            }

            Engine = engine;
        }

        public NodePropertyMap(NodeMappingEngine engine, PropertyInfo destinationProperty, string sourcePropertyAlias)
        {
            if (engine == null)
            {
                throw new ArgumentNullException("engine");
            }

            Engine = engine;

            SourceAlias = sourcePropertyAlias;

            // Mappings
            if (destinationProperty.PropertyType != typeof(string)
                && typeof(IEnumerable).IsAssignableFrom(destinationProperty.PropertyType))
            {
                // A collection
                var itemType = destinationProperty.PropertyType.GetGenericArguments().FirstOrDefault();

                if (itemType == null || itemType.IsAssignableFrom(typeof(int)))
                {
                    // Map IDs
                    bool assignCollectionDirectly = CheckCollectionCanBeAssigned(destinationProperty.PropertyType, typeof(IEnumerable<int>));

                    Mapping = (node, alias) =>
                    {
                        var ids = GetNodeIds(node);

                        return assignCollectionDirectly
                            ? ids
                            : Activator.CreateInstance(destinationProperty.PropertyType, ids);
                    };

                    IsRelationship = false;
                }
                else
                {
                    // Map relationship
                    var sourceCollectionType = typeof(IEnumerable<>).MakeGenericType(itemType);
                    bool assignCollectionDirectly = CheckCollectionCanBeAssigned(destinationProperty.PropertyType, sourceCollectionType);

                    Mapping = (node, alias) =>
                    {
                        var ids = GetNodeIds(node);
                        var sourceListType = typeof(List<>).MakeGenericType(itemType);
                        var items = Activator.CreateInstance(sourceListType);

                        foreach (var id in ids)
                        {
                            var itemNode = new Node(id);

                            if (!string.IsNullOrEmpty(node.Name))
                            {
                                var item = engine.Map(itemNode, itemType, false);
                                // items.Add(item) but for generic list
                                sourceListType.InvokeMember("Add", BindingFlags.InvokeMethod, null, items, new object[] { item });
                            }
                        }

                        return assignCollectionDirectly
                            ? items
                            : Activator.CreateInstance(destinationProperty.PropertyType, items);
                    };

                    IsRelationship = true;
                }
            }
            else if (destinationProperty.PropertyType.Module.ScopeName == "CommonLanguageRuntimeLibrary")
            {
                // Basic system types
                var method = NodeMappingEngine.GetNodePropertyMethod.MakeGenericMethod(destinationProperty.PropertyType);

                Mapping = (node, alias) => method.Invoke(null, new object[] { node, alias });
                IsRelationship = false;
            }
            else
            {
                // Try to map single relationship
                Mapping = (node, alias) =>
                {
                    if (!engine.Mappings.ContainsKey(destinationProperty.PropertyType))
                    {
                        throw new MapNotFoundException(destinationProperty.PropertyType);
                    }

                    var id = node.GetProperty<int?>(alias);

                    if (id.HasValue)
                    {
                        var relatedNode = new Node(id.Value);

                        if (!string.IsNullOrEmpty(relatedNode.Name))
                        {
                            return engine.Map(relatedNode, destinationProperty.PropertyType, false);
                        }
                    }

                    return null;
                };

                IsRelationship = true;
            }
        }

        /// <summary>
        /// Checks collection assignment and instantation
        /// </summary>
        /// <param name="destinationCollectionType">The type of the collection to populate</param>
        /// <param name="sourceCollectionType">The type of collection the items are coming from</param>
        /// <returns>True if the collection can be directly assigned,
        /// False if the collection needs to be instatiated.</returns>
        /// <exception cref="CollectionTypeNotSupported">The collection type cannot be 
        /// instatiated or assigned.</exception>
        private bool CheckCollectionCanBeAssigned(Type destinationCollectionType, Type sourceCollectionType)
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
                    throw new CollectionTypeNotSupported(destinationCollectionType);
                }
            }

            return assignCollectionDirectly;
        }

        /// <summary>
        /// Gets the CSV string of node IDs (or null) and gets the
        /// nodes IDs as integers.  Does not make sure the nodes
        /// actually exist.
        /// </summary>
        /// <exception cref="RelationPropertyFormatNotSupported">
        /// If propertyValue is not a CSV comma separated list of node IDs.
        /// </exception>
        public IEnumerable<int> GetNodeIds(Node node)
        {
            var csv = node.GetProperty<string>(SourceAlias);
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

    public class RelationPropertyFormatNotSupported : Exception
    {
        public string Message { get; private set; }

        public RelationPropertyFormatNotSupported(string propertyValue, Type destinationType)
        {
            this.Message = string.Format(@"Could not parse '{0}' into integer IDs for destination type '{1}'.  
Trying storing your relation properties as CSV (e.g. '1234,2345,4576')", propertyValue, destinationType.FullName);
        }
    }
}
