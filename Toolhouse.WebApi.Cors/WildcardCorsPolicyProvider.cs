using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Cors;
using System.Web.Http.Cors;

namespace Toolhouse.WebApi.Cors
{
    /// <summary>
    /// An implementation of ICorsPolicyProvider that allows enabling CORS for 
    /// certain host/port combinations, allowing a wildcard.
    /// Examples of valid patterns:
    ///     - "http://localhost:*"
    ///     - "https://*.example.org"
    /// </summary>
    public class WildcardCorsPolicyProvider : ICorsPolicyProvider
    {
        private readonly IEnumerable<Regex> regexes;

        /// <summary>
        /// Creates a new 
        /// </summary>
        /// <param name="patterns"></param>
        public WildcardCorsPolicyProvider(params string[] patterns)
        {
            regexes = patterns.Select(ParseOriginPattern);
        }

        public Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var policy = new CorsPolicy();
            policy.Origins.Clear();

            var allowedOrigin = GetAllowedOrigin(request);
            if (allowedOrigin != null)
            {
                policy.Origins.Add(allowedOrigin);
            }

            policy.AllowAnyMethod = true;
            policy.AllowAnyHeader = true;

            return Task.FromResult(policy);
        }

        public bool IsOriginAllowed(string origin)
        {
            if (origin == null)
            {
                return false;
            }

            origin = origin.Trim();
            if (origin.Length == 0)
            {
                return false;
            }

            return regexes.Any(r => r.IsMatch(origin));
        }

        private static Regex ParseOriginPattern(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                return null;
            }

            pattern = Regex.Escape(pattern);

            // Allow a couple simple substitutions:
            //  - ":*" is  used to match a port number
            //  - "*." is used to match a subdomain
            pattern = pattern.Replace(@":\*", @"(:\d+|)$");
            pattern = pattern.Replace("\\*\\.", @"[a-z0-9-]+\.");

            return new Regex(string.Format("^{0}$", pattern), RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Attempts to return an origin to allow for the given request. If no origin
        /// can be determined, returns null.
        /// </summary>
        public string GetAllowedOrigin(HttpRequestMessage request)
        {
            var origin = request.Headers.GetValues("Origin").FirstOrDefault();

            if (origin == null)
            {
                return null;
            }

            foreach (var rx in regexes)
            {
                if (rx.IsMatch(origin))
                {
                    return origin;
                }
            }

            return null;
        }
    }
}
