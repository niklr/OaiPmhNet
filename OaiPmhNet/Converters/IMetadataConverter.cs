using System.Xml.Linq;

namespace OaiPmhNet.Converters
{
    public interface IMetadataConverter<T>
    {
        T Decode(XElement metadata);
        XElement Encode(T metadata);
    }
}
