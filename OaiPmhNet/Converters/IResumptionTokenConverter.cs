using System.Xml.Linq;
using OaiPmhNet.Models;

namespace OaiPmhNet.Converters
{
    public interface IResumptionTokenConverter
    {
        IResumptionToken Decode(string resumptionToken);

        string Encode(IResumptionToken resumptionToken);

        XElement ToXElement(IResumptionToken resumptionToken);
    }
}
