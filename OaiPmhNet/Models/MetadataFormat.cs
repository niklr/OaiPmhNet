using System.Xml.Linq;

namespace OaiPmhNet.Models
{
    public class MetadataFormat
    {
        public string Prefix { get; private set; }

        public XNamespace Namespace { get; private set; }

        public XNamespace Schema { get; private set; }

        public XNamespace SchemaLocation { get; private set; }

        public MetadataFormat(string prefix, XNamespace ns, XNamespace schema, XNamespace schemaLocation)
        {
            Prefix = prefix;
            Namespace = ns;
            Schema = schema;
            SchemaLocation = schemaLocation;
        }
    }
}
