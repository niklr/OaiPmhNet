using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using OaiPmhNet.Converters;
using OaiPmhNet.Models;

namespace OaiPmhNet
{
    /// <summary>
    /// A Data Provider is a participant in the OAI-PMH framework. It administers systems that support the OAI-PMH as a means of exposing metadata.
    /// </summary>
    public class DataProvider
    {
        private readonly IOaiConfiguration _configuration;
        private readonly IDateConverter _dateConverter;
        private readonly IResumptionTokenConverter _resumptionTokenConverter;
        private readonly IMetadataFormatRepository _metadataFormatRepository;
        private readonly IRecordRepository _recordRepository;
        private readonly ISetRepository _setRepository;

        public DataProvider(IOaiConfiguration configuration, IMetadataFormatRepository metadataFormatRepository, 
            IRecordRepository recordRepository, ISetRepository setRepository)
            : this(configuration, metadataFormatRepository, recordRepository, setRepository,
                  new DateConverter(), new ResumptionTokenConverter(configuration, new DateConverter()))
        {
        }

        public DataProvider(IOaiConfiguration configuration, IMetadataFormatRepository metadataFormatRepository, IRecordRepository recordRepository,
            ISetRepository setRepository, IDateConverter dateConverter, IResumptionTokenConverter resumptionTokenConverter)
        {
            _configuration = configuration;
            _dateConverter = dateConverter;
            _resumptionTokenConverter = resumptionTokenConverter;
            _metadataFormatRepository = metadataFormatRepository;
            _recordRepository = recordRepository;
            _setRepository = setRepository;
        }

        /// <summary>
        /// Transforms the provided OAI-PMH arguments to an XML document.
        /// </summary>
        /// <param name="date">The date to be included in the response.</param>
        /// <param name="arguments">The OAI-PMH arguments.</param>
        /// <returns>The transformed OAI-PMH arguments as an XML document.</returns>
        public XDocument ToXDocument(DateTime date, ArgumentContainer arguments)
        {
            if (Enum.TryParse(arguments.Verb, out OaiVerb parsedVerb))
            {
                switch (parsedVerb)
                {
                    case OaiVerb.Identify:
                        return CreateIdentify(date, arguments);
                    case OaiVerb.ListMetadataFormats:
                        return CreateListMetadataFormats(date, arguments);
                    case OaiVerb.ListRecords:
                        return CreateListIdentifiersOrRecords(date, arguments, parsedVerb);
                    case OaiVerb.ListIdentifiers:
                        return CreateListIdentifiersOrRecords(date, arguments, parsedVerb);
                    case OaiVerb.GetRecord:
                        return CreateGetRecord(date, arguments);
                    case OaiVerb.ListSets:
                        return CreateListSets(date, arguments);
                    case OaiVerb.None:
                    default:
                        return CreateErrorDocument(date, OaiVerb.None, arguments, OaiErrors.BadVerb);
                }
            }
            else
            {
                return CreateErrorDocument(date, OaiVerb.None, arguments, OaiErrors.BadVerb);
            }
        }

        /// <summary>
        /// Transforms the provided OAI-PMH arguments to a string.
        /// </summary>
        /// <param name="date">The date to be included in the response.</param>
        /// <param name="arguments">The OAI-PMH arguments.</param>
        /// <returns>The transformed OAI-PMH arguments as a string.</returns>
        public string ToString(DateTime now, ArgumentContainer arguments)
        {
            return ToString(ToXDocument(now, arguments));
        }

        /// <summary>
        /// Transforms the provided XML document to a string.
        /// </summary>
        /// <param name="document">The XML document to be transformed.</param>
        /// <returns>The transformed XML document as a string.</returns>
        public string ToString(XDocument document)
        {
            string declaration = document.Declaration == null ? new XDeclaration("1.0", "utf-8", "no").ToString() : document.Declaration.ToString();

            return (string.Concat(declaration, Environment.NewLine, document.ToString()));
        }

