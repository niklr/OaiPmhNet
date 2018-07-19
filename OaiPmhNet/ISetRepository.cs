using OaiPmhNet.Models;

namespace OaiPmhNet
{
    /// <summary>
    /// The interface representing the repository for sets.
    /// </summary>
    public interface ISetRepository
    {
        /// <summary>
        /// Gets an incomplete list of sets corresponding to the provided arguments and resumption token.
        /// </summary>
        /// <param name="arguments">The OAI-PMH arguments.</param>
        /// <param name="resumptionToken">The optional token used for resumption.</param>
        /// <returns>An incomplete list of sets corresponding to the provided arguments and resumption token.</returns>
        SetContainer GetSets(ArgumentContainer arguments, IResumptionToken resumptionToken = null);
    }
}
