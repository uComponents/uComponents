using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using umbraco.NodeFactory;
using umbraco;
using umbraco.cms.businesslogic.web;

namespace uComponents.Mapping
{
    /// <summary>
    /// Mapper which maps from Umbraco Node properties to strongly typed model properties
    /// </summary>
    internal class NodeMapper : INodeMapper
    {
        public NodeMappingEngine Engine { get; private set; }
        public Type DestinationType { get; private set; }
        public List<NodePropertyMapper> PropertyMappers { get; private set; }
        public string SourceNodeTypeAlias { get; private set; }

        public NodeMapper(NodeMappingEngine engine, Type destinationType, DocumentType sourceDocumentType)
        {
            if (sourceDocumentType == null)
            {
                throw new ArgumentNullException("sourceDocumentType");
            }
            else if (engine == null)
            {
                throw new ArgumentNullException("engine");
            }
            else if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }

            SourceNodeTypeAlias = sourceDocumentType.Alias;
            Engine = engine;
            DestinationType = destinationType;
            PropertyMappers = new List<NodePropertyMapper>();

            // See if base properties have been mapped already.
            var baseNodeMapper = Engine.GetParentNodeMapperForType(destinationType);
            if (baseNodeMapper != null)
            {
                // Use the property mappings of the closest parent
                // (ideally they will already have the mappings of all their ancestors)
                PropertyMappers.AddRange(baseNodeMapper.PropertyMappers);
            }

            // Map properties
            foreach (var destinationProperty in destinationType.GetProperties())
            {
                if (PropertyMappers.Any(mapper => mapper.DestinationInfo.Name == destinationProperty.Name))
                {
                    // A mapping already exists for this property on a base type.
                    continue;
                }

                NodePropertyMapper customPropertyMapper = null;
                Func<Node, object> defaultPropertyMapping = null;

                // Default node properties
                switch (destinationProperty.Name.ToLowerInvariant())
                {
                    case "createdate":
                        defaultPropertyMapping = node => node.CreateDate;
                        break;
                    case "creatorid":
                        defaultPropertyMapping = node => node.CreatorID;
                        break;
                    case "creatorname":
                        defaultPropertyMapping = node => node.CreatorName;
                        break;
                    case "id":
                        defaultPropertyMapping = node => node.Id;
                        break;
                    case "level":
                        defaultPropertyMapping = node => node.Level;
                        break;
                    case "name":
                        defaultPropertyMapping = node => node.Name;
                        break;
                    case "niceurl":
                        defaultPropertyMapping = node => node.NiceUrl;
                        break;
                    case "nodetypealias":
                        defaultPropertyMapping = node => node.NodeTypeAlias;
                        break;
                    case "path":
                        defaultPropertyMapping = node => node.Path;
                        break;
                    case "sortorder":
                        defaultPropertyMapping = node => node.SortOrder;
                        break;
                    case "template":
                        defaultPropertyMapping = node => node.template;
                        break;
                    case "updatedate":
                        defaultPropertyMapping = node => node.UpdateDate;
                        break;
                    case "url":
                        defaultPropertyMapping = node => node.Url;
                        break;
                    case "urlname":
                        defaultPropertyMapping = node => node.UrlName;
                        break;
                    case "version":
                        defaultPropertyMapping = node => node.Version;
                        break;
                    case "writerid":
                        defaultPropertyMapping = node => node.WriterID;
                        break;
                    case "writername":
                        defaultPropertyMapping = node => node.WriterName;
                        break;
                    default:
                        // Map custom properties
                        var sourcePropertyAlias = sourceDocumentType.PropertyTypes
                            .Select(prop => prop.Alias)
                            .Where(alias => string.Equals(alias, destinationProperty.Name, StringComparison.InvariantCultureIgnoreCase))
                            .SingleOrDefault();

                        if (sourcePropertyAlias != null
                            || destinationProperty.PropertyType.IsModelCollection()
                            || destinationProperty.PropertyType.IsModel())
                        {
                            customPropertyMapper = new NodePropertyMapper(this, destinationProperty, sourcePropertyAlias);
                        }
                        break;
                }

                if (customPropertyMapper != null)
                {
                    PropertyMappers.Add(customPropertyMapper);
                }
                else if (defaultPropertyMapping != null)
                {
                    var defaultNodePropertyMapper = new NodePropertyMapper(this, destinationProperty, defaultPropertyMapping, false);
                    PropertyMappers.Add(defaultNodePropertyMapper);
                }
            }
        }

        /// <summary>
        /// Maps a Node to a strongly typed model, excluding relationships except those specified.
        /// </summary>
        /// <param name="sourceNode">The node to map from</param>
        /// <param name="includedRelationships">An array of properties on the model which
        /// relationships should be mapped to, or null to map all properties.</param>
        public object MapNode(Node sourceNode, PropertyInfo[] includedRelationships)
        {
            var destination = Activator.CreateInstance(DestinationType);

            if (includedRelationships != null)
            {
                // Check relationships actually refer to relationships
                foreach (var relationshipInfo in includedRelationships)
                {
                    var propertyMapper = PropertyMappers.SingleOrDefault(x => x.DestinationInfo == relationshipInfo);

                    if (propertyMapper == null)
                    {
                        throw new ArgumentException(
                            string.Format(@"The property '{0}' specified by 'includedRelationships' does 
not have a valid map", relationshipInfo.Name), "includedRelationships"
                         );
                    }
                    else if (!propertyMapper.IsRelationship)
                    {
                        throw new ArgumentException(@"One of the properties on 'destinationType' does not 
refer to a relationship.", "includedRelationships");
                    }
                }
            }

            foreach (var propertyMapper in PropertyMappers)
            {
                if (includedRelationships == null // include all relationships
                    || !propertyMapper.IsRelationship  // map all non-relationship properties
                    || includedRelationships.Contains(propertyMapper.DestinationInfo)) // check this relationship is included
                {
                    var destinationValue = propertyMapper.MapProperty(sourceNode);
                    propertyMapper.DestinationInfo.SetValue(destination, destinationValue, null);
                }
            }

            return destination;
        }
    }
}