        /// <summary>
        /// Creates the response in the form of an XML document.
        /// </summary>
        /// <param name="date">The date to be used in the response.</param>
        /// <param name="oaiElements">First OAI element should be the request. 
        /// The second OAI element should be either an error or a verb.</param>
        /// <returns>Complete response as XML document.</returns>
        private XDocument CreateXml(DateTime date, XElement[] oaiElements)
        {
            foreach (XElement oaiElement in oaiElements)
                SetDefaultXNamespace(oaiElement, OaiNamespaces.OaiNamespace);

            return new XDocument(new XDeclaration("1.0", "utf-8", "no"),
                new XElement(OaiNamespaces.OaiNamespace + "OAI-PMH",
                    new XAttribute(XNamespace.Xmlns + "xsi", OaiNamespaces.XsiNamespace),
                    new XAttribute(OaiNamespaces.XsiNamespace + "schemaLocation", OaiNamespaces.OaiSchemaLocation),
                        new XElement(OaiNamespaces.OaiNamespace + "responseDate",
                            _dateConverter.Encode(_configuration.Granularity, date)),
                            oaiElements));
        }

        private void SetDefaultXNamespace(XElement xelem, XNamespace xmlns)
        {
            if (xelem.Name.NamespaceName == string.Empty)
                xelem.Name = xmlns + xelem.Name.LocalName;

            foreach (var currentElement in xelem.Elements())
                SetDefaultXNamespace(currentElement, xmlns);
        }

        private XElement CreateRequest(OaiVerb verb, ArgumentContainer arguments)
        {
            IList<object> attributes = new List<object>();

            switch (verb)
            {
                case OaiVerb.None:
                    // don't add any attributes
                    break;
                default:
                    attributes.Add(new XAttribute("verb", verb.ToString()));
                    if (!string.IsNullOrWhiteSpace(arguments.Identifier))
                        attributes.Add(new XAttribute("identifier", arguments.Identifier));
                    if (!string.IsNullOrWhiteSpace(arguments.MetadataPrefix))
                        attributes.Add(new XAttribute("metadataPrefix", arguments.MetadataPrefix));
                    if (!string.IsNullOrWhiteSpace(arguments.ResumptionToken))
                        attributes.Add(new XAttribute("resumptionToken", arguments.ResumptionToken));
                    if (!string.IsNullOrWhiteSpace(arguments.From) && _dateConverter.TryDecode(arguments.From, out DateTime fromDateTime))
                        attributes.Add(new XAttribute("from", arguments.From));
                    if (!string.IsNullOrWhiteSpace(arguments.Until) && _dateConverter.TryDecode(arguments.Until, out DateTime untilDateTime))
                        attributes.Add(new XAttribute("until", arguments.Until));
                    if (!string.IsNullOrWhiteSpace(arguments.Set))
                        attributes.Add(new XAttribute("set", arguments.Set));
                    break;
            }

            return new XElement("request", _configuration.BaseUrl(), attributes);
        }

        private XDocument CreateErrorDocument(DateTime date, OaiVerb verb, ArgumentContainer arguments, XElement error)
        {
            IList<XElement> root = new List<XElement>();
            root.Add(CreateRequest(verb, arguments));
            root.Add(error);

            return CreateXml(date, root.ToArray());
        }

        private XElement CreateHeaderXElement(RecordHeader header)
        {
            XElement element = new XElement("header");

            TryAddXElement(element, "identifier", header.Identifier);
            TryAddXElement(element, "datestamp", _dateConverter.Encode(_configuration.Granularity, header.Datestamp));
            foreach (var setSpec in header.SetSpecs)
                TryAddXElement(element, "setSpec", setSpec);
            TryAddXElement(element, "status", header.Status);

            return element;
        }

        private XElement CreateMetadataXElement(RecordMetadata metadata)
        {
            return new XElement("metadata", metadata.Content);
        }

        private void TryAddXElement(XElement root, XName name, string content)
        {
            if (!string.IsNullOrWhiteSpace(content))
                root.Add(new XElement(name, content));
        }

        private string GetDisplayGranularity()
        {
            if (string.IsNullOrWhiteSpace(_configuration.Granularity))
            {
                return string.Empty;
            }
            else
            {
                string granularity = _configuration.Granularity.Replace("'", "").ToLowerInvariant();
                switch (granularity)
                {
                    case "yyyy-mm-dd":
                        return "YYYY-MM-DD";
                    case "yyyy-mm-ddthh:mm:ssz":
                        return "YYYY-MM-DDThh:mm:ssZ";
                    default:
                        return string.Empty;
                }
            }
        }

