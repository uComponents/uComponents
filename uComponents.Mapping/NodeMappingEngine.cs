using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using umbraco;
using umbraco.cms.businesslogic.web;
using umbraco.NodeFactory;

namespace uComponents.Mapping
{
    public class NodeMappingEngine
    {
        private readonly static MethodInfo _getNodePropertyMethod = typeof(NodeExtensions).GetMethod("GetProperty");
        private Dictionary<Type, NodeMap> _mappings = new Dictionary<Type, NodeMap>();

        public void CreateMap<TDestination>(string documentTypeAlias)
            where TDestination : class, new()
        {
            var destinationType = typeof(TDestination);

            // Already have a mapping
            if (_mappings.ContainsKey(destinationType))
            {
                return;
            }

            // Get document type
            var docType = DocumentType.GetByAlias(documentTypeAlias);

            if (docType == null)
            {
                throw new DocumentTypeNotFoundException(documentTypeAlias);
            }

            var nodeMap = new NodeMap<TDestination>();

            foreach (var destinationProperty in destinationType.GetProperties())
            {
                var propertyMap = new NodePropertyMap();
                propertyMap.DestinationInfo = destinationProperty;

                // Default node properties
                switch (destinationProperty.Name.ToLowerInvariant())
                {
                    case "createdate":
                        propertyMap.Mapping = (node, alias) => node.CreateDate;
                        break;
                    case "creatorid":
                        propertyMap.Mapping = (node, alias) => node.CreatorID;
                        break;
                    case "creatorname":
                        propertyMap.Mapping = (node, alias) => node.CreatorName;
                        break;
                    case "id":
                        propertyMap.Mapping = (node, alias) => node.Id;
                        break;
                    case "level":
                        propertyMap.Mapping = (node, alias) => node.Level;
                        break;
                    case "name":
                        propertyMap.Mapping = (node, alias) => node.Name;
                        break;
                    case "nodetypealias":
                        propertyMap.Mapping = (node, alias) => node.NodeTypeAlias;
                        break;
                    case "path":
                        propertyMap.Mapping = (node, alias) => node.Path;
                        break;
                    case "sortorder":
                        propertyMap.Mapping = (node, alias) => node.SortOrder;
                        break;
                    case "template":
                        propertyMap.Mapping = (node, alias) => node.template;
                        break;
                    case "updatedate":
                        propertyMap.Mapping = (node, alias) => node.UpdateDate;
                        break;
                    case "url":
                        propertyMap.Mapping = (node, alias) => node.Url;
                        break;
                    case "urlname":
                        propertyMap.Mapping = (node, alias) => node.UrlName;
                        break;
                    case "version":
                        propertyMap.Mapping = (node, alias) => node.Version;
                        break;
                    case "writerid":
                        propertyMap.Mapping = (node, alias) => node.WriterID;
                        break;
                    case "writername":
                        propertyMap.Mapping = (node, alias) => node.WriterName;
                        break;
                    default:
                        // Map custom properties
                        var sourcePropertyAlias = docType.PropertyTypes
                            .Select(prop => prop.Alias)
                            .Where(alias => string.Equals(alias, destinationProperty.Name, StringComparison.InvariantCultureIgnoreCase))
                            .SingleOrDefault();

                        if (sourcePropertyAlias != null)
                        {
                            MapPropertyFromAlias(propertyMap, sourcePropertyAlias);
                        }
                        break;
                }

                if (propertyMap.Mapping != null)
                {
                    nodeMap.PropertyMappings.Add(propertyMap);
                }
            }

            _mappings[destinationType] = nodeMap;
        }

        public void CreateMap<TDestination>()
            where TDestination : class, new()
        {
            var destinationType = typeof(TDestination);

            this.CreateMap<TDestination>(destinationType.Name);
        }

