using OaiPmhNet.Models;

namespace OaiPmhNet
{
    public interface IRecordRepository
    {
        Record GetRecord(string identifier, string metadataPrefix);
        RecordContainer GetRecords(ArgumentContainer arguments, IResumptionToken resumptionToken = null);
        RecordContainer GetIdentifiers(ArgumentContainer arguments, IResumptionToken resumptionToken = null);
    }
}