        #region OAI-PMH 2.0 verbs

        private XDocument CreateGetRecord(DateTime date, ArgumentContainer arguments)
        {
            OaiVerb verb = OaiVerb.GetRecord;

            OaiArgument allowedArguments = OaiArgument.Identifier | OaiArgument.MetadataPrefix;

            if (!OaiErrors.ValidateArguments(arguments, allowedArguments, out XElement errorElement))
                return CreateErrorDocument(date, verb, arguments, errorElement);

            // Check if required identifier is included in the request
            if (string.IsNullOrWhiteSpace(arguments.Identifier))
                return CreateErrorDocument(date, verb, arguments, OaiErrors.BadIdentifierArgument);

            // Check if required metadata prefix is included in the request
            if (string.IsNullOrWhiteSpace(arguments.MetadataPrefix))
                return CreateErrorDocument(date, verb, arguments, OaiErrors.BadMetadataArgument);

            // Check if metadata prefix is supported
            var metadataPrefix = _metadataFormatRepository.GetMetadataFormat(arguments.MetadataPrefix);
            if (metadataPrefix == null)
                return CreateErrorDocument(date, verb, arguments, OaiErrors.CannotDisseminateFormat);

            Record record = _recordRepository.GetRecord(arguments.Identifier, arguments.MetadataPrefix);

            if (record == null)
                return CreateErrorDocument(date, verb, arguments, OaiErrors.IdDoesNotExist);

            IList<XElement> root = new List<XElement>();
            root.Add(CreateRequest(verb, arguments));

            XElement content = new XElement(verb.ToString());
            root.Add(content);

            XElement recordElement = new XElement("record");
            content.Add(recordElement);

            // Header
            if (record.Header != null)
                recordElement.Add(CreateHeaderXElement(record.Header));
            // Metadata
            if (record.Metadata != null)
                recordElement.Add(CreateMetadataXElement(record.Metadata));

            return CreateXml(date, root.ToArray());
        }

        private XDocument CreateIdentify(DateTime date, ArgumentContainer arguments)
        {
            OaiVerb verb = OaiVerb.Identify;

            OaiArgument allowedArguments = OaiArgument.None;

            if (!OaiErrors.ValidateArguments(arguments, allowedArguments, out XElement errorElement))
                return CreateErrorDocument(date, verb, arguments, errorElement);

            IList<XElement> root = new List<XElement>();
            root.Add(CreateRequest(verb, arguments));

            XElement content = new XElement(verb.ToString());
            root.Add(content);

            TryAddXElement(content, "repositoryName", _configuration.RepositoryName);
            TryAddXElement(content, "baseURL", _configuration.BaseUrl());
            TryAddXElement(content, "protocolVersion", _configuration.ProtocolVersion);

            if (_configuration.AdminEmails != null)
            {
                foreach (var adminEmail in _configuration.AdminEmails)
                    TryAddXElement(content, "adminEmail", adminEmail);
            }

            TryAddXElement(content, "earliestDatestamp", _configuration.EarliestDatestamp);
            TryAddXElement(content, "deletedRecord", _configuration.DeletedRecord);
            TryAddXElement(content, "granularity", GetDisplayGranularity());

            if (_configuration.Compressions != null)
            {
                foreach (var compression in _configuration.Compressions)
                    TryAddXElement(content, "compression", compression);
            }

            if (_configuration.Descriptions != null)
            {
                foreach (var description in _configuration.Descriptions)
                    TryAddXElement(content, "description", description);
            }

            return CreateXml(date, root.ToArray());
        }