        private void MapPropertyFromAlias(NodePropertyMap propertyMap, string sourcePropertyAlias)
        {
            propertyMap.SourceAlias = sourcePropertyAlias;
            var destinationProperty = propertyMap.DestinationInfo;

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

                    propertyMap.Mapping = (node, alias) =>
                    {
                        var ids = propertyMap.GetNodeIds(node);

                        return assignCollectionDirectly
                            ? ids
                            : Activator.CreateInstance(destinationProperty.PropertyType, ids);
                    };

                    propertyMap.IsRelationship = false;
                }
                else
                {
                    // Map relationship
                    var sourceCollectionType = typeof(IEnumerable<>).MakeGenericType(itemType);
                    bool assignCollectionDirectly = CheckCollectionCanBeAssigned(destinationProperty.PropertyType, sourceCollectionType);

                    propertyMap.Mapping = (node, alias) =>
                    {
                        var ids = propertyMap.GetNodeIds(node);
                        var sourceListType = typeof(List<>).MakeGenericType(itemType);
                        var items = Activator.CreateInstance(sourceListType);

                        foreach (var id in ids)
                        {
                            var itemNode = new Node(id);

                            if (!string.IsNullOrEmpty(node.Name))
                            {
                                var item = Map(itemNode, itemType, false);
                                // items.Add(item) but for generic list
                                sourceListType.InvokeMember("Add", BindingFlags.InvokeMethod, null, items, new object[] { item });
                            }
                        }

                        return assignCollectionDirectly
                            ? items
                            : Activator.CreateInstance(destinationProperty.PropertyType, items);
                    };

                    propertyMap.IsRelationship = true;
                }
            }
            else if (destinationProperty.PropertyType.Module.ScopeName == "CommonLanguageRuntimeLibrary")
            {
                // Basic system types
                var method = _getNodePropertyMethod.MakeGenericMethod(destinationProperty.PropertyType);

                propertyMap.Mapping = (node, alias) => method.Invoke(null, new object[] { node, alias });
                propertyMap.IsRelationship = false;
            }
            else
            {
                // Try to map single relationship
                propertyMap.Mapping = (node, alias) =>
                {
                    if (!_mappings.ContainsKey(destinationProperty.PropertyType))
                    {
                        throw new MapNotFoundException(destinationProperty.PropertyType);
                    }

                    var id = node.GetProperty<int?>(alias);

                    if (id.HasValue)
                    {
                        var relatedNode = new Node(id.Value);

                        if (!string.IsNullOrEmpty(relatedNode.Name))
                        {
                            return Map(relatedNode, destinationProperty.PropertyType, false);
                        }
                    }

                    return null;
                };

                propertyMap.IsRelationship = true;
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
        
        private object Map(Node sourceNode, Type destinationType, bool includeRelationships)
        {
            if (sourceNode == null
                || string.IsNullOrEmpty(sourceNode.Name))
            {
                return null;
            }

            if (!_mappings.ContainsKey(destinationType))
            {
                throw new MapNotFoundException(destinationType);
            }

            var mapping = _mappings[destinationType];
            var destination = Activator.CreateInstance(destinationType);

            foreach (var propertyMapping in mapping.PropertyMappings)
            {
                if (includeRelationships || !propertyMapping.IsRelationship)
                {
                    var destinationValue = propertyMapping.Mapping(sourceNode, propertyMapping.SourceAlias);
                    propertyMapping.DestinationInfo.SetValue(destination, destinationValue, null);
                }
            }

            return destination;
        }

        public TDestination Map<TDestination>(Node sourceNode, bool includeRelationships)
            where TDestination : class, new()
        {
            return (TDestination)Map(sourceNode, typeof(TDestination), includeRelationships);
        }
    }

    #region Exceptions

    public class DocumentTypeNotFoundException : Exception
    {
        public string Message { get; private set; }

        public DocumentTypeNotFoundException(string documentTypeAlias)
        {
            this.Message = string.Format(@"The document type with alias '{0}' could not be found.  
Consider using the overload of CreateMap which specifies a document type alias", documentTypeAlias);
        }
    }

    public class MapNotFoundException : Exception
    {
        public string Message { get; private set; }

        public MapNotFoundException(Type destinationType)
        {
            this.Message = string.Format(@"No map could be found for type '{0}'.  Remember
to run CreateMap for every model type you are using.", destinationType.FullName);
        }
    }

    public class CollectionTypeNotSupported : Exception
    {
        public string Message { get; private set; }

        public CollectionTypeNotSupported(Type type)
        {
            this.Message = string.Format(@"Could not map to collection of type '{0}'.  
Use IEnumerable, or alternatively make sure your collection type has
a single parameter constructor which takes an IEnumerable (such as List)", type.FullName);
        }
    }

    #endregion
}
