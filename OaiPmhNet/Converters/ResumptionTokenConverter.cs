using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using OaiPmhNet.Models;

namespace OaiPmhNet.Converters
{
    public sealed class ResumptionTokenConverter : IResumptionTokenConverter
    {
        private readonly IOaiConfiguration _configuration;
        private readonly IDateConverter _dateConverter;

        public ResumptionTokenConverter(IOaiConfiguration configuration, IDateConverter dateConverter)
        {
            _configuration = configuration;
            _dateConverter = dateConverter;
        }

        public IResumptionToken Decode(string resumptionToken)
        {
            ResumptionToken decoded = new ResumptionToken();

            if (string.IsNullOrWhiteSpace(resumptionToken))
                return decoded;

            NameValueCollection nvc = HttpUtility.ParseQueryString(HttpUtility.UrlDecode(resumptionToken));

            foreach (string key in nvc)
            {
                switch (key.ToLowerInvariant())
                {
                    case "metadataprefix":
                        decoded.MetadataPrefix = nvc[key];
                        break;
                    case "from":
                        if (_dateConverter.TryDecode(nvc[key], out DateTime fromDate))
                            decoded.From = fromDate;
                        break;
                    case "until":
                        if (_dateConverter.TryDecode(nvc[key], out DateTime untilDate))
                            decoded.Until = untilDate;
                        break;
                    case "set":
                        decoded.Set = nvc[key];
                        break;
                    case "expirationdate":
                        if (_dateConverter.TryDecode(nvc[key], out DateTime expirationDate))
                            decoded.ExpirationDate = expirationDate;
                        break;
                    case "completelistsize":
                        if (int.TryParse(nvc[key], out int completeListSize))
                            decoded.CompleteListSize = completeListSize;
                        break;
                    case "cursor":
                        if (int.TryParse(nvc[key], out int cursor))
                            decoded.Cursor = cursor;
                        break;
                    default:
                        if (_configuration.ResumptionTokenCustomParameterNames.Contains(key))
                        {
                            if (decoded.Custom.ContainsKey(key))
                                decoded.Custom[key] = nvc[key];
                            else
                                decoded.Custom.Add(key, nvc[key]);
                        }
                        break;
                }
            }
            return decoded;
        }

        public string Encode(IResumptionToken resumptionToken)
        {
            IList<string> properties = new List<string>();

            if (!string.IsNullOrWhiteSpace(resumptionToken.MetadataPrefix))
                properties.Add(EncodeOne("metadataPrefix", resumptionToken.MetadataPrefix));
            if (resumptionToken.From.HasValue)
                properties.Add(EncodeOne("from", _dateConverter.Encode(_configuration.Granularity, resumptionToken.From)));
            if (resumptionToken.Until.HasValue)
                properties.Add(EncodeOne("until", _dateConverter.Encode(_configuration.Granularity, resumptionToken.Until)));
            if (!string.IsNullOrWhiteSpace(resumptionToken.Set))
                properties.Add(EncodeOne("set", resumptionToken.Set));

            foreach (var custom in resumptionToken.Custom)
            {
                if (_configuration.ResumptionTokenCustomParameterNames.Contains(custom.Key))
                    properties.Add(EncodeOne(custom.Key, custom.Value));
            }

            return HttpUtility.UrlEncode(string.Join("&", properties.ToArray()));
        }

        public string EncodeOne(string name, object value)
        {
            return string.Concat(name, "=", value.ToString());
        }

        public XElement ToXElement(IResumptionToken resumptionToken)
        {
            XElement root = new XElement("resumptionToken", Encode(resumptionToken));

            if (resumptionToken.ExpirationDate.HasValue)
                root.Add(new XAttribute("expirationDate", _dateConverter.Encode(_configuration.Granularity, resumptionToken.ExpirationDate)));
            if (resumptionToken.CompleteListSize.HasValue)
                root.Add(new XAttribute("completeListSize", resumptionToken.CompleteListSize.Value));
            if (resumptionToken.Cursor.HasValue)
                root.Add(new XAttribute("cursor", resumptionToken.Cursor.Value));

            foreach (var custom in resumptionToken.Custom)
            {
                if (_configuration.ResumptionTokenCustomParameterNames.Contains(custom.Key))
                    root.Add(new XAttribute(custom.Key, custom.Value));
            }

            return root;
        }
    }
}
