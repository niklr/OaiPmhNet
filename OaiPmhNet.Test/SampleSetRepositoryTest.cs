using NUnit.Framework;
using OaiPmhNet.Models;
using System.Collections.Generic;
using System.Linq;

namespace OaiPmhNet.Test
{
    [TestFixture]
    public class SampleSetRepositoryTest
    {
        private SampleSetRepository _repository;

        public static SampleSetRepository Create()
        {
            IList<Set> sets = new List<Set>()
            {
                new Set()
                {
                    Spec = "video",
                    Name = "Video Collection",
                    Description = "This set contains videos."
                },
                new Set()
                {
                    Spec = "image",
                    Name = "Image Collection"
                }
            };

            IOaiConfiguration configuration = OaiConfiguration.Instance;

            return new SampleSetRepository(configuration, sets);
        }

        [OneTimeSetUp]
        public void Init()
        {
            OaiConfigurationTest.Init();

            _repository = Create();
        }

        [Test]
        public void SampleSetRepository_GetSets()
        {
            var expected = 2;

            var actual = _repository.GetSets(new ArgumentContainer(OaiVerb.ListSets.ToString())).Sets.Count();

            Assert.AreEqual(expected, actual);
        }
    }
}
