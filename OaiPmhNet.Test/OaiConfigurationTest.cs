using NUnit.Framework;
using OaiPmhNet;

namespace OaiPmhNet.Test
{
    [TestFixture]
    public class OaiConfigurationTest
    {
        [OneTimeSetUp]
        public static void Init()
        {
            var config = OaiConfiguration.Instance;

            config.BaseUrl = () => { return "http://localhost/test"; };
            config.PageSize = 2;
            config.ResumptionTokenCustomParameterNames.Clear();
            config.ResumptionTokenCustomParameterNames.Add("placeholder");
        }
    }
}
