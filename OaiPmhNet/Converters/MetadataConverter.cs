using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace OaiPmhNet.Converters
{
    public abstract class MetadataConverter<T> : IMetadataConverter<T>
    {
        protected readonly IOaiConfiguration _configuration;
        protected readonly IDateConverter _dateConverter;

        public MetadataConverter(IOaiConfiguration configuration, IDateConverter dateConverter)
        {
            _configuration = configuration;
            _dateConverter = dateConverter;
        }

        public abstract T Decode(XElement metadata);

        public abstract XElement Encode(T metadata);

        public virtual IList<T1> DecodeList<T1>(XElement root, XName element, Func<string, T1> toT1Func)
        {
            IList<T1> decodedList = new List<T1>();

            if (root == null || element == null || toT1Func == null)
                return decodedList;

            foreach (var item in root.Elements(element))
            {
                if (item != null)
                    decodedList.Add(toT1Func(item.Value));
            }

            return decodedList;
        }

        public virtual IList<XElement> EncodeList<T1>(XName name, IList<T1> list, Func<T1, string> toStringFunc = null)
        {
            IList<XElement> encodedList = new List<XElement>();

            if (list == null)
                return encodedList;

            foreach (var item in list)
            {
                if (item != null)
                    encodedList.Add(new XElement(name, toStringFunc == null ? item.ToString() : toStringFunc(item)));
            }

            return encodedList;
        }
    }
}
