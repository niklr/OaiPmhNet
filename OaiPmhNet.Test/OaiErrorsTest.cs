using System.Xml.Linq;
using NUnit.Framework;
using OaiPmhNet.Models;

namespace OaiPmhNet.Test
{
    [TestFixture]
    public class OaiErrorsTest
    {
        [TestCase(true, null, OaiArgument.None)]
        [TestCase(true, null, OaiArgument.MetadataPrefix, "metadataPrefix123")]
        [TestCase(true, null, OaiArgument.MetadataPrefix | OaiArgument.ResumptionToken, "metadataPrefix123", "token123")]
        [TestCase(false, @"<error code=""badArgument"">The request includes a 'metadataPrefix' argument that is not allowed for this verb.</error>", OaiArgument.None, "metadataPrefix123")]
        [TestCase(false, @"<error code=""badArgument"">The request includes a 'resumptionToken' argument that is not allowed for this verb.</error>", OaiArgument.MetadataPrefix, null, "token123")]
        [TestCase(false, @"<error code=""badArgument"">The request includes a 'identifier' argument that is not allowed for this verb.</error>", OaiArgument.MetadataPrefix | OaiArgument.ResumptionToken, null, null, "identifier123")]
        public void OaiErrors_ValidateArguments(bool expected, string expectedElement, OaiArgument allowedArguments, 
            string metadataPrefix = null,
            string resumptionToken = null,
            string identifier = null,
            string from = null,
            string until = null,
            string set = null)
        {
            var arguments = new ArgumentContainer(OaiVerb.None.ToString(), metadataPrefix, resumptionToken, identifier, from, until, set);
            var actual = OaiErrors.ValidateArguments(arguments, allowedArguments, out XElement errorElement);

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expectedElement, errorElement?.ToString());
        }
    }
}
