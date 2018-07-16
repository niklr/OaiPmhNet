using OaiPmhNet.Converters;
using OaiPmhNet.Models;
using System.Collections.Generic;
using System.Linq;

namespace OaiPmhNet.Test
{
    public class SampleSetRepository : ISetRepository
    {
        private readonly IOaiConfiguration _configuration;
        private readonly IList<Set> _sets;

        public SampleSetRepository(IOaiConfiguration configuration, IList<Set> sets)
        {
            _configuration = configuration;
            _sets = sets;
        }

        public SetContainer GetSets(ArgumentContainer arguments, IResumptionToken resumptionToken = null)
        {
            SetContainer container = new SetContainer();

            IQueryable<Set> sets = _sets.AsQueryable().OrderBy(s => s.Name);

            int totalCount = sets.Count();

            container.Sets = sets.Take(_configuration.PageSize);

            return container;
        }
    }
}
