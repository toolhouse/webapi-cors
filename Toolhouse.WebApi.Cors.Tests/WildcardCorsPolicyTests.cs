using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Toolhouse.WebApi.Cors.Tests
{
    [TestClass]
    public class WildcardCorsPolicyTests
    {
        [DataTestMethod]
        [DataRow("http://localhost:*", "http://localhost:3000", true)]
        [DataRow("http://localhost:*", "http://localhost:8080", true)]
        [DataRow("http://localhost:*", "http://localhost", true)]
        [DataRow("http://localhost:*", "https://localhost:3000", false)]
        [DataRow("http://localhost:*", "https://localhost", false)]
        [DataRow("http://localhost:*", "", false)]
        [DataRow("http://localhost:*", null, false)]
        [DataRow("http://*.example.org", "http://example.org", false)]
        [DataRow("http://*.example.org", "http://foo.example.org", true)]
        [DataRow("http://*.example.org", "http://foo.bar.example.org", false)]
        [DataRow("http://*.*.example.org", "http://foo.bar.example.org", true)]
        [DataRow("http://*.*.example.org:*", "http://foo.bar.example.org:8000", true)]
        [DataRow("http://*.*.example.org:*", "http://foo.bar.example.org", true)]
        public void TestParsing(string pattern, string test, bool shouldBeAllowed)
        {
            var provider = new WildcardCorsPolicyProvider(pattern);
            var isAllowed = provider.IsOriginAllowed(test);
            Assert.AreEqual(shouldBeAllowed, isAllowed, "Origin '{0}' {1} be allowed for pattern '{2}'", test, shouldBeAllowed ? "should be" : "should not be", pattern);
        }
    }
}
