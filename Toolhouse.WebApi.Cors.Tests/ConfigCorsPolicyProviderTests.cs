using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Toolhouse.WebApi.Cors.Tests
{
    [TestClass]
    public class ConfigCorsPolicyProviderTests
    {
        [DataTestMethod]
        [DataRow("http://localhost:3000", true)]
        [DataRow("http://foo.example.org", true)]
        [DataRow("  http://localhost:3000 ", true)]
        [DataRow("https://localhost:3000", false)]
        [DataRow("http://bar.example.org", false)]
        [DataRow("", false)]
        [DataRow("  ", false)]
        [DataRow(null, false)]
        public void TestConfigCorsPolicyProvider(string test, bool shouldBeAllowed)
        {
            var provider = new ConfigCorsPolicyProvider("Test.AllowedOrigins");
            var isAllowed = provider.IsOriginAllowed(test);
            Assert.AreEqual(shouldBeAllowed, isAllowed, "Origin '{0}' {1} be allowed for pattern '{2}'", test, shouldBeAllowed ? "should be" : "should not be", ConfigurationManager.AppSettings[provider.ConfigurationKey]);
        }
    }
}
