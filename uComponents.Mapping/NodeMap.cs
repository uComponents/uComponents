using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using umbraco.NodeFactory;
using umbraco;

namespace uComponents.Mapping
{
    public class NodeMap
    {
        public Type Type { get; set; }
        public List<NodePropertyMap> PropertyMappings { get; set; }

        public NodeMap(Type destinationType)
        {
            Type = destinationType;
            PropertyMappings = new List<NodePropertyMap>();
        }
    }

    public class NodeMap<TDestination> : NodeMap
    {
        public NodeMap()
            : base(typeof(TDestination))
        {
        }
    }

    public class NodePropertyMap
    {
        public PropertyInfo DestinationInfo { get; set; }
        public string SourceAlias { get; set; }

        /// <summary>
        /// A function taking the node and property alias which returns
        /// the strongly typed property value.
        /// </summary>
        public Func<Node, string, object> Mapping { get; set; }
        public bool IsRelationship { get; set; }

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
