using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OaiPmhNet.Models;

namespace OaiPmhNet.Test
{
    [TestFixture]
    public class SampleMetadataFormatRepositoryTest
    {
        private SampleMetadataFormatRepository _repository;

        public static SampleMetadataFormatRepository Create()
        {
            IList<MetadataFormat> metadataFormats = new List<MetadataFormat>()
            {
                new MetadataFormat(
                    "oai_dc",
                    OaiNamespaces.OaiDcNamespace,
                    OaiNamespaces.OaiDcSchema,
                    OaiNamespaces.OaiDcSchemaLocation),
                new MetadataFormat(
                    "rdf",
                    "http://www.w3.org/1999/02/22-rdf-syntax-ns#",
                    "http://www.openarchives.org/OAI/2.0/rdf.xsd",
                    "http://www.w3.org/1999/02/22-rdf-syntax-ns# http://www.openarchives.org/OAI/2.0/rdf.xsd")
            };
            return new SampleMetadataFormatRepository(metadataFormats);
        }

        [OneTimeSetUp]
        public void Init()
        {
            OaiConfigurationTest.Init();

            _repository = Create();
        }

        [Test]
        public void SampleMetadataFormatRepository_GetByPrefix()
        {
            var expected = new MetadataFormat(
                    "oai_dc",
                    OaiNamespaces.OaiDcNamespace,
                    OaiNamespaces.OaiDcSchema,
                    OaiNamespaces.OaiDcSchemaLocation);

            var actual = _repository.GetMetadataFormat("oai_dc");

            Assert.AreEqual(expected.Prefix, actual.Prefix);
            Assert.AreEqual(expected.Namespace, actual.Namespace);
            Assert.AreEqual(expected.Schema, actual.Schema);
            Assert.AreEqual(expected.SchemaLocation, actual.SchemaLocation);
        }

        [Test]
        public void SampleMetadataFormatRepository_GetQuery()
        {
            var expected = 2;

            var actual = _repository.GetMetadataFormats().Count();

            Assert.AreEqual(expected, actual);
        }
    }
}
