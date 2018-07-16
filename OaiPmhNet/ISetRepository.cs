using OaiPmhNet.Models;

namespace OaiPmhNet
{
    public interface ISetRepository
    {
        SetContainer GetSets(ArgumentContainer arguments, IResumptionToken resumptionToken = null);
    }
}
