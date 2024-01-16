using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Http.Cors;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading;
using System.Web.Cors;

namespace Toolhouse.WebApi.Cors
{
    /// <summary>
    /// An implementation of ICorsPolicyProvider that reads a key from
    /// Web.config to enable CORS for one or more origins.
    /// Under the hood, this uses WildcardCorsPolicyProvider, so origins
    /// can contain wildcards.
    /// </summary>
    public class ConfigCorsPolicyProvider : ICorsPolicyProvider
    {
        private readonly Lazy<WildcardCorsPolicyProvider> wildcardCorsPolicyProvider;

        public ConfigCorsPolicyProvider(string configurationKey)
        {
            ConfigurationKey = configurationKey ?? throw new ArgumentNullException(nameof(configurationKey));
            wildcardCorsPolicyProvider = new Lazy<WildcardCorsPolicyProvider>(CreateWildcardPolicy);
        }

        /// <summary>
        /// Key in &lt;appSettings&gt; that will be read for the set of origins to allow.
        /// </summary>
        public string ConfigurationKey { get; private set;  }

        public Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return wildcardCorsPolicyProvider.Value.GetCorsPolicyAsync(request, cancellationToken);
        }

        public bool IsOriginAllowed(string origin)
        {
            return wildcardCorsPolicyProvider.Value.IsOriginAllowed(origin);
        }

        private WildcardCorsPolicyProvider CreateWildcardPolicy()
        {
            return new WildcardCorsPolicyProvider(
                (ConfigurationManager.AppSettings[ConfigurationKey] ?? "")
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(pattern => pattern.Trim())
                    .Where(pattern => pattern.Length > 0)
                    .ToArray()
            );
        }
    }
}
