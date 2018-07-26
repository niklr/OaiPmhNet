using System;
using NUnit.Framework;
using OaiPmhNet.Converters;
using OaiPmhNet.Models;

namespace OaiPmhNet.Test.Converters
{
    [TestFixture]
    public class ResumptionTokenConverterTest
    {
        private IResumptionTokenConverter _converter;

        [OneTimeSetUp]
        public void Init()
        {
            OaiConfigurationTest.Init();
            _converter = new ResumptionTokenConverter(OaiConfiguration.Instance, new DateConverter());
        }

        [Test]
        public void ResumptionTokenConverter_Decode()
        {
            string token = "from=1987-02-16T00%3a00%3a00Z&until=2018-02-16T00%3a00%3a00Z&metadataPrefix=oai_dc" +
                "&set=test123&expirationDate=2019-02-16T00%3a00%3a00Z&completeListSize=123&cursor=321&placeholder=123&test=123";

            IResumptionToken actual = _converter.Decode(token);
            IResumptionToken expected = new ResumptionToken()
            {
                From = new DateTime(1987, 2, 16, 0, 0, 0, DateTimeKind.Utc),
                Until = new DateTime(2018, 2, 16, 0, 0, 0, DateTimeKind.Utc),
                MetadataPrefix = "oai_dc",
                Set = "test123",
                ExpirationDate = new DateTime(2019, 2, 16, 0, 0, 0, DateTimeKind.Utc),
                CompleteListSize = 123,
                Cursor = 321
            };
            expected.Custom["placeholder"] = "123";

            Assert.AreEqual(expected.From.Value.Kind, actual.From.Value.Kind);
            Assert.AreEqual(expected.From, actual.From);
            Assert.AreEqual(expected.Until.Value.Kind, actual.Until.Value.Kind);
            Assert.AreEqual(expected.Until, actual.Until);
            Assert.AreEqual(expected.MetadataPrefix, actual.MetadataPrefix);
            Assert.AreEqual(expected.Set, actual.Set);
            Assert.AreEqual(expected.ExpirationDate.Value.Kind, actual.ExpirationDate.Value.Kind);
            Assert.AreEqual(expected.ExpirationDate, actual.ExpirationDate);
            Assert.AreEqual(expected.CompleteListSize, actual.CompleteListSize);
            Assert.AreEqual(expected.Cursor, actual.Cursor);
            Assert.AreEqual(expected.Custom["placeholder"], actual.Custom["placeholder"]);
            Assert.IsFalse(actual.Custom.ContainsKey("test"));
        }

        [Test]
        public void ResumptionTokenConverter_Encode()
        {
            ResumptionToken token = new ResumptionToken()
            {
                From = new DateTime(1987, 2, 16, 0, 0, 0, DateTimeKind.Utc),
                Until = new DateTime(2018, 2, 16, 0, 0, 0, DateTimeKind.Utc),
                MetadataPrefix = "oai_dc",
                Set = "test123",
                ExpirationDate = new DateTime(2019, 2, 16, 0, 0, 0, DateTimeKind.Utc),
                CompleteListSize = 123,
                Cursor = 321
            };
            token.Custom.Add("placeholder", "123");
            token.Custom.Add("test", "123");

            string actual = _converter.Encode(token);
            string expected = "metadataPrefix%3doai_dc%26from%3d1987-02-16T00%3a00%3a00Z%26until%3d2018-02-16T00%3a00%3a00Z%26" +
                "set%3dtest123%26placeholder%3d123";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ResumptionTokenConverter_ToXElement()
        {
            ResumptionToken token = new ResumptionToken()
            {
                From = new DateTime(1987, 2, 16, 0, 0, 0, DateTimeKind.Utc),
                Until = new DateTime(2018, 2, 16, 0, 0, 0, DateTimeKind.Utc),
                MetadataPrefix = "oai_dc",
                Set = "test123",
                ExpirationDate = new DateTime(2019, 2, 16, 0, 0, 0, DateTimeKind.Utc),
                CompleteListSize = 123,
                Cursor = 321
            };
            token.Custom.Add("placeholder", "123");
            token.Custom.Add("test", "123");

            var actual = _converter.ToXElement(token).ToString();
            var expected = "<resumptionToken expirationDate=\"2019-02-16T00:00:00Z\" completeListSize=\"123\" cursor=\"321\">" +
                "metadataPrefix%3doai_dc%26from%3d1987-02-16T00%3a00%3a00Z%26until%3d2018-02-16T00%3a00%3a00Z%26set%3dtest123%26placeholder%3d123</resumptionToken>";

            Assert.AreEqual(expected, actual);
        }
    }
}
