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
        public readonly static MethodInfo GetNodePropertyMethod = typeof(NodeExtensions).GetMethod("GetProperty");
        protected Dictionary<Type, NodeMap> Mappings { get; set; }

        public NodeMappingEngine()
        {
            this.Mappings = new Dictionary<Type, NodeMap>();
        }

        public NodeMappingExpression<TDestination> CreateMap<TDestination>(string documentTypeAlias)
            where TDestination : class, new()
        {
            var destinationType = typeof(TDestination);

            // Already have a mapping
            if (Mappings.ContainsKey(destinationType))
            {
                return new NodeMappingExpression<TDestination>(this, (NodeMap<TDestination>)Mappings[destinationType]);
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
                var propertyMap = new NodePropertyMap(this);
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
                            propertyMap = new NodePropertyMap(this, destinationProperty, sourcePropertyAlias);
                        }
                        break;
                }

                if (propertyMap.Mapping != null)
                {
                    nodeMap.PropertyMappings.Add(propertyMap);
                }
            }

            Mappings[destinationType] = nodeMap;

            return new NodeMappingExpression<TDestination>(this, nodeMap);
        }

        public NodeMappingExpression<TDestination> CreateMap<TDestination>()
            where TDestination : class, new()
        {
            var destinationType = typeof(TDestination);

            return this.CreateMap<TDestination>(destinationType.Name);
        }
        
        public object Map(Node sourceNode, Type destinationType, bool includeRelationships)
        {
            if (sourceNode == null
                || string.IsNullOrEmpty(sourceNode.Name))
            {
                return null;
            }

            if (!Mappings.ContainsKey(destinationType))
            {
                throw new MapNotFoundException(destinationType);
            }

            var mapping = Mappings[destinationType];
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
