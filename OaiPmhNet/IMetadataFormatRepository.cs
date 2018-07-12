using System.Linq;
using OaiPmhNet.Models;

namespace OaiPmhNet
{
    public interface IMetadataFormatRepository
    {
        IQueryable<MetadataFormat> GetQuery();

        MetadataFormat GetByPrefix(string prefix);
    }
}