        private XDocument CreateListMetadataFormats(DateTime date, ArgumentContainer arguments)
        {
            OaiVerb verb = OaiVerb.ListMetadataFormats;

            OaiArgument allowedArguments = OaiArgument.Identifier;

            if (!OaiErrors.ValidateArguments(arguments, allowedArguments, out XElement errorElement))
                return CreateErrorDocument(date, verb, arguments, errorElement);

            if (!string.IsNullOrWhiteSpace(arguments.Identifier) && _recordRepository.GetRecord(arguments.Identifier, arguments.MetadataPrefix) == null)
                return CreateErrorDocument(date, verb, arguments, OaiErrors.IdDoesNotExist);

            var formats = _metadataFormatRepository.GetMetadataFormats().OrderBy(e => e.Prefix);

            if (formats.Count() <= 0)
                return CreateErrorDocument(date, verb, arguments, OaiErrors.NoMetadataFormats);

            IList<XElement> root = new List<XElement>();
            root.Add(CreateRequest(verb, arguments));

            XElement content = new XElement(verb.ToString());
            root.Add(content);

            foreach (var format in formats)
            {
                XElement formatElement = new XElement("metadataFormat");
                content.Add(formatElement);
                TryAddXElement(formatElement, "metadataPrefix", format.Prefix);
                TryAddXElement(formatElement, "schema", format.Schema?.ToString());
                TryAddXElement(formatElement, "metadataNamespace", format.Namespace?.ToString());
            }

            return CreateXml(date, root.ToArray());
        }

        private XDocument CreateListIdentifiersOrRecords(DateTime date, ArgumentContainer arguments, OaiVerb verb, IResumptionToken resumptionToken = null)
        {
            OaiArgument allowedArguments = OaiArgument.MetadataPrefix | OaiArgument.ResumptionToken |
                                           OaiArgument.From | OaiArgument.Until | OaiArgument.Set;

            if (!OaiErrors.ValidateArguments(arguments, allowedArguments, out XElement errorElement))
                return CreateErrorDocument(date, verb, arguments, errorElement);

            // Set
            if (!string.IsNullOrWhiteSpace(arguments.Set) && !_configuration.SupportSets)
                return CreateErrorDocument(date, verb, arguments, OaiErrors.NoSetHierarchy);

            // From
            DateTime fromDate = DateTime.MinValue;
            if (!string.IsNullOrWhiteSpace(arguments.From) && !_dateConverter.TryDecode(arguments.From, out fromDate))
                return CreateErrorDocument(date, verb, arguments, OaiErrors.BadFromArgument);

            // Until
            DateTime untilDate = DateTime.MinValue;
            if (!string.IsNullOrWhiteSpace(arguments.Until) && !_dateConverter.TryDecode(arguments.Until, out untilDate))
                return CreateErrorDocument(date, verb, arguments, OaiErrors.BadUntilArgument);

            // The from argument must be less than or equal to the until argument. 
            if (fromDate > untilDate)
                return CreateErrorDocument(date, verb, arguments, OaiErrors.BadFromUntilCombinationArgument);

            // Decode ResumptionToken
            if (resumptionToken == null && !string.IsNullOrWhiteSpace(arguments.ResumptionToken))
            {
                if (!OaiErrors.ValidateArguments(arguments, OaiArgument.ResumptionToken))
                    return CreateErrorDocument(date, verb, arguments, OaiErrors.BadArgumentExclusiveResumptionToken);

                try
                {
                    IResumptionToken decodedResumptionToken = _resumptionTokenConverter.Decode(arguments.ResumptionToken);
                    if (decodedResumptionToken.ExpirationDate >= DateTime.UtcNow)
                        return CreateErrorDocument(date, verb, arguments, OaiErrors.BadResumptionToken);

                    ArgumentContainer resumptionTokenArguments = new ArgumentContainer(
                        verb.ToString(), decodedResumptionToken.MetadataPrefix, arguments.ResumptionToken, null,
                        _dateConverter.Encode(_configuration.Granularity, decodedResumptionToken.From),
                        _dateConverter.Encode(_configuration.Granularity, decodedResumptionToken.Until),
                        decodedResumptionToken.Set);

                    return CreateListIdentifiersOrRecords(date, resumptionTokenArguments, verb, decodedResumptionToken);
                }
                catch (Exception)
                {
                    return CreateErrorDocument(date, verb, arguments, OaiErrors.BadResumptionToken);
                }
            }

            // Check if required metadata prefix is included in the request
            if (string.IsNullOrWhiteSpace(arguments.MetadataPrefix))
                return CreateErrorDocument(date, verb, arguments, OaiErrors.BadMetadataArgument);

            // Check if metadata prefix is supported
            var metadataPrefix = _metadataFormatRepository.GetMetadataFormat(arguments.MetadataPrefix);
            if (metadataPrefix == null)
                return CreateErrorDocument(date, verb, arguments, OaiErrors.CannotDisseminateFormat);

            var recordContainer = new RecordContainer();

            if (verb == OaiVerb.ListRecords)
                recordContainer = _recordRepository.GetRecords(arguments, resumptionToken);
            else
                recordContainer = _recordRepository.GetIdentifiers(arguments, resumptionToken);

            if (recordContainer == null || recordContainer.Records.Count() <= 0)
                return CreateErrorDocument(date, verb, arguments, OaiErrors.NoRecordsMatch);

            IList<XElement> root = new List<XElement>();
            root.Add(CreateRequest(verb, arguments));

            XElement content = new XElement(verb.ToString());
            root.Add(content);

            foreach (var record in recordContainer.Records)
            {
                XElement recordElement = content;

                if (verb == OaiVerb.ListRecords)
                {
                    recordElement = new XElement("record");
                    content.Add(recordElement);
                }

                // Header
                if (record.Header != null)
                    recordElement.Add(CreateHeaderXElement(record.Header));
                // Metadata
                if (record.Metadata != null && verb == OaiVerb.ListRecords)
                    recordElement.Add(CreateMetadataXElement(record.Metadata));
            }

            if (recordContainer.ResumptionToken != null)
                content.Add(_resumptionTokenConverter.ToXElement(recordContainer.ResumptionToken));

            return CreateXml(date, root.ToArray());
        }

