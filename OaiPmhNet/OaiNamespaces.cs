using System.Xml.Linq;

namespace OaiPmhNet
{
    public class OaiNamespaces
    {
        // Common
        public static XNamespace XsiNamespace = "http://www.w3.org/2001/XMLSchema-instance";

        // OAI
        public static XNamespace OaiNamespace = "http://www.openarchives.org/OAI/2.0/";
        public static XNamespace OaiSchema = "http://www.openarchives.org/OAI/2.0/OAI-PMH.xsd";
        public static XNamespace OaiSchemaLocation = "http://www.openarchives.org/OAI/2.0/ " +
                                                     "http://www.openarchives.org/OAI/2.0/OAI-PMH.xsd";
        public static XNamespace OaiStaticRepositoryNamespace = "http://www.openarchives.org/OAI/2.0/static-repository";

        // Dublin Core
        public static XNamespace DcNamespace = "http://purl.org/dc/elements/1.1/";
        public static XNamespace OaiDcNamespace = "http://www.openarchives.org/OAI/2.0/oai_dc/";
        public static XNamespace OaiDcSchema = "http://www.openarchives.org/OAI/2.0/oai_dc.xsd";
        public static XNamespace OaiDcSchemaLocation = "http://www.openarchives.org/OAI/2.0/oai_dc/ " +
                                                       "http://www.openarchives.org/OAI/2.0/oai_dc.xsd";
    }
}
