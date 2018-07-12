namespace OaiPmhNet.Models
{
    public class ArgumentContainer
    {
        public string Verb { get; private set; }
        public string MetadataPrefix { get; private set; }
        public string ResumptionToken { get; private set; }
        public string Identifier { get; private set; }
        public string From { get; private set; }
        public string Until { get; private set; }
        public string Set { get; private set; }

        public ArgumentContainer(string verb,
            string metadataPrefix = null,
            string resumptionToken = null,
            string identifier = null,
            string from = null,
            string until = null,
            string set = null)
        {
            Verb = verb;
            MetadataPrefix = metadataPrefix;
            ResumptionToken = resumptionToken;
            Identifier = identifier;
            From = from;
            Until = until;
            Set = set;
        }
    }
}