        private XDocument CreateListSets(DateTime date, ArgumentContainer arguments, IResumptionToken resumptionToken = null)
        {
            OaiVerb verb = OaiVerb.ListSets;

            OaiArgument allowedArguments = OaiArgument.ResumptionToken;

            if (!OaiErrors.ValidateArguments(arguments, allowedArguments, out XElement errorElement))
                return CreateErrorDocument(date, verb, arguments, errorElement);

            // Set
            if (!_configuration.SupportSets)
                return CreateErrorDocument(date, verb, arguments, OaiErrors.NoSetHierarchy);

            // Decode ResumptionToken
            if (resumptionToken == null && !string.IsNullOrWhiteSpace(arguments.ResumptionToken))
            {
                if (!OaiErrors.ValidateArguments(arguments, OaiArgument.ResumptionToken))
                    return CreateErrorDocument(date, verb, arguments, OaiErrors.BadArgumentExclusiveResumptionToken);

                IResumptionToken decodedResumptionToken = _resumptionTokenConverter.Decode(arguments.ResumptionToken);
                if (decodedResumptionToken.ExpirationDate >= DateTime.UtcNow)
                    return CreateErrorDocument(date, verb, arguments, OaiErrors.BadResumptionToken);

                ArgumentContainer resumptionTokenArguments = new ArgumentContainer(
                    verb.ToString(), decodedResumptionToken.MetadataPrefix, arguments.ResumptionToken, null,
                    _dateConverter.Encode(_configuration.Granularity, decodedResumptionToken.From),
                    _dateConverter.Encode(_configuration.Granularity, decodedResumptionToken.Until),
                    decodedResumptionToken.Set);

                return CreateListSets(date, resumptionTokenArguments, decodedResumptionToken);
            }

            var setContainer = _setRepository.GetSets(arguments, resumptionToken);

            IList<XElement> root = new List<XElement>();
            root.Add(CreateRequest(verb, arguments));

            XElement content = new XElement(verb.ToString());
            root.Add(content);

            if (setContainer != null)
            {
                foreach (var set in setContainer.Sets)
                {
                    XElement setElement = new XElement("set");
                    content.Add(setElement);
                    TryAddXElement(setElement, "setSpec", set.Spec);
                    TryAddXElement(setElement, "setName", set.Name);
                    TryAddXElement(setElement, "setDescription", set.Description);
                    foreach (var additionalDescription in set.AdditionalDescriptions)
                        setElement.Add(new XElement("setDescription", additionalDescription));
                }

                if (setContainer.ResumptionToken != null)
                    content.Add(_resumptionTokenConverter.ToXElement(setContainer.ResumptionToken));
            }

            return CreateXml(date, root.ToArray());
        }

        #endregion
    }
}
