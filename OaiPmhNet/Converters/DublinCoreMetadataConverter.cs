using System;
using System.Xml.Linq;
using OaiPmhNet.Models;

namespace OaiPmhNet.Converters
{
    public class DublinCoreMetadataConverter : MetadataConverter<DublinCoreMetadata>, IDublinCoreMetadataConverter
    {
        public DublinCoreMetadataConverter(IOaiConfiguration configuration, IDateConverter dateConverter)
            : base(configuration, dateConverter)
        {

        }

        public override DublinCoreMetadata Decode(XElement metadata)
        {
            XElement root = metadata.Element(OaiNamespaces.OaiDcNamespace + "dc");

            if (root == null)
                root = metadata;

            return new DublinCoreMetadata()
            {
                Title = DecodeList(root, OaiNamespaces.DcNamespace + "title", (e) => { return e; }),
                Creator = DecodeList(root, OaiNamespaces.DcNamespace + "creator", (e) => { return e; }),
                Subject = DecodeList(root, OaiNamespaces.DcNamespace + "subject", (e) => { return e; }),
                Description = DecodeList(root, OaiNamespaces.DcNamespace + "description", (e) => { return e; }),
                Publisher = DecodeList(root, OaiNamespaces.DcNamespace + "publisher", (e) => { return e; }),
                Contributor = DecodeList(root, OaiNamespaces.DcNamespace + "contributor", (e) => { return e; }),
                Date = DecodeList(root, OaiNamespaces.DcNamespace + "date", (date) =>
                {
                    if (_dateConverter.TryDecode(date, out DateTime dateTime))
                        return dateTime;
                    else
                        return DateTime.MinValue;
                }),
                Type = DecodeList(root, OaiNamespaces.DcNamespace + "type", (e) => { return e; }),
                Format = DecodeList(root, OaiNamespaces.DcNamespace + "format", (e) => { return e; }),
                Identifier = DecodeList(root, OaiNamespaces.DcNamespace + "identifier", (e) => { return e; }),
                Source = DecodeList(root, OaiNamespaces.DcNamespace + "source", (e) => { return e; }),
                Language = DecodeList(root, OaiNamespaces.DcNamespace + "language", (e) => { return e; }),
                Relation = DecodeList(root, OaiNamespaces.DcNamespace + "relation", (e) => { return e; }),
                Coverage = DecodeList(root, OaiNamespaces.DcNamespace + "coverage", (e) => { return e; }),
                Rights = DecodeList(root, OaiNamespaces.DcNamespace + "rights", (e) => { return e; })
            };
        }

        public override XElement Encode(DublinCoreMetadata metadata)
        {
            return new XElement(OaiNamespaces.OaiDcNamespace + "dc",
                    new XAttribute(XNamespace.Xmlns + "oai_dc", OaiNamespaces.OaiDcNamespace),
                    new XAttribute(XNamespace.Xmlns + "dc", OaiNamespaces.DcNamespace),
                    new XAttribute(XNamespace.Xmlns + "xsi", OaiNamespaces.XsiNamespace),
                    new XAttribute(OaiNamespaces.XsiNamespace + "schemaLocation", OaiNamespaces.OaiDcSchemaLocation),
                    EncodeList(OaiNamespaces.DcNamespace + "title", metadata.Title),
                    EncodeList(OaiNamespaces.DcNamespace + "creator", metadata.Creator),
                    EncodeList(OaiNamespaces.DcNamespace + "subject", metadata.Subject),
                    EncodeList(OaiNamespaces.DcNamespace + "description", metadata.Description),
                    EncodeList(OaiNamespaces.DcNamespace + "publisher", metadata.Publisher),
                    EncodeList(OaiNamespaces.DcNamespace + "contributor", metadata.Contributor),
                    EncodeList(OaiNamespaces.DcNamespace + "date", metadata.Date, (date) =>
                    {
                        return _dateConverter.Encode(_configuration.Granularity, date);
                    }),
                    EncodeList(OaiNamespaces.DcNamespace + "type", metadata.Type),
                    EncodeList(OaiNamespaces.DcNamespace + "format", metadata.Format),
                    EncodeList(OaiNamespaces.DcNamespace + "identifier", metadata.Identifier),
                    EncodeList(OaiNamespaces.DcNamespace + "source", metadata.Source),
                    EncodeList(OaiNamespaces.DcNamespace + "language", metadata.Language),
                    EncodeList(OaiNamespaces.DcNamespace + "relation", metadata.Relation),
                    EncodeList(OaiNamespaces.DcNamespace + "coverage", metadata.Coverage),
                    EncodeList(OaiNamespaces.DcNamespace + "rights", metadata.Rights));
        }
    }
}
